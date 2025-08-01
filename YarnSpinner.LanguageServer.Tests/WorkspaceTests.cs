using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using YarnLanguageServer.Diagnostics;

namespace YarnLanguageServer.Tests
{
    public class WorkspaceTests
    {
        private static string Project1Path = Path.Combine(TestUtility.PathToTestWorkspace, "Project1", "Project1.yarnproject");
        private static string Project2Path = Path.Combine(TestUtility.PathToTestWorkspace, "Project2", "Project2.yarnproject");
        private static string NoProjectPath = Path.Combine(TestUtility.PathToTestWorkspace, "FilesWithNoProject");
        private static string MultipleDefsPath = Path.Combine(TestUtility.PathToTestWorkspace, "ProjectWithMultipleDefinitionsFiles");
        private static string JumpsAndDetoursPath = Path.Combine(TestUtility.PathToTestWorkspace, "JumpsAndDetours");

        [Fact]
        public async Task Projects_CanOpen()
        {
            // Given
            var project = new Project(Project1Path);

            // When
            await project.ReloadProjectFromDiskAsync(false, CancellationToken.None);

            // Then
            project.Files.Should().NotBeEmpty();
            project.Nodes.Should().NotBeEmpty();
            project.Files.Should().AllSatisfy(file => file.Project.Should().Be(project));

            var testFilePath = DocumentUri.FromFileSystemPath(Path.Combine(TestUtility.PathToTestWorkspace, "Project1", "Test.yarn"));

            project.MatchesUri(Project1Path).Should().BeTrue();
            project.MatchesUri(testFilePath).Should().BeTrue();
        }

        [Fact]
        public async Task Workspaces_CanOpen()
        {
            var workspace = new Workspace();
            workspace.Root = TestUtility.PathToTestWorkspace;
            await workspace.InitializeAsync();

            var diagnostics = workspace.GetDiagnostics();

            workspace.Projects.SelectMany(p => p.Nodes).Should().NotBeEmpty();

            workspace.Projects.Should().NotBeEmpty();

            // The node NotIncludedInProject is inside a file that is not
            // included in a .yarnproject; because we have opened a workspace
            // that includes .yarnprojects, the file will not be included
            workspace.Projects.Should().AllSatisfy(p => p.Nodes.Should().NotContain(n => n.UniqueTitle == "NotIncludedInProject"));

            workspace.Projects.Should().AllSatisfy(p => p.Uri!.Should().NotBeNull());

            var firstProject = workspace.Projects.Should().ContainSingle(p => p.Uri!.Path.Contains("Project1.yarnproject")).Subject;
            var fileInFirstProject = firstProject.Files.Should().Contain(f => f.Uri.LocalPath.Contains("Test.yarn")).Subject;

            // Validate that diagnostics are being generated by looking for a warning that 
            // '<<unknown_command>>' is being warned about.
            var fileDiagnostics = diagnostics.Should().ContainKey(fileInFirstProject.Uri).WhoseValue;
            fileDiagnostics.Should().NotBeEmpty();
            fileDiagnostics.Should().Contain(d => d.Code!.Value.String == nameof(YarnDiagnosticCode.YRNMsngCmdDef) && d.Message.Contains("unknown_command"));
        }

        [Fact]
        public async Task Workspaces_WithNoProjects_HaveImplicitProject()
        {
            // Given
            var workspace = new Workspace();
            workspace.Root = NoProjectPath;
            await workspace.InitializeAsync();

            // Then
            var project = workspace.Projects.Should().ContainSingle().Subject;
            var file = project.Files.Should().ContainSingle().Subject;
            file.NodeInfos.Should().Contain(n => n.UniqueTitle == "NotIncludedInProject");
            project.Diagnostics.Should().NotContain(d => d.Severity == Yarn.Compiler.Diagnostic.DiagnosticSeverity.Error);

        }

        [Fact]
        public void ActionsDefFile_ParsesCorrectly()
        {
            // Given
            var path = Path.Combine(NoProjectPath, "Commands.ysls.json");

            // When
            var json = new JsonConfigFile(File.ReadAllText(path), false);

            // Then
            json.GetActions().Should().Contain(n => n.YarnName == "custom_command" && n.Type == ActionType.Command);
            json.GetActions().Should().Contain(n => n.YarnName == "custom_function" && n.Type == ActionType.Function);
        }

        [Fact]
        public async Task Workspaces_WithDefsJsonAndNoProject_FindsCommands()
        {
            // Given
            var workspace = new Workspace();
            workspace.Root = NoProjectPath;

            // When
            await workspace.InitializeAsync();

            // Then
            var project = workspace.Projects.Should().ContainSingle().Subject;
            project.Commands.Should().Contain(c => c.YarnName == "custom_command");
            project.Functions.Should().Contain(f => f.YarnName == "custom_function");

            project.Diagnostics.Should().NotContain(d => d.Severity == Yarn.Compiler.Diagnostic.DiagnosticSeverity.Warning);
            project.Diagnostics.Should().NotContain(d => d.Severity == Yarn.Compiler.Diagnostic.DiagnosticSeverity.Error);
        }

        [Fact]
        public async Task Workspaces_WithDefinitionsFile_UseDefinitions()
        {
            // Given
            var workspace = new Workspace();
            workspace.Root = Path.GetDirectoryName(Project2Path);
            await workspace.InitializeAsync();

            // Then
            var project = workspace.Projects.Should().ContainSingle().Subject;
            project.Commands.Should().Contain(c => c.YarnName == "custom_command");
            project.Functions.Should().Contain(c => c.YarnName == "custom_function");
        }

        [Fact]
        public async Task Workspace_WithNullRoot_OpensSuccessfully()
        {
            // Given
            var workspace = new Workspace();
            workspace.Root = null;

            await workspace.InitializeAsync();
        }

        [Fact]
        public async Task Workspace_WithMultipleDefinitionsFiles_UsesMultipleFiles()
        {
            // Given
            var workspace = new Workspace();
            workspace.Root = MultipleDefsPath;
            await workspace.InitializeAsync();

            // When
            var projects = workspace.Projects;
            var relativeProject = projects.Should().Contain(p => p.Uri!.Path.EndsWith("Relative.yarnproject")).Subject;
            var relativeToWorkspaceProject = projects.Should().Contain(p => p.Uri!.Path.EndsWith("RelativeToWorkspace.yarnproject")).Subject;

            // Then
            relativeProject.Commands.Should().Contain(c => c.YarnName == "custom_command_1");
            relativeProject.Functions.Should().Contain(c => c.YarnName == "custom_function_1");
            relativeProject.Commands.Should().Contain(c => c.YarnName == "custom_command_2");
            relativeProject.Functions.Should().Contain(c => c.YarnName == "custom_function_2");

            relativeToWorkspaceProject.Commands.Should().Contain(c => c.YarnName == "custom_command_1");
            relativeToWorkspaceProject.Functions.Should().Contain(c => c.YarnName == "custom_function_1");
            relativeToWorkspaceProject.Commands.Should().Contain(c => c.YarnName == "custom_command_2");
            relativeToWorkspaceProject.Functions.Should().Contain(c => c.YarnName == "custom_function_2");

        }

        [Fact]
        public async Task Workspace_WithJumpsBetweenFiles_IdentifiesJumpsToOtherFiles()
        {
            var workspace = new Workspace();
            workspace.Root = JumpsAndDetoursPath;
            await workspace.InitializeAsync();

            var project = workspace.Projects.Single();
            var file = project.Files.Single(f => f.Uri.AbsolutePath.EndsWith("JumpsAndDetours.yarn"));
            var node1 = file.NodeInfos.Single(n => n.UniqueTitle == "Node1");
            var node2 = file.NodeInfos.Single(n => n.UniqueTitle == "Node2");

            node1.ContainsExternalJumps.Should().BeTrue();
            node2.ContainsExternalJumps.Should().BeFalse();
        }
    }
}

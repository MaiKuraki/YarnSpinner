using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using Yarn;
using Yarn.Compiler;
using Yarn.Saliency;

namespace YarnSpinner.Tests
{


    public class SaliencyTests : TestBase
    {
        public SaliencyTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        private CompilationResult CompileAndPrepareDialogue(string source, string node = "Start")
        {
            var job = CompilationJob.CreateFromString("input", source);
            var result = Compiler.Compile(job);
            result.Diagnostics.Should().BeEmpty();

            this.dialogue.SetProgram(result.Program);
            this.dialogue.SetNode(node);

            this.dialogue.LineHandler = (line) => { };
            this.dialogue.OptionsHandler = (opts) => this.dialogue.SetSelectedOption(opts.Options.First().ID);
            this.dialogue.CommandHandler = (cmd) => { };
            this.dialogue.NodeStartHandler = (node) => { };
            this.dialogue.NodeCompleteHandler = (node) => { };
            this.dialogue.DialogueCompleteHandler = () => { };

            return result;

        }

        [Fact]
        public void TestMocking()
        {
            // Given
            var mockSaliencyStrategy = new Mock<IContentSaliencyStrategy>(MockBehavior.Strict);

            // Create a mock saliency strategy that mimics the
            // FirstContentStrategy (i.e. it returns the first item in the list,
            // every time)
            mockSaliencyStrategy.Setup(
                s => s.QueryBestContent(It.IsAny<IEnumerable<ContentSaliencyOption>>())).Returns((IEnumerable<ContentSaliencyOption> a) => a.First()
            );

            var item = new ContentSaliencyOption("mock");

            // When
            var result = mockSaliencyStrategy.Object.QueryBestContent(new[] { item });

            // Then
            result.Should().Be(item);
            mockSaliencyStrategy.Verify(
                s => s.QueryBestContent(
                    It.Is<IEnumerable<ContentSaliencyOption>>(e => e.Count() == 1)
                )
            );
            mockSaliencyStrategy.VerifyNoOtherCalls();
        }

        [Fact]
        public void TestConditionCounts()
        {
            // Given
            var source = @"
title: Start
---
<<set $condition = true>>
<<jump NodeGroup>>
===
title: NodeGroup
when: $condition
expected: 1
---
<<stop>>
===
title: NodeGroup
when: true
expected: 1
---
<<stop>>
===
title: NodeGroup
when: !false
expected: 1
---
<<stop>>
===
title: NodeGroup
when: $condition is true
expected: 1
---
<<stop>>
===
title: NodeGroup
when: once
expected: 1
---
<<stop>>
===
title: NodeGroup
when: once if $condition
expected: 2
---
<<stop>>
===
title: NodeGroup
when: once if $condition && true
expected: 3
---
<<stop>>
===
title: NodeGroup
when: once if $condition && true
when: always
expected: 3
---
<<stop>>
===
title: NodeGroup
when: always
expected: 0
---
<<stop>>
===
title: NodeGroup
when: $condition && ($condition || false)
expected: 3
---
<<stop>>
===
title: NodeGroup
when: demo_function($condition && true)
expected: 2
---
<<stop>>
===
";

            var mockSaliencyStrategy = new Mock<IContentSaliencyStrategy>(MockBehavior.Strict);

            var content = new List<ContentSaliencyOption>();

            // Create a mock saliency strategy that mimics the
            // FirstContentStrategy (i.e. it returns the first item in the list
            // that has no failing conditions, every time)
            mockSaliencyStrategy.Setup((s) => s.QueryBestContent(It.IsAny<IEnumerable<ContentSaliencyOption>>()))
                .Callback<IEnumerable<ContentSaliencyOption>>(options =>
                {
                    content.AddRange(options.Where(option => option.FailingConditionValueCount == 0));
                })
                .Returns((IEnumerable<ContentSaliencyOption> a) => a.Where(a => a.FailingConditionValueCount == 0).FirstOrDefault());

            mockSaliencyStrategy.Setup(
                (s) => s.ContentWasSelected(It.IsAny<ContentSaliencyOption>()));

            dialogue.Library.RegisterFunction("demo_function", (bool a) => { return true; });

            var result = CompileAndPrepareDialogue(source);

            var expectedComplexities = new Dictionary<string, int>();

            int nodesInNodeGroup = 0;
            string nodeGroupName = "NodeGroup";

            foreach (var node in result.Program.Nodes)
            {
                var nodeGroupHeader = node.Value.Headers.SingleOrDefault(h => h.Key == Yarn.Node.NodeGroupHeader);

                if (nodeGroupHeader == null)
                {
                    continue;
                }

                nodeGroupHeader.Value.Should().Be(nodeGroupName);

                var expectedTag = node.Value.Headers.SingleOrDefault(h => h.Key == "expected")
                    ?? throw new Exception("Node " + node.Key + " is in node group but lacks an 'expected' header");

                expectedComplexities[node.Key] = int.Parse(expectedTag.Value);

                nodesInNodeGroup += 1;
            }

            this.dialogue.ContentSaliencyStrategy = mockSaliencyStrategy.Object;

            // When
            this.dialogue.Continue();

            // Then
            // The saliency strategy was invoked one time
            mockSaliencyStrategy.Verify(
                s => s.QueryBestContent(
                    It.IsAny<IEnumerable<ContentSaliencyOption>>()
                ), Times.Once
            );

            // The saliency strategy was given two options to choose from
            content.Should().HaveCount(nodesInNodeGroup, $"there are {nodesInNodeGroup} nodes in the node group, and every node is a valid option");

            // All options had a complexity of 1
            content.Should()
                   .AllSatisfy(c =>
                   {
                       c.ComplexityScore
                           .Should().Be(
                               expectedComplexities[c.ContentID],
                               $"{c.ContentID} should have passing complexity {expectedComplexities[c.ContentID]}"
                       );
                       c.PassingConditionValueCount.Should().BeGreaterThan(0, $"{c.ContentID} should have at least one passing condition");
                       c.FailingConditionValueCount.Should().Be(0, $"{c.ContentID} should have no failing conditions");
                   });
        }

        [Fact]
        public void TestQueryingCandidates()
        {
            // Given
            var source = @"
title: NodeGroup
when: $condition1
---
<<stop>>
===
title: NodeGroup
when: $condition2
---
<<stop>>
===
title: NodeGroup
when: always
---
<<stop>>
===
";

            // When
            var result = CompileAndPrepareDialogue(source, "NodeGroup");
            dialogue.VariableStorage.SetValue("$condition1", true);

            IEnumerable<ContentSaliencyOption> availableContent = dialogue.GetSaliencyOptionsForNodeGroup("NodeGroup");

            // Then
            availableContent.Should().HaveCount(3, "All nodes are part of the same group");

            availableContent.Should().AllSatisfy(
                c => result.Program.Nodes.Should().Contain(
                    n => n.Key == c.ContentID, $"{c.ContentID} should be one of the nodes in the program"
                )
            );

            availableContent.Where(c => c.FailingConditionValueCount == 0).Should().HaveCount(2, "2 nodes are passing");
            availableContent.Where(c => c.FailingConditionValueCount > 0).Should().HaveCount(1, "1 node is failing");
        }

        [Fact]
        public void TestNodesWithOnceHeaderOnlyAppearOnce()
        {
            // Given

            string source = @"
title: Start
when: once
---
This content is only seen once.
===
";

            CompileAndPrepareDialogue(source);

            this.dialogue.ContentSaliencyStrategy = new FirstSaliencyStrategy();

            var availableContentBeforeRun = this.dialogue.GetSaliencyOptionsForNodeGroup("Start");

            availableContentBeforeRun.Where(c => c.FailingConditionValueCount == 0).Should().HaveCount(1);

            // When
            this.dialogue.Continue();

            // Then
            var availableContentAfterRun = this.dialogue.GetSaliencyOptionsForNodeGroup("Start");
            availableContentAfterRun.Where(c => c.FailingConditionValueCount == 0).Should().HaveCount(0);
        }

        [Fact]
        public void TestDialogueCanBeQueriedForNodeGroups()
        {
            // Given

            string source = @"
title: Start
when: once
---
This content is only seen once.
===
title: Start
when: $a == 2
---
This content is only seen when a is 2.
===
title: NotAGroup
---
This node is not part of a node group.
===
";

            CompileAndPrepareDialogue(source);

            this.dialogue.NodeExists("DoesntExist").Should().BeFalse();

            this.dialogue.NodeExists("Start").Should().BeTrue();
            this.dialogue.IsNodeGroup("Start").Should().BeTrue();

            this.dialogue.NodeExists("NotAGroup").Should().BeTrue();
            this.dialogue.IsNodeGroup("NotAGroup").Should().BeFalse();

            // We can always ask for saliency options given a valid node name,
            // even if it's not a node group

            var queryInvalidNodeName = () => { this.dialogue.GetSaliencyOptionsForNodeGroup("DoesntExist"); };
            queryInvalidNodeName.Should().ThrowExactly<ArgumentException>().WithMessage("*not a valid node name*");

            this.dialogue.GetSaliencyOptionsForNodeGroup("Start").Should().HaveCount(2);
            this.dialogue.GetSaliencyOptionsForNodeGroup("NotAGroup").Should().HaveCount(1);

        }

        [Fact]
        public void TestNodeGroupWithSparseSubtitles()
        {
            string source = @"
title: Start
subtitle: Special
when: always
---
This is a special start node which should get a subtitle name.
===
title: Start
when: always
---
This is a random start node which should get a UUID name.
===
title: Start
when: always
---
This is a random start node which should get a UUID name.
===
";
            var result = CompileAndPrepareDialogue(source);

            this.dialogue.IsNodeGroup("Start").Should().BeTrue();
            this.dialogue.NodeExists("Start.Special").Should().BeTrue();
            // the program should now contain nodes named
            // ["Start.Special", "Start.<UUID>", "Start.<a different UUID"]
            // (plus probably 4 generated internal nodes)
            var nodes = result.Program.Nodes.Values;
            var numNodes = nodes.Where(n => n.Name.StartsWith("Start.")).Count();
            (numNodes == 3).Should().BeTrue();
        }
    }
}

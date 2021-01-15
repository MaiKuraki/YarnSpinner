﻿using Xunit;
using System;
using System.Collections;
using System.Collections.Generic;
using Yarn;
using System.Reflection;
using System.IO;
using System.Linq;
using System.Globalization;

namespace YarnSpinner.Tests
{

    public class TestBase
    {
        protected IVariableStorage storage = new MemoryVariableStore();
        protected Dialogue dialogue;
        protected IDictionary<string, Yarn.Compiler.StringInfo> stringTable;
        protected IEnumerable<Yarn.Compiler.Declaration> declarations;

        public string locale = "en";
        
        protected bool errorsCauseFailures = true;

        // Returns the path that contains the test case files.

        public static string ProjectRootPath {
            get {
                var path = Assembly.GetCallingAssembly().Location.Split(Path.DirectorySeparatorChar).ToList();

                var index = path.FindIndex(x => x == "YarnSpinner.Tests");

                if (index == -1)
                {
                    throw new System.IO.DirectoryNotFoundException("Cannot find test data directory");                    
                }

                var testDataDirectory = path.Take(index).ToList();

                var pathToTestData = string.Join(Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture), testDataDirectory.ToArray());

                return pathToTestData;
            }
        }


        public static string TestDataPath
        {
            get
            {
                return Path.Combine(ProjectRootPath, "Tests");
            }
        }

        public static string SpaceDemoScriptsPath
        {
            get
            {
                return Path.Combine(ProjectRootPath, "Tests/Projects/Space");
            }
        }

        protected TestPlan testPlan;

        public string GetComposedTextForLine(Line line) {

            var substitutedText = Dialogue.ExpandSubstitutions(stringTable[line.ID].text, line.Substitutions);

            return dialogue.ParseMarkup(substitutedText).Text;
        }
        
        public TestBase()
        {

            dialogue = new Dialogue (storage);

            dialogue.LanguageCode = "en";

            dialogue.LogDebugMessage = delegate(string message) {
                Console.ResetColor();
                Console.WriteLine (message);

            };

            dialogue.LogErrorMessage = delegate(string message) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine ("ERROR: " + message);
                Console.ResetColor ();

                if (errorsCauseFailures == true) {
                    Assert.NotNull(message);
                }
                    
            };

            dialogue.LineHandler = delegate (Line line) {
                var id = line.ID;

                Assert.Contains(id, stringTable.Keys);

                var lineNumber = stringTable[id].lineNumber;

                var text = GetComposedTextForLine(line);

                Console.WriteLine("Line: " + text);

                if (testPlan != null) {
                    testPlan.Next();

                    if (testPlan.nextExpectedType == TestPlan.Step.Type.Line) {
                        Assert.Equal($"Line {lineNumber}: {testPlan.nextExpectedValue}", $"Line {lineNumber}: {text}");
                    } else {
                        throw new Xunit.Sdk.XunitException($"Received line {text}, but was expecting a {testPlan.nextExpectedType.ToString()}");
                    }
                }
            };

            dialogue.OptionsHandler = delegate (OptionSet optionSet) {
                var optionCount = optionSet.Options.Length;

                Console.WriteLine("Options:");
                foreach (var option in optionSet.Options) {
                    var optionText = GetComposedTextForLine(option.Line);
                    Console.WriteLine(" - " + optionText);
                }

                if (testPlan != null) {
                    testPlan.Next();

                    if (testPlan.nextExpectedType != TestPlan.Step.Type.Select) {
                        throw new Xunit.Sdk.XunitException($"Received {optionCount} options, but wasn't expecting them (was expecting {testPlan.nextExpectedType.ToString()})");
                    }

                    // Assert that the list of options we were given is
                    // identical to the list of options we expect
                    var actualOptionList = optionSet.Options
                        .Select(o => (GetComposedTextForLine(o.Line), o.IsAvailable))
                        .ToList();
                    Assert.Equal(testPlan.nextExpectedOptions, actualOptionList);

                    var expectedOptionCount = testPlan.nextExpectedOptions.Count();

                    Assert.Equal (expectedOptionCount, optionCount);
                    
                    if (testPlan.nextOptionToSelect != -1) {
                        dialogue.SetSelectedOption(testPlan.nextOptionToSelect - 1);                    
                    } else {
                        dialogue.SetSelectedOption(0);                    
                    }
                }

                
            };

            dialogue.CommandHandler = delegate (Command command) {
                Console.WriteLine("Command: " + command.Text);
                
                if (testPlan != null) {
                    testPlan.Next();
                    if (testPlan.nextExpectedType != TestPlan.Step.Type.Command)
                    {
                        throw new Xunit.Sdk.XunitException($"Received command {command.Text}, but wasn't expecting to select one (was expecting {testPlan.nextExpectedType.ToString()})");
                    }
                    else
                    {
                        // We don't need to get the composed string for a
                        // command because it's been done for us in the
                        // virtual machine. The VM can do this because
                        // commands are not localised, so we don't need to
                        // refer to the string table to get the text.
                        Assert.Equal(testPlan.nextExpectedValue, command.Text);
                    }
                }
            };

            dialogue.Library.RegisterFunction ("assert", delegate(Yarn.Value value) {
                if (value.ConvertTo<bool>() == false) {
                        Assert.NotNull ("Assertion failed");
                }
                return true;
            });

            
            // When a node is complete, do nothing
            dialogue.NodeCompleteHandler = (string nodeName) => {};

            // When dialogue is complete, check that we expected a stop
            dialogue.DialogueCompleteHandler = () => {
                if (testPlan != null) {
                    testPlan.Next();

                    if (testPlan.nextExpectedType != TestPlan.Step.Type.Stop) {
                        throw new Xunit.Sdk.XunitException($"Stopped dialogue, but wasn't expecting to select it (was expecting {testPlan.nextExpectedType.ToString()})");
                    }
                }
            };

            // The Space test scripts call a function called "visited",
            // which is defined in the Unity runtime and returns true if a
            // node with the given name has been run before. For type
            // correctness, we stub it out here with an implementation that
            // just returns false
            dialogue.Library.RegisterFunction("visited", (string nodeName) => false );

        }

        /// <summary>
        /// Executes the named node, and checks any assertions made during
        /// execution. Fails the test if an assertion made in Yarn fails.
        /// </summary>
        /// <param name="nodeName">The name of the node to start the test
        /// from. Defaults to "Start".</param>
        protected void RunStandardTestcase(string nodeName = "Start") {

            if (testPlan == null) {
                throw new Xunit.Sdk.XunitException("Cannot run test: no test plan provided.");
            }

            dialogue.SetNode(nodeName);

            do {
                dialogue.Continue();
            } while (dialogue.IsActive);
            
        }

        protected string CreateTestNode(string source, string name="Start") {
            return $"title: {name}\n---\n{source}\n===";
            
        }

        /// <summary>
        /// Sets the current test plan to one loaded from a given path.
        /// </summary>
        /// <param name="path">The path of the file containing the test
        /// plan.</param>
        public void LoadTestPlan(string path) {
            this.testPlan = new TestPlan(path);
        }

        // Returns the list of .node and.yarn files in the
        // Tests/<directory> directory.
        public static IEnumerable<object[]> FileSources(string directoryComponents) {

            var allowedExtensions = new[] { ".node", ".yarn" };

            var directory = Path.Combine(directoryComponents.Split('/'));

            var path = Path.Combine(TestDataPath, directory);

            var files = GetFilesInDirectory(path);

            return files.Where(p => allowedExtensions.Contains(Path.GetExtension(p)))
                        .Where(p => p.EndsWith(".upgraded.yarn") == false) // don't include ".upgraded.yarn" (used in UpgraderTests)
                        .Select(p => new[] {Path.Combine(directory, Path.GetFileName(p))});
        }

        public static IEnumerable<object[]> DirectorySources(string directoryComponents) {
            var directory = Path.Combine(directoryComponents.Split('/'));

            var path = Path.Combine(TestDataPath, directory);

            try {
                return Directory.EnumerateDirectories(path)
                    .Select(d => new[] {d});
            } catch (DirectoryNotFoundException) {
                return new string[] { }.Select(d => new[] {d});
            }
        }

        // Returns the list of files in a directory. If that directory doesn't
        // exist, returns an empty list.
        static IEnumerable<string> GetFilesInDirectory(string path)
        {
            try
            {
                return Directory.EnumerateFiles(path);
            }
            catch (DirectoryNotFoundException)
            {
                return new string[] { };
            }
        }
    }
}


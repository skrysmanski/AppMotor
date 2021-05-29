#region License
// Copyright 2020 AppMotor Framework (https://github.com/skrysmanski/AppMotor)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;

using AppMotor.CliApp.CommandLine;
using AppMotor.CliApp.TestUtils;

using JetBrains.Annotations;

using Shouldly;

using Xunit;

namespace AppMotor.CliApp.Tests.CommandLine
{
    public sealed class GenerateHelpTests
    {
        [Fact]
        public void TestGenerateMainHelp()
        {
            // Setup
            var app = new TestApplication();

            // Test
            app.Run("--help");

            // Verify
            app.TerminalOutput.ShouldContain("The app's description");

            app.TerminalOutput.ShouldContain("#move#");
            app.TerminalOutput.ShouldContain("#mov#");
            app.TerminalOutput.ShouldContain("#mv#");
            app.TerminalOutput.ShouldContain("Moves items");

            app.TerminalOutput.ShouldContain("#subgroup#");
            app.TerminalOutput.ShouldContain("#subgrp#");
            app.TerminalOutput.ShouldContain("#sg#");
            app.TerminalOutput.ShouldContain("A simple verb group");
        }

        [Theory]
        [InlineData("--help", "#move#")]
        [InlineData("-h", "#mv#")]
        [InlineData("-?", "#mov#")]
        [InlineData("/?", "#move#")]
        [InlineData("/h", "#move#")]
        [InlineData("#move#", "--help")]
        [InlineData("#mov#", "-h")]
        [InlineData("#mv#", "-?")]
        [InlineData("#move#", "/h")]
        [InlineData("#move#", "/?")]
        [InlineData("help", "#move#")]
        [InlineData("help", "#mov#")]
        [InlineData("help", "#mv#")]
        public void TestGenerateCommandHelp(string firstArg, string secondArg)
        {
            // Setup
            var app = new TestApplication();

            // Test
            app.Run(firstArg, secondArg);

            // Verify
            app.TerminalOutput.ShouldContain("#move#");
            app.TerminalOutput.ShouldContain("Moves items");
            app.TerminalOutput.ShouldContain("--value");
            app.TerminalOutput.ShouldContain("--val");
            app.TerminalOutput.ShouldContain("A value to move");
            app.TerminalOutput.ShouldContain("42");
            app.TerminalOutput.ShouldContain("#name#");
            app.TerminalOutput.ShouldContain("The name to move");

            app.TerminalOutput.ShouldContain("#sub1#");
            app.TerminalOutput.ShouldContain("A sub command named #sub1#");
        }

        [Theory]
        [InlineData("--help", "#move#", "#sub1")]
        [InlineData("-h", "#move#", "#sub1")]
        [InlineData("-?", "#move#", "#sub1")]
        [InlineData("/?", "#move#", "#sub1")]
        [InlineData("/h", "#move#", "#sub1")]
        [InlineData("#move#", "#sub1", "--help")]
        [InlineData("#move#", "#sub1", "-h")]
        [InlineData("#move#", "#sub1", "-?")]
        [InlineData("#move#", "#sub1", "/h")]
        [InlineData("#move#", "#sub1", "/?")]
        [InlineData("help", "#move#", "#sub1")]
        public void TestGenerateSubCommandHelp(string firstArg, string secondArg, string thirdArg)
        {
            // Setup
            var app = new TestApplication();

            // Test
            app.Run(firstArg, secondArg, thirdArg);

            // Verify
            app.TerminalOutput.ShouldContain("#sub1#");
            app.TerminalOutput.ShouldContain("A sub command named #sub1#");
        }

        [Theory]
        [InlineData("--help", "#subgroup#")]
        [InlineData("-h", "#subgrp#")]
        [InlineData("-?", "#sg#")]
        [InlineData("/?", "#subgroup#")]
        [InlineData("/h", "#subgroup#")]
        [InlineData("#subgroup#", "--help")]
        [InlineData("#subgrp#", "-h")]
        [InlineData("#sg#", "-?")]
        [InlineData("#subgroup#", "/h")]
        [InlineData("#subgroup#", "/?")]
        [InlineData("help", "#subgroup#")]
        [InlineData("help", "#subgrp#")]
        [InlineData("help", "#sg#")]
        public void TestGenerateSubVerbGroupHelp(string firstArg, string secondArg)
        {
            // Setup
            var app = new TestApplication();

            // Test
            app.Run(firstArg, secondArg);

            // Verify
            app.TerminalOutput.ShouldContain("#subgroup#");
            app.TerminalOutput.ShouldContain("A simple verb group");

            app.TerminalOutput.ShouldContain("#sub2#");
            app.TerminalOutput.ShouldContain("A sub command named #sub2#");
        }

        /// <summary>
        /// Verifies that the named parameters of the command appear before the general parameters (--debug, help, version).
        /// </summary>
        [Fact]
        public void Test_NamedParameterOrder()
        {
            // Setup
            var testCommand = new CommandWithNamedParam();
            var app = new TestApplicationWithCommand()
            {
                Command = testCommand,
            };

            // Test
            app.Run("--help");

            // Verify
            int valueParamIndex = app.TerminalOutput.IndexOf("--value", StringComparison.Ordinal);
            valueParamIndex.ShouldNotBe(-1);

            // These parameters must appear after "--value".
            app.TerminalOutput.IndexOf("--debug", StringComparison.Ordinal).ShouldBeGreaterThan(valueParamIndex);
            app.TerminalOutput.IndexOf("--help", StringComparison.Ordinal).ShouldBeGreaterThan(valueParamIndex);
            app.TerminalOutput.IndexOf("--version", StringComparison.Ordinal).ShouldBeGreaterThan(valueParamIndex);
        }

        private sealed class TestApplication : TestApplicationWithVerbs
        {
            /// <inheritdoc />
            public TestApplication() : base(new MoveVerb(), new MySubGroup())
            {
                this.AppDescription = "The app's description";
            }

            private sealed class MoveCommand : CliCommand
            {
                /// <inheritdoc />
                public override string HelpText => "Moves items";

                /// <inheritdoc />
                protected override CliCommandExecutor Executor => new(Execute);

                [UsedImplicitly]
                public CliParam<int> NamedParam { get; } = new("--value", "--val")
                {
                    DefaultValue = 42,
                    HelpText = "A value to move",
                };

                [UsedImplicitly]
                public CliParam<string> PositionalParam { get; } = new("#name#", positionIndex: 0)
                {
                    HelpText = "The name to move",
                };

                private static void Execute()
                {
                    throw new InvalidOperationException("We should not get here.");
                }
            }

            private sealed class MoveVerb : CliVerb
            {
                public MoveVerb() : base("#move#", "#mv#", "#mov#")
                {
                    this.Command = new MoveCommand();
                    var subVerb = new SubVerb("#sub1#");
                    this.SubVerbs = new[] { subVerb };
                }
            }

            private sealed class MySubGroup : CliVerb
            {
                public MySubGroup() : base("#subgroup#", "#subgrp#", "#sg#")
                {
                    this.HelpText = "A simple verb group";
                    this.SubVerbs = new[] { new SubVerb("#sub2#") };
                }
            }

            private sealed class SubVerb : CliVerb
            {
                public SubVerb(string name) : base(name, new NonExecutableCommand())
                {
                    this.HelpText = $"A sub command named {name}";
                }
            }

            private sealed class NonExecutableCommand : CliCommand
            {
                /// <inheritdoc />
                protected override CliCommandExecutor Executor => new(Execute);

                private static void Execute()
                {
                    throw new InvalidOperationException("We should not get here.");
                }
            }
        }

        private sealed class CommandWithNamedParam : CliCommand
        {
            /// <inheritdoc />
            protected override CliCommandExecutor Executor => new(Execute);

            // ReSharper disable once UnusedMember.Local
            public readonly CliParam<int> ValueParam = new("--value");

            private void Execute()
            {
                throw new NotSupportedException();
            }
        }
    }
}

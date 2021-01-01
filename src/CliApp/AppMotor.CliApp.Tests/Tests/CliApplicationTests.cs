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
using System.IO;
using System.Text;

using AppMotor.CliApp.Terminals;
using AppMotor.CliApp.TestUtils;

using Shouldly;

using Xunit;

namespace AppMotor.CliApp.Tests
{
    public sealed class CliApplicationTests
    {
        [Fact]
        public void TestRun()
        {
            var testTerminal = new TestTerminal();
            CliApplication.Run<TestApplication>(new[] { "abc", "def" }, testTerminal).ShouldBe(0, testTerminal.CurrentOutput);

            testTerminal.CurrentOutput.Trim().ShouldBe("Hello, app with 2 args!");
        }

        [Fact]
        public void TestWaitForKey()
        {
            var testTerminal = new WaitForKeyTerminal();
            var testApp = new TestApplication(waitForKeyPressOnExit: true)
            {
                Terminal = testTerminal,
            };
            testApp.Run().ShouldBe(0, testTerminal.CurrentOutput);

            testTerminal.CurrentOutput.Trim().ShouldBe("Hello, app with 0 args!" + Environment.NewLine + Environment.NewLine + "Press any key to exit...");
            testTerminal.ReadKeyCalled.ShouldBe(true);
        }

        [Fact]
        public void TestUnhandledExceptionHandling()
        {
            // Setup
            var testApp = new TestApplicationForExceptionHandling();

            // Test
            testApp.Run().ShouldBe(-1, testApp.TerminalOutput);

            // Verify
            testApp.TerminalOutput.ShouldContain("This is a test");
            testApp.TerminalOutput.ShouldContain("My support message");
        }

        private sealed class TestApplication : CliApplication
        {
            /// <inheritdoc />
            protected override bool WaitForKeyPressOnExit { get; }

            /// <inheritdoc />
            protected override CliApplicationExecutor MainExecutor => new(Execute);

            /// <inheritdoc />
            public TestApplication()
            {
            }

            public TestApplication(bool waitForKeyPressOnExit)
            {
                this.WaitForKeyPressOnExit = waitForKeyPressOnExit;
            }

            private void Execute(string[] args)
            {
                this.Terminal.WriteLine($"Hello, app with {args.Length} args!");
            }
        }

        private sealed class WaitForKeyTerminal : ITerminal
        {
            /// <inheritdoc />
            public TextReader Input => throw new NotSupportedException();

            /// <inheritdoc />
            public bool IsInputRedirected => false;

            /// <inheritdoc />
            public TextWriter Out { get; }

            /// <inheritdoc />
            public bool IsOutputRedirected => throw new NotSupportedException();

            /// <inheritdoc />
            public TextWriter Error => throw new NotSupportedException();

            /// <inheritdoc />
            public bool IsErrorRedirected => throw new NotSupportedException();

            /// <inheritdoc />
            public ConsoleColor BackgroundColor
            {
                get => throw new NotSupportedException();
                set => throw new NotSupportedException();
            }

            /// <inheritdoc />
            public bool IsKeyAvailable => true;

            private readonly StringBuilder _outWriter = new();

            public string CurrentOutput => this._outWriter.ToString();

            public bool ReadKeyCalled { get; private set; }

            public WaitForKeyTerminal()
            {
                var threadSafeWriter = TextWriter.Synchronized(new StringWriter(this._outWriter));
                this.Out = threadSafeWriter;
            }

            /// <inheritdoc />
            public ConsoleKeyInfo ReadKey(bool displayPressedKey = true)
            {
                this.ReadKeyCalled = true;
                return new('a', ConsoleKey.A, shift: false, alt: false, control: false);
            }

            /// <inheritdoc />
            public string ReadLine() => throw new NotSupportedException();

            /// <inheritdoc />
            public void Write(ColoredString? coloredString)
            {
                this.Out.Write(coloredString);
            }
        }

        private sealed class TestApplicationForExceptionHandling : TestCliApplicationBase
        {
            /// <inheritdoc />
            protected override CliApplicationExecutor MainExecutor => new(Execute);

            private void Execute()
            {
                try
                {
                    Thrower();
                }
                catch (Exception ex)
                {
                    throw new AggregateException(ex);
                }
            }

            private static void Thrower()
            {
                throw new InvalidOperationException("This is a test");
            }

            /// <inheritdoc />
            protected override string GetSupportMessage(Exception exception)
            {
                return "My support message";
            }
        }
    }
}

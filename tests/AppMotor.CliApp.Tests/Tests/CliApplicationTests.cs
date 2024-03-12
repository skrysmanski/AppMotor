// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Text;

using AppMotor.CliApp.Terminals;
using AppMotor.CliApp.TestUtils;
using AppMotor.TestCore.Utils;

using Shouldly;

using Xunit;

namespace AppMotor.CliApp.Tests;

/// <summary>
/// Tests for <see cref="CliApplication"/>.
/// </summary>
public sealed class CliApplicationTests
{
    [Fact]
    public void Test_Run_Static_Delegate_WithoutExitCode()
    {
        // Setup
        bool executed = false;

        // Test
        int exitCode = CliApplication.Run(() =>
        {
            executed = true;
        });

        // Verify
        exitCode.ShouldBe(0);
        executed.ShouldBe(true);
    }

    [Fact]
    public void Test_Run_Static_Delegate_WithExitCode()
    {
        // Setup
        bool executed = false;

        // Test
        int exitCode = CliApplication.Run(() =>
        {
            executed = true;
            return 42;
        });

        // Verify
        exitCode.ShouldBe(42);
        executed.ShouldBe(true);
    }

    [Fact]
    public async Task Test_RunAsync_Static_Delegate_WithoutExitCode()
    {
        // Setup
        bool executed = false;

        // Test
        int exitCode = await CliApplication.RunAsync(async () =>
        {
            await Task.Delay(1);
            executed = true;
        });

        // Verify
        exitCode.ShouldBe(0);
        executed.ShouldBe(true);
    }

    [Fact]
    public async Task Test_RunAsync_Static_Delegate_WithExitCode()
    {
        // Setup
        bool executed = false;

        // Test
        int exitCode = await CliApplication.RunAsync(async () =>
        {
            await Task.Delay(1);
            executed = true;
            return 42;
        });

        // Verify
        exitCode.ShouldBe(42);
        executed.ShouldBe(true);
    }

    [Fact]
    public void Test_Run_Static()
    {
        // Setup
        var testTerminal = new TestTerminal();

        // Test
        int exitCode = CliApplication.Run<TestApplication>(["abc", "def"], testTerminal);

        // Verify
        exitCode.ShouldBe(42, testTerminal.CurrentOutput);
        testTerminal.CurrentOutput.Trim().ShouldBe("Hello, app with 2 args!");
    }

    [Fact]
    public async Task Test_RunAsync_Static()
    {
        // Setup
        var testTerminal = new TestTerminal();

        //  Test
        int exitCode = await CliApplication.RunAsync<TestApplication>(["abc", "def"], testTerminal);

        // Verify
        exitCode.ShouldBe(42, testTerminal.CurrentOutput);
        testTerminal.CurrentOutput.Trim().ShouldBe("Hello, app with 2 args!");
    }

    [Fact]
    public void Test_Run_Args()
    {
        // Setup
        var testTerminal = new TestTerminal();
        var testApp = new TestApplication()
        {
            Terminal = testTerminal,
        };

        // Test
        int exitCode = testApp.Run("abc", "def");

        // Verify
        exitCode.ShouldBe(42, testTerminal.CurrentOutput);
        testTerminal.CurrentOutput.Trim().ShouldBe("Hello, app with 2 args!");
    }

    [Fact]
    public void Test_Run_CancellationToken()
    {
        // Setup
        var testTerminal = new TestTerminal();
        var testApp = new TestApplication()
        {
            Terminal = testTerminal,
        };

        using var cts = new CancellationTokenSource();

        // Test
        int exitCode = testApp.Run(cts.Token);

        // Verify
        exitCode.ShouldBe(42, testTerminal.CurrentOutput);
        testTerminal.CurrentOutput.Trim().ShouldBe("Hello, app with 0 args!");
        testApp.CancellationToken.ShouldBe(cts.Token);
    }

    [Fact]
    public void Test_Run_ArgsAndCancellationToken()
    {
        // Setup
        var testTerminal = new TestTerminal();
        var testApp = new TestApplication()
        {
            Terminal = testTerminal,
        };

        using var cts = new CancellationTokenSource();

        // Test
        int exitCode = testApp.Run(["abc", "def"], cts.Token);

        // Verify
        exitCode.ShouldBe(42, testTerminal.CurrentOutput);
        testTerminal.CurrentOutput.Trim().ShouldBe("Hello, app with 2 args!");
        testApp.CancellationToken.ShouldBe(cts.Token);
    }

    [Fact]
    public async Task Test_RunAsync_Args()
    {
        // Setup
        var testTerminal = new TestTerminal();
        var testApp = new TestApplication()
        {
            Terminal = testTerminal,
        };

        // Test
        int exitCode = await testApp.RunAsync("abc", "def");

        // Verify
        exitCode.ShouldBe(42, testTerminal.CurrentOutput);
        testTerminal.CurrentOutput.Trim().ShouldBe("Hello, app with 2 args!");
    }

    [Fact]
    public async Task Test_RunAsync_CancellationToken()
    {
        // Setup
        var testTerminal = new TestTerminal();
        var testApp = new TestApplication()
        {
            Terminal = testTerminal,
        };

        using var cts = new CancellationTokenSource();

        // Test
        int exitCode = await testApp.RunAsync(cts.Token);

        // Verify
        exitCode.ShouldBe(42, testTerminal.CurrentOutput);
        testTerminal.CurrentOutput.Trim().ShouldBe("Hello, app with 0 args!");
        testApp.CancellationToken.ShouldBe(cts.Token);
    }

    [Fact]
    public async Task Test_RunAsync_ArgsAndCancellationToken()
    {
        // Setup
        var testTerminal = new TestTerminal();
        var testApp = new TestApplication()
        {
            Terminal = testTerminal,
        };

        using var cts = new CancellationTokenSource();

        // Test
        int exitCode = await testApp.RunAsync(["abc", "def"], cts.Token);

        // Verify
        exitCode.ShouldBe(42, testTerminal.CurrentOutput);
        testTerminal.CurrentOutput.Trim().ShouldBe("Hello, app with 2 args!");
        testApp.CancellationToken.ShouldBe(cts.Token);
    }

    [Fact]
    public void Test_WaitForKey()
    {
        // Setup
        var testTerminal = new WaitForKeyTerminal();
        var testApp = new TestApplication(waitForKeyPressOnExit: true)
        {
            Terminal = testTerminal,
        };

        // Test
        int exitCode = testApp.Run();

        // Verify
        exitCode.ShouldBe(42, testTerminal.CurrentOutput);
        testTerminal.CurrentOutput.Trim().ShouldBe("Hello, app with 0 args!" + Environment.NewLine + Environment.NewLine + "Press any key to exit...");
        testTerminal.ReadKeyCalled.ShouldBe(true);
    }

    [Fact]
    public void Test_UnhandledExceptionHandling()
    {
        // Setup
        var testApp = new TestApplicationForExceptionHandling();

        // Test
        testApp.RunWithExpectedException().ShouldBeOfType<AggregateException>();

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

        public CancellationToken? CancellationToken { get; private set; }

        /// <inheritdoc />
        public TestApplication()
        {
        }

        public TestApplication(bool waitForKeyPressOnExit)
        {
            this.WaitForKeyPressOnExit = waitForKeyPressOnExit;
        }

        private int Execute(string[] args, CancellationToken cancellationToken)
        {
            this.Terminal.WriteLine($"Hello, app with {args.Length} args!");
            this.CancellationToken = cancellationToken;

            return 42;
        }
    }

    private sealed class WaitForKeyTerminal : ITerminal
    {
        /// <inheritdoc />
        public TextReader In => throw new NotSupportedException();

        /// <inheritdoc />
        public bool IsInputRedirected => false;

        /// <inheritdoc />
        public ITerminalWriter Out { get; }

        /// <inheritdoc />
        public bool IsOutputRedirected => throw new NotSupportedException();

        /// <inheritdoc />
        public ITerminalWriter Error => throw new NotSupportedException();

        /// <inheritdoc />
        public bool IsErrorRedirected => throw new NotSupportedException();

        /// <inheritdoc />
        public bool IsKeyAvailable => true;

        private readonly StringBuilder _outWriter = new();

        public string CurrentOutput => this._outWriter.ToString();

        public bool ReadKeyCalled { get; private set; }

        public WaitForKeyTerminal()
        {
            this.Out = new SimpleTerminalWriter(value => this._outWriter.Append(value));
        }

        /// <inheritdoc />
        public ConsoleKeyInfo ReadKey(bool displayPressedKey = true)
        {
            this.ReadKeyCalled = true;
            return new('a', ConsoleKey.A, shift: false, alt: false, control: false);
        }

        /// <inheritdoc />
        public string ReadLine() => throw new NotSupportedException();
    }

    private sealed class TestApplicationForExceptionHandling : TestCliApplicationBase
    {
        /// <inheritdoc />
        protected override CliApplicationExecutor MainExecutor => new(Execute);

        private static void Execute()
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

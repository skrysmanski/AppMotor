//
// NOTE: This file has been AUTOMATICALLY GENERATED from 'CliCommandExecutorTests.cs.mustache'. Any changes made to
//   this file will be LOST on the next build.
//

// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.CliApp.CommandLine;
using AppMotor.CliApp.TestUtils;

using Shouldly;

using Xunit;

namespace AppMotor.CliApp.Tests.CommandLine;

/// <summary>
/// Tests <see cref="CliCommandExecutor"/>.
/// </summary>
public sealed class CliCommandExecutorTests
{
    private const string COMMAND_NAME = "test";

    [Fact]
    public void Test_Sync_Void_NoCancellationToken()
    {
        // Setup
        bool called = false;

        void Execute()
        {
            called = true;
        }

        var testApplication = new TestApplication(new CliCommandExecutor(Execute));

        // Test
        testApplication.AppHelper.Run(new[] { COMMAND_NAME }, expectedExitCode: 0);

        // Verify
        called.ShouldBe(true);
        testApplication.ShouldHaveNoOutput();
    }

    [Fact]
    public void Test_Sync_Void_WithCancellationToken()
    {
        // Setup
        bool called = false;

        using var cts = new CancellationTokenSource();

        void Execute(CancellationToken cancellationToken)
        {
            called = true;
            cancellationToken.IsCancellationRequested.ShouldBe(false);
            // ReSharper disable once AccessToDisposedClosure
            cts.Cancel();
            cancellationToken.IsCancellationRequested.ShouldBe(true); // Validates we actually got the token from "cts"
        }

        var testApplication = new TestApplication(new CliCommandExecutor(Execute));

        // Test
        testApplication.AppHelper.Run(new[] { COMMAND_NAME }, expectedExitCode: 0, cts.Token);

        // Verify
        called.ShouldBe(true);
        testApplication.ShouldHaveNoOutput();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Test_Sync_Bool_NoCancellationToken(bool retVal)
    {
        // Setup
        bool called = false;

        bool Execute()
        {
            called = true;
            return retVal;
        }

        var testApplication = new TestApplication(new CliCommandExecutor(Execute));

        // Test
        testApplication.AppHelper.Run(new[] { COMMAND_NAME }, expectedExitCode: retVal ? 0 : 1);

        // Verify
        called.ShouldBe(true);
        testApplication.ShouldHaveNoOutput();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Test_Sync_Bool_WithCancellationToken(bool retVal)
    {
        // Setup
        bool called = false;

        using var cts = new CancellationTokenSource();

        bool Execute(CancellationToken cancellationToken)
        {
            called = true;
            cancellationToken.IsCancellationRequested.ShouldBe(false);
            // ReSharper disable once AccessToDisposedClosure
            cts.Cancel();
            cancellationToken.IsCancellationRequested.ShouldBe(true); // Validates we actually got the token from "cts"
            return retVal;
        }

        var testApplication = new TestApplication(new CliCommandExecutor(Execute));

        // Test
        testApplication.AppHelper.Run(new[] { COMMAND_NAME }, expectedExitCode: retVal ? 0 : 1, cts.Token);

        // Verify
        called.ShouldBe(true);
        testApplication.ShouldHaveNoOutput();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(42)]
    public void Test_Sync_Int_NoCancellationToken(int retVal)
    {
        // Setup
        bool called = false;

        int Execute()
        {
            called = true;
            return retVal;
        }

        var testApplication = new TestApplication(new CliCommandExecutor(Execute));

        // Test
        testApplication.AppHelper.Run(new[] { COMMAND_NAME }, expectedExitCode: retVal);

        // Verify
        called.ShouldBe(true);
        testApplication.ShouldHaveNoOutput();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(42)]
    public void Test_Sync_Int_WithCancellationToken(int retVal)
    {
        // Setup
        bool called = false;

        using var cts = new CancellationTokenSource();

        int Execute(CancellationToken cancellationToken)
        {
            called = true;
            cancellationToken.IsCancellationRequested.ShouldBe(false);
            // ReSharper disable once AccessToDisposedClosure
            cts.Cancel();
            cancellationToken.IsCancellationRequested.ShouldBe(true); // Validates we actually got the token from "cts"
            return retVal;
        }

        var testApplication = new TestApplication(new CliCommandExecutor(Execute));

        // Test
        testApplication.AppHelper.Run(new[] { COMMAND_NAME }, expectedExitCode: retVal, cts.Token);

        // Verify
        called.ShouldBe(true);
        testApplication.ShouldHaveNoOutput();
    }

    [Fact]
    public void Test_Async_Void_NoCancellationToken()
    {
        // Setup
        bool called = false;

        async Task Execute()
        {
            await Task.Delay(1);
            called = true;
        }

        var testApplication = new TestApplication(new CliCommandExecutor(Execute));

        // Test
        testApplication.AppHelper.Run(new[] { COMMAND_NAME }, expectedExitCode: 0);

        // Verify
        called.ShouldBe(true);
        testApplication.ShouldHaveNoOutput();
    }

    [Fact]
    public void Test_Async_Void_WithCancellationToken()
    {
        // Setup
        bool called = false;

        using var cts = new CancellationTokenSource();

        async Task Execute(CancellationToken cancellationToken)
        {
            // ReSharper disable once MethodSupportsCancellation
            await Task.Delay(1);
            called = true;
            cancellationToken.IsCancellationRequested.ShouldBe(false);
            // ReSharper disable once AccessToDisposedClosure
            cts.Cancel();
            cancellationToken.IsCancellationRequested.ShouldBe(true); // Validates we actually got the token from "cts"
        }

        var testApplication = new TestApplication(new CliCommandExecutor(Execute));

        // Test
        testApplication.AppHelper.Run(new[] { COMMAND_NAME }, expectedExitCode: 0, cts.Token);

        // Verify
        called.ShouldBe(true);
        testApplication.ShouldHaveNoOutput();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Test_Async_Bool_NoCancellationToken(bool retVal)
    {
        // Setup
        bool called = false;

        async Task<bool> Execute()
        {
            await Task.Delay(1);
            called = true;
            return retVal;
        }

        var testApplication = new TestApplication(new CliCommandExecutor(Execute));

        // Test
        testApplication.AppHelper.Run(new[] { COMMAND_NAME }, expectedExitCode: retVal ? 0 : 1);

        // Verify
        called.ShouldBe(true);
        testApplication.ShouldHaveNoOutput();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Test_Async_Bool_WithCancellationToken(bool retVal)
    {
        // Setup
        bool called = false;

        using var cts = new CancellationTokenSource();

        async Task<bool> Execute(CancellationToken cancellationToken)
        {
            // ReSharper disable once MethodSupportsCancellation
            await Task.Delay(1);
            called = true;
            cancellationToken.IsCancellationRequested.ShouldBe(false);
            // ReSharper disable once AccessToDisposedClosure
            cts.Cancel();
            cancellationToken.IsCancellationRequested.ShouldBe(true); // Validates we actually got the token from "cts"
            return retVal;
        }

        var testApplication = new TestApplication(new CliCommandExecutor(Execute));

        // Test
        testApplication.AppHelper.Run(new[] { COMMAND_NAME }, expectedExitCode: retVal ? 0 : 1, cts.Token);

        // Verify
        called.ShouldBe(true);
        testApplication.ShouldHaveNoOutput();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(42)]
    public void Test_Async_Int_NoCancellationToken(int retVal)
    {
        // Setup
        bool called = false;

        async Task<int> Execute()
        {
            await Task.Delay(1);
            called = true;
            return retVal;
        }

        var testApplication = new TestApplication(new CliCommandExecutor(Execute));

        // Test
        testApplication.AppHelper.Run(new[] { COMMAND_NAME }, expectedExitCode: retVal);

        // Verify
        called.ShouldBe(true);
        testApplication.ShouldHaveNoOutput();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(42)]
    public void Test_Async_Int_WithCancellationToken(int retVal)
    {
        // Setup
        bool called = false;

        using var cts = new CancellationTokenSource();

        async Task<int> Execute(CancellationToken cancellationToken)
        {
            // ReSharper disable once MethodSupportsCancellation
            await Task.Delay(1);
            called = true;
            cancellationToken.IsCancellationRequested.ShouldBe(false);
            // ReSharper disable once AccessToDisposedClosure
            cts.Cancel();
            cancellationToken.IsCancellationRequested.ShouldBe(true); // Validates we actually got the token from "cts"
            return retVal;
        }

        var testApplication = new TestApplication(new CliCommandExecutor(Execute));

        // Test
        testApplication.AppHelper.Run(new[] { COMMAND_NAME }, expectedExitCode: retVal, cts.Token);

        // Verify
        called.ShouldBe(true);
        testApplication.ShouldHaveNoOutput();
    }

    private sealed class TestApplication : TestApplicationWithVerbsBase
    {
        public TestApplication(CliCommandExecutor commandExecutor)
        {
            this.Verbs = new[] { new CliVerb(COMMAND_NAME, new TestCommand(commandExecutor)) };
        }
    }

    private sealed class TestCommand : CliCommand
    {
        /// <inheritdoc />
        protected override CliCommandExecutor Executor { get; }

        /// <inheritdoc />
        public TestCommand(CliCommandExecutor commandExecutor)
        {
            this.Executor = commandExecutor;
        }
    }
}

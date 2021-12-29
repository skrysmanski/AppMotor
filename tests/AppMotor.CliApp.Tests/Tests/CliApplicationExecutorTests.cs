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

using System.Threading;
using System.Threading.Tasks;

using AppMotor.CliApp.TestUtils;

using Shouldly;

using Xunit;

namespace AppMotor.CliApp.Tests;

/// <summary>
/// Tests <see cref="CliApplicationExecutor"/>.
/// </summary>
public sealed class CliApplicationExecutorTests
{
    private static readonly string[] TEST_ARGS = { "abc", "def" };

    // START MARKER: Generated code

    //
    // NOTE: The code of this class has been generated with the 'ExecutorGenerator' tool. Do
    //   not make manual changes to this class or they may get lost (by accident) when the code
    //   for this class is generated the next time!!!
    //

    [Fact]
    public void Test_Sync_Void_NoArgs_NoCancellationToken()
    {
        // Setup
        bool called = false;

        void Execute()
        {
            called = true;
        }

        var testApplication = new TestApplication(new CliApplicationExecutor(Execute));

        // Test
        testApplication.AppHelper.Run(TEST_ARGS, expectedExitCode: 0);

        // Verify
        called.ShouldBe(true);
        testApplication.ShouldHaveNoOutput();
    }

    [Fact]
    public void Test_Sync_Void_WithArgs_NoCancellationToken()
    {
        // Setup
        bool called = false;

        void Execute(string[] args)
        {
            called = true;
            args.ShouldBe(TEST_ARGS);
        }

        var testApplication = new TestApplication(new CliApplicationExecutor(Execute));

        // Test
        testApplication.AppHelper.Run(TEST_ARGS, expectedExitCode: 0);

        // Verify
        called.ShouldBe(true);
        testApplication.ShouldHaveNoOutput();
    }

    [Fact]
    public void Test_Sync_Void_NoArgs_WithCancellationToken()
    {
        // Setup
        bool called = false;

        using var cts = new CancellationTokenSource();

        void Execute(CancellationToken cancellationToken)
        {
            called = true;
            cancellationToken.IsCancellationRequested.ShouldBe(false);
            cts.Cancel();
            cancellationToken.IsCancellationRequested.ShouldBe(true); // Validates we actually got the token from "cts"
        }

        var testApplication = new TestApplication(new CliApplicationExecutor(Execute));

        // Test
        testApplication.AppHelper.Run(TEST_ARGS, expectedExitCode: 0, cts.Token);

        // Verify
        called.ShouldBe(true);
        testApplication.ShouldHaveNoOutput();
    }

    [Fact]
    public void Test_Sync_Void_WithArgs_WithCancellationToken()
    {
        // Setup
        bool called = false;

        using var cts = new CancellationTokenSource();

        void Execute(string[] args, CancellationToken cancellationToken)
        {
            called = true;
            args.ShouldBe(TEST_ARGS);
            cancellationToken.IsCancellationRequested.ShouldBe(false);
            cts.Cancel();
            cancellationToken.IsCancellationRequested.ShouldBe(true); // Validates we actually got the token from "cts"
        }

        var testApplication = new TestApplication(new CliApplicationExecutor(Execute));

        // Test
        testApplication.AppHelper.Run(TEST_ARGS, expectedExitCode: 0, cts.Token);

        // Verify
        called.ShouldBe(true);
        testApplication.ShouldHaveNoOutput();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Test_Sync_Bool_NoArgs_NoCancellationToken(bool retVal)
    {
        // Setup
        bool called = false;

        bool Execute()
        {
            called = true;
            return retVal;
        }

        var testApplication = new TestApplication(new CliApplicationExecutor(Execute));

        // Test
        testApplication.AppHelper.Run(TEST_ARGS, expectedExitCode: retVal ? 0 : 1);

        // Verify
        called.ShouldBe(true);
        testApplication.ShouldHaveNoOutput();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Test_Sync_Bool_WithArgs_NoCancellationToken(bool retVal)
    {
        // Setup
        bool called = false;

        bool Execute(string[] args)
        {
            called = true;
            args.ShouldBe(TEST_ARGS);
            return retVal;
        }

        var testApplication = new TestApplication(new CliApplicationExecutor(Execute));

        // Test
        testApplication.AppHelper.Run(TEST_ARGS, expectedExitCode: retVal ? 0 : 1);

        // Verify
        called.ShouldBe(true);
        testApplication.ShouldHaveNoOutput();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Test_Sync_Bool_NoArgs_WithCancellationToken(bool retVal)
    {
        // Setup
        bool called = false;

        using var cts = new CancellationTokenSource();

        bool Execute(CancellationToken cancellationToken)
        {
            called = true;
            cancellationToken.IsCancellationRequested.ShouldBe(false);
            cts.Cancel();
            cancellationToken.IsCancellationRequested.ShouldBe(true); // Validates we actually got the token from "cts"
            return retVal;
        }

        var testApplication = new TestApplication(new CliApplicationExecutor(Execute));

        // Test
        testApplication.AppHelper.Run(TEST_ARGS, expectedExitCode: retVal ? 0 : 1, cts.Token);

        // Verify
        called.ShouldBe(true);
        testApplication.ShouldHaveNoOutput();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Test_Sync_Bool_WithArgs_WithCancellationToken(bool retVal)
    {
        // Setup
        bool called = false;

        using var cts = new CancellationTokenSource();

        bool Execute(string[] args, CancellationToken cancellationToken)
        {
            called = true;
            args.ShouldBe(TEST_ARGS);
            cancellationToken.IsCancellationRequested.ShouldBe(false);
            cts.Cancel();
            cancellationToken.IsCancellationRequested.ShouldBe(true); // Validates we actually got the token from "cts"
            return retVal;
        }

        var testApplication = new TestApplication(new CliApplicationExecutor(Execute));

        // Test
        testApplication.AppHelper.Run(TEST_ARGS, expectedExitCode: retVal ? 0 : 1, cts.Token);

        // Verify
        called.ShouldBe(true);
        testApplication.ShouldHaveNoOutput();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(42)]
    public void Test_Sync_Int_NoArgs_NoCancellationToken(int retVal)
    {
        // Setup
        bool called = false;

        int Execute()
        {
            called = true;
            return retVal;
        }

        var testApplication = new TestApplication(new CliApplicationExecutor(Execute));

        // Test
        testApplication.AppHelper.Run(TEST_ARGS, expectedExitCode: retVal);

        // Verify
        called.ShouldBe(true);
        testApplication.ShouldHaveNoOutput();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(42)]
    public void Test_Sync_Int_WithArgs_NoCancellationToken(int retVal)
    {
        // Setup
        bool called = false;

        int Execute(string[] args)
        {
            called = true;
            args.ShouldBe(TEST_ARGS);
            return retVal;
        }

        var testApplication = new TestApplication(new CliApplicationExecutor(Execute));

        // Test
        testApplication.AppHelper.Run(TEST_ARGS, expectedExitCode: retVal);

        // Verify
        called.ShouldBe(true);
        testApplication.ShouldHaveNoOutput();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(42)]
    public void Test_Sync_Int_NoArgs_WithCancellationToken(int retVal)
    {
        // Setup
        bool called = false;

        using var cts = new CancellationTokenSource();

        int Execute(CancellationToken cancellationToken)
        {
            called = true;
            cancellationToken.IsCancellationRequested.ShouldBe(false);
            cts.Cancel();
            cancellationToken.IsCancellationRequested.ShouldBe(true); // Validates we actually got the token from "cts"
            return retVal;
        }

        var testApplication = new TestApplication(new CliApplicationExecutor(Execute));

        // Test
        testApplication.AppHelper.Run(TEST_ARGS, expectedExitCode: retVal, cts.Token);

        // Verify
        called.ShouldBe(true);
        testApplication.ShouldHaveNoOutput();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(42)]
    public void Test_Sync_Int_WithArgs_WithCancellationToken(int retVal)
    {
        // Setup
        bool called = false;

        using var cts = new CancellationTokenSource();

        int Execute(string[] args, CancellationToken cancellationToken)
        {
            called = true;
            args.ShouldBe(TEST_ARGS);
            cancellationToken.IsCancellationRequested.ShouldBe(false);
            cts.Cancel();
            cancellationToken.IsCancellationRequested.ShouldBe(true); // Validates we actually got the token from "cts"
            return retVal;
        }

        var testApplication = new TestApplication(new CliApplicationExecutor(Execute));

        // Test
        testApplication.AppHelper.Run(TEST_ARGS, expectedExitCode: retVal, cts.Token);

        // Verify
        called.ShouldBe(true);
        testApplication.ShouldHaveNoOutput();
    }

    [Fact]
    public void Test_Async_Void_NoArgs_NoCancellationToken()
    {
        // Setup
        bool called = false;

        async Task Execute()
        {
            await Task.Delay(1);
            called = true;
        }

        var testApplication = new TestApplication(new CliApplicationExecutor(Execute));

        // Test
        testApplication.AppHelper.Run(TEST_ARGS, expectedExitCode: 0);

        // Verify
        called.ShouldBe(true);
        testApplication.ShouldHaveNoOutput();
    }

    [Fact]
    public void Test_Async_Void_WithArgs_NoCancellationToken()
    {
        // Setup
        bool called = false;

        async Task Execute(string[] args)
        {
            await Task.Delay(1);
            called = true;
            args.ShouldBe(TEST_ARGS);
        }

        var testApplication = new TestApplication(new CliApplicationExecutor(Execute));

        // Test
        testApplication.AppHelper.Run(TEST_ARGS, expectedExitCode: 0);

        // Verify
        called.ShouldBe(true);
        testApplication.ShouldHaveNoOutput();
    }

    [Fact]
    public void Test_Async_Void_NoArgs_WithCancellationToken()
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
            cts.Cancel();
            cancellationToken.IsCancellationRequested.ShouldBe(true); // Validates we actually got the token from "cts"
        }

        var testApplication = new TestApplication(new CliApplicationExecutor(Execute));

        // Test
        testApplication.AppHelper.Run(TEST_ARGS, expectedExitCode: 0, cts.Token);

        // Verify
        called.ShouldBe(true);
        testApplication.ShouldHaveNoOutput();
    }

    [Fact]
    public void Test_Async_Void_WithArgs_WithCancellationToken()
    {
        // Setup
        bool called = false;

        using var cts = new CancellationTokenSource();

        async Task Execute(string[] args, CancellationToken cancellationToken)
        {
            // ReSharper disable once MethodSupportsCancellation
            await Task.Delay(1);
            called = true;
            args.ShouldBe(TEST_ARGS);
            cancellationToken.IsCancellationRequested.ShouldBe(false);
            cts.Cancel();
            cancellationToken.IsCancellationRequested.ShouldBe(true); // Validates we actually got the token from "cts"
        }

        var testApplication = new TestApplication(new CliApplicationExecutor(Execute));

        // Test
        testApplication.AppHelper.Run(TEST_ARGS, expectedExitCode: 0, cts.Token);

        // Verify
        called.ShouldBe(true);
        testApplication.ShouldHaveNoOutput();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Test_Async_Bool_NoArgs_NoCancellationToken(bool retVal)
    {
        // Setup
        bool called = false;

        async Task<bool> Execute()
        {
            await Task.Delay(1);
            called = true;
            return retVal;
        }

        var testApplication = new TestApplication(new CliApplicationExecutor(Execute));

        // Test
        testApplication.AppHelper.Run(TEST_ARGS, expectedExitCode: retVal ? 0 : 1);

        // Verify
        called.ShouldBe(true);
        testApplication.ShouldHaveNoOutput();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Test_Async_Bool_WithArgs_NoCancellationToken(bool retVal)
    {
        // Setup
        bool called = false;

        async Task<bool> Execute(string[] args)
        {
            await Task.Delay(1);
            called = true;
            args.ShouldBe(TEST_ARGS);
            return retVal;
        }

        var testApplication = new TestApplication(new CliApplicationExecutor(Execute));

        // Test
        testApplication.AppHelper.Run(TEST_ARGS, expectedExitCode: retVal ? 0 : 1);

        // Verify
        called.ShouldBe(true);
        testApplication.ShouldHaveNoOutput();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Test_Async_Bool_NoArgs_WithCancellationToken(bool retVal)
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
            cts.Cancel();
            cancellationToken.IsCancellationRequested.ShouldBe(true); // Validates we actually got the token from "cts"
            return retVal;
        }

        var testApplication = new TestApplication(new CliApplicationExecutor(Execute));

        // Test
        testApplication.AppHelper.Run(TEST_ARGS, expectedExitCode: retVal ? 0 : 1, cts.Token);

        // Verify
        called.ShouldBe(true);
        testApplication.ShouldHaveNoOutput();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Test_Async_Bool_WithArgs_WithCancellationToken(bool retVal)
    {
        // Setup
        bool called = false;

        using var cts = new CancellationTokenSource();

        async Task<bool> Execute(string[] args, CancellationToken cancellationToken)
        {
            // ReSharper disable once MethodSupportsCancellation
            await Task.Delay(1);
            called = true;
            args.ShouldBe(TEST_ARGS);
            cancellationToken.IsCancellationRequested.ShouldBe(false);
            cts.Cancel();
            cancellationToken.IsCancellationRequested.ShouldBe(true); // Validates we actually got the token from "cts"
            return retVal;
        }

        var testApplication = new TestApplication(new CliApplicationExecutor(Execute));

        // Test
        testApplication.AppHelper.Run(TEST_ARGS, expectedExitCode: retVal ? 0 : 1, cts.Token);

        // Verify
        called.ShouldBe(true);
        testApplication.ShouldHaveNoOutput();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(42)]
    public void Test_Async_Int_NoArgs_NoCancellationToken(int retVal)
    {
        // Setup
        bool called = false;

        async Task<int> Execute()
        {
            await Task.Delay(1);
            called = true;
            return retVal;
        }

        var testApplication = new TestApplication(new CliApplicationExecutor(Execute));

        // Test
        testApplication.AppHelper.Run(TEST_ARGS, expectedExitCode: retVal);

        // Verify
        called.ShouldBe(true);
        testApplication.ShouldHaveNoOutput();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(42)]
    public void Test_Async_Int_WithArgs_NoCancellationToken(int retVal)
    {
        // Setup
        bool called = false;

        async Task<int> Execute(string[] args)
        {
            await Task.Delay(1);
            called = true;
            args.ShouldBe(TEST_ARGS);
            return retVal;
        }

        var testApplication = new TestApplication(new CliApplicationExecutor(Execute));

        // Test
        testApplication.AppHelper.Run(TEST_ARGS, expectedExitCode: retVal);

        // Verify
        called.ShouldBe(true);
        testApplication.ShouldHaveNoOutput();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(42)]
    public void Test_Async_Int_NoArgs_WithCancellationToken(int retVal)
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
            cts.Cancel();
            cancellationToken.IsCancellationRequested.ShouldBe(true); // Validates we actually got the token from "cts"
            return retVal;
        }

        var testApplication = new TestApplication(new CliApplicationExecutor(Execute));

        // Test
        testApplication.AppHelper.Run(TEST_ARGS, expectedExitCode: retVal, cts.Token);

        // Verify
        called.ShouldBe(true);
        testApplication.ShouldHaveNoOutput();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(42)]
    public void Test_Async_Int_WithArgs_WithCancellationToken(int retVal)
    {
        // Setup
        bool called = false;

        using var cts = new CancellationTokenSource();

        async Task<int> Execute(string[] args, CancellationToken cancellationToken)
        {
            // ReSharper disable once MethodSupportsCancellation
            await Task.Delay(1);
            called = true;
            args.ShouldBe(TEST_ARGS);
            cancellationToken.IsCancellationRequested.ShouldBe(false);
            cts.Cancel();
            cancellationToken.IsCancellationRequested.ShouldBe(true); // Validates we actually got the token from "cts"
            return retVal;
        }

        var testApplication = new TestApplication(new CliApplicationExecutor(Execute));

        // Test
        testApplication.AppHelper.Run(TEST_ARGS, expectedExitCode: retVal, cts.Token);

        // Verify
        called.ShouldBe(true);
        testApplication.ShouldHaveNoOutput();
    }


    // END MARKER: Generated code

    private sealed class TestApplication : TestCliApplicationBase
    {
        /// <inheritdoc />
        protected override CliApplicationExecutor MainExecutor { get; }

        /// <inheritdoc />
        public TestApplication(CliApplicationExecutor executor)
        {
            this.MainExecutor = executor;
        }

    }
}
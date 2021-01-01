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

using System.Collections.Generic;
using System.Threading.Tasks;

using AppMotor.CliApp.CommandLine;
using AppMotor.CliApp.TestUtils;

using Shouldly;

using Xunit;

namespace AppMotor.CliApp.Tests.CommandLine
{
    /// <summary>
    /// Tests <see cref="CliCommandExecutor"/>.
    /// </summary>
    public sealed class CliCommandExecutorTests
    {
        private const string COMMAND_NAME = "test";

        [Fact]
        public void Test_Sync_Void()
        {
            // Setup
            bool called = false;

            void Execute()
            {
                called = true;
            }

            var testApplication = new TestApplication(new CliCommandExecutor(Execute));

            // Test
            var exitCode = testApplication.Run(COMMAND_NAME);

            // Verify
            called.ShouldBe(true);
            exitCode.ShouldBe(0);
            testApplication.ShouldHaveNoOutput();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Test_Sync_Bool(bool retVal)
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
            var exitCode = testApplication.Run(COMMAND_NAME);

            // Verify
            called.ShouldBe(true);
            if (retVal)
            {
                exitCode.ShouldBe(0);
            }
            else
            {
                exitCode.ShouldBe(1);
            }
            testApplication.ShouldHaveNoOutput();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(42)]
        public void Test_Sync_Int(int retVal)
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
            var exitCode = testApplication.Run(COMMAND_NAME);

            // Verify
            called.ShouldBe(true);
            exitCode.ShouldBe(retVal);
            testApplication.ShouldHaveNoOutput();
        }

        [Fact]
        public async Task Test_Async_Void()
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
            var exitCode = await testApplication.RunAsync(COMMAND_NAME);

            // Verify
            called.ShouldBe(true);
            exitCode.ShouldBe(0);
            testApplication.ShouldHaveNoOutput();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Test_Async_Bool(bool retVal)
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
            var exitCode = await testApplication.RunAsync(COMMAND_NAME);

            // Verify
            called.ShouldBe(true);
            if (retVal)
            {
                exitCode.ShouldBe(0);
            }
            else
            {
                exitCode.ShouldBe(1);
            }
            testApplication.ShouldHaveNoOutput();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(42)]
        public async Task Test_Async_Int(int retVal)
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
            var exitCode = await testApplication.RunAsync(COMMAND_NAME);

            // Verify
            called.ShouldBe(true);
            exitCode.ShouldBe(retVal);
            testApplication.ShouldHaveNoOutput();
        }

        private sealed class TestApplication : TestApplicationWithCommandsBase
        {
            private readonly CliCommandExecutor _commandExecutor;

            /// <inheritdoc />
            public TestApplication(CliCommandExecutor commandExecutor)
            {
                this._commandExecutor = commandExecutor;
            }

            /// <inheritdoc />
            protected override IEnumerable<CliCommand> GetVerbs()
            {
                yield return new TestCommand(this._commandExecutor);
            }
        }

        private sealed class TestCommand : CliCommand
        {
            /// <inheritdoc />
            protected override CliCommandExecutor Executor { get; }

            /// <inheritdoc />
            public TestCommand(CliCommandExecutor commandExecutor) : base(COMMAND_NAME)
            {
                this.Executor = commandExecutor;
            }
        }
    }
}

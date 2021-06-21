#region License
// Copyright 2021 AppMotor Framework (https://github.com/skrysmanski/AppMotor)
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

using AppMotor.CliApp.CommandLine.Hosting;
using AppMotor.CliApp.TestUtils;

using Microsoft.Extensions.Hosting;

using Shouldly;

using Xunit;

namespace AppMotor.CliApp.Tests.CommandLine.Hosting
{
    public sealed class GenericHostCliCommandTests
    {
        [Fact]
        public void TestCustomHostBuilderFactory()
        {
            // Setup
            var command = new CommandWithCustomHostBuilderFactory();
            var testApp = new TestApplicationWithCommand(command);

            command.LifetimeEvents.Started += (_, _) => command.Stop();

            // Test
            testApp.Run();

            // Verify
            command.CustomHostBuilderFactoryCalled.ShouldBe(true);
        }

        private sealed class CommandWithCustomHostBuilderFactory : GenericHostCliCommand
        {
            public bool CustomHostBuilderFactoryCalled { get; private set; }

            /// <inheritdoc />
            protected override IHostBuilderFactory HostBuilderFactory { get; }

            /// <inheritdoc />
            public CommandWithCustomHostBuilderFactory()
            {
                this.HostBuilderFactory = new MethodHostBuilderFactory(
                    () =>
                    {
                        this.CustomHostBuilderFactoryCalled = true;
                        return new HostBuilder();
                    }
                );
            }
        }
    }
}

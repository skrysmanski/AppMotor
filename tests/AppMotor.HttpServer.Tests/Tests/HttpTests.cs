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

using AppMotor.CliApp.CommandLine;
using AppMotor.CliApp.CommandLine.Hosting;
using AppMotor.CliApp.HttpServer.TestUtils;
using AppMotor.Core.Net;
using AppMotor.Core.Net.Http;
using AppMotor.HttpServer;
using AppMotor.HttpServer.Startups;
using AppMotor.TestCore;
using AppMotor.TestCore.Extensions;
using AppMotor.TestCore.Logging;
using AppMotor.TestCore.Networking;

using Microsoft.AspNetCore.Hosting;

using Shouldly;

using Xunit;
using Xunit.Abstractions;

namespace AppMotor.CliApp.HttpServer.Tests;

public sealed class HttpTests : TestBase
{
    public HttpTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    [Fact]
    public async Task TestHttpApiCall()
    {
        int testPort = ServerPortProvider.GetNextTestPort();

        using var cts = new CancellationTokenSource();

        var app = new CliApplicationWithCommand(new TestHttpServerCommand(testPort, this.TestConsole));
        Task appTask = app.RunAsync(cts.Token);

        using (var httpClient = HttpClientFactory.CreateHttpClient())
        {
            // ReSharper disable once MethodSupportsCancellation
            var response = await httpClient.GetAsync($"http://localhost:{testPort}/api/ping");

            response.EnsureSuccessStatusCode();

            // ReSharper disable once MethodSupportsCancellation
            var responseString = await response.Content.ReadAsStringAsync();

            responseString.ShouldBe("Hello World!");
        }

        cts.Cancel();

        await appTask.OrTimeoutAfter(TimeSpan.FromSeconds(10));
    }

    private sealed class TestHttpServerCommand : HttpServerCommandBase
    {
        private readonly int _testPort;

        /// <inheritdoc />
        protected override IHostBuilderFactory HostBuilderFactory { get; }

        public TestHttpServerCommand(int testPort, ITestOutputHelper testOutputHelper)
        {
            this._testPort = testPort;

            this.HostBuilderFactory = new DefaultHostBuilderFactory()
            {
                LoggingConfigurationProvider = (_, builder) =>
                {
                    builder.AddXUnitLogger(testOutputHelper);
                },
            };
        }

        /// <inheritdoc />
        protected override IEnumerable<HttpServerPort> GetServerPorts(IServiceProvider serviceProvider)
        {
            yield return new HttpServerPort(SocketListenAddresses.Loopback, this._testPort);
        }

        /// <inheritdoc />
        protected override IAspNetStartup CreateStartupClass(WebHostBuilderContext context)
        {
            return new SimplePingStartup();
        }
    }
}

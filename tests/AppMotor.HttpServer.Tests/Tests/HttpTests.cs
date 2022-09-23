// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.CliApp.CommandLine;
using AppMotor.CliApp.CommandLine.Hosting;
using AppMotor.CliApp.HttpServer.TestUtils;
using AppMotor.Core.Net;
using AppMotor.Core.Net.Http;
using AppMotor.HttpServer;
using AppMotor.HttpServer.Startups;
using AppMotor.TestCore;
using AppMotor.TestCore.Logging;
using AppMotor.TestCore.Networking;
using AppMotor.TestCore.Utils;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

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

        await TestTimeout.TimeoutAfter(appTask, TimeSpan.FromSeconds(10));
    }

    private sealed class TestHttpServerCommand : HttpServerCommandBase
    {
        private readonly int _testPort;

        private readonly DefaultHostBuilderFactory _hostBuilderFactory;

        public TestHttpServerCommand(int testPort, ITestOutputHelper testOutputHelper)
        {
            this._testPort = testPort;

            this._hostBuilderFactory = new DefaultHostBuilderFactory()
            {
                LoggingConfigurationProvider = (_, builder) =>
                {
                    builder.AddXUnitLogger(testOutputHelper);
                },
            };
        }

        /// <inheritdoc />
        protected override IHostBuilder CreateHostBuilder()
        {
            return this._hostBuilderFactory.CreateHostBuilder();
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

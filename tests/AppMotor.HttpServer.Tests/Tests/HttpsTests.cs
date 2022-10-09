// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.CliApp.AppBuilding;
using AppMotor.CliApp.CommandLine;
using AppMotor.CliApp.HttpServer.TestUtils;
using AppMotor.Core.Certificates;
using AppMotor.Core.Net;
using AppMotor.Core.Net.Http;
using AppMotor.HttpServer;
using AppMotor.HttpServer.Startups;
using AppMotor.TestCore;
using AppMotor.TestCore.AppBuilding;
using AppMotor.TestCore.Networking;
using AppMotor.TestCore.Utils;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

using Shouldly;

using Xunit;
using Xunit.Abstractions;

namespace AppMotor.CliApp.HttpServer.Tests;

public sealed class HttpsTests : TestBase
{
    private const string SERVER_HOSTNAME = "localhost";

    public HttpsTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    [Fact]
    public async Task TestHttpsApiCall()
    {
        int testPort = ServerPortProvider.GetNextTestPort();

        using var testCertificate = TlsCertificate.CreateSelfSigned(SERVER_HOSTNAME, TimeSpan.FromDays(1));

        using var cts = new CancellationTokenSource();

        var app = new CliApplicationWithCommand(new TestServerCommand(testPort, testCertificate, this.TestConsole));
        Task appTask = app.RunAsync(cts.Token);

        using (var httpClient = HttpClientFactory.CreateHttpClient(serverCertificate: testCertificate))
        {
            // ReSharper disable once MethodSupportsCancellation
            var response = await httpClient.GetAsync($"https://{SERVER_HOSTNAME}:{testPort}/api/ping");

            response.EnsureSuccessStatusCode();

            // ReSharper disable once MethodSupportsCancellation
            var responseString = await response.Content.ReadAsStringAsync();

            responseString.ShouldBe("Hello World!");
        }

        cts.Cancel();

        await TestTimeout.TimeoutAfter(appTask, TimeSpan.FromSeconds(10));
    }

    private sealed class TestServerCommand : HttpServerCommandBase
    {
        private readonly int _port;

        private readonly TlsCertificate _testCertificate;

        private readonly DefaultHostBuilderFactory _hostBuilderFactory;

        public TestServerCommand(int port, TlsCertificate testCertificate, ITestOutputHelper testOutputHelper)
        {
            this._port = port;
            this._testCertificate = testCertificate;

            this._hostBuilderFactory = new XUnitHostBuilderFactory(testOutputHelper);
        }

        /// <inheritdoc />
        protected override IHostBuilder CreateHostBuilder()
        {
            return this._hostBuilderFactory.CreateHostBuilder();
        }

        /// <inheritdoc />
        protected override IEnumerable<HttpServerPort> GetServerPorts(IServiceProvider serviceProvider)
        {
            yield return new HttpsServerPort(
                SocketListenAddresses.Loopback,
                port: this._port,
                () => this._testCertificate,
                certificateProviderCallerOwnsCertificates: false
            );
        }

        /// <inheritdoc />
        protected override IAspNetStartup CreateStartupClass(WebHostBuilderContext context)
        {
            return new SimplePingStartup();
        }
    }
}

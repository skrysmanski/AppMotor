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

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using AppMotor.CliApp.CommandLine;
using AppMotor.CliApp.CommandLine.Hosting;
using AppMotor.CliApp.HttpServer.TestUtils;
using AppMotor.Core.Certificates;
using AppMotor.Core.Net;
using AppMotor.Core.Net.Http;
using AppMotor.Core.TestUtils;
using AppMotor.HttpServer;

using Microsoft.AspNetCore.Hosting;

using Shouldly;

using Xunit;
using Xunit.Abstractions;

namespace AppMotor.CliApp.HttpServer.Tests
{
    public sealed class HttpsTests
    {
        private const string SERVER_HOSTNAME = "localhost";

        private readonly ITestOutputHelper _testOutputHelper;

        public HttpsTests(ITestOutputHelper testOutputHelper)
        {
            this._testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task TestHttpsApiCall()
        {
            int testPort = ServerPortProvider.GetTestPort();

            using var testCertificate = TlsCertificate.CreateSelfSigned(SERVER_HOSTNAME, TimeSpan.FromDays(1));

            using var cts = new CancellationTokenSource();

            var app = new CliApplicationWithCommand(new TestServerCommand(testPort, testCertificate, this._testOutputHelper));
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

            appTask.ShouldFinishWithin(TimeSpan.FromSeconds(10));
        }

        private sealed class TestServerCommand : HttpServerCommandBase
        {
            private readonly int _port;

            private readonly TlsCertificate _testCertificate;

            /// <inheritdoc />
            protected override IHostBuilderFactory HostBuilderFactory { get; }

            public TestServerCommand(int port, TlsCertificate testCertificate, ITestOutputHelper testOutputHelper)
            {
                this._port = port;
                this._testCertificate = testCertificate;

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
                yield return new HttpsServerPort(
                    SocketListenAddresses.Loopback,
                    port: this._port,
                    () => this._testCertificate,
                    certificateProviderCallerOwnsCertificates: false
                );
            }

            /// <inheritdoc />
            protected override object CreateStartupClass(WebHostBuilderContext context)
            {
                return new SimplePingStartup();
            }
        }
    }
}

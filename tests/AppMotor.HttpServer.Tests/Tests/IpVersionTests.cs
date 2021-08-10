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
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

using AppMotor.CliApp.HttpServer.TestUtils;
using AppMotor.Core.Exceptions;
using AppMotor.Core.Net;
using AppMotor.Core.Net.Http;
using AppMotor.Core.TestUtils;
using AppMotor.HttpServer;

using JetBrains.Annotations;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

using Shouldly;

using Xunit;
using Xunit.Abstractions;

namespace AppMotor.CliApp.HttpServer.Tests
{
    /// <summary>
    /// These tests test that serving from and connecting to the various combinations of <see cref="SocketListenAddresses"/>
    /// and <see cref="IPVersions"/> works.
    /// </summary>
    public sealed class IpVersionTests
    {
        private static readonly Lazy<string> s_ownIPv4Address = new(() => GetLocalIpAddress(IPVersions.IPv4));

        private static readonly Lazy<string> s_ownIPv6Address = new(() => GetLocalIpAddress(IPVersions.IPv6));

        private readonly ITestOutputHelper _testOutputHelper;

        public IpVersionTests(ITestOutputHelper testOutputHelper)
        {
            this._testOutputHelper = testOutputHelper;
        }

        [Theory]
        [InlineData(SocketListenAddresses.Loopback, IPVersions.IPv4)]
        [InlineData(SocketListenAddresses.Loopback, IPVersions.IPv6)]
        [InlineData(SocketListenAddresses.Loopback, IPVersions.DualStack)]
        [InlineData(SocketListenAddresses.Any, IPVersions.IPv4)]
        [InlineData(SocketListenAddresses.Any, IPVersions.IPv6)]
        [InlineData(SocketListenAddresses.Any, IPVersions.DualStack)]
        public async Task TestConnection(SocketListenAddresses listenAddress, IPVersions ipVersion)
        {
            int testPort = ServerPortProvider.GetTestPort();

            using var cts = new CancellationTokenSource();

            var app = new HttpServerApplication(
                new HttpServerPort(listenAddress, testPort)
                {
                    IPVersion = ipVersion,
                },
                new Startup()
            );
            Task appTask = app.RunAsync(cts.Token);

            using (var httpClient = HttpClientFactory.CreateHttpClient())
            {
                switch (ipVersion)
                {
                    case IPVersions.IPv4:
                        await ExecuteRequest(httpClient, "127.0.0.1", testPort);
                        if (listenAddress == SocketListenAddresses.Any)
                        {
                            await ExecuteRequest(httpClient, s_ownIPv4Address.Value, testPort);
                        }
                        break;

                    case IPVersions.IPv6:
                        await ExecuteRequest(httpClient, "[::1]", testPort);
                        if (listenAddress == SocketListenAddresses.Any)
                        {
                            await ExecuteRequest(httpClient, $"[{s_ownIPv6Address.Value}]", testPort);
                        }
                        break;

                    case IPVersions.DualStack:
                        await ExecuteRequest(httpClient, "127.0.0.1", testPort);
                        await ExecuteRequest(httpClient, "[::1]", testPort);
                        if (listenAddress == SocketListenAddresses.Any)
                        {
                            await ExecuteRequest(httpClient, s_ownIPv4Address.Value, testPort);
                            await ExecuteRequest(httpClient, $"[{s_ownIPv6Address.Value}]", testPort);
                        }
                        break;

                    default:
                        throw new UnexpectedSwitchValueException(nameof(ipVersion), ipVersion);
                }
            }

            cts.Cancel();

            await appTask.ShouldFinishWithin(TimeSpan.FromSeconds(10));
        }

        private async Task ExecuteRequest(HttpClient httpClient, string hostIpAddress, int testPort)
        {
            this._testOutputHelper.WriteLine($"Running query against: {hostIpAddress}");

            var response = await httpClient.GetAsync($"http://{hostIpAddress}:{testPort}/api/ping");

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            if (hostIpAddress.StartsWith('['))
            {
                // IPv6
                responseString.ShouldBe($"Caller ip address family: {AddressFamily.InterNetworkV6}");
            }
            else
            {
                // IPv4
                responseString.ShouldBe($"Caller ip address family: {AddressFamily.InterNetwork}");
            }
        }

        [MustUseReturnValue]
        private static string GetLocalIpAddress(IPVersions ipVersion)
        {
            // Code via: https://stackoverflow.com/a/27376368/614177

            using Socket socket = new(ipVersion == IPVersions.IPv4 ? AddressFamily.InterNetwork : AddressFamily.InterNetworkV6, SocketType.Dgram, 0);

            if (ipVersion == IPVersions.IPv4)
            {
                socket.Connect("8.8.8.8", 65530);
            }
            else
            {
                socket.Connect("2001:4860:4860::8888", 65530);
            }

            IPEndPoint endPoint = (IPEndPoint)socket.LocalEndPoint!;

            endPoint.Address.ShouldNotBe(IPAddress.IPv6Loopback);

            var address = endPoint.Address.ToString();
            address.ShouldNotStartWith("127.");

            return address;
        }

        private sealed class Startup
        {
            public void Configure(IApplicationBuilder app)
            {
                // Enable routing feature; required for defining endpoints below.
                // See: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/routing#routing-basics
                app.UseRouting();

                // Define endpoints (invokable actions). Requires call to "UseRouting()" above.
                // See: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/routing#endpoint
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapGet("/api/ping", async context =>
                    {
                        IPAddress? callerIpAddress = context.Request.HttpContext.Connection.RemoteIpAddress;
                        callerIpAddress.ShouldNotBeNull();

                        var addressFamily = callerIpAddress.IsIPv4MappedToIPv6 ? AddressFamily.InterNetwork : callerIpAddress.AddressFamily;

                        await context.Response.WriteAsync($"Caller ip address family: {addressFamily}");
                    });
                });
            }
        }
    }
}

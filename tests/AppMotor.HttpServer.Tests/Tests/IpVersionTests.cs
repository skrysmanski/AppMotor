// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

using AppMotor.CliApp.CommandLine;
using AppMotor.CliApp.CommandLine.Hosting;
using AppMotor.Core.Exceptions;
using AppMotor.Core.Logging;
using AppMotor.Core.Net;
using AppMotor.Core.Net.Http;
using AppMotor.HttpServer;
using AppMotor.HttpServer.Startups;
using AppMotor.TestCore;
using AppMotor.TestCore.Extensions;
using AppMotor.TestCore.Logging;
using AppMotor.TestCore.Networking;

using JetBrains.Annotations;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Shouldly;

using Xunit;
using Xunit.Abstractions;

namespace AppMotor.CliApp.HttpServer.Tests;

/// <summary>
/// These tests test that serving from and connecting to the various combinations of <see cref="SocketListenAddresses"/>
/// and <see cref="IPVersions"/> works.
/// </summary>
public sealed class IpVersionTests : TestBase
{
    private static readonly Lazy<string> s_ownIPv4Address = new(() => GetLocalIpAddress(IPVersions.IPv4));

    private static readonly Lazy<string> s_ownIPv6Address = new(() => GetLocalIpAddress(IPVersions.IPv6));

    public IpVersionTests(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
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
        int testPort = ServerPortProvider.GetNextTestPort();

        using var cts = new CancellationTokenSource();

        var serverPort = new HttpServerPort(listenAddress, testPort)
        {
            IPVersion = ipVersion,
        };
        var app = new CliApplicationWithCommand(new TestHttpServerCommand(serverPort, this.TestConsole));
        Task appTask = app.RunAsync(cts.Token);

        try
        {
            using var httpClient = HttpClientFactory.CreateHttpClient();

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
        finally
        {
            this.TestConsole.WriteLine();

            cts.Cancel();

            if (TestEnvInfo.RunsInCiPipeline)
            {
                await appTask.OrTimeoutAfter(TimeSpan.FromSeconds(10));
            }
            else
            {
                await appTask;
            }
        }
    }

    private async Task ExecuteRequest(HttpClient httpClient, string hostIpAddress, int testPort)
    {
        this.TestConsole.WriteLine();
        this.TestConsole.WriteLine($"Running query against: {hostIpAddress}");

        var requestUri = new Uri($"http://{hostIpAddress}:{testPort}/api/ping");

        var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);

        if (hostIpAddress.StartsWith('['))
        {
            // IPv6 address
            // Fix for bug in HttpClient: https://github.com/dotnet/runtime/issues/59341
            requestMessage.Headers.Host = requestUri.Authority;
        }

        HttpResponseMessage response;
        try
        {
            response = await httpClient.SendAsync(requestMessage);
        }
        catch (Exception ex)
        {
            this.TestConsole.WriteLine(ex.ToStringExtended());
            throw;
        }

        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (Exception)
        {
            var errorResponseString = await response.Content.ReadAsStringAsync();
            this.TestConsole.WriteLine("[START ERROR RESPONSE]");
            this.TestConsole.WriteLine(errorResponseString);
            this.TestConsole.WriteLine("[END ERROR RESPONSE]");
            throw;
        }

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

        this.TestConsole.WriteLine("Done");
    }

    [MustUseReturnValue]
    private static string GetLocalIpAddress(IPVersions ipVersion)
    {
        if (ipVersion == IPVersions.IPv4)
        {
            // Code via: https://stackoverflow.com/a/27376368/614177
            //
            // NOTE: This code also works with IPv6 but there's no IPv6 network in
            //   GitHub actions. So we'll have to use a different approach.
            //   See: https://github.com/actions/virtual-environments/issues/668

            using Socket socket = new(AddressFamily.InterNetwork, SocketType.Dgram, 0);

            socket.Connect("8.8.8.8", 65530);

            IPEndPoint endPoint = (IPEndPoint)socket.LocalEndPoint!;

            IPAddress.IsLoopback(endPoint.Address).ShouldBe(false);

            return endPoint.Address.ToString();
        }
        else if (ipVersion == IPVersions.IPv6)
        {
            foreach (var networkInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (networkInterface.OperationalStatus != OperationalStatus.Up || !networkInterface.Supports(NetworkInterfaceComponent.IPv6))
                {
                    continue;
                }

                var ipProperties = networkInterface.GetIPProperties();

                if (ipProperties.GatewayAddresses.Count == 0)
                {
                    continue;
                }

                foreach (var ip in ipProperties.UnicastAddresses)
                {
                    if (ip.Address.AddressFamily != AddressFamily.InterNetworkV6 || IPAddress.IsLoopback(ip.Address))
                    {
                        continue;
                    }

                    return ip.Address.ToString();
                }
            }

            throw new InvalidOperationException("No usable IPv6 network interface exists.");
        }
        else
        {
            throw new NotSupportedException($"Unsupported ipVersion: {ipVersion}");
        }
    }

    private sealed class TestHttpServerCommand : HttpServerCommandBase
    {
        private readonly HttpServerPort _testPort;

        /// <inheritdoc />
        protected override IHostBuilderFactory HostBuilderFactory { get; }

        public TestHttpServerCommand(HttpServerPort testPort, ITestOutputHelper testOutputHelper)
        {
            this._testPort = testPort;

            this.HostBuilderFactory = new DefaultHostBuilderFactory()
            {
                LogLevelConfiguration = new LogLevelConfiguration(defaultLogLevel: LogLevel.Debug),

                LoggingConfigurationProvider = (_, builder) =>
                {
                    builder.AddXUnitLogger(testOutputHelper);
                },
            };
        }

        /// <inheritdoc />
        protected override IEnumerable<HttpServerPort> GetServerPorts(IServiceProvider serviceProvider)
        {
            yield return this._testPort;
        }

        /// <inheritdoc />
        protected override IAspNetStartup CreateStartupClass(WebHostBuilderContext context)
        {
            return new Startup();
        }
    }

    private sealed class Startup : IAspNetStartup
    {
        /// <inheritdoc />
        public void ConfigureServices(IServiceCollection services)
        {
            // Nothing to do
        }

        /// <inheritdoc />
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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

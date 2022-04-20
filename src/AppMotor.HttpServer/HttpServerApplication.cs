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

using System.Reflection;

using AppMotor.CliApp.CommandLine;
using AppMotor.Core.Net;

using JetBrains.Annotations;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AppMotor.HttpServer;

/// <summary>
/// A very simple HTTP server reachable only on a single HTTP port. Enables ASP.NET Core MVC (with Razor views).
/// </summary>
/// <remarks>
/// The primary goal of this class is to make the on-boarding process as easy as possible. If you
/// need more flexibility, you need to create your own instance of <see cref="CliApplicationWithCommand"/>
/// and implement your own server command (inheriting from <see cref="HttpServerCommandBase"/>).
/// </remarks>
public class HttpServerApplication : CliApplicationWithCommand
{
    /// <summary>
    /// This collection can be used to register additional services into the application's dependency injection system.
    /// </summary>
    public IServiceCollection Services { get; }

    /// <summary>
    /// Creates an HTTP server application with the specified HTTP port.
    /// </summary>
    /// <param name="port">The HTTP port to use (will be bound to <see cref="SocketListenAddresses.Loopback"/>)</param>
    /// <param name="startupClass">The ASP.NET Core Startup class to use. If <c>null</c>,
    /// <see cref="MvcStartup"/> will be used.</param>
    [PublicAPI]
    public HttpServerApplication(int port, object? startupClass = null)
        : this(port, SocketListenAddresses.Loopback, startupClass)
    {
    }

    /// <summary>
    /// Creates an HTTP server application with the specified HTTP port.
    /// </summary>
    /// <param name="port">The HTTP port to use</param>
    /// <param name="listenAddresses">Whether <paramref name="port"/> should be reachable only from the local machine
    /// (<see cref="SocketListenAddresses.Loopback"/>) or from anywhere on the network (<see cref="SocketListenAddresses.Any"/>).</param>
    /// <param name="startupClass">The ASP.NET Core Startup class to use. If <c>null</c>,
    /// <see cref="MvcStartup"/> will be used.</param>
    [PublicAPI]
    public HttpServerApplication(int port, SocketListenAddresses listenAddresses, object? startupClass = null)
        : this(new HttpServerPort(listenAddresses, port), startupClass)
    {
    }

    /// <summary>
    /// Creates an HTTP server application with the specified HTTP port.
    /// </summary>
    /// <param name="port">The HTTP port to use</param>
    /// <param name="startupClass">The ASP.NET Core Startup class to use. If <c>null</c>,
    /// <see cref="MvcStartup"/> will be used.</param>
    [PublicAPI]
    public HttpServerApplication(HttpServerPort port, object? startupClass = null)
        : this(new HttpServerCommand(port, startupClass))
    {
    }

    private HttpServerApplication(HttpServerCommand httpServerCommand)
        : base(httpServerCommand)
    {
        this.Services = httpServerCommand.AppServiceCollection;
    }

    /// <summary>
    /// Runs an HTTP server at the specified port.
    /// </summary>
    /// <param name="port">The HTTP port to use (will be bound to <see cref="SocketListenAddresses.Loopback"/>)</param>
    /// <param name="cancellationToken">A cancellation token to stop the HTTP server application.</param>
    /// <returns>The exit code to use.</returns>
    [PublicAPI]
    public static int Run(int port, CancellationToken cancellationToken = default)
    {
        var app = new HttpServerApplication(port);
        return app.Run(cancellationToken);
    }

    /// <summary>
    /// Runs an HTTP server at the specified port.
    /// </summary>
    /// <param name="port">The HTTP port to use</param>
    /// <param name="listenAddresses">Whether <paramref name="port"/> should be reachable only from the local machine
    /// (<see cref="SocketListenAddresses.Loopback"/>) or from anywhere on the network (<see cref="SocketListenAddresses.Any"/>).</param>
    /// <param name="cancellationToken">A cancellation token to stop the HTTP server application.</param>
    /// <returns>The exit code to use.</returns>
    [PublicAPI]
    public static int Run(int port, SocketListenAddresses listenAddresses, CancellationToken cancellationToken = default)
    {
        var app = new HttpServerApplication(port, listenAddresses);
        return app.Run(cancellationToken);
    }

    /// <summary>
    /// Runs an HTTP server at the specified port.
    /// </summary>
    /// <param name="port">The HTTP port to use (will be bound to <see cref="SocketListenAddresses.Loopback"/>)</param>
    /// <param name="cancellationToken">A cancellation token to stop the HTTP server application.</param>
    /// <returns>The exit code to use.</returns>
    [PublicAPI]
    public static Task<int> RunAsync(int port, CancellationToken cancellationToken = default)
    {
        var app = new HttpServerApplication(port);
        return app.RunAsync(cancellationToken);
    }

    /// <summary>
    /// Runs an HTTP server at the specified port.
    /// </summary>
    /// <param name="port">The HTTP port to use</param>
    /// <param name="listenAddresses">Whether <paramref name="port"/> should be reachable only from the local machine
    /// (<see cref="SocketListenAddresses.Loopback"/>) or from anywhere on the network (<see cref="SocketListenAddresses.Any"/>).</param>
    /// <param name="cancellationToken">A cancellation token to stop the HTTP server application.</param>
    /// <returns>The exit code to use.</returns>
    [PublicAPI]
    public static Task<int> RunAsync(int port, SocketListenAddresses listenAddresses, CancellationToken cancellationToken = default)
    {
        var app = new HttpServerApplication(port, listenAddresses);
        return app.RunAsync(cancellationToken);
    }

    private sealed class HttpServerCommand : HttpServerCommandBase
    {
        private readonly HttpServerPort _httpPort;

        private readonly object? _startupClass;

        public IServiceCollection AppServiceCollection { get; } = new ServiceCollection();

        public HttpServerCommand(HttpServerPort httpPort, object? startupClass)
        {
            this._httpPort = httpPort;
            this._startupClass = startupClass;
        }

        /// <inheritdoc />
        protected override void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            base.ConfigureServices(context, services);

            foreach (var serviceDescriptor in this.AppServiceCollection)
            {
                services.Add(serviceDescriptor);
            }
        }

        /// <inheritdoc />
        protected override object CreateStartupClass(WebHostBuilderContext context)
        {
            if (this._startupClass is not null)
            {
                return this._startupClass;
            }

            var entryAssembly = Assembly.GetEntryAssembly() ?? throw new InvalidOperationException("Could not determine main assembly.");
            return new MvcStartup(entryAssembly);
        }

        /// <inheritdoc />
        protected override IEnumerable<HttpServerPort> GetServerPorts(IServiceProvider serviceProvider)
        {
            yield return this._httpPort;
        }
    }
}

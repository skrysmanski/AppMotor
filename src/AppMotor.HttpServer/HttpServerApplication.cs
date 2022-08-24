// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Reflection;

using AppMotor.CliApp.CommandLine;
using AppMotor.Core.Net;
using AppMotor.HttpServer.Startups;

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
    public HttpServerApplication(int port, IAspNetStartup? startupClass = null)
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
    public HttpServerApplication(int port, SocketListenAddresses listenAddresses, IAspNetStartup? startupClass = null)
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
    public HttpServerApplication(HttpServerPort port, IAspNetStartup? startupClass = null)
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

        private readonly IAspNetStartup? _startupClass;

        public IServiceCollection AppServiceCollection { get; } = new ServiceCollection();

        public HttpServerCommand(HttpServerPort httpPort, IAspNetStartup? startupClass)
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
        protected override IAspNetStartup CreateStartupClass(WebHostBuilderContext context)
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

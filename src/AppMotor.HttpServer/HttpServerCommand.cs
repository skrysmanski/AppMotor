using System;
using System.Collections.Generic;
using System.Net;

using AppMotor.CliApp.CommandLine;
using AppMotor.CliApp.Hosting;
using AppMotor.Core.Exceptions;
using AppMotor.Core.Net;

using JetBrains.Annotations;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AppMotor.HttpServer
{
    /// <summary>
    /// A <see cref="CliCommand"/> for running an HTTP(S) server (Kestrel). You can use it as root
    /// command with <see cref="CliApplicationWithCommand"/> or as a verb with <see cref="CliApplicationWithVerbs"/>.
    /// See <see cref="CliCommand"/> for more details.
    /// </summary>
    [PublicAPI]
    public abstract class HttpServerCommand : CliCommandWithGenericHost
    {
        /// <summary>
        /// The <c>Startup</c> class to use. See https://docs.microsoft.com/en-us/aspnet/core/fundamentals/startup
        /// for more details.
        ///
        /// <para>Note: This type must be a class.</para>
        /// </summary>
        protected abstract Type StartupClass { get; }

        /// <summary>
        /// Returns the server port definitions this HTTP service should listen on.
        /// </summary>
        /// <param name="serviceProvider">The dependency injection service provider.</param>
        protected abstract IEnumerable<HttpServerPort> GetServerPorts(IServiceProvider serviceProvider);

        /// <inheritdoc />
        protected sealed override void SetupApplication(IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureWebHostDefaults(webBuilder => // Create the HTTP host
            {
                // Clear any "pre-defined" list of URLs (otherwise there will be a warning when
                // this app runs).
                webBuilder.UseUrls("");

                // Configure Kestrel.
                webBuilder.UseKestrel(ConfigureKestrel);

                // Use our "Startup" class for any further configuration.
                webBuilder.UseStartup(this.StartupClass);
            });
        }

        private void ConfigureKestrel(KestrelServerOptions options)
        {
            var logger = options.ApplicationServices.GetRequiredService<ILogger<HttpServerCommand>>();

            foreach (var serverPort in GetServerPorts(options.ApplicationServices))
            {
                Action<ListenOptions> configure;

                if (serverPort is HttpsServerPort httpsServerPort)
                {
                    var certificate = httpsServerPort.CertificateProvider();

                    logger.LogInformation("Using certificate '{thumbprint}' for server port {port}.", certificate.Thumbprint, httpsServerPort.Port);

                    configure = listenOptions =>
                    {
                        listenOptions.UseHttps(certificate);
                    };
                }
                else
                {
                    configure = _ => { };
                }

                if (serverPort.IPVersion == IPVersions.IPv4 || serverPort.IPVersion == IPVersions.DualStack)
                {
                    switch (serverPort.ListenAddress)
                    {
                        case SocketListenAddresses.Any:
                            options.Listen(IPAddress.Any, serverPort.Port, configure);
                            break;
                        case SocketListenAddresses.Loopback:
                            options.Listen(IPAddress.Loopback, serverPort.Port, configure);
                            break;
                        default:
                            throw new UnexpectedSwitchValueException(nameof(serverPort.ListenAddress), serverPort.ListenAddress);
                    }
                }
                if (serverPort.IPVersion == IPVersions.IPv6 || serverPort.IPVersion == IPVersions.DualStack)
                {
                    switch (serverPort.ListenAddress)
                    {
                        case SocketListenAddresses.Any:
                            options.Listen(IPAddress.IPv6Any, serverPort.Port, configure);
                            break;
                        case SocketListenAddresses.Loopback:
                            options.Listen(IPAddress.IPv6Loopback, serverPort.Port, configure);
                            break;
                        default:
                            throw new UnexpectedSwitchValueException(nameof(serverPort.ListenAddress), serverPort.ListenAddress);
                    }
                }
            }
        }
    }
}

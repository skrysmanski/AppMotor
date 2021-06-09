using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography.X509Certificates;

using AppMotor.CliApp.CommandLine;
using AppMotor.CliApp.Hosting;
using AppMotor.Core.Certificates;
using AppMotor.Core.Exceptions;
using AppMotor.Core.Net;
using AppMotor.Core.Utils;

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
    public abstract class HttpServerCommandBase : CliCommandWithGenericHost
    {
        /// <summary>
        /// Returns the server port definitions this HTTP service should listen on.
        /// </summary>
        /// <param name="serviceProvider">The dependency injection service provider.</param>
        protected abstract IEnumerable<HttpServerPort> GetServerPorts(IServiceProvider serviceProvider);

        /// <summary>
        /// Creates the <c>Startup</c> class instance to use. See https://docs.microsoft.com/en-us/aspnet/core/fundamentals/startup
        /// for more details.
        ///
        /// <para>The default implementation creates an instance of <see cref="MvcStartup"/>.</para>
        /// </summary>
        protected virtual object CreateStartupClass(WebHostBuilderContext context)
        {
            return new MvcStartup(GetType().Assembly);
        }

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
                webBuilder.UseStartup(CreateStartupClass);
            });
        }

        private void ConfigureKestrel(KestrelServerOptions options)
        {
            var logger = options.ApplicationServices.GetRequiredService<ILogger<HttpServerCommandBase>>();

            options.ConfigureHttpsDefaults(configureOptions =>
            {
                configureOptions.SslProtocols = TlsSettings.EnabledTlsProtocols;
            });

            foreach (var serverPort in GetServerPorts(options.ApplicationServices))
            {
                Action<ListenOptions> configure;

                if (serverPort is HttpsServerPort httpsServerPort)
                {
                    var certificate = httpsServerPort.CertificateProvider();

                    if (OperatingSystem.IsWindows())
                    {
                        //
                        // Workaround for error "No credentials are available in the security package".
                        //
                        // Basically, the problem is that on Windows TLS is handled out-of-process. But
                        // if the private key for certificate only in-memory of the current process,
                        // the out-of-process TLS handler is unable to get the private key (see
                        // https://github.com/dotnet/runtime/issues/23749#issuecomment-485947319 )
                        //
                        // Full discussion: https://github.com/dotnet/runtime/issues/23749
                        //
                        // Workaround: https://github.com/dotnet/runtime/issues/23749#issuecomment-739895373
                        //
                        var originalCertificate = certificate;

                        byte[] exportedCertificateBytes = ((X509Certificate2)originalCertificate).Export(X509ContentType.Pkcs12);
#pragma warning disable CA2000 // Dispose objects before losing scope
                        var reimportedCertificate = new X509Certificate2(exportedCertificateBytes, password: (string?)null, X509KeyStorageFlags.Exportable);
                        certificate = new TlsCertificate(reimportedCertificate, allowPrivateKeyExport: true);
#pragma warning restore CA2000 // Dispose objects before losing scope

                        if (httpsServerPort.CertificateProviderCallerOwnsCertificates)
                        {
                            originalCertificate.Dispose();
                        }
                    }

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

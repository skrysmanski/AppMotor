using System;
using System.Collections.Generic;

using AppMotor.CliApp.CommandLine;
using AppMotor.Core.Certificates;
using AppMotor.Core.Net;
using AppMotor.HttpServer.Sample.Services;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace AppMotor.HttpServer.Sample
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            var app = new CliApplicationWithCommand(new SampleServerCommand());
            return app.Run(args);
        }

        private sealed class SampleServerCommand : HttpServerCommand
        {
            private const int HTTPS_PORT = 1235;

            /// <inheritdoc />
            protected override void RegisterServices(IServiceCollection services)
            {
                base.RegisterServices(services);

                services.AddSingleton(new ServiceSettingsProvider(httpsPort: HTTPS_PORT));
            }

            /// <inheritdoc />
            protected override IEnumerable<HttpServerPort> GetServerPorts(IServiceProvider serviceProvider)
            {
                yield return new HttpServerPort(SocketListenAddresses.Loopback, port: 1234);

                yield return new HttpsServerPort(
                    SocketListenAddresses.Loopback,
                    port: HTTPS_PORT,
                    certificateProvider: () => TlsCertificate.CreateSelfSigned(Environment.MachineName, TimeSpan.FromDays(90))
                );
            }
        }
    }
}

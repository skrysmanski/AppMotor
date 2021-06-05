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
using System.Reflection;

using AppMotor.CliApp.CommandLine;
using AppMotor.Core.Net;

using JetBrains.Annotations;

using Microsoft.AspNetCore.Hosting;

namespace AppMotor.HttpServer
{
    /// <summary>
    /// A very simple HTTP server reachable only on a single HTTP port.
    /// </summary>
    /// <remarks>
    /// The primary goal of this class is to make the on-boarding process as easy as possible. If you
    /// need more flexibility, you need to write your own application (inheriting from <see cref="CliApplicationWithCommand"/>)
    /// and server command (inheriting from <see cref="HttpServerCommandBase"/>).
    /// </remarks>
    public class HttpServerApplication : CliApplicationWithCommand
    {
        /// <summary>
        /// Creates an HTTP server application with the specified HTTP port.
        /// </summary>
        /// <param name="httpPort">The HTTP port to use</param>
        [PublicAPI]
        public HttpServerApplication(HttpServerPort httpPort)
            // NOTE: For security reasons we only bind to localhost. If the user wants to bind
            //   to all IP addresses they simply need to use the other constructor.
            : base(new HttpServerCommand(httpPort))
        {
        }

        /// <summary>
        /// Runs an HTTP server at the specified port.
        /// </summary>
        /// <param name="port">The HTTP port to use</param>
        /// <param name="bindToLoopbackOnly">Whether to bind <paramref name="port"/> to the loopback adapter only
        /// (i.e. reachable only from this machine itself).</param>
        /// <returns>The exit code to use.</returns>
        public static int Run(int port, bool bindToLoopbackOnly = true)
        {
            var serverPort = new HttpServerPort(bindToLoopbackOnly ? SocketListenAddresses.Loopback : SocketListenAddresses.Any, port);
            var app = new HttpServerApplication(serverPort);
            return app.Run();
        }

        private sealed class HttpServerCommand : HttpServerCommandBase
        {
            private readonly HttpServerPort _httpPort;

            public HttpServerCommand(HttpServerPort httpPort)
            {
                this._httpPort = httpPort;
            }

            /// <inheritdoc />
            protected override object CreateStartupClass(WebHostBuilderContext context)
            {
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
}

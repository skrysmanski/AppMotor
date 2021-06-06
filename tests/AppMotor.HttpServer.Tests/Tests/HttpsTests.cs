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
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using AppMotor.CliApp.CommandLine;
using AppMotor.Core.Extensions;
using AppMotor.Core.Net;
using AppMotor.HttpServer;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

using Shouldly;

using Xunit;

namespace AppMotor.CliApp.HttpServer.Tests
{
    public sealed class HttpsTests
    {
        [Fact]
        public async Task TestApiCall()
        {
            using var cts = new CancellationTokenSource();

            var app = new CliApplicationWithCommand(new TestServerCommand());
            Task appTask = app.RunAsync(cts.Token);

            using (var httpClient = new HttpClient())
            {
                // ReSharper disable once MethodSupportsCancellation
                var response = await httpClient.GetAsync("http://localhost:1234/api/ping");

                response.EnsureSuccessStatusCode();

                // ReSharper disable once MethodSupportsCancellation
                var responseString = await response.Content.ReadAsStringAsync();

                responseString.ShouldBe("Hello World!");
            }

            cts.Cancel();
            // ReSharper disable once MethodSupportsCancellation
            await appTask.WithTimeout(TimeSpan.FromSeconds(10));
        }

        private sealed class TestServerCommand : HttpServerCommandBase
        {
            /// <inheritdoc />
            protected override object CreateStartupClass(WebHostBuilderContext context)
            {
                return new Startup();
            }

            /// <inheritdoc />
            protected override IEnumerable<HttpServerPort> GetServerPorts(IServiceProvider serviceProvider)
            {
                //var cert = TlsCertificate.CreateSelfSigned(Environment.MachineName, TimeSpan.FromDays(90));
                //yield return new HttpsServerPort(SocketListenAddresses.Loopback, port: 1234, () => cert);

                yield return new HttpServerPort(SocketListenAddresses.Loopback, port: 1234);
            }

            private sealed class Startup
            {
                /// <summary>
                /// This method gets called by the ASP.NET Core runtime. It registers services in the dependency injection
                /// system exposed via <paramref name="services"/>.
                /// </summary>
                /// <remarks>
                /// The name of this method is pre-defined and must not be changed.
                /// </remarks>
                public void ConfigureServices(IServiceCollection services)
                {
                }

                /// <summary>
                /// This method gets called by the ASP.NET Core runtime. It creates the ASP.NET Core Middleware
                /// pipeline.
                /// </summary>
                /// <remarks>
                /// You can request any registered service as parameter in this method. Parameters are provided
                /// by the dependency injection framework.
                ///
                /// <para>The name of this method is pre-defined and must not be changed.</para>
                /// </remarks>
                public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
                {
                    //
                    // Each "Use...()" method registers a middleware in the pipeline.
                    //
                    // Calls to "Map...()" methods create branches in the middleware pipeline; see:
                    // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/#branch-the-middleware-pipeline
                    //
                    // For a full overview, see: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/
                    //
                    // IMPORTANT: The order of the "Use...()" method calls is important as it defines
                    //   the order of the middleware components in the pipeline!
                    //

                    app.UseDeveloperExceptionPage();

                    // Enable routing feature; required for defining endpoints below.
                    // See: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/routing#routing-basics
                    app.UseRouting();

                    // Define endpoints (invokable actions). Requires call to "UseRouting()" above.
                    // See: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/routing#endpoint
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapGet("/api/ping", async context =>
                        {
                            await context.Response.WriteAsync("Hello World!");
                        });
                    });
                }
            }
        }
    }
}

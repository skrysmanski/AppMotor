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
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using AppMotor.Core.Extensions;
using AppMotor.Core.Net;
using AppMotor.HttpServer;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

using Shouldly;

using Xunit;

namespace AppMotor.CliApp.HttpServer.Tests
{
    public class HttpTests
    {
        private const int TEST_PORT = 1234;

        [Fact]
        public async Task TestApiCall()
        {
            using var cts = new CancellationTokenSource();

            var app = new HttpServerApplication(
                new HttpServerPort(SocketListenAddresses.Loopback, TEST_PORT),
                new Startup()
            );
            Task appTask = app.RunAsync(cts.Token);

            using (var httpClient = new HttpClient())
            {
                // ReSharper disable once MethodSupportsCancellation
                var response = await httpClient.GetAsync($"http://localhost:{TEST_PORT}/api/ping");

                response.EnsureSuccessStatusCode();

                // ReSharper disable once MethodSupportsCancellation
                var responseString = await response.Content.ReadAsStringAsync();

                responseString.ShouldBe("Hello World!");
            }

            cts.Cancel();
            // ReSharper disable once MethodSupportsCancellation
            await appTask.WithTimeout(TimeSpan.FromSeconds(10));
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
                        await context.Response.WriteAsync("Hello World!");
                    });
                });
            }
        }
    }
}

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

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace AppMotor.CliApp.HttpServer.TestUtils
{
    internal sealed class SimplePingStartup
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

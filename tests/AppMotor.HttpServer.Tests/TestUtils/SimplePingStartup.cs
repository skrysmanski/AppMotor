// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.HttpServer.Startups;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace AppMotor.CliApp.HttpServer.TestUtils;

internal sealed class SimplePingStartup : IAspNetStartup
{
    /// <inheritdoc />
    public void ConfigureServices(IServiceCollection services)
    {
        // Nothing to do.
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
                await context.Response.WriteAsync("Hello World!");
            });
        });
    }
}

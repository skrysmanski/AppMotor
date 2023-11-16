// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using JetBrains.Annotations;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace AppMotor.HttpServer.Startups;

/// <summary>
/// Represents a startup class for this ASP.NET Core application.
///
/// <para>See also: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/startup </para>
/// </summary>
public interface IAspNetStartup
{
    /// <summary>
    /// This method gets called by the ASP.NET Core runtime. It registers services in the dependency injection
    /// system exposed via <paramref name="services"/>.
    /// </summary>
    /// <remarks>
    /// The name of this method is pre-defined and must not be changed.
    /// </remarks>
    [UsedImplicitly]
    void ConfigureServices(IServiceCollection services);

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
    [UsedImplicitly]
    void Configure(IApplicationBuilder app, IWebHostEnvironment env);
}

// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Reflection;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AppMotor.HttpServer.Startups;

/// <summary>
/// Startup class for this ASP.NET Core web api application. The idea behind this class is to make bootstrapping
/// a web api application extremely easy by providing the most common defaults out-of-the-box.
/// </summary>
public class WebApiStartup : IAspNetStartup
{
    private readonly Assembly _mainAssembly;

    /// <summary>
    /// Constructor. Uses <see cref="Assembly.GetEntryAssembly"/> as main assembly (i.e. where controllers are located).
    /// </summary>
    public WebApiStartup()
        : this(Assembly.GetEntryAssembly() ?? throw new InvalidOperationException("Could not determine main assembly."))
    {
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="mainAssembly">The assembly where all the controllers are located.</param>
    public WebApiStartup(Assembly mainAssembly)
    {
        this._mainAssembly = mainAssembly;
    }

    /// <inheritdoc />
    public void ConfigureServices(IServiceCollection services)
    {
        var mvcBuilder = services.AddControllers();

        //
        // Enable controllers from the main assembly. (Otherwise, they won't be found.)
        //
        // See: https://docs.microsoft.com/en-us/aspnet/core/mvc/advanced/app-parts?#load-aspnet-core-features
        //
        mvcBuilder.AddApplicationPart(this._mainAssembly);
    }

    /// <inheritdoc />
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}

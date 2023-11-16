// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Reflection;

using JetBrains.Annotations;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace AppMotor.HttpServer.Startups;

/// <summary>
/// Startup class for this ASP.NET Core MVC application (with Razor views). The idea behind this
/// class is to make bootstrapping an MVC application extremely easy by providing the most common
/// defaults out-of-the-box.
/// </summary>
public class MvcStartup : IAspNetStartup
{
    private readonly Assembly _mainAssembly;

    /// <summary>
    /// The name of the default controller. This controller is used (together with <see cref="DefaultActionName"/>)
    /// when the user doesn't enter an URI path (e.g. just "http://localhost:1234").
    /// </summary>
    [PublicAPI]
    public string DefaultControllerName { get; init; } = "Home";

    /// <summary>
    /// The name of the default action (method) to use when the user doesn't specify an action in the URI (but
    /// just a controller name) - e.g. http://localhost:1234/Home/ .
    /// </summary>
    [PublicAPI]
    public string DefaultActionName { get; init; } = "Index";

    /// <summary>
    /// The path to the error page in case of unhandled exceptions. Must be in format <c>/Controller/Action</c>.
    /// </summary>
    [PublicAPI]
    public string ErrorPagePath { get; init; } = "/Home/Error";

    /// <summary>
    /// Constructor. Uses <see cref="Assembly.GetEntryAssembly"/> as main assembly (i.e. where controllers and views are located).
    /// </summary>
    public MvcStartup()
        : this(Assembly.GetEntryAssembly() ?? throw new InvalidOperationException("Could not determine main assembly."))
    {
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="mainAssembly">The assembly where all the controllers and views are located.</param>
    public MvcStartup(Assembly mainAssembly)
    {
        this._mainAssembly = mainAssembly;
    }

    /// <inheritdoc />
    public virtual void ConfigureServices(IServiceCollection services)
    {
        var mvcBuilder = services.AddControllersWithViews();

        //
        // Enable views and controllers from the main assembly. (Otherwise, they won't be found.)
        //
        // See: https://docs.microsoft.com/en-us/aspnet/core/mvc/advanced/app-parts?#load-aspnet-core-features
        //
        mvcBuilder.AddApplicationPart(this._mainAssembly);

        // Load the precompiled Razor views - if they exist.
        // NOTE: I could never figure out where this is done in ASP.NET Core. So this behavior may
        //   break in the future.
        string viewsAssemblyPath = Path.ChangeExtension(this._mainAssembly.Location, "Views.dll");
        if (File.Exists(viewsAssemblyPath))
        {
            var viewsAssembly = Assembly.LoadFrom(viewsAssemblyPath);
            mvcBuilder.AddApplicationPart(viewsAssembly);
        }
        else
        {
            // No precompiled Razor views exist. Enable runtime compilation instead.
            // NOTE: Runtime compiling Razor views takes a lot longer than using precompiled
            //   views - at least on the first call. This is why this is not enabled by default.
            mvcBuilder.AddRazorRuntimeCompilation();
        }

        // NOTE: This is taken from the example at the link above. Not sure what it does exactly.
        services.Configure<MvcRazorRuntimeCompilationOptions>(options =>
        {
            options.FileProviders.Add(new EmbeddedFileProvider(this._mainAssembly));
        });
    }

    /// <inheritdoc />
    public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            // TODO: Provide useful default implementation
            app.UseExceptionHandler(this.ErrorPagePath);
        }

        app.UseStaticFiles();

        // Enable routing feature; required for defining endpoints below.
        // See: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/routing#routing-basics
        app.UseRouting();

        app.UseAuthorization();

        // Define endpoints (invokable actions). Requires call to "UseRouting()" above.
        // See: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/routing#endpoint
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: $"{{controller={this.DefaultControllerName}}}/{{action={this.DefaultActionName}}}/{{id?}}"
            );
        });
    }
}

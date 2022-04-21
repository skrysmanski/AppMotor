#region License
// Copyright 2022 AppMotor Framework (https://github.com/skrysmanski/AppMotor)
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

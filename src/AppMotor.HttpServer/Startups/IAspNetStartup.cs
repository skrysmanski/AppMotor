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
[CLSCompliant(false)]
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
    [CLSCompliant(false)]
    void Configure(IApplicationBuilder app, IWebHostEnvironment env);
}

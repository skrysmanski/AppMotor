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
using System.Threading.Tasks;

using AppMotor.CliApp.CommandLine;

using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AppMotor.CliApp.Hosting
{
    /// <summary>
    /// Combines usage of <see cref="IHost"/>/<see cref="IHostBuilder"/> with <see cref="CliCommand"/>.
    ///
    /// <para>For more information, see https://docs.microsoft.com/en-us/dotnet/core/extensions/generic-host </para>
    ///
    /// <para>For ASP.NET, see also: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host </para>
    /// </summary>
    public abstract class CliCommandWithGenericHost : CliCommand
    {
        /// <inheritdoc />
        protected sealed override CliCommandExecutor Executor => new(Execute);

        private async Task Execute()
        {
            IHostBuilder hostBuilder = CreateHostBuilder();

            SetupServiceProvider(hostBuilder);

            hostBuilder.ConfigureServices(RegisterServices);

            SetupApplication(hostBuilder);

            IHost host = hostBuilder.Build();

            await host.RunAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Creates the <see cref="IHostBuilder"/> to be used to setup the application. The
        /// default implementation uses <see cref="Host.CreateDefaultBuilder()"/>.
        /// </summary>
        [PublicAPI, MustUseReturnValue]
        protected virtual IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder();
        }

        /// <summary>
        /// Sets the <see cref="IServiceProvider"/> (i.e. the dependency injection framework) to be used by
        /// the application. The default implementation uses the built-in service provider (via
        /// <see cref="HostingHostBuilderExtensions.UseDefaultServiceProvider(IHostBuilder,Action{ServiceProviderOptions})"/>)
        /// with scope validation enabled (see <see cref="ServiceProviderOptions.ValidateScopes"/>).
        /// </summary>
        /// <param name="hostBuilder"></param>
        [PublicAPI]
        protected virtual void SetupServiceProvider(IHostBuilder hostBuilder)
        {
            hostBuilder.UseDefaultServiceProvider(options =>
            {
                // Enable all validations (by default, they're only enabled in the development environment)
                options.ValidateScopes = true;
                options.ValidateOnBuild = true;
            });
        }

        /// <summary>
        /// Registers all services with the dependency injection framework (as created by <see cref="SetupServiceProvider"/>).
        ///
        /// <para>Note: <see cref="IHostedService"/> (registered via <see cref="ServiceCollectionHostedServiceExtensions.AddHostedService{THostedService}(IServiceCollection)"/>)
        /// are the primary way to run workloads in this application type (unless it's an ASP.NET Core application).</para>
        /// </summary>
        [PublicAPI]
        protected virtual void RegisterServices(IServiceCollection services)
        {
            // Does nothing by default.
        }

        /// <summary>
        /// Construct the application itself (via extension methods to <paramref name="hostBuilder"/>).
        /// </summary>
        [PublicAPI]
        protected virtual void SetupApplication(IHostBuilder hostBuilder)
        {
            // Does nothing by default.
        }
    }
}

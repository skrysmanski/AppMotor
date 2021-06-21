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
using System.Threading;
using System.Threading.Tasks;

using AppMotor.CliApp.CommandLine;

using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AppMotor.CliApp.Hosting
{
    /// <summary>
    /// A <see cref="CliCommand"/> that integrates .NET's Generic Host functionality (https://docs.microsoft.com/en-us/dotnet/core/extensions/generic-host).
    /// This basically gives you access to .NET's dependency injection system (i.e. <see cref="IServiceProvider"/>) and its
    /// associated services (like logging and configuration).
    ///
    /// <para>To register your own services with the dependency injection system, override <see cref="ConfigureServices"/>.</para>
    ///
    /// <para>This command runs indefinitely by default. It can be stopped with a <see cref="CancellationToken"/> provided
    /// to the <c>Run()</c> method of the main application or via <see cref="IHostApplicationLifetime.StopApplication"/>.</para>
    ///
    /// <para>You can use it as root command with <see cref="CliApplicationWithCommand"/> or as a verb with
    /// <see cref="CliApplicationWithVerbs"/>.</para>
    ///
    /// <para>This class provides access to the Generic Host functionality for non ASP.NET Core (console) application. For ASP.NET
    /// Core applications, use <c>HttpServerCommandBase</c> (from the "AppMotor.HttpServer" NuGet package) instead.</para>
    /// </summary>
    public abstract class GenericHostCliCommand : CliCommand
    {
        /// <inheritdoc />
        protected sealed override CliCommandExecutor Executor => new(Execute);

        private async Task Execute(CancellationToken cancellationToken)
        {
            IHostBuilder hostBuilder = CreateHostBuilder();

            hostBuilder.UseServiceProviderFactory(CreateServiceProviderFactory);

            hostBuilder.ConfigureServices(ConfigureServices);

            ConfigureApplication(hostBuilder);

            IHost host = hostBuilder.Build();

            await host.RunAsync(cancellationToken).ConfigureAwait(false);
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
        /// Create the <see cref="IServiceProvider"/> factory (i.e. the dependency injection framework) to be used by
        /// the application.
        ///
        /// <para>The default implementation uses the built-in service provider (via <see cref="DefaultServiceProviderFactory"/>)
        /// with scope validation enabled (see <see cref="ServiceProviderOptions.ValidateScopes"/>).</para>
        /// </summary>
        [PublicAPI]
        protected virtual IServiceProviderFactory<IServiceCollection> CreateServiceProviderFactory(HostBuilderContext context)
        {
            var options = new ServiceProviderOptions()
            {
                // Enable all validations (by default, they're only enabled in the development environment)
                ValidateScopes = true,
                ValidateOnBuild = true,
            };

            return new DefaultServiceProviderFactory(options);
        }

        /// <summary>
        /// Registers all services with the dependency injection framework (as created by <see cref="CreateServiceProviderFactory"/>).
        ///
        /// <para>Note: <see cref="IHostedService"/> (registered via <see cref="ServiceCollectionHostedServiceExtensions.AddHostedService{THostedService}(IServiceCollection)"/>)
        /// are the primary way to run workloads in this application type (unless it's an ASP.NET Core application).</para>
        /// </summary>
        /// <remarks>
        /// If you need to configure the application itself, you can use <see cref="ConfigureApplication"/>.
        /// </remarks>
        [PublicAPI]
        protected virtual void ConfigureServices(IServiceCollection services)
        {
            // Does nothing by default.
        }

        /// <summary>
        /// Adds additional configuration to the application itself (other than registering services with is done
        /// by <see cref="ConfigureServices"/> instead) - via extension methods to <paramref name="hostBuilder"/>.
        ///
        /// <para>The default implementation does nothing by itself and you may not even need to override this
        /// method.</para>
        /// </summary>
        [PublicAPI]
        protected virtual void ConfigureApplication(IHostBuilder hostBuilder)
        {
            // Does nothing by default.
        }
    }
}

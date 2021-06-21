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

using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AppMotor.CliApp.CommandLine.Hosting
{
    /// <summary>
    /// A <see cref="CliCommand"/> that integrates .NET's Generic Host functionality - i.e. <see cref="IHost"/> and its builder
    /// <see cref="IHostBuilder"/>. This is .NET's new standard way of configuring applications (see
    /// https://docs.microsoft.com/en-us/dotnet/core/extensions/generic-host for more details).
    ///
    /// <para>The main benefit here is that you get access to .NET's dependency injection system (i.e. <see cref="IServiceProvider"/>)
    /// and its associated services (like logging and configuration). You also get support for running services via
    /// <see cref="IHostedService"/>.</para>
    ///
    /// <para>To register your own services with the dependency injection system, override <see cref="ConfigureServices"/>.</para>
    ///
    /// <para>By default, this command runs indefinitely until it's stopped manually (see <see cref="ExplicitExecutor"/> for details on
    /// how to stop the command). Alternatively, you can set <see cref="ExplicitExecutor"/> which automatically stops this command
    /// once it has finished.</para>
    ///
    /// <para>You can use this class as root command with <see cref="CliApplicationWithCommand"/> or as a verb with
    /// <see cref="CliApplicationWithVerbs"/>.</para>
    ///
    /// <para>This class provides access to the Generic Host functionality for non ASP.NET Core (console) application. For ASP.NET
    /// Core applications, use <c>HttpServerCommandBase</c> (from the "AppMotor.HttpServer" NuGet package) instead.</para>
    /// </summary>
    public abstract class GenericHostCliCommand : CliCommand
    {
        /// <inheritdoc />
        protected sealed override CliCommandExecutor Executor => new(Execute);

        /// <summary>
        /// Returns the <see cref="IHostBuilderFactory"/> to be used for this command. The
        /// default implementation uses <see cref="DefaultHostBuilderFactory"/>.
        /// </summary>
        /// <remarks>
        /// The default implementation does not use <see cref="Host.CreateDefaultBuilder()"/>. This is
        /// because <c>CreateDefaultBuilder()</c> adds lots of services to the host builder - services
        /// that the user did not request. We want the user to explicitly choose services and dependencies.
        ///
        /// <para>If you want all the features provided by <c>CreateDefaultBuilder()</c>, simply wrap
        /// <see cref="Host.CreateDefaultBuilder()"/> in an instance of <see cref="MethodHostBuilderFactory"/>.</para>
        /// </remarks>
        protected virtual IHostBuilderFactory HostBuilderFactory => DefaultHostBuilderFactory.Instance;

        /// <summary>
        /// If set, this executor automatically stops this command once the executor has finished.
        ///
        /// <para>If this property is <c>null</c> (the default), this command runs indefinitely until it's stopped
        /// manually. This can be done by canceling the <see cref="CancellationToken"/> provided to the <c>application.Run()</c>
        /// call (if one was provided), or by calling either <see cref="Stop"/> or <see cref="IHostApplicationLifetime.StopApplication"/>.</para>
        ///
        /// <para>Note: The executor can get access to the registered services via the <see cref="Services"/>
        /// property.</para>
        /// </summary>
        protected virtual CliCommandExecutor? ExplicitExecutor => null;

        /// <summary>
        /// The registered services (i.e. dependency injection).
        /// </summary>
        /// <remarks>
        /// This property is only available after the application has been created. Basically it's only available
        /// in <see cref="ExplicitExecutor"/> (its primary use case).
        /// </remarks>
        protected IServiceProvider Services => this._serviceProvider ?? throw new InvalidOperationException("The ServiceProvide is not yet initialized.");

        private IServiceProvider? _serviceProvider;

        /// <summary>
        /// The lifetime events for this command.
        /// </summary>
        public GenericHostCliCommandLifetimeEvents LifetimeEvents { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected GenericHostCliCommand()
        {
            this.LifetimeEvents = new(this);
        }

        private async Task<int> Execute(CancellationToken cancellationToken)
        {
            IHostBuilder hostBuilder = this.HostBuilderFactory.CreateHostBuilder();

           hostBuilder.ConfigureServices(ConfigureServices);

            ConfigureApplication(hostBuilder);

            IHost host = hostBuilder.Build();

            this._serviceProvider = host.Services;

            try
            {
                await host.StartAsync(cancellationToken).ConfigureAwait(false);

                this.LifetimeEvents.TriggerStarted();

                var applicationLifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();
                applicationLifetime.ApplicationStopping.Register(
                    state =>
                    {
                        var lifetimeEvents = (GenericHostCliCommandLifetimeEvents)state!;
                        lifetimeEvents.TriggerStopping();
                    },
                    state: this.LifetimeEvents
                );

                int exitCode;

                if (this.ExplicitExecutor is null)
                {
                    await host.WaitForShutdownAsync(cancellationToken).ConfigureAwait(false);

                    exitCode = 0;
                }
                else
                {
                    exitCode = await this.ExplicitExecutor.Execute(cancellationToken).ConfigureAwait(false);

                    //
                    // Shut down host. For details, see implementation of "WaitForShutdownAsync()".
                    //
                    applicationLifetime.StopApplication();

                    // IMPORTANT: Don't pass "cancellationToken" here because it may have already been canceled and we don't
                    //   want to cancel "StopAsync" in this case.
                    await host.StopAsync(CancellationToken.None).ConfigureAwait(false);
                }

                this.LifetimeEvents.TriggerStopped();

                return exitCode;
            }
            finally
            {
                if (host is IAsyncDisposable asyncDisposable)
                {
                    await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                }
                else
                {
                    host.Dispose();
                }
            }
        }

        /// <summary>
        /// Stops this command.
        ///
        /// <para>Note: If <see cref="ExplicitExecutor"/> is set, this call may not have any effect.</para>
        /// </summary>
        public void Stop()
        {
            var applicationLifetime = this.Services.GetRequiredService<IHostApplicationLifetime>();
            applicationLifetime.StopApplication();
        }

        /// <summary>
        /// Registers all services with the dependency injection framework.
        ///
        /// <para>Note: <see cref="IHostedService"/> (registered via <see cref="ServiceCollectionHostedServiceExtensions.AddHostedService{THostedService}(IServiceCollection)"/>)
        /// are the primary way to run workloads in this application type (unless it's an ASP.NET Core application).</para>
        /// </summary>
        /// <remarks>
        /// If you need to configure the application itself, you can use <see cref="ConfigureApplication"/>.
        /// </remarks>
        [PublicAPI]
        protected virtual void ConfigureServices(HostBuilderContext context, IServiceCollection services)
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

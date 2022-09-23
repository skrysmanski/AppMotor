// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.CliApp.AppBuilding;
using AppMotor.CliApp.Terminals;
using AppMotor.Core.Utils;

using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AppMotor.CliApp.CommandLine.Hosting;

/// <summary>
/// <para>A <see cref="CliCommand"/> that integrates .NET's Generic Host functionality - i.e. <see cref="IHost"/> and its builder
/// <see cref="IHostBuilder"/>. This is .NET's new standard way of configuring applications (see
/// https://docs.microsoft.com/en-us/dotnet/core/extensions/generic-host for more details).</para>
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
    /// Action that configures <see cref="Microsoft.Extensions.Hosting.Internal.ConsoleLifetime"/> (via
    /// <see cref="ConsoleLifetimeOptions"/>). Primarily useful if you want to suppress status messages
    /// like "Press Ctrl+C to shut down.". If <c>null</c> and <see cref="ExplicitExecutor"/> is set,
    /// the status messages are suppressed automatically.
    /// </summary>
    [PublicAPI]
    protected virtual Action<ConsoleLifetimeOptions>? ConsoleLifetimeConfiguration => null;

    /// <summary>
    /// If set, this executor automatically stops this command once the executor has finished.
    ///
    /// <para>If this property is <c>null</c> (the default), this command runs indefinitely until it's stopped
    /// manually. This can be done by canceling the <see cref="CancellationToken"/> provided to the <c>application.Run()</c>
    /// call (if one was provided), or by calling either <see cref="Stop"/> or <see cref="IHostApplicationLifetime.StopApplication"/>.
    /// If this commands runs "interactively", the user can also hit "Ctrl+C".</para>
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
    /// in <see cref="ExplicitExecutor"/> (its primary use case). This is also why this property is <c>protected</c>
    /// rather than <c>public</c>.
    /// </remarks>
    protected IServiceProvider Services => this._serviceProvider ?? throw new InvalidOperationException("This ServiceProvider is not yet available.");

    private IServiceProvider? _serviceProvider;

    /// <summary>
    /// The lifetime events for this command.
    /// </summary>
    public IGenericHostCliCommandLifetimeEvents LifetimeEvents => this._lifetimeEvents;

    private readonly GenericHostCliCommandLifetimeEvents _lifetimeEvents = new();

    private async Task<int> Execute(CancellationToken cancellationToken)
    {
        IHostBuilder hostBuilder = CreateHostBuilder();

        hostBuilder.ConfigureServices(services =>
        {
            // ReSharper disable once AccessToDisposedClosure
            services.AddSingleton<IGenericHostCliCommandLifetimeEvents>(this._lifetimeEvents);

            // Required for the terminal logger
            services.AddSingleton<ITerminalOutput>(this.Terminal);
        });

        hostBuilder.ConfigureServices(ConfigureServices);

        ConfigureApplication(hostBuilder);

        var explicitExecutor = this.ExplicitExecutor;

        //
        // Configure ConsoleLifetime.
        //
        if (this.ConsoleLifetimeConfiguration is not null)
        {
            hostBuilder.UseConsoleLifetime(this.ConsoleLifetimeConfiguration);
        }
        else if (explicitExecutor is not null)
        {
            // Suppress ConsoleLifetime status message (e.g. "Press Ctrl+C to shut down.") if we
            // have an explicit executor (that controls the lifetime of the process).
            hostBuilder.UseConsoleLifetime(options =>
            {
                options.SuppressStatusMessages = true;
            });
        }

        IHost host = hostBuilder.Build();

        this._serviceProvider = host.Services;

        try
        {
            await host.StartAsync(cancellationToken).ConfigureAwait(false);

            // Signal that the application has started.
            await this._lifetimeEvents.StartedEventSource.RaiseEventAsync().ConfigureAwait(false);

            // Register event handler for underlying stop event.
            var applicationLifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();
            // NOTE: If "ApplicationStopping" has already been raised/fired, the callback will be called immediately.
            applicationLifetime.ApplicationStopping.Register(
                static state =>
                {
                    var lifetimeEvents = (GenericHostCliCommandLifetimeEvents)state!;
                    lifetimeEvents.RaiseStoppingEvent();
                },
                state: this._lifetimeEvents
            );

            int exitCode;

            if (explicitExecutor is null)
            {
                // Wait for the stopping event.
                await host.WaitForShutdownAsync(cancellationToken).ConfigureAwait(false);

                exitCode = 0;
            }
            else
            {
                exitCode = await explicitExecutor.Execute(cancellationToken).ConfigureAwait(false);

                //
                // Shut down host. For details, see implementation of "WaitForShutdownAsync()".
                //
                applicationLifetime.StopApplication();

                // IMPORTANT: Don't pass "cancellationToken" here because it may have already been canceled and we don't
                //   want to cancel "StopAsync" in this case.
                await host.StopAsync(CancellationToken.None).ConfigureAwait(false);
            }

            // Signal that the application has stopped.
            await this._lifetimeEvents.StoppedEventSource.RaiseEventAsync().ConfigureAwait(false);

            return exitCode;
        }
        finally
        {
            await DisposeHelper.DisposeWithAsyncSupport(host).ConfigureAwait(false);
            this._lifetimeEvents.Dispose();
        }
    }

    /// <summary>
    /// Stops this command. This initiates a graceful shutdown.
    ///
    /// <para>Note: If <see cref="ExplicitExecutor"/> is set, this call may not have any effect.</para>
    /// </summary>
    /// <remarks>
    /// This method is just convenience wrapper around <see cref="IHostApplicationLifetime.StopApplication"/>.
    /// </remarks>
    public void Stop()
    {
        var applicationLifetime = this.Services.GetRequiredService<IHostApplicationLifetime>();
        applicationLifetime.StopApplication();
    }

    /// <summary>
    /// Creates the <see cref="IHostBuilder"/> to be used by this command. The default implementation
    /// uses <see cref="DefaultHostBuilderFactory.CreateHostBuilder"/> but you could also use
    /// <see cref="Host.CreateDefaultBuilder()"/>.
    /// </summary>
    /// <remarks>
    /// Since <see cref="CliCommand"/>s have their own command line parameter parsing, this method doesn't
    /// get the command line parameters as parameter (for example to pass to <see cref="Host.CreateDefaultBuilder(string[])"/>).
    /// You can still get them via <see cref="Environment.GetCommandLineArgs"/> if you really want.
    /// </remarks>
    [MustUseReturnValue]
    protected virtual IHostBuilder CreateHostBuilder()
    {
        return DefaultHostBuilderFactory.Instance.CreateHostBuilder();
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

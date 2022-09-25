// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

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
/// <para>To register your own services with the dependency injection system, override <see cref="CliCommand.ConfigureServices"/>.</para>
///
/// <para>This command runs indefinitely until it's stopped manually. This can be done by canceling the <see cref="CancellationToken"/>
/// provided to the <c>application.Run()</c> call (if one was provided), or by calling either <see cref="Stop"/> or
/// <see cref="IHostApplicationLifetime.StopApplication"/>. If this commands runs "interactively", the user can also hit "Ctrl+C".</para>
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
    /// The lifetime events for this command.
    /// </summary>
    public IGenericHostCliCommandLifetimeEvents LifetimeEvents => this._lifetimeEvents;

    private readonly GenericHostCliCommandLifetimeEvents _lifetimeEvents = new();

    private async Task Execute(CancellationToken cancellationToken)
    {
        try
        {
            // Signal that the application has started.
            await this._lifetimeEvents.StartedEventSource.RaiseEventAsync().ConfigureAwait(false);

            // Register event handler for underlying stop event.
            var applicationLifetime = this.Services.GetRequiredService<IHostApplicationLifetime>();

            // Wait for the stopping event.
            await WaitForShutdownAsync(applicationLifetime, cancellationToken).ConfigureAwait(false);

            this._lifetimeEvents.RaiseStoppingEvent();

            // Signal that the application has stopped.
            await this._lifetimeEvents.StoppedEventSource.RaiseEventAsync().ConfigureAwait(false);
        }
        finally
        {
            this._lifetimeEvents.Dispose();
        }
    }

    private static async Task WaitForShutdownAsync(IHostApplicationLifetime applicationLifetime, CancellationToken cancellationToken)
    {
        // NOTE: If this cancellation token has already been canceled, the callback will be called immediately.
        cancellationToken.Register(
            static state =>
            {
                ((IHostApplicationLifetime)state!).StopApplication();
            },
            applicationLifetime
        );

        var waitForStop = new TaskCompletionSource<object?>(TaskCreationOptions.RunContinuationsAsynchronously);
        // NOTE: If "ApplicationStopping" has already been raised/fired, the callback will be called immediately.
        // IMPORTANT: Do NOT use "this._lifetimeEvent.Stopping" here as this could mean that other ApplicationStopping
        //   event handlers still execute while the lifetime is already disposed in Execute() (since we would not(!) be
        //   waiting for the completion of the other event handlers here).
        applicationLifetime.ApplicationStopping.Register(() => waitForStop.TrySetResult(null));

        await waitForStop.Task.ConfigureAwait(false);
    }

    /// <summary>
    /// Stops this command. This initiates a graceful shutdown.
    /// </summary>
    /// <remarks>
    /// This method is just convenience wrapper around <see cref="IHostApplicationLifetime.StopApplication"/>.
    /// </remarks>
    public void Stop()
    {
        var applicationLifetime = this.Services.GetRequiredService<IHostApplicationLifetime>();
        applicationLifetime.StopApplication();
    }

    /// <inheritdoc />
    protected override void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        base.ConfigureServices(context, services);

        services.AddSingleton<IGenericHostCliCommandLifetimeEvents>(this._lifetimeEvents);
    }

    /// <inheritdoc />
    protected override void ConfigureApplication(IHostBuilder hostBuilder)
    {
        base.ConfigureApplication(hostBuilder);

        // Use the "default" lifetime for console GenericHost applications. Overrides
        // the "NullLifetime" of "CliCommand".
        hostBuilder.UseConsoleLifetime();
    }
}

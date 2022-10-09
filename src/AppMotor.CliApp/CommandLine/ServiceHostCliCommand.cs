// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AppMotor.CliApp.CommandLine;

/// <summary>
/// <para>A command that just runs one or more servers.</para>
///
/// <para>These servers must implement <see cref="IHostedService"/> and be registered in <see cref="ConfigureServices"/> via
/// <see cref="ServiceCollectionHostedServiceExtensions.AddHostedService{THostedService}(IServiceCollection)"/>.</para>
///
/// <para>This command runs indefinitely until it's stopped manually. This can be done by canceling the <see cref="CancellationToken"/>
/// provided to the <c>application.Run()</c> call (if one was provided), or by calling either <see cref="Stop"/> or
/// <see cref="IHostApplicationLifetime.StopApplication"/>. If this commands runs "interactively", the user can also hit "Ctrl+C".</para>
/// </summary>
/// <remarks>
/// You can use this class as root command with <see cref="CliApplicationWithCommand"/> or as a verb with
/// <see cref="CliApplicationWithVerbs"/>.
/// </remarks>
/// <remarks>
/// For ASP.NET Core applications, use <c>HttpServerCommandBase</c> (from the "AppMotor.HttpServer" NuGet package) instead.
/// </remarks>
public abstract class ServiceHostCliCommand : CliCommand
{
    /// <inheritdoc />
    protected sealed override CliCommandExecutor Executor => new(Execute);

    /// <summary>
    /// The lifetime events for this command.
    /// </summary>
    public IServiceHostLifetimeEvents LifetimeEvents => this._lifetimeEvents;

    private readonly ServiceHostLifetimeEvents _lifetimeEvents = new();

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

            this._lifetimeEvents.CancelCancellationToken();

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
        // IMPORTANT: Do NOT use "this._lifetimeEvent.Stopped" here as this could mean that other ApplicationStopped
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

        services.AddSingleton<IServiceHostLifetimeEvents>(this._lifetimeEvents);
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

// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.Core.Events;
using AppMotor.Core.Utils;

namespace AppMotor.CliApp.CommandLine.Hosting;

/// <summary>
/// Implementation of <see cref="IGenericHostCliCommandLifetimeEvents"/>.
/// </summary>
internal sealed class GenericHostCliCommandLifetimeEvents : Disposable, IGenericHostCliCommandLifetimeEvents
{
    public OneTimeEvent Started => this.StartedEventSource.Event;

    internal readonly OneTimeEventSource StartedEventSource = new();

    public OneTimeEvent Stopping => this._stoppingEventSource.Event;

    private readonly OneTimeEventSource _stoppingEventSource = new();

    public OneTimeEvent Stopped => this.StoppedEventSource.Event;

    internal readonly OneTimeEventSource StoppedEventSource = new();

    /// <summary>
    /// This token is canceled when the <see cref="Stopping"/> event is triggered.
    /// </summary>
    public CancellationToken CancellationToken => this._cts.Token;

    private readonly CancellationTokenSource _cts = new();

    /// <inheritdoc />
    protected override void DisposeManagedResources()
    {
        this._cts.Dispose();
    }

    internal void RaiseStoppingEvent()
    {
        this._cts.Cancel();
        this._stoppingEventSource.RaiseEvent();
    }
}

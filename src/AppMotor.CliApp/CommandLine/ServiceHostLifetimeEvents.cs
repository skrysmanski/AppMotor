// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.Core.Events;
using AppMotor.Core.Utils;

namespace AppMotor.CliApp.CommandLine;

/// <summary>
/// Implementation of <see cref="IServiceHostLifetimeEvents"/>.
/// </summary>
internal sealed class ServiceHostLifetimeEvents : Disposable, IServiceHostLifetimeEvents
{
    /// <inheritdoc />
    public OneTimeEvent Started => this.StartedEventSource.Event;

    internal readonly OneTimeEventSource StartedEventSource = new();

    /// <inheritdoc />
    public OneTimeEvent Stopped => this.StoppedEventSource.Event;

    internal readonly OneTimeEventSource StoppedEventSource = new();

    /// <inheritdoc />
    public CancellationToken CancellationToken => this._cts.Token;

    private readonly CancellationTokenSource _cts = new();

    /// <inheritdoc />
    protected override void DisposeManagedResources()
    {
        this._cts.Dispose();
    }

    internal void CancelCancellationToken()
    {
        this._cts.Cancel();
    }
}

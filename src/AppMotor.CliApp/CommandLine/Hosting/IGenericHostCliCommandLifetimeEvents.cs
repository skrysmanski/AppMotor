// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.Core.Events;

namespace AppMotor.CliApp.CommandLine.Hosting;

/// <summary>
/// The lifetime events for a <see cref="GenericHostCliCommand"/>.
/// </summary>
public interface IGenericHostCliCommandLifetimeEvents
{
    /// <summary>
    /// Triggered when the <see cref="GenericHostCliCommand"/> has fully started.
    /// </summary>
    public OneTimeEvent Started { get; }

    /// <summary>
    /// Triggered when the <see cref="GenericHostCliCommand"/> is starting a graceful shutdown.
    /// Shutdown will block until all event handlers registered on this event have completed.
    /// </summary>
    public OneTimeEvent Stopping { get; }

    /// <summary>
    /// Triggered when the <see cref="GenericHostCliCommand"/> has completed a graceful shutdown.
    /// The application will not exit until all event handlers registered on this event have completed.
    /// </summary>
    public OneTimeEvent Stopped { get; }

    /// <summary>
    /// This token is canceled when the <see cref="Stopping"/> event is triggered.
    ///
    /// <para>Note: This token can no longer be obtained once <see cref="Stopped"/> has been
    /// triggered (because the underlying <see cref="CancellationTokenSource"/> will be disposed
    /// at this time and you'll get an <see cref="ObjectDisposedException"/>).</para>
    /// </summary>
    public CancellationToken CancellationToken { get; }
}

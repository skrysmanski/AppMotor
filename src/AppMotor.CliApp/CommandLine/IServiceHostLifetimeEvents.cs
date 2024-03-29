﻿// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.Core.Events;

namespace AppMotor.CliApp.CommandLine;

/// <summary>
/// The lifetime events for a <see cref="ServiceHostCliCommand"/>.
/// </summary>
public interface IServiceHostLifetimeEvents
{
    /// <summary>
    /// Triggered when the <see cref="ServiceHostCliCommand"/> has fully started.
    /// </summary>
    public OneTimeEvent Started { get; }

    /// <summary>
    /// Triggered when the <see cref="ServiceHostCliCommand"/> has completed a graceful shutdown.
    /// The application will not exit until all event handlers registered on this event have completed.
    /// </summary>
    public OneTimeEvent Stopped { get; }

    /// <summary>
    /// This token is canceled just before the <see cref="Stopped"/> event is triggered.
    ///
    /// <para>Note: This token can no longer be obtained once <see cref="Stopped"/> has been
    /// triggered (because the underlying <see cref="CancellationTokenSource"/> will be disposed
    /// at this time and you'll get an <see cref="ObjectDisposedException"/>).</para>
    /// </summary>
    public CancellationToken CancellationToken { get; }
}

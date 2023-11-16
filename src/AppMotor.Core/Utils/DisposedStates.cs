// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

namespace AppMotor.Core.Utils;

/// <summary>
/// Describes the "disposed" state of an object.
/// </summary>
public enum DisposedStates
{
    /// <summary>
    /// The object has not yet been disposed.
    /// </summary>
    NotDisposed = 0,

    /// <summary>
    /// The object is currently being disposed (i.e. <see cref="IDisposable.Dispose"/> or
    /// <see cref="IAsyncDisposable.DisposeAsync"/> have been called but they have not yet
    /// finished).
    /// </summary>
    Disposing = 1,

    /// <summary>
    /// The object has been disposed.
    /// </summary>
    Disposed = 2,
}

// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

namespace AppMotor.Core.Utils;

/// <summary>
/// Helper methods for working with <see cref="IDisposable"/> and <see cref="IAsyncDisposable"/>.
/// </summary>
public static class DisposeHelper
{
    /// <summary>
    /// Checks if the <paramref name="disposable"/> is also <see cref="IAsyncDisposable"/> and uses
    /// <see cref="IAsyncDisposable.DisposeAsync"/>, if it is - or <see cref="IDisposable.Dispose"/>
    /// if it's not.
    /// </summary>
    public static async Task DisposeWithAsyncSupport(IDisposable disposable)
    {
        if (disposable is IAsyncDisposable asyncDisposable)
        {
            await asyncDisposable.DisposeAsync().ConfigureAwait(false);
        }
        else
        {
            disposable.Dispose();
        }
    }
}
// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

namespace AppMotor.Core.Utils;

/// <summary>
/// Implements <see cref="IAsyncDisposable"/> (and <see cref="IDisposable"/>) and an easy to use
/// way.
/// </summary>
public abstract class AsyncDisposable : Disposable, IAsyncDisposable
{
    // NOTE: For details about this implementation, see: https://docs.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-disposeasync

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (BeginDispose())
        {
            try
            {
                await DisposeAsyncCore().ConfigureAwait(continueOnCapturedContext: false);

                DisposeUnmanagedResources();

                GC.SuppressFinalize(this);
            }
            catch (Exception)
            {
                EndDispose(exception: true);
                throw;
            }

            EndDispose(exception: false);
        }
    }

    /// <summary>
    /// Disposes any managed and unmanaged resources. Note that this must dispose the
    /// same resources as <see cref="Disposable.DisposeManagedResources"/> and
    /// <see cref="Disposable.DisposeUnmanagedResources"/>.
    /// </summary>
    /// <remarks>
    /// In addition to this method, inheritors must also implements the respective methods
    /// from <see cref="Disposable"/>.
    /// </remarks>
    protected abstract ValueTask DisposeAsyncCore();
}
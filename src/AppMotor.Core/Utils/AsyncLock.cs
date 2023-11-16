// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using JetBrains.Annotations;

namespace AppMotor.Core.Utils;

/// <summary>
/// Like using <c>lock</c> but can be used with <c>async</c> statements. Use one of the <c>AcquireAsync</c> methods
/// to acquire a lock with a <c>using</c> (or <c>try-finally</c>) block.
///
/// <para>Be careful: This lock is not reentrant!</para>
/// </summary>
public sealed class AsyncLock : Disposable
{
    private readonly SemaphoreSlim _lock = new(initialCount: 1, maxCount: 1);

    /// <inheritdoc />
    protected override void DisposeManagedResources()
    {
        this._lock.Dispose();
    }

    /// <summary>
    /// Acquires the lock. Waits indefinitely for it to acquire (unless <paramref name="cancellationToken"/>
    /// is canceled).
    /// </summary>
    /// <returns>The lock releaser; dispose it to release the lock.</returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/>
    /// is canceled.</exception>
    [MustUseReturnValue]
    public async Task<IDisposable> AcquireAsync(CancellationToken cancellationToken = default)
    {
        await this._lock.WaitAsync(cancellationToken).ConfigureAwait(false);

        return new LockReleaser(this._lock);
    }

    /// <summary>
    /// Acquires the lock. Waits for the specified time to acquire the lock (unless <paramref name="cancellationToken"/>
    /// is canceled first). Throws a <see cref="TimeoutException"/> if the lock could not be acquired in time.
    /// </summary>
    /// <returns>The lock releaser; dispose it to release the lock.</returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/>
    /// is canceled.</exception>
    /// <exception cref="TimeoutException">Thrown if the lock could not be acquired in time.</exception>
    [MustUseReturnValue]
    public async Task<IDisposable> AcquireAsync(TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        bool acquired = await this._lock.WaitAsync(timeout, cancellationToken).ConfigureAwait(false);
        if (!acquired)
        {
            throw new TimeoutException("The lock could not be acquired.");
        }

        return new LockReleaser(this._lock);
    }

    // ReSharper disable once IdentifierTypo
    private sealed class LockReleaser : Disposable
    {
        private readonly SemaphoreSlim _acquiredLock;

        /// <inheritdoc />
        // ReSharper disable once IdentifierTypo
        public LockReleaser(SemaphoreSlim acquiredLock)
        {
            this._acquiredLock = acquiredLock;
        }

        /// <inheritdoc />
        protected override void DisposeManagedResources()
        {
            this._acquiredLock.Release();
        }
    }
}

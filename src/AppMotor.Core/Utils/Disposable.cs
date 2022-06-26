#region License
// Copyright 2020 AppMotor Framework (https://github.com/skrysmanski/AppMotor)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using JetBrains.Annotations;

namespace AppMotor.Core.Utils;

/// <summary>
/// Implements <see cref="IDisposable"/> in an easy to use way. Implementers can override
/// <see cref="DisposeManagedResources"/> and/or <see cref="DisposeUnmanagedResources"/>.
/// If a child class defines a finalizer, it must call <see cref="DisposeFromFinalizer"/>.
/// </summary>
/// <seealso cref="AsyncDisposable"/>
public abstract class Disposable : IDisposable
{
    private int _disposeState = DisposedStatesAsIntegers.NOT_DISPOSED;

    /// <summary>
    /// The "disposed" state of this instance.
    /// </summary>
    [PublicAPI]
    public DisposedStates DisposedState => (DisposedStates)this._disposeState;

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(disposing: true);

        // NOTE: We put this here even though we don't have a finalizer - but
        //   a child class may have a finalizer.
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Call this method from the finalizer of your class, if your class has a finalizer.
    /// </summary>
    [PublicAPI]
    protected void DisposeFromFinalizer()
    {
        Dispose(disposing: false);
    }

#pragma warning disable CA1063 // Implement IDisposable Correctly - we have our own IDisposable pattern
    private void Dispose(bool disposing)
#pragma warning restore CA1063 // Implement IDisposable Correctly
    {
        if (BeginDispose())
        {
            try
            {
                // Always dispose un(!)-managed resources
                DisposeUnmanagedResources();

                if (disposing)
                {
                    DisposeManagedResources();
                }
            }
            catch (Exception)
            {
                EndDispose(exception: true);
                throw;
            }

            EndDispose(exception: false);
        }
    }

    [MustUseReturnValue]
    internal bool BeginDispose()
    {
        var origValue = Interlocked.CompareExchange(ref this._disposeState, DisposedStatesAsIntegers.DISPOSING, comparand: DisposedStatesAsIntegers.NOT_DISPOSED);
        if (origValue != DisposedStatesAsIntegers.NOT_DISPOSED)
        {
            // Already disposed or disposing.
            return false;
        }

        return true;
    }

    internal void EndDispose(bool exception)
    {
        if (exception)
        {
            Interlocked.Exchange(ref this._disposeState, DisposedStatesAsIntegers.NOT_DISPOSED);
        }
        else
        {
            Interlocked.Exchange(ref this._disposeState, DisposedStatesAsIntegers.DISPOSED);
        }
    }

    /// <summary>
    /// Disposes all managed resources of this class.
    ///
    /// <para>Note: Do not call this method directly!</para>
    /// </summary>
    /// <seealso cref="DisposeUnmanagedResources"/>
    [PublicAPI]
    protected abstract void DisposeManagedResources();

    /// <summary>
    /// Disposes all unmanaged resources of this class.
    ///
    /// <para>Note: Do not call this method directly!</para>
    /// </summary>
    /// <seealso cref="DisposeManagedResources"/>
    [PublicAPI]
    protected virtual void DisposeUnmanagedResources()
    {
        // Does nothing by default.
    }

    /// <summary>
    /// Verifies that this instance is not yet disposed. Throws an <see cref="ObjectDisposedException"/>
    /// if it has already been disposed.
    /// </summary>
    [PublicAPI]
    public void VerifyNotDisposed()
    {
        var value = Interlocked.CompareExchange(ref this._disposeState, DisposedStatesAsIntegers.NOT_DISPOSED, DisposedStatesAsIntegers.NOT_DISPOSED);
        if (value != DisposedStatesAsIntegers.NOT_DISPOSED)
        {
            throw CreateObjectDisposedException();
        }
    }

    /// <summary>
    /// Creates a new instance of <see cref="ObjectDisposedException"/> to be used in situations where you want to throw one.
    /// </summary>
    [PublicAPI, MustUseReturnValue]
    protected ObjectDisposedException CreateObjectDisposedException()
    {
        return new(GetType().Name);
    }

    // NOTE: This should be an enum but it can't be because "Interlocked" requires an int field.
    private static class DisposedStatesAsIntegers
    {
        public const int NOT_DISPOSED = (int)DisposedStates.NotDisposed;
        public const int DISPOSING = (int)DisposedStates.Disposing;
        public const int DISPOSED = (int)DisposedStates.Disposed;
    }
}

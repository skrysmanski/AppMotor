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

using System;
using System.Threading;

using JetBrains.Annotations;

namespace AppMotor.Core.Utils
{
    /// <summary>
    /// Implements <see cref="IDisposable"/> in an easy to use way. Implementers can override
    /// <see cref="DisposeManagedResources"/> and/or <see cref="DisposeUnmanagedResources"/>.
    /// If a child class defines a finalizer, it must call <see cref="DisposeFromFinalizer"/>.
    /// </summary>
    public abstract class Disposable : IDisposable
    {
        private int m_disposeState = DisposeStates.NOT_DISPOSED;

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
            var origValue = Interlocked.CompareExchange(ref this.m_disposeState, DisposeStates.DISPOSING, comparand: DisposeStates.NOT_DISPOSED);
            if (origValue != DisposeStates.NOT_DISPOSED)
            {
                // Already disposed or disposing.
                return;
            }

            // Always dispose un(!)-managed resources
            DisposeUnmanagedResources();

            if (disposing)
            {
                DisposeManagedResources();
            }

            Interlocked.Exchange(ref this.m_disposeState, DisposeStates.DISPOSED);
        }

        /// <summary>
        /// Disposes all managed resources of this class.
        ///
        /// <para>Note: Do not call this method directly!</para>
        /// </summary>
        /// <seealso cref="DisposeUnmanagedResources"/>
        protected virtual void DisposeManagedResources()
        {
            // Does nothing by default.
        }

        /// <summary>
        /// Disposes all unmanaged resources of this class.
        ///
        /// <para>Note: Do not call this method directly!</para>
        /// </summary>
        /// <seealso cref="DisposeManagedResources"/>
        protected virtual void DisposeUnmanagedResources()
        {
            // Does nothing by default.
        }

        /// <summary>
        /// Verifies that this instance is not yet disposed. Throws an <see cref="ObjectDisposedException"/>
        /// if it has already been disposed.
        /// </summary>
        [PublicAPI]
        protected void VerifyNotDisposed()
        {
            var value = Interlocked.CompareExchange(ref this.m_disposeState, DisposeStates.NOT_DISPOSED, DisposeStates.NOT_DISPOSED);
            if (value != DisposeStates.NOT_DISPOSED)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        // NOTE: This should be an enum but it can't be because "Interlocked" requires an int field.
        private static class DisposeStates
        {
            public const int NOT_DISPOSED = 0;
            public const int DISPOSING = 1;
            public const int DISPOSED = 2;
        }
    }
}

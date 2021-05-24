#region License
// Copyright 2021 AppMotor Framework (https://github.com/skrysmanski/AppMotor)
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
using System.Runtime.InteropServices;

using AppMotor.Core.Utils;

namespace AppMotor.Core.Security.Secrets
{
    internal sealed class SecretsArray<T> : Disposable where T : struct
    {
        private GCHandle _handle;

        private T[]? _data;

        public T[] UnderlyingArray
        {
            get
            {
                VerifyNotDisposed();
                return this._data!;
            }
        }

        public ReadOnlySpan<T> Span
        {
            get
            {
                VerifyNotDisposed();
                if (this._customLength is null)
                {
                    return this._data!;
                }
                else
                {
                    return new ReadOnlySpan<T>(this._data, start: 0, length: this._customLength.Value);
                }
            }
        }

        public int Length
        {
            get
            {
                VerifyNotDisposed();
                return this._customLength ?? this._data!.Length;
            }
        }

        private int? _customLength;

        public SecretsArray(int length)
        {
            this._data = new T[length];
            // Pin this array to prevent the garbage collector from moving it around in memory
            // (thereby effectively creating copies of the data in memory).
            this._handle = GCHandle.Alloc(this.UnderlyingArray, GCHandleType.Pinned);
        }

        /// <inheritdoc />
        protected override void DisposeManagedResources()
        {
            var originalData = this._data;
            this._data = null;

            if (originalData is not null)
            {
                Array.Clear(originalData, index: 0, length: originalData.Length);
            }
        }

        /// <inheritdoc />
        protected override void DisposeUnmanagedResources()
        {
            if (this._handle.IsAllocated)
            {
                this._handle.Free();
            }
        }

        public void SetLength(int length)
        {
            VerifyNotDisposed();

            if (length > this._data!.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            this._customLength = length;
        }
    }
}

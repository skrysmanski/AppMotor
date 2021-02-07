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

using AppMotor.Core.Utils;

namespace AppMotor.Core.Security
{
    public sealed class SecureSecret : Disposable
    {
        private byte[]? _secretBytes;

        public ReadOnlySpan<byte> AsSpan => this._secretBytes;

        public SecureSecret(ReadOnlySpan<byte> secretBytes)
        {
            this._secretBytes = secretBytes.ToArray();
        }

        /// <inheritdoc />
        protected override void DisposeManagedResources()
        {
            var originalBytes = this._secretBytes;
            this._secretBytes = null;

            if (originalBytes is not null)
            {
                Array.Clear(originalBytes, index: 0, length: originalBytes.Length);
            }
        }
    }

    public sealed class SecureStringSecret : Disposable
    {
        private char[]? _secret;

        public ReadOnlySpan<char> AsSpan => this._secret;

        public SecureStringSecret(ReadOnlySpan<char> secretBytes)
        {
            this._secret = secretBytes.ToArray();
        }

        /// <inheritdoc />
        protected override void DisposeManagedResources()
        {
            var originalSecret = this._secret;
            this._secret = null;

            if (originalSecret is not null)
            {
                Array.Clear(originalSecret, index: 0, length: originalSecret.Length);
            }
        }
    }
}

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
using System.IO;
using System.IO.Abstractions;

using AppMotor.Core.IO;
using AppMotor.Core.Utils;

namespace AppMotor.Core.Security.Secrets
{
    public sealed class SecureSecret
    {
        private readonly byte[] _encryptedSecretBytes;

        private readonly int _decryptedBytesCount;

        public SecureSecret(ReadOnlySpan<byte> encryptedSecretBytes, int decryptedBytesCount)
        {
            this._encryptedSecretBytes = encryptedSecretBytes.ToArray();
            this._decryptedBytesCount = decryptedBytesCount;
        }

        public DecryptedSecret Decrypt(SecretsScope secretsScope)
        {
            return secretsScope.Decrypt(this._encryptedSecretBytes, this._decryptedBytesCount);
        }
    }

    public sealed class SecureStringSecret : Disposable
    {
        private char[]? _secret;

        public ReadOnlySpan<char> Span => this._secret;

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

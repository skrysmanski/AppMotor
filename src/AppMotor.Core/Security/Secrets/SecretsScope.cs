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
using System.Security.Cryptography;

using AppMotor.Core.Utils;

using JetBrains.Annotations;

namespace AppMotor.Core.Security.Secrets
{
    public sealed class SecretsScope : Disposable
    {
        private Aes? _symmetricEncryptionAlgorithm;

        /// <inheritdoc />
        public SecretsScope()
        {
            // TODO: https://docs.microsoft.com/en-us/dotnet/api/system.threading.threadlocal-1?view=net-5.0
            // NOTE: A new encryption key is automatically created; see: https://docs.microsoft.com/en-us/dotnet/standard/security/generating-keys-for-encryption-and-decryption
            this._symmetricEncryptionAlgorithm = Aes.Create();
        }

        /// <inheritdoc />
        protected override void DisposeManagedResources()
        {
            this._symmetricEncryptionAlgorithm?.Dispose();
            this._symmetricEncryptionAlgorithm = null;

            base.DisposeManagedResources();
        }

        [MustUseReturnValue]
        internal DecryptedSecret Decrypt(byte[] encryptedBytes, int decryptedBytesCount)
        {
            VerifyNotDisposed();

            using var memoryStream = new MemoryStream(encryptedBytes, writable: false);
            using var cryptoStream = new CryptoStream(memoryStream, this._symmetricEncryptionAlgorithm!.CreateDecryptor(), CryptoStreamMode.Read);

            return new DecryptedSecret(cryptoStream, decryptedBytesCount);
        }
    }
}

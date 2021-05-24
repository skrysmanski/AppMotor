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
using System.IO.Abstractions;
using System.Threading.Tasks;

using AppMotor.Core.IO;

using JetBrains.Annotations;

namespace AppMotor.Core.Certificates.Exporting
{
    /// <summary>
    /// Exports two certificate blobs: the private key and the public key.
    /// </summary>
    /// <seealso cref="SingleBlobExporter"/>
    public sealed class DoubleBlobExporter
    {
        private readonly byte[] _publicKeyBytes;

        private readonly Func<byte[]> _privateKeyBytesExporterFunc;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="publicKeyBytes">The public key bytes.</param>
        /// <param name="privateKeyBytesExporterFunc">The exported function for the private key bytes; NOTE:
        /// Since the bytes contain sensitive data, we store just the exporter function and only use it when
        /// needed. This way the sensitive data doesn't float around in memory unnecessarily.</param>
        public DoubleBlobExporter(byte[] publicKeyBytes, Func<byte[]> privateKeyBytesExporterFunc)
        {
            this._publicKeyBytes = publicKeyBytes;
            this._privateKeyBytesExporterFunc = privateKeyBytesExporterFunc;
        }

        /// <summary>
        /// Returns the blobs as <see cref="ReadOnlyMemory{T}"/>.
        /// </summary>
        /// <returns></returns>
        [MustUseReturnValue]
        public (ReadOnlyMemory<byte> publicKeyBytes, ReadOnlyMemory<byte> privateKeyBytes) ToBytes()
        {
            return (this._publicKeyBytes, this._privateKeyBytesExporterFunc());
        }

        /// <summary>
        /// Stores the blobs in the file system.
        /// </summary>
        /// <param name="publicKeyFilePath">The file path to the public key file.</param>
        /// <param name="privateKeyFilePath">The file path to the private key file.</param>
        /// <param name="fileSystem">The file system to use; if <c>null</c>, <see cref="RealFileSystem.Instance"/> will be used.</param>
        public void ToFile(string publicKeyFilePath, string privateKeyFilePath, IFileSystem? fileSystem = null)
        {
            fileSystem ??= RealFileSystem.Instance;

            fileSystem.File.WriteAllBytes(publicKeyFilePath, this._publicKeyBytes);
            fileSystem.File.WriteAllBytes(privateKeyFilePath, this._privateKeyBytesExporterFunc());
        }

        /// <summary>
        /// Stores the blobs in the file system.
        /// </summary>
        /// <param name="publicKeyFilePath">The file path to the public key file.</param>
        /// <param name="privateKeyFilePath">The file path to the private key file.</param>
        /// <param name="fileSystem">The file system to use; if <c>null</c>, <see cref="RealFileSystem.Instance"/> will be used.</param>
        public async Task ToFileAsync(string publicKeyFilePath, string privateKeyFilePath, IFileSystem? fileSystem = null)
        {
            fileSystem ??= RealFileSystem.Instance;

            var publicKeyWriteTask = fileSystem.File.WriteAllBytesAsync(publicKeyFilePath, this._publicKeyBytes);
            var privateKeyWriteTask = fileSystem.File.WriteAllBytesAsync(privateKeyFilePath, this._privateKeyBytesExporterFunc());

            await Task.WhenAll(publicKeyWriteTask, privateKeyWriteTask).ConfigureAwait(false);
        }
    }
}

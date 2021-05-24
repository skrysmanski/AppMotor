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
using System.Text.Json;

using AppMotor.Core.IO;
using AppMotor.Core.Utils;

namespace AppMotor.Core.Security.Secrets
{
    /// <summary>
    /// An immutable secret consisting of bytes.
    /// </summary>
    /// <seealso cref="SecretString"/>
    public sealed class SecretBytes : Disposable
    {
        private readonly SecretsArray<byte> _secretData;

        public ReadOnlySpan<byte> Span => this._secretData.Span;

        internal SecretBytes(int size)
        {
            this._secretData = new SecretsArray<byte>(size);
        }

        private SecretBytes(ReadOnlySpan<byte> source)
        {
            this._secretData = new SecretsArray<byte>(source.Length);

            source.CopyTo(this._secretData.UnderlyingArray);
        }

        public static SecretBytes FromInMemorySource(ReadOnlySpan<byte> source)
        {
            return new SecretBytes(source);
        }

        /// <inheritdoc />
        protected override void DisposeManagedResources()
        {
            this._secretData.Dispose();
        }

        internal SecretsArray<byte> GetUnderlyingArray()
        {
            return this._secretData;
        }

        // NOTE: There is no Async version here on purpose to prevent the secret to be stored at more places than necessary (also
        //   secrets are usually so small that there's no real point in providing an async version).
        public void WriteToFile(string filePath, IFileSystem? fileSystem = null)
        {
            VerifyNotDisposed();

            fileSystem ??= RealFileSystem.Instance;

            var secretData = this.Span;

            int dataSize = secretData.Length;
            int bufferSize = dataSize / 24;
            if (bufferSize < 16)
            {
                bufferSize = 16;
            }
            else if (bufferSize > 128)
            {
                bufferSize = 128;
            }

            using var writeStream = fileSystem.FileStream.Create(filePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: bufferSize);

            writeStream.Write(secretData);
        }

        public SecretString ToStringSecret(SecretString.SupportedEncodings encoding)
        {
            return new SecretString(this, encoding);
        }

        public T? DeserializeAsJson<T>(JsonSerializerOptions? options = null) where T : ObjectWithSecrets
        {
            var byteSecretConverter = new ByteSecretJsonConverter();
            var stringSecretConverter = new StringSecretJsonConverter();

            // NOTE: We create a copy of the options here so that we don't mess around
            //   with the original options (and don't leak the converters).
            var actualOptions = options is not null ? new JsonSerializerOptions(options) : new JsonSerializerOptions();
            actualOptions.Converters.Add(byteSecretConverter);
            actualOptions.Converters.Add(stringSecretConverter);

            try
            {
                return JsonSerializer.Deserialize<T>(this.Span, actualOptions);
            }
            catch (Exception)
            {
                foreach (var createdSecret in byteSecretConverter.CreatedSecrets)
                {
                    createdSecret.Dispose();
                }

                foreach (var createdSecret in stringSecretConverter.CreatedSecrets)
                {
                    createdSecret.Dispose();
                }

                throw;
            }
        }
    }
}

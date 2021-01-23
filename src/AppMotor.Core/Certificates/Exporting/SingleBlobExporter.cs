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
using System.Threading.Tasks;

using AppMotor.Core.IO;

using JetBrains.Annotations;

namespace AppMotor.Core.Certificates.Exporting
{
    public sealed class SingleBlobExporter
    {
        private readonly byte[] _bytes;

        internal SingleBlobExporter(byte[] bytes)
        {
            this._bytes = bytes;
        }

        [MustUseReturnValue]
        public ReadOnlyMemory<byte> ToBytes()
        {
            return this._bytes;
        }

        public void ToFile(string filePath, IFileSystem? fileSystem = null)
        {
            fileSystem ??= RealFileSystem.Instance;

            fileSystem.File.WriteAllBytes(filePath, this._bytes);
        }

        public async Task ToFileAsync(string filePath, IFileSystem? fileSystem = null)
        {
            fileSystem ??= RealFileSystem.Instance;

            await fileSystem.File.WriteAllBytesAsync(filePath, this._bytes).ConfigureAwait(false);
        }
    }
}

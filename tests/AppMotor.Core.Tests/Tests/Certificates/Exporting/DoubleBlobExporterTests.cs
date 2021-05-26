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
using System.IO.Abstractions.TestingHelpers;
using System.Security.Cryptography;
using System.Threading.Tasks;

using AppMotor.Core.Certificates.Exporting;

using JetBrains.Annotations;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.Certificates.Exporting
{
    public sealed class DoubleBlobExporterTests
    {
        private readonly byte[] _publicKeyBytes = GenerateRandomBytes(40);

        private readonly byte[] _privateKeyBytes = GenerateRandomBytes(40);

        [MustUseReturnValue]
        private static byte[] GenerateRandomBytes(int byteCount)
        {
            // NOTE: We use a crypto random generate here so that the public and the private key
            //   bytes are not the same.
            using var rng = new RNGCryptoServiceProvider();

            byte[] bytes = new byte[byteCount];
            rng.GetBytes(bytes);

            return bytes;
        }

        [Fact]
        public void Test_ToBytes()
        {
            // Setup
            var exporter = new DoubleBlobExporter(this._publicKeyBytes, () => this._privateKeyBytes);

            // Test
            var (publicKeyBytes, privateKeyBytes) = exporter.ToBytes();

            // Validate
            publicKeyBytes.ShouldBe(this._publicKeyBytes);
            privateKeyBytes.ShouldBe(this._privateKeyBytes);
        }

        [Fact]
        public void Test_ToFile()
        {
            const string? PUBLIC_KEY_FILE_PATH = "/test/export.cert";
            const string? PRIVATE_KEY_FILE_PATH = "/test/export.key";

            // Setup
            var exporter = new DoubleBlobExporter(this._publicKeyBytes, () => this._privateKeyBytes);

            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddDirectory("/test");

            // Test
            exporter.ToFile(PUBLIC_KEY_FILE_PATH, PRIVATE_KEY_FILE_PATH, mockFileSystem);

            // Validate
            mockFileSystem.File.ReadAllBytes(PUBLIC_KEY_FILE_PATH).ShouldBe(this._publicKeyBytes);
            mockFileSystem.File.ReadAllBytes(PRIVATE_KEY_FILE_PATH).ShouldBe(this._privateKeyBytes);
        }

        [Fact]
        public async Task Test_ToFileAsync()
        {
            const string? PUBLIC_KEY_FILE_PATH = "/test/export.cert";
            const string? PRIVATE_KEY_FILE_PATH = "/test/export.key";

            // Setup
            var exporter = new DoubleBlobExporter(this._publicKeyBytes, () => this._privateKeyBytes);

            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddDirectory("/test");

            // Test
            await exporter.ToFileAsync(PUBLIC_KEY_FILE_PATH, PRIVATE_KEY_FILE_PATH, mockFileSystem);

            // Validate
            // ReSharper disable MethodHasAsyncOverload
            mockFileSystem.File.ReadAllBytes(PUBLIC_KEY_FILE_PATH).ShouldBe(this._publicKeyBytes);
            mockFileSystem.File.ReadAllBytes(PRIVATE_KEY_FILE_PATH).ShouldBe(this._privateKeyBytes);
            // ReSharper restore MethodHasAsyncOverload
        }
    }
}

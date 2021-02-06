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
using System.Threading.Tasks;

using AppMotor.Core.Certificates.Exporting;

using JetBrains.Annotations;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.Certificates.Exporting
{
    public sealed class SingleBlobExporterTests
    {
        private readonly byte[] _testBytes = GenerateRandomBytes(40);

        [MustUseReturnValue]
        private static byte[] GenerateRandomBytes(int byteCount)
        {
            var bytes = new byte[byteCount];
            new Random().NextBytes(bytes);
            return bytes;
        }

        [Fact]
        public void Test_ToBytes()
        {
            // Setup
            var exporter = new SingleBlobExporter(() => this._testBytes);

            // Test
            exporter.ToBytes().ShouldBe(this._testBytes);
        }

        [Fact]
        public void Test_ToFile()
        {
            const string? FILE_PATH = "/test/export.bin";

            // Setup
            var exporter = new SingleBlobExporter(() => this._testBytes);

            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddDirectory("/test");

            // Test
            exporter.ToFile(FILE_PATH, mockFileSystem);

            // Validate
            mockFileSystem.File.ReadAllBytes(FILE_PATH).ShouldBe(this._testBytes);
        }

        [Fact]
        public async Task Test_ToFileAsync()
        {
            const string? FILE_PATH = "/test/export.bin";

            // Setup
            var exporter = new SingleBlobExporter(() => this._testBytes);

            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddDirectory("/test");

            // Test
            await exporter.ToFileAsync(FILE_PATH, mockFileSystem);

            // Validate
            // ReSharper disable once MethodHasAsyncOverload
            mockFileSystem.File.ReadAllBytes(FILE_PATH).ShouldBe(this._testBytes);
        }
    }
}

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

using AppMotor.Core.Certificates;
using AppMotor.Core.Certificates.Exporting;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.Certificates
{
    public sealed class TlsCertificatePublicKeyExporterTests : TlsCertificateExporterTestBase
    {
        [Theory]
        [InlineData(CertificateFileFormats.DER)]
        [InlineData(CertificateFileFormats.PEM)]
        [InlineData(CertificateFileFormats.PFX)]
        public void Test_ToByteArray(CertificateFileFormats exportFormat)
        {
            // Setup
            using var originalCert = TlsCertificate.CreateSelfSigned("example.com", TimeSpan.FromDays(20));

            // Test our assumptions
            originalCert.HasPrivateKey.ShouldBe(true);

            // Test
            var exportedBytes = originalCert.ExportPublicKey().ToByteArray(exportFormat);

            // Validate
            CheckExportedBytesForCorrectFormat(exportedBytes, exportFormat);

            using var reimportedCert = new TlsCertificate(TlsCertificateSource.FromBytes(exportedBytes));
            reimportedCert.PublicKey.ShouldBe(originalCert.PublicKey);
            reimportedCert.HasPrivateKey.ShouldBe(false);
        }

        [Theory]
        [InlineData(CertificateFileFormats.DER)]
        [InlineData(CertificateFileFormats.PEM)]
        [InlineData(CertificateFileFormats.PFX)]
        public void Test_ToFile(CertificateFileFormats exportFormat)
        {
            // Setup
            using var originalCert = TlsCertificate.CreateSelfSigned("example.com", TimeSpan.FromDays(20));

            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddDirectory("/test");

            var filePath = $@"/test/certificate{CertFileExtensionHelper.GetFileExtensionFor(CertExportOptions.PublicKeyOnly, exportFormat)}";

            // Test our assumptions
            originalCert.HasPrivateKey.ShouldBe(true);

            // Test
            originalCert.ExportPublicKey().ToFile(filePath, exportFormat, mockFileSystem);

            // Validate
            using var reimportedCert = new TlsCertificate(TlsCertificateSource.FromFile(filePath, mockFileSystem));
            reimportedCert.PublicKey.ShouldBe(originalCert.PublicKey);
            reimportedCert.HasPrivateKey.ShouldBe(false);
        }

        [Theory]
        [InlineData(CertificateFileFormats.DER)]
        [InlineData(CertificateFileFormats.PEM)]
        [InlineData(CertificateFileFormats.PFX)]
        public async Task Test_ToFileAsync(CertificateFileFormats exportFormat)
        {
            // Setup
            using var originalCert = TlsCertificate.CreateSelfSigned("example.com", TimeSpan.FromDays(20));

            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddDirectory("/test");

            var filePath = $@"/test/certificate{CertFileExtensionHelper.GetFileExtensionFor(CertExportOptions.PublicKeyOnly, exportFormat)}";

            // Test our assumptions
            originalCert.HasPrivateKey.ShouldBe(true);

            // Test
            await originalCert.ExportPublicKey().ToFileAsync(filePath, exportFormat, mockFileSystem);

            // Validate
            using var reimportedCert = new TlsCertificate(TlsCertificateSource.FromFile(filePath, mockFileSystem));
            reimportedCert.PublicKey.ShouldBe(originalCert.PublicKey);
            reimportedCert.HasPrivateKey.ShouldBe(false);
        }
    }
}

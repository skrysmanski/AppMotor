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

using AppMotor.Core.Certificates;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.Certificates.Exporting
{
    public sealed class TlsCertificatePublicKeyExporterTests : TlsCertificateExporterTestBase
    {
        [Theory]
        [InlineData(CertificateFileFormats.PEM)]
        [InlineData(CertificateFileFormats.PFX)]
        public void Test_Export(CertificateFileFormats exportFormat)
        {
            // Setup
            using var originalCert = TlsCertificate.CreateSelfSigned("example.com", TimeSpan.FromDays(20));

            // Test our assumptions
            originalCert.HasPrivateKey.ShouldBe(true);

            // Test
            var exportedBytes = originalCert.ExportPublicKey().As(exportFormat).ToBytes();

            // Validate
            CheckExportedBytesForCorrectFormat(exportedBytes.Span, exportFormat);

            using var reimportedCert = new TlsCertificate(TlsCertificateSource.FromBytes(exportedBytes));
            reimportedCert.PublicKey.ShouldBe(originalCert.PublicKey);
            reimportedCert.HasPrivateKey.ShouldBe(false);
        }
    }
}

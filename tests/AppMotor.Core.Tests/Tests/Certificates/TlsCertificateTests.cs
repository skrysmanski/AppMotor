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

namespace AppMotor.Core.Tests.Certificates
{
    public sealed class TlsCertificateTests
    {
        [Fact]
        public void Test_CreateSelfSigned()
        {
            // Test
            var cert = TlsCertificate.CreateSelfSigned("example.com", TimeSpan.FromDays(20));

            // Validate
            cert.SubjectName.Name.ShouldBe("CN=example.com");
            cert.NotAfter.Date.ShouldBe(DateTime.Today + TimeSpan.FromDays(20));
            cert.KeyAlgorithm.ShouldBe(CertificateKeyAlgorithms.RSA);
        }

        [Fact]
        public void Test_AllowPrivateKeyExportWithoutPrivateKey()
        {
            // Setup
            using var originalCert = TlsCertificate.CreateSelfSigned("example.com", TimeSpan.FromDays(20));
            var exportedBytes = originalCert.ExportPublicKey().ToByteArray(CertificateFileFormats.DER);

            // Should not throw
            // Users may not know beforehand whether a certificate has a private key or not.
            using var certWithoutPrivateKey = Should.NotThrow(() => new TlsCertificate(TlsCertificateSource.FromBytes(exportedBytes), allowPrivateKeyExport: true));

            // Test our assumption
            certWithoutPrivateKey.HasPrivateKey.ShouldBe(false);
        }

    }
}

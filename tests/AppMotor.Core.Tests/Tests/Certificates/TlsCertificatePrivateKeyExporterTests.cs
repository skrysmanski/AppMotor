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
using System.Security.Cryptography;

using AppMotor.Core.Certificates;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.Certificates
{
    public sealed class TlsCertificatePrivateKeyExporterTests : TlsCertificateExporterTestBase
    {
        [Fact]
        public void Test_AsPfx()
        {
            // Setup
            using var originalCert = TlsCertificate.CreateSelfSigned("example.com", TimeSpan.FromDays(20));
            var originalPrivateKey = ((RSA)originalCert.UnderlyingCertificate.PrivateKey!).ExportRSAPrivateKey();

            // Test our assumptions
            originalCert.HasPrivateKey.ShouldBe(true);

            // Test
            var exportedBytes = originalCert.ExportPublicAndPrivateKey().AsPfx().ToBytes();

            // Validate
            CheckExportedBytesForCorrectFormat(exportedBytes.Span, CertificateFileFormats.PFX);

            using var reimportedCert = new TlsCertificate(TlsCertificateSource.FromBytes(exportedBytes), allowPrivateKeyExport: true);
            reimportedCert.PublicKey.ShouldBe(originalCert.PublicKey);
            reimportedCert.HasPrivateKey.ShouldBe(true);
            reimportedCert.ExportPublicAndPrivateKey().ExportPrivateKey().ShouldBe(originalPrivateKey);

            // Test: Export of private key after it has been (re)imported first
            var exportedBytes2 = reimportedCert.ExportPublicAndPrivateKey().AsPfx().ToBytes();
            using var reimportedCert2 = new TlsCertificate(TlsCertificateSource.FromBytes(exportedBytes2), allowPrivateKeyExport: true);
            reimportedCert2.PublicKey.ShouldBe(originalCert.PublicKey);
            reimportedCert2.HasPrivateKey.ShouldBe(true);
            reimportedCert2.ExportPublicAndPrivateKey().ExportPrivateKey().ShouldBe(originalPrivateKey);
        }

        [Fact]
        public void Test_AsPem()
        {
            // Setup
            using var originalCert = TlsCertificate.CreateSelfSigned("example.com", TimeSpan.FromDays(20));
            var originalPrivateKey = ((RSA)originalCert.UnderlyingCertificate.PrivateKey!).ExportRSAPrivateKey();

            // Test our assumptions
            originalCert.HasPrivateKey.ShouldBe(true);

            // Test
            var (publicKeyBytes, privateKeyBytes) = originalCert.ExportPublicAndPrivateKey().AsPem().ToBytes();

            // Validate
            CheckExportedBytesForCorrectFormat(publicKeyBytes.Span, CertificateFileFormats.PEM);
            CheckExportedBytesForCorrectFormat(privateKeyBytes.Span, CertificateFileFormats.PEM);

            using var reimportedCert = new TlsCertificate(TlsCertificateSource.FromBytes(publicKeyBytes, privateKeyBytes), allowPrivateKeyExport: true);
            reimportedCert.PublicKey.ShouldBe(originalCert.PublicKey);
            reimportedCert.HasPrivateKey.ShouldBe(true);
            reimportedCert.ExportPublicAndPrivateKey().ExportPrivateKey().ShouldBe(originalPrivateKey);

            // Test: Export of private key after it has been (re)imported first
            var (exportedPublicKeyBytes2, exportedPrivateKeyBytes2) = reimportedCert.ExportPublicAndPrivateKey().AsPem().ToBytes();
            using var reimportedCert2 = new TlsCertificate(TlsCertificateSource.FromBytes(exportedPublicKeyBytes2, exportedPrivateKeyBytes2), allowPrivateKeyExport: true);
            reimportedCert2.PublicKey.ShouldBe(originalCert.PublicKey);
            reimportedCert2.HasPrivateKey.ShouldBe(true);
            reimportedCert2.ExportPublicAndPrivateKey().ExportPrivateKey().ShouldBe(originalPrivateKey);
        }

        [Fact]
        public void Test_Export_NonExportable()
        {
            // Setup
            using var originalCert = TlsCertificate.CreateSelfSigned("example.com", TimeSpan.FromDays(20));

            // Export to reimport because a self signed certificate is always exportable.
            var exportedBytes = originalCert.ExportPublicAndPrivateKey().AsPfx().ToBytes();
            // ReSharper disable once IdentifierTypo
            using var unexportableCert = new TlsCertificate(TlsCertificateSource.FromBytes(exportedBytes), allowPrivateKeyExport: false);
            // Test our assumptions
            unexportableCert.HasPrivateKey.ShouldBe(true);

            // Test
            Should.Throw<InvalidOperationException>(() => unexportableCert.ExportPublicAndPrivateKey());
        }

        [Fact]
        public void Test_Export_WithoutPrivateKey()
        {
            // Setup
            using var originalCert = TlsCertificate.CreateSelfSigned("example.com", TimeSpan.FromDays(20));
            var exportedBytes = originalCert.ExportPublicKey().ToByteArray(CertificateFileFormats.PEM);
            using var certWithoutPrivateKey = new TlsCertificate(TlsCertificateSource.FromBytes(exportedBytes), allowPrivateKeyExport: true);

            // Test our assumptions
            certWithoutPrivateKey.HasPrivateKey.ShouldBe(false);

            // Test
            Should.Throw<InvalidOperationException>(() => certWithoutPrivateKey.ExportPublicAndPrivateKey());
        }
    }
}

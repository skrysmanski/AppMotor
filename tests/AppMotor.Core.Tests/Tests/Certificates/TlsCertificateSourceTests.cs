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
using AppMotor.Core.Security;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.Certificates
{
    public sealed class TlsCertificateSourceTests
    {
        private const string TEST_CERT_FILES_BASE_PATH = "Tests/Certificates/cert-files";

        [Fact]
        public void Test_ImportPem()
        {
            // Test
            var source = TlsCertificateSource.FromFile($"{TEST_CERT_FILES_BASE_PATH}/cert.pem", $"{TEST_CERT_FILES_BASE_PATH}/key.pem");
            using var cert = new TlsCertificate(source);

            // Validate
            cert.HasPrivateKey.ShouldBe(true);
            cert.SubjectName.Name.ShouldBe("CN=www.example.com, OU=Org, O=Company Name, L=Portland, S=Oregon, C=US");
        }

        // https://stackoverflow.com/questions/49230122/encrypted-private-key-to-rsa-private-key
        [Fact]
        public void Test_ImportPem_EncryptedPrivateKey()
        {
            using var unencryptedCert = new TlsCertificate(
                TlsCertificateSource.FromFile($"{TEST_CERT_FILES_BASE_PATH}/cert.pfx"),
                allowPrivateKeyExport: true
            );

            using var password = new SecureStringSecret("P@ssw0rd");

            // Test
            var source = TlsCertificateSource.FromFile($"{TEST_CERT_FILES_BASE_PATH}/cert.pem", $"{TEST_CERT_FILES_BASE_PATH}/key_encrypted.pem");
            using var cert = new TlsCertificate(source, password.AsSpan, allowPrivateKeyExport: true);

            // Validate
            cert.SubjectName.Name.ShouldBe("CN=www.example.com, OU=Org, O=Company Name, L=Portland, S=Oregon, C=US");
            cert.HasPrivateKey.ShouldBe(true);
            cert.ExportPublicAndPrivateKey().ExportPrivateKey().ShouldBe(unencryptedCert.ExportPublicAndPrivateKey().ExportPrivateKey());
        }

        [Fact]
        public void Test_ImportPem_EncryptedPrivateKey_Pkcs1()
        {
            using var unencryptedCert = new TlsCertificate(
                TlsCertificateSource.FromFile($"{TEST_CERT_FILES_BASE_PATH}/cert.pfx"),
                allowPrivateKeyExport: true
            );

            using var password = new SecureStringSecret("P@ssw0rd");

            // Test
            var source = TlsCertificateSource.FromFile($"{TEST_CERT_FILES_BASE_PATH}/cert.pem", $"{TEST_CERT_FILES_BASE_PATH}/key_encrypted_pkcs1.pem");
            var ex = Should.Throw<NotSupportedException>(() => new TlsCertificate(source, password.AsSpan));
            ex.Message.ShouldContain("PKCS#1");
        }

        [Fact]
        public void Test_ImportPfx()
        {
            // Test
            var source = TlsCertificateSource.FromFile($"{TEST_CERT_FILES_BASE_PATH}/cert.pfx");
            using var cert = new TlsCertificate(source);

            // Validate
            cert.HasPrivateKey.ShouldBe(true);
            cert.SubjectName.Name.ShouldBe("CN=www.example.com, OU=Org, O=Company Name, L=Portland, S=Oregon, C=US");
        }

        [Fact]
        public void Test_ImportPfx_Encrypted()
        {
            using var unencryptedCert = new TlsCertificate(
                TlsCertificateSource.FromFile($"{TEST_CERT_FILES_BASE_PATH}/cert.pfx"),
                allowPrivateKeyExport: true
            );

            using var password = new SecureStringSecret("P@ssw0rd");

            // Test
            var source = TlsCertificateSource.FromFile($"{TEST_CERT_FILES_BASE_PATH}/cert_encrypted.pfx");
            using var cert = new TlsCertificate(source, password.AsSpan, allowPrivateKeyExport: true);

            // Validate
            cert.SubjectName.Name.ShouldBe("CN=www.example.com, OU=Org, O=Company Name, L=Portland, S=Oregon, C=US");
            cert.HasPrivateKey.ShouldBe(true);
            cert.ExportPublicAndPrivateKey().ExportPrivateKey().ShouldBe(unencryptedCert.ExportPublicAndPrivateKey().ExportPrivateKey());
        }
    }
}

// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Security.Cryptography.X509Certificates;

using AppMotor.Core.Certificates;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.Certificates;

public sealed class TlsCertificateTests
{
    private const string TEST_CERT_FILES_BASE_PATH = "Tests/Certificates/cert-files";

    [Fact]
    public void Test_CreateSelfSigned()
    {
        // Test
        using var cert = TlsCertificate.CreateSelfSigned("example.com", TimeSpan.FromDays(20));

        // Validate
        cert.SubjectName.Name.ShouldBe("CN=example.com");
        cert.NotAfter.Date.ShouldBe(DateTime.Today + TimeSpan.FromDays(20));
        cert.NotBefore.Date.ShouldBe(DateTime.Today);
        cert.KeyAlgorithm.ShouldBe(CertificateKeyAlgorithms.RSA);
        cert.Thumbprint.ShouldMatch("[A-F0-9]{40}");
    }

    [Fact]
    public void Test_SubjectAlternativeNames()
    {
        var cert = TlsCertificate.CreateFromFile($"{TEST_CERT_FILES_BASE_PATH}/www-microsoft-com.pem");

        cert.SubjectAlternativeNames.Length.ShouldNotBe(0);
        cert.SubjectAlternativeNames.ShouldBe(
            new[]
            {
                "wwwqa.microsoft.com",
                "www.microsoft.com",
                "staticview.microsoft.com",
                "i.s-microsoft.com",
                "microsoft.com",
                "c.s-microsoft.com",
                "privacy.microsoft.com",
            }
        );
    }

    [Fact]
    public void Test_SubjectAlternativeNames_NotExisting()
    {
        var cert = TlsCertificate.CreateFromFile($"{TEST_CERT_FILES_BASE_PATH}/cert.pem");

        cert.SubjectAlternativeNames.Length.ShouldBe(0);
    }

    [Fact]
    public void Test_AllowPrivateKeyExportWithoutPrivateKey()
    {
        // Setup
        using var originalCert = TlsCertificate.CreateSelfSigned("example.com", TimeSpan.FromDays(20));
        var exportedBytes = originalCert.ExportPublicKey().AsPem().ToBytes();

        // Should not throw
        // Users may not know beforehand whether a certificate has a private key or not.
        using var certWithoutPrivateKey = Should.NotThrow(() => new TlsCertificate(TlsCertificateSource.FromBytes(exportedBytes), allowPrivateKeyExport: true));

        // Test our assumption
        certWithoutPrivateKey.HasPrivateKey.ShouldBe(false);
    }

    [Fact]
    public void Test_ConversionToX509Certificate()
    {
        using var cert = TlsCertificate.CreateSelfSigned("example.com", TimeSpan.FromDays(20));

        X509Certificate2 nativeCert = cert;

        nativeCert.ShouldBe(cert.UnderlyingCertificate);
    }
}

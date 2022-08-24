// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.Core.Certificates;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.Certificates;

public sealed class TlsCertificateSourceTests
{
    private const string TEST_CERT_FILES_BASE_PATH = "Tests/Certificates/cert-files";

    [Fact]
    public void Test_ImportPem_FromFile()
    {
        // Test
        var source = TlsCertificateSource.FromFile($"{TEST_CERT_FILES_BASE_PATH}/cert.pem", $"{TEST_CERT_FILES_BASE_PATH}/key.pem");
        using var cert = new TlsCertificate(source);

        // Validate
        cert.HasPrivateKey.ShouldBe(true);
        cert.SubjectName.Name.ShouldBe("CN=www.example.com, OU=Org, O=Company Name, L=Portland, S=Oregon, C=US");
    }

    [Fact]
    public void Test_ImportPem_FromString()
    {
        // Setup
        var pemPubKey = File.ReadAllText($"{TEST_CERT_FILES_BASE_PATH}/cert.pem");

        // Test
        var source = TlsCertificateSource.FromPemString(pemPubKey);
        using var cert = new TlsCertificate(source);

        // Validate
        cert.HasPrivateKey.ShouldBe(false);
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

        // Test
        var source = TlsCertificateSource.FromFile($"{TEST_CERT_FILES_BASE_PATH}/cert.pem", $"{TEST_CERT_FILES_BASE_PATH}/key_encrypted.pem");
        using var cert = new TlsCertificate(source, "P@ssw0rd", allowPrivateKeyExport: true);

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

        // Test
        var source = TlsCertificateSource.FromFile($"{TEST_CERT_FILES_BASE_PATH}/cert.pem", $"{TEST_CERT_FILES_BASE_PATH}/key_encrypted_pkcs1.pem");
        var ex = Should.Throw<NotSupportedException>(() => new TlsCertificate(source, "P@ssw0rd"));
        ex.Message.ShouldContain("PKCS#1");
    }

    /// <summary>
    /// This test primarily exists to get code coverage to 100%.
    /// </summary>
    [Fact]
    public void Test_ImportPem_ArgumentExceptionInMalformedPrivateKey()
    {
        var pubKey = File.ReadAllBytes($"{TEST_CERT_FILES_BASE_PATH}/cert.pem");
        var privateKey = File.ReadAllBytes($"{TEST_CERT_FILES_BASE_PATH}/key.pem");

        // Break private key Base64
        privateKey[50] = 240;

        var source = TlsCertificateSource.FromBytes(pubKey, privateKey);

        // Test
        Should.Throw<ArgumentException>(() => new TlsCertificate(source, password: "abc"));
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

        // Test
        var source = TlsCertificateSource.FromFile($"{TEST_CERT_FILES_BASE_PATH}/cert_encrypted.pfx");
        using var cert = new TlsCertificate(source, "P@ssw0rd", allowPrivateKeyExport: true);

        // Validate
        cert.SubjectName.Name.ShouldBe("CN=www.example.com, OU=Org, O=Company Name, L=Portland, S=Oregon, C=US");
        cert.HasPrivateKey.ShouldBe(true);
        cert.ExportPublicAndPrivateKey().ExportPrivateKey().ShouldBe(unencryptedCert.ExportPublicAndPrivateKey().ExportPrivateKey());
    }

    [Fact]
    public void Test_ImportPfx_WithSeparatePrivateKey()
    {
        // Test
        Should.Throw<NotSupportedException>(() => TlsCertificateSource.FromFile($"{TEST_CERT_FILES_BASE_PATH}/cert.pfx", separatePrivateKeyFilePath: $"{TEST_CERT_FILES_BASE_PATH}/cert.pfx"));
    }

    [Fact]
    public void Test_UnsupportedFormat()
    {
        var data = new byte[] { 0x00, 0x01, 0x02 };

        var ex = Should.Throw<NotSupportedException>(() => TlsCertificateSource.FromBytes(data));
        ex.Message.ShouldBe("The certificate type could not be determined.");
    }

    [Fact]
    public void Test_UnsupportedKeyAlgorithm()
    {
        // Setup
        // NOTE: We use a DSA certificate here to trigger the exception below. It's very unlikely that we ever add support for DSA.
        var source = TlsCertificateSource.FromFile($"{TEST_CERT_FILES_BASE_PATH}/dsa_cert.pem", $"{TEST_CERT_FILES_BASE_PATH}/dsa_key.pem");

        // Test
        var ex = Should.Throw<NotSupportedException>(() => new TlsCertificate(source));

        ex.Message.ShouldBe("Unsupported certificate key algorithm: DSA");
    }
}
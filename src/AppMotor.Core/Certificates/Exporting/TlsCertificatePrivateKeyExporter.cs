﻿// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

using JetBrains.Annotations;

namespace AppMotor.Core.Certificates.Exporting;

/// <summary>
/// Exports both the public key and the private key. Obtained via <see cref="TlsCertificate.ExportPublicAndPrivateKey"/>.
/// Use one of the <c>As...()</c> methods to specify the desired export format.
/// </summary>
/// <seealso cref="TlsCertificatePublicKeyExporter"/>
public sealed class TlsCertificatePrivateKeyExporter
{
    private readonly TlsCertificate _certificate;

    internal TlsCertificatePrivateKeyExporter(TlsCertificate certificate)
    {
        this._certificate = certificate;
    }

    /// <summary>
    /// Exports the public key and the private key in the PFX format.
    /// </summary>
    [MustUseReturnValue]
    public SingleBlobExporter AsPfx()
    {
        return new(() => this._certificate.UnderlyingCertificate.Export(X509ContentType.Pfx));
    }

    /// <summary>
    /// Exports the public key and the private key in the PEM format.
    /// </summary>
    [MustUseReturnValue]
    public DoubleBlobExporter AsPem()
    {
        Func<byte[]> privateKeyBytesExporterFunc;

        if (this._certificate.KeyAlgorithm == CertificateKeyAlgorithms.RSA)
        {
            privateKeyBytesExporterFunc = () => ConvertToPem(ExportPrivateKey(), "RSA PRIVATE KEY");
        }
        else
        {
            throw new NotSupportedException($"Exporting private key of type '{this._certificate.KeyAlgorithm}' is not supported.");
        }

        return new DoubleBlobExporter(
            publicKeyBytes: ConvertToPem(this._certificate.UnderlyingCertificate.Export(X509ContentType.Cert), "CERTIFICATE"),
            privateKeyBytesExporterFunc: privateKeyBytesExporterFunc
        );
    }

    [MustUseReturnValue]
    private static byte[] ConvertToPem(byte[] bytes, string type)
    {
        var outputBuilder = new StringBuilder();

        outputBuilder.Append(CultureInfo.InvariantCulture, $"-----BEGIN {type}-----\r\n");
        outputBuilder.Append(Convert.ToBase64String(bytes, Base64FormattingOptions.InsertLineBreaks));
        outputBuilder.Append(CultureInfo.InvariantCulture, $"\r\n-----END {type}-----");

        return Encoding.ASCII.GetBytes(outputBuilder.ToString());
    }

    [MustUseReturnValue]
    [ExcludeFromCodeCoverage]
    internal byte[] ExportPrivateKey()
    {
        if (this._certificate.KeyAlgorithm == CertificateKeyAlgorithms.RSA)
        {
            var rsaPrivateKey = this._certificate.UnderlyingCertificate.GetRSAPrivateKey();
            return ExportRsaPrivateKey(rsaPrivateKey!);
        }
        else
        {
            throw new NotSupportedException($"Exporting private keys of type '{this._certificate.KeyAlgorithm}' is not supported.");
        }
    }

    /// <summary>
    /// Exports the private key into a byte array.
    /// </summary>
    [MustUseReturnValue]
    private static byte[] ExportRsaPrivateKey(RSA privateKey)
    {
        const string NOT_SECRET_PASSWORD = "passwd";

        //
        // IMPORTANT: At least on Windows, we can't just use "privateKey.ExportRSAPrivateKey()"
        //   because this method does not always work (it throws a "NotSupportedException" in
        //   this case). This is why we have this workaround where we export the private key
        //   encrypted and then reimport it afterwards.
        //

        byte[] encryptedPrivateKey = privateKey.ExportEncryptedPkcs8PrivateKey(
            NOT_SECRET_PASSWORD,
            new PbeParameters(
                PbeEncryptionAlgorithm.Aes128Cbc,
                HashAlgorithmName.SHA256,
                // Only 1 iteration because we don't need to secure the exported value
                // since it's immediately re-imported.
                iterationCount: 1
            )
        );

        // ReSharper disable once IdentifierTypo
        using var rsaReexporter = RSA.Create();

        rsaReexporter.ImportEncryptedPkcs8PrivateKey(NOT_SECRET_PASSWORD, encryptedPrivateKey, bytesRead: out _);

        return rsaReexporter.ExportRSAPrivateKey();
    }
}

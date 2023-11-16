// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Security.Cryptography.X509Certificates;
using System.Text;

using AppMotor.Core.Exceptions;

using JetBrains.Annotations;

namespace AppMotor.Core.Certificates.Exporting;

/// <summary>
/// Exports the public key (but not the private key). Obtained via <see cref="TlsCertificate.ExportPublicKey"/>.
/// Use one of the <c>As...()</c> methods to specify the desired export format.
/// </summary>
/// <seealso cref="TlsCertificatePublicKeyExporter"/>
public sealed class TlsCertificatePublicKeyExporter
{
    private readonly TlsCertificate _certificate;

    internal TlsCertificatePublicKeyExporter(TlsCertificate certificate)
    {
        this._certificate = certificate;
    }

    /// <summary>
    /// Exports the public key in the PFX format.
    /// </summary>
    [PublicAPI, MustUseReturnValue]
    public SingleBlobExporter AsPfx()
    {
        return new(() =>
            {
                using var tempCert = new X509Certificate2(this._certificate.UnderlyingCertificate.Export(X509ContentType.Cert));
                return tempCert.Export(X509ContentType.Pfx);
            }
        );
    }

    /// <summary>
    /// Exports the public key in the PEM format.
    /// </summary>
    [PublicAPI, MustUseReturnValue]
    public SingleBlobExporter AsPem()
    {
        return new(
            () =>
            {
                var outputBuilder = new StringBuilder();

                outputBuilder.Append("-----BEGIN CERTIFICATE-----\r\n");
                outputBuilder.Append(
                    Convert.ToBase64String(
                        this._certificate.UnderlyingCertificate.Export(X509ContentType.Cert),
                        Base64FormattingOptions.InsertLineBreaks
                    )
                );
                outputBuilder.Append("\r\n-----END CERTIFICATE-----");

                return Encoding.UTF8.GetBytes(outputBuilder.ToString());
            }
        );
    }

    /// <summary>
    /// Exports the public key as the specified export format.
    /// </summary>
    [PublicAPI, MustUseReturnValue]
    public SingleBlobExporter As(CertificateFileFormats exportFormat)
    {
        switch (exportFormat)
        {
            case CertificateFileFormats.PFX:
                return AsPfx();

            case CertificateFileFormats.PEM:
                return AsPem();

            default:
                throw new UnexpectedSwitchValueException(nameof(exportFormat), exportFormat);
        }
    }
}

// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using AppMotor.Core.Utils;

using JetBrains.Annotations;

namespace AppMotor.Core.Certificates;

/// <summary>
/// Represents a certificate request (i.e. a request to be signed by a CA to get a certificate).
/// </summary>
public sealed class TlsCertificateRequest : Disposable
{
    private readonly CertificateRequest _underlyingCertificateRequest;

    private readonly RSA _rsaAlgorithm;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="hostname">The hostname for which to create the request (or certificate).</param>
    /// <param name="additionalHostNames">Additional hostnames for which the generated certificate is to be valid for.</param>
    public TlsCertificateRequest(string hostname, IEnumerable<string>? additionalHostNames = null)
    {
        Validate.ArgumentWithName(nameof(hostname)).IsNotNullOrWhiteSpace(hostname);

        this._rsaAlgorithm = RSA.Create();

        this._underlyingCertificateRequest = new CertificateRequest(
            $"cn={hostname}",
            this._rsaAlgorithm,
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1
        );

        var sanNames = new List<string>()
        {
            hostname,
        };

        if (additionalHostNames is not null)
        {
            sanNames.AddRange(additionalHostNames);
        }

        this._underlyingCertificateRequest.AddSanExtension(sanNames);
    }

    /// <inheritdoc />
    protected override void DisposeManagedResources()
    {
        this._rsaAlgorithm.Dispose();
    }

    /// <summary>
    /// Creates a self signed certificate from this certificate request.
    /// </summary>
    /// <param name="certificateLifetime">for how long this certificate is to be valid - from now.</param>
    [PublicAPI, MustUseReturnValue]
    public TlsCertificate CreateSelfSignedCertificate(TimeSpan certificateLifetime)
    {
        var cert = this._underlyingCertificateRequest.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now + certificateLifetime);

        // A self signed certificate is automatically exportable.
        return new TlsCertificate(cert, allowPrivateKeyExport: true);
    }
}

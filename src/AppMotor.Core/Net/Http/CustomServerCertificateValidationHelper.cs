// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

using AppMotor.Core.Utils;

using JetBrains.Annotations;

namespace AppMotor.Core.Net.Http;

/// <summary>
/// Convenience class to support custom TLS certificates in <see cref="HttpClient"/> (or <see cref="HttpClientHandler"/>
/// to be more precise).
///
/// <para>You can use this class either directly or via <see cref="HttpClientFactory"/>.</para>
/// </summary>
public sealed class CustomServerCertificateValidationHelper
{
    private readonly X509Certificate2Collection _customCertificates;

    /// <summary>
    /// Constructs this helper with a single certificate.
    /// </summary>
    public CustomServerCertificateValidationHelper(X509Certificate2 customCertificate)
    {
        Validate.ArgumentWithName(nameof(customCertificate)).IsNotNull(customCertificate);

        this._customCertificates = new X509Certificate2Collection(customCertificate);
    }

    /// <summary>
    /// Constructs this helper with multiple certificates.
    /// </summary>
    public CustomServerCertificateValidationHelper(IEnumerable<X509Certificate2> customCertificates)
    {
        Validate.ArgumentWithName(nameof(customCertificates)).IsNotNull(customCertificates);

        this._customCertificates = new X509Certificate2Collection();

        foreach (var customCertificate in customCertificates)
        {
            this._customCertificates.Add(customCertificate);
        }
    }

    /// <summary>
    /// Validation callback for <see cref="HttpClientHandler.ServerCertificateCustomValidationCallback"/>. Use this
    /// method as callback on your <see cref="HttpClientHandler"/> to enable server certificate(s) specified in the
    /// constructor.
    /// </summary>
    [PublicAPI]
    public bool ValidationCallback(
#pragma warning disable CA1801 // Review unused parameters -> required for signature
            HttpRequestMessage request,
#pragma warning restore CA1801 // Review unused parameters
            X509Certificate2? serverCert,
            X509Chain? certChain,
            SslPolicyErrors errors
        )
    {
        return ValidationCallback(serverCert, certChain, errors);
    }

    /// <summary>
    /// Validation callback for <see cref="HttpClientHandler.ServerCertificateCustomValidationCallback"/>. Use this
    /// method if you want to use this class in your own callback.
    /// </summary>
    [PublicAPI]
    public bool ValidationCallback(
            X509Certificate2? serverCert,
            X509Chain? certChain,
            SslPolicyErrors errors
        )
    {
        // For any other error (e.g. missing certificate or wrong host name), don't do anything special.
        // NOTE: We don't use "HasFlags()" here as we only care about this specific error case - and not
        //   if it's combined with other error cases.
        if (errors != SslPolicyErrors.RemoteCertificateChainErrors)
        {
            return false;
        }

        if (serverCert == null || certChain == null)
        {
            return false;
        }

        certChain.ChainPolicy.TrustMode = X509ChainTrustMode.CustomRootTrust;
        certChain.ChainPolicy.CustomTrustStore.AddRange(this._customCertificates);
        return certChain.Build(serverCert);
    }
}

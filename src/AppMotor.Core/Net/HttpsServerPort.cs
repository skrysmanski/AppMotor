﻿// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.Core.Certificates;
using AppMotor.Core.Utils;

using JetBrains.Annotations;

namespace AppMotor.Core.Net;

/// <summary>
/// Represents an HTTPS server port.
/// </summary>
public class HttpsServerPort : HttpServerPort
{
    /// <summary>
    /// The default HTTPS port.
    /// </summary>
    [PublicAPI]
    public new const int DEFAULT_PORT = 443;

    /// <summary>
    /// This function returns the HTTPS certificate to be used. Note that the certificate
    /// must contain a private key.
    /// </summary>
    /// <remarks>
    /// For security reasons, this property is a function rather than storing the certificate itself. This way
    /// it doesn't keep the certificate in memory.
    /// </remarks>
    [PublicAPI]
    public Func<TlsCertificate> CertificateProvider { get; }

    /// <summary>
    /// Whether callers of <see cref="CertificateProvider"/> own the certificates created/returned by
    /// the method. The owner is responsible for disposing the certificate.
    /// </summary>
    public bool CertificateProviderCallerOwnsCertificates { get; }

    /// <summary>
    /// Constructor. Uses <see cref="DEFAULT_PORT"/> as port.
    /// </summary>
    /// <param name="listenAddress">From where to accept connections.</param>
    /// <param name="certificateProvider">Provides the certificate to be used; see <see cref="CertificateProvider"/> for more details.</param>
    /// <param name="certificateProviderCallerOwnsCertificates">See <see cref="CertificateProviderCallerOwnsCertificates"/> for details</param>
    [PublicAPI]
    public HttpsServerPort(SocketListenAddresses listenAddress, Func<TlsCertificate> certificateProvider, bool certificateProviderCallerOwnsCertificates = true)
        : this(listenAddress, port: DEFAULT_PORT, certificateProvider, certificateProviderCallerOwnsCertificates)
    {
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="listenAddress">From where to accept connections.</param>
    /// <param name="port">The port to use</param>
    /// <param name="certificateProvider">Provides the certificate to be used; see <see cref="CertificateProvider"/> for more details.</param>
    /// /// <param name="certificateProviderCallerOwnsCertificates">See <see cref="CertificateProviderCallerOwnsCertificates"/> for details</param>
    [PublicAPI]
    public HttpsServerPort(SocketListenAddresses listenAddress, int port, Func<TlsCertificate> certificateProvider, bool certificateProviderCallerOwnsCertificates = true)
        : base(listenAddress, port)
    {
        Validate.ArgumentWithName(nameof(certificateProvider)).IsNotNull(certificateProvider);

        this.CertificateProvider = certificateProvider;
        this.CertificateProviderCallerOwnsCertificates = certificateProviderCallerOwnsCertificates;
    }
}

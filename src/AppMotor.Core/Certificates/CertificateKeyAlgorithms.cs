// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

namespace AppMotor.Core.Certificates;

/// <summary>
/// The various key algorithms used in certificates.
/// </summary>
public enum CertificateKeyAlgorithms
{
    /// <summary>
    /// Other/unkown algorithm.
    /// </summary>
    Other,

    // ReSharper disable InconsistentNaming

    /// <summary>
    /// RSA (Rivest–Shamir–Adleman) is the default key algorithm for certificates as of 2021.
    /// It's widely supported. The minimum recommended key length for RSA keys is 2048.
    /// </summary>
    RSA,

    /// <summary>
    /// DSA (Digital Signature Algorithm) is the key algorithm primarily used for digital
    /// signature. As such, it is not widely supported for certificates.
    /// </summary>
    /// <seealso cref="ECDSA"/>
    DSA,

    /// <summary>
    /// ECDSA (Elliptic Curve Digital Signature Algorithm) is related to DSA and uses ECC (Elliptic Curve Cryptography).
    /// ECDSA requires a smaller key size than RSA. Because of this, performance is better.
    /// </summary>
    ECDSA,

    // ReSharper restore InconsistentNaming
}
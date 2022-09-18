// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;

using AppMotor.Core.Utils;

using JetBrains.Annotations;

namespace AppMotor.Core.Certificates;

/// <summary>
/// Represents an OID (object identifier) for (X.509) certificates. Each component of such a certificate (like Distinguished
/// Names, Public Key, ...) has its own OID. The OID's value can be obtained through <see cref="Value"/>. For more details,
/// see <see cref="Value"/>.
/// </summary>
/// <remarks>
/// This struct is just a wrapper around <see cref="Oid"/> to make this class easier to use and easier to document.
/// </remarks>
/// <seealso cref="CertificateKeyAlgorithmOids"/>
[DebuggerDisplay("{" + nameof(DebuggerDisplayValue) + "}")]
public readonly struct CertificateOid : IEquatable<CertificateOid>
{
    private static readonly ConcurrentDictionary<string, Oid> s_oidCache = new(StringComparer.OrdinalIgnoreCase);

    private readonly Oid? _underlyingOid;

    /// <summary>
    /// The underlying <see cref="Oid"/> instance.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [PublicAPI]
    private Oid UnderlyingOid => this._underlyingOid ?? throw new InvalidOperationException("Uninitialized value.");

    /// <summary>
    /// <para>The value of this OID in numbered notation (e.g. "1.2.840.113549.1.1.1").</para>
    ///
    /// <para>The numbers represent a tree structure with each number being a node. The example above means:</para>
    ///
    /// <para><c>iso(1) member-body(2) us(840) rsadsi(113549) pkcs(1) pkcs-1(1) rsaEncryption(1)</c></para>
    ///
    /// <para>For more information, see: https://www.alvestrand.no/objectid/ </para>
    ///
    /// <para>For decoding OIDs, see: http://www.oid-info.com/ </para>
    /// </summary>
    [PublicAPI]
    public string Value => this.UnderlyingOid.Value ?? throw new InvalidOperationException("The OID value is null.");

    /// <summary>
    /// Value to be disabled in a debugger.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [ExcludeFromCodeCoverage]
    private string DebuggerDisplayValue
    {
        get
        {
            if (this._underlyingOid is null)
            {
                return "<uninitialized oid>";
            }

            if (this._underlyingOid.FriendlyName is not null)
            {
                return $"{this._underlyingOid.Value} ({this._underlyingOid.FriendlyName})";
            }
            else
            {
                return this._underlyingOid.Value ?? "<oid without id>";
            }
        }
    }

    /// <summary>
    /// Constructor for <c>string</c> as input.
    /// </summary>
    /// <param name="oid">The oid as string - either as numbered notation (e.g. "1.2.840.113549.1.1.1")
    /// or a friendly name (e.g. "RSA").</param>
    public CertificateOid(string oid)
    {
        Validate.ArgumentWithName(nameof(oid)).IsNotNullOrWhiteSpace(oid);

        if (!s_oidCache.TryGetValue(oid, out this._underlyingOid))
        {
            this._underlyingOid = s_oidCache.GetOrAdd(oid, CreateOid);
        }
    }

    /// <summary>
    /// Constructor for <see cref="Oid"/> as input.
    /// </summary>
    /// <param name="oid">The oid.</param>
    public CertificateOid(Oid oid)
    {
        Validate.ArgumentWithName(nameof(oid)).IsNotNull(oid);

        this._underlyingOid = oid;
    }

    [MustUseReturnValue]
    private static Oid CreateOid(string oid)
    {
        return new(oid);
    }

    /// <summary>
    /// Converts <see cref="Oid"/> into <see cref="CertificateOid"/>.
    /// </summary>
    public static implicit operator CertificateOid(Oid source)
    {
        return new(source);
    }

    /// <summary>
    /// Converts <see cref="CertificateOid"/> into <see cref="Oid"/>.
    /// </summary>
    public static implicit operator Oid(CertificateOid source)
    {
        return source.UnderlyingOid;
    }

    /// <summary>
    /// Compares two <see cref="CertificateOid"/> instances for equality.
    /// </summary>
    public static bool operator ==(CertificateOid left, CertificateOid right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Compares two <see cref="CertificateOid"/> instances for inequality.
    /// </summary>
    public static bool operator !=(CertificateOid left, CertificateOid right)
    {
        return !left.Equals(right);
    }

    /// <inheritdoc />
    public bool Equals(CertificateOid other)
    {
        if (ReferenceEquals(this._underlyingOid, other._underlyingOid))
        {
            return true;
        }

        if (this._underlyingOid is not null && other._underlyingOid is not null)
        {
            return this._underlyingOid.Value == other._underlyingOid.Value;
        }

        return false;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is CertificateOid other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return this._underlyingOid?.Value?.GetHashCode() ?? 0;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return this._underlyingOid?.Value ?? "";
    }
}

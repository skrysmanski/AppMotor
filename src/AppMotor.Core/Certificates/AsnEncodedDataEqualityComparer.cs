// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Security.Cryptography;

using AppMotor.Core.Utils;

namespace AppMotor.Core.Certificates;

/// <summary>
/// Equality comparer for <see cref="AsnEncodedData"/>.
/// </summary>
public sealed class AsnEncodedDataEqualityComparer : SimpleRefTypeEqualityComparer<AsnEncodedData>
{
    /// <summary>
    /// The instance to use.
    /// </summary>
    public static AsnEncodedDataEqualityComparer Instance { get; } = new();

    private AsnEncodedDataEqualityComparer()
    {
    }

    /// <inheritdoc />
    protected override bool EqualsCore(AsnEncodedData x, AsnEncodedData y)
    {
        return x.Oid?.Value == y.Oid?.Value && ByteArrayEquals(x.RawData, y.RawData);
    }

    /// <inheritdoc />
    protected override int GetHashCodeCore(AsnEncodedData value)
    {
        return HashCode.Combine(value.Oid?.Value, value.RawData);
    }

    private static bool ByteArrayEquals(ReadOnlySpan<byte> a1, ReadOnlySpan<byte> a2)
    {
        return a1.SequenceEqual(a2);
    }
}

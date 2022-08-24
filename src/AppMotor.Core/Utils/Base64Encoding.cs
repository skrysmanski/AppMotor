// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.Core.IO;

using JetBrains.Annotations;

namespace AppMotor.Core.Utils;

/// <summary>
/// Represents the default Base64 encoding (as defined by RFC 4648).
/// </summary>
public class Base64Encoding : Rfc4648Encoding
{
    /// <summary>
    /// The Base64 converter with the default symbols and default padding character
    /// (see <see cref="Rfc4648Encoding.DEFAULT_PADDING_CHAR"/>).
    /// </summary>
    [PublicAPI]
    public static Base64Encoding DefaultWithPadding { get; } = new();

    /// <inheritdoc />
    public override char? PaddingChar => DEFAULT_PADDING_CHAR;

    /// <inheritdoc />
    public override int Base => 64;

    private Base64Encoding()
    {
    }

    /// <inheritdoc />
    public override string Encode(Memory<byte> data)
    {
        return Convert.ToBase64String(data.Span);
    }

    /// <inheritdoc />
    public override void Encode(IReadOnlyStream data, TextWriter outputWriter)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public override Task EncodeAsync(IReadOnlyStream data, TextWriter outputWriter)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public override byte[] Decode(string encodedString)
    {
        return Convert.FromBase64String(encodedString);
    }

    /// <inheritdoc />
    public override void Decode(TextReader encodedString, Stream destination)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public override Task DecodeAsync(TextReader encodedString, Stream destination)
    {
        throw new NotImplementedException();
    }
}
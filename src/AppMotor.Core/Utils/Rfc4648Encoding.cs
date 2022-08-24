// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.Core.IO;

using JetBrains.Annotations;

namespace AppMotor.Core.Utils;

/// <summary>
/// Represents an RFC 4648 encoding - i.e. Base16, Base32, or Base64.
///
/// <para>See: https://tools.ietf.org/html/rfc4648 </para>
/// </summary>
public abstract class Rfc4648Encoding
{
    /// <summary>
    /// The default padding character - as defined by RFC 4648.
    /// </summary>
    /// <seealso cref="PaddingChar"/>
    [PublicAPI]
    public const char DEFAULT_PADDING_CHAR = '=';

    /// <summary>
    /// The padding character (usually <see cref="DEFAULT_PADDING_CHAR"/>). If <c>null</c>, no padding will be used.
    /// </summary>
    /// <remarks>
    /// The RFC 4648 encodings encode bytes in groups of symbols. Base64 uses 4 symbols (à 6 bit) per group (3 bytes) and
    /// Base32 uses 8 symbols (à 5 bit) per group (5 bytes). If not enough input bytes are available to fill the whole group,
    /// the remaining symbols are replaced with this padding character.
    ///
    /// <para>Note that Base16 always fits nicely into a byte and thus doesn't need padding.</para>
    ///
    /// <para>Note also that padding is optional. Both encoding and decoding work fine even without padding.</para>
    /// </remarks>
    [PublicAPI]
    public abstract char? PaddingChar { get; }

    /// <summary>
    /// The base this encoding represents (i.e. 16, 32, 64).
    /// </summary>
    [PublicAPI]
    public abstract int Base { get; }

    /// <summary>
    /// Encodes the specified data as BaseX string.
    /// </summary>
    [PublicAPI, Pure]
    public abstract string Encode(Memory<byte> data);

    /// <summary>
    /// Encodes the specified data as BaseX string.
    /// </summary>
    [PublicAPI]
    public abstract void Encode(IReadOnlyStream data, TextWriter outputWriter);

    /// <summary>
    /// Encodes the specified data as BaseX string.
    /// </summary>
    [PublicAPI]
    public abstract Task EncodeAsync(IReadOnlyStream data, TextWriter outputWriter);

    /// <summary>
    /// Decodes the specified BaseX string.
    /// </summary>
    [PublicAPI, Pure]
    public abstract byte[] Decode(string encodedString);

    /// <summary>
    /// Decodes the specified BaseX string.
    /// </summary>
    [PublicAPI]
    public abstract void Decode(TextReader encodedString, Stream destination);

    /// <summary>
    /// Decodes the specified BaseX string.
    /// </summary>
    [PublicAPI]
    public abstract Task DecodeAsync(TextReader encodedString, Stream destination);

    /// <summary>
    /// Converts a list of symbols into their inverse lookup dictionary.
    /// </summary>
    [PublicAPI, MustUseReturnValue]
    public static Dictionary<char, byte> CreateInverseSymbolsDictionary(ReadOnlySpan<char> symbols)
    {
        var inverseSymbols = new Dictionary<char, byte>(symbols.Length);

        for (byte i = 0; i < symbols.Length; i++)
        {
            var symbol = symbols[i];
            if (!inverseSymbols.TryAdd(symbol, i))
            {
                throw new ArgumentException($"The list of symbols contains '{symbol}' multiple times.", nameof(symbols));
            }
        }

        return inverseSymbols;
    }
}
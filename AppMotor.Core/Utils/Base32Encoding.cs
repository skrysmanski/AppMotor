#region License
// Copyright 2020 AppMotor Framework (https://github.com/skrysmanski/AppMotor)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using AppMotor.Core.Extensions;
using AppMotor.Core.IO;

using JetBrains.Annotations;

namespace AppMotor.Core.Utils
{
    /// <summary>
    /// Represents a Base32 converter (as defined by RFC 4648) with a customizable symbol list.
    /// </summary>
    public class Base32Encoding : Rfc4648Encoding
    {
        /// <summary>
        /// The default Base32 symbols - as defined by RFC 4648.
        /// </summary>
        /// <seealso cref="DefaultWithPadding"/>
        /// <seealso cref="DefaultWithoutPadding"/>
        [PublicAPI]
        public const string DEFAULT_SYMBOLS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

        /// <summary>
        /// The Base32 converter with the default symbols (see <see cref="DEFAULT_SYMBOLS"/>) and default padding character
        /// (see <see cref="Rfc4648Encoding.DEFAULT_PADDING_CHAR"/>).
        /// </summary>
        /// <seealso cref="DefaultWithoutPadding"/>
        [PublicAPI]
        public static Base32Encoding DefaultWithPadding { get; } = new Base32Encoding(DEFAULT_SYMBOLS, paddingChar: DEFAULT_PADDING_CHAR);

        /// <summary>
        /// The Base32 converter with the default symbols (see <see cref="DEFAULT_SYMBOLS"/>) and but without padding.
        /// </summary>
        /// <seealso cref="DefaultWithPadding"/>
        [PublicAPI]
        public static Base32Encoding DefaultWithoutPadding { get; } = new Base32Encoding(DEFAULT_SYMBOLS, paddingChar: null);

        private const int BITS_PER_SYMBOL = 5;

        private const int SYMBOLS_PER_GROUP = 8;

        private const int BITS_PER_BYTE = 8;

        private const int BYTES_PER_GROUP = 5; // = BITS_PER_SYMBOL * SYMBOLS_PER_GROUP / 8 (bits per byte)

        private readonly char[] m_symbols;

        private readonly Dictionary<char, byte> m_reverseSymbols;

        /// <summary>
        /// The padding character. If <c>null</c>, no padding will be used.
        /// </summary>
        [PublicAPI]
        public sealed override char? PaddingChar { get; }

        /// <inheritdoc />
        public sealed override int Base => 32;

        [PublicAPI]
        public Base32Encoding(string symbols, char? paddingChar = DEFAULT_PADDING_CHAR)
            : this(symbols.AsNotNullArgument(nameof(symbols)).ToCharArray(), createCopyOfSymbols: false, paddingChar)
        {
        }

        [PublicAPI]
        public Base32Encoding(char[] symbols, char? paddingChar = DEFAULT_PADDING_CHAR)
            : this(symbols, createCopyOfSymbols: true, paddingChar)
        {
        }

        private Base32Encoding(char[] symbols, bool createCopyOfSymbols, char? paddingChar)
        {
            Validate.Argument.IsNotNull(symbols, nameof(symbols));

            if (symbols.Length != 32)
            {
                throw new ArgumentException($"You need to specify at 32 symbols (but only specified {symbols.Length}).", nameof(symbols));
            }

            if (createCopyOfSymbols)
            {
                this.m_symbols = (char[])symbols.Clone();
            }
            else
            {
                this.m_symbols = symbols;
            }

            this.m_reverseSymbols = new Dictionary<char, byte>(this.m_symbols.Length);
            for (byte i = 0; i < this.m_symbols.Length; i++)
            {
                var symbol = this.m_symbols[i];
                if (!this.m_reverseSymbols.TryAdd(symbol, i))
                {
                    throw new ArgumentException($"The list of symbols contains '{symbol}' multiple times.", nameof(symbols));
                }
            }

            if (paddingChar != null)
            {
                if (this.m_reverseSymbols.ContainsKey(paddingChar.Value))
                {
                    throw new ArgumentException($"The padding character ('{paddingChar}') must not be in the list of symbols.", nameof(paddingChar));
                }

                this.PaddingChar = paddingChar;
            }
        }

        public override string Encode(Memory<byte> data)
        {
            if (data.Length == 0)
            {
                return "";
            }

            var resultBuilder = new StringBuilder(CalcEncodedStringLength(data.Length));

            using var encoder = new MemoryBasedSymbolGroupEncoder(data, this.m_symbols, this.PaddingChar);

            while (true)
            {
                var readSymbols = encoder.EncodeNextGroup();
                if (readSymbols.Count == 0)
                {
                    break;
                }

                resultBuilder.Append(readSymbols.AsSpan());
            }

            return resultBuilder.ToString();
        }

        public override void Encode(IReadOnlyStream data, TextWriter outputWriter)
        {
            Validate.Argument.IsNotNull(outputWriter, nameof(outputWriter));
            Validate.Argument.IsNotNull(data, nameof(data));

            using var encoder = new StreamBasedSymbolGroupEncoder(data, this.m_symbols, this.PaddingChar);

            while (true)
            {
                var readSymbols = encoder.EncodeNextGroup();
                if (readSymbols.Count == 0)
                {
                    break;
                }

                outputWriter.Write(readSymbols.AsSpan());
            }
        }

        public override async Task EncodeAsync(IReadOnlyStream data, TextWriter outputWriter)
        {
            Validate.Argument.IsNotNull(outputWriter, nameof(outputWriter));
            Validate.Argument.IsNotNull(data, nameof(data));

            using var encoder = new StreamBasedSymbolGroupEncoder(data, this.m_symbols, this.PaddingChar);

            while (true)
            {
                var readSymbols = await encoder.EncodeNextGroupAsync().ConfigureAwait(false);
                if (readSymbols.Count == 0)
                {
                    break;
                }

                await outputWriter.WriteAsync(readSymbols.AsMemory()).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Returns the length of a Base32 string based on the specified number of bytes. Note
        /// that for simplicity reasons, we don't return a smaller number if padding is disabled.
        /// </summary>
        [Pure]
        private static int CalcEncodedStringLength(int byteCount)
        {
            int groupCount;

            if (byteCount % BYTES_PER_GROUP == 0)
            {
                groupCount = byteCount / BYTES_PER_GROUP;
            }
            else
            {
                groupCount = (int)Math.Ceiling((double)byteCount / BYTES_PER_GROUP);
            }

            return groupCount * SYMBOLS_PER_GROUP;
        }

        public override byte[] Decode(string encodedString)
        {
            Validate.Argument.IsNotNull(encodedString, nameof(encodedString));

            if (encodedString.Length == 0)
            {
                return Array.Empty<byte>();
            }

            int bufferSize = CalcDecodedByteCount(encodedString.Length);
            byte[] sharedWriteBuffer = ArrayPool<byte>.Shared.Rent(bufferSize);
            try
            {
                using var decoder = new StringBasedSymbolGroupDecoder(encodedString, this.m_reverseSymbols, this.PaddingChar);

                int offset = 0;

                while (true)
                {
                    var nextDecodedGroup = decoder.DecodeNextGroup();
                    if (nextDecodedGroup.Count == 0)
                    {
                        break;
                    }

                    nextDecodedGroup.CopyTo(sharedWriteBuffer, offset);
                    offset += nextDecodedGroup.Count;
                }

                var decodedBytes = new byte[offset];
                sharedWriteBuffer.AsSpan(0, offset).CopyTo(decodedBytes);

                return decodedBytes;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(sharedWriteBuffer);
            }
        }

        public override void Decode(TextReader encodedString, Stream destination)
        {
            Validate.Argument.IsNotNull(encodedString, nameof(encodedString));
            Validate.Argument.IsNotNull(destination, nameof(destination));

            using var decoder = new StringReaderBasedSymbolGroupDecoder(encodedString, this.m_reverseSymbols, this.PaddingChar);

            while (true)
            {
                var nextDecodedGroup = decoder.DecodeNextGroup();
                if (nextDecodedGroup.Count == 0)
                {
                    break;
                }

                destination.Write(nextDecodedGroup);
            }
        }

        public override async Task DecodeAsync(TextReader encodedString, Stream destination)
        {
            Validate.Argument.IsNotNull(encodedString, nameof(encodedString));
            Validate.Argument.IsNotNull(destination, nameof(destination));

            using var decoder = new StringReaderBasedSymbolGroupDecoder(encodedString, this.m_reverseSymbols, this.PaddingChar);

            while (true)
            {
                var nextDecodedGroup = await decoder.DecodeNextGroupAsync().ConfigureAwait(false);
                if (nextDecodedGroup.Count == 0)
                {
                    break;
                }

                await destination.WriteAsync(nextDecodedGroup).ConfigureAwait(false);
            }
        }

        [Pure]
        private static int CalcDecodedByteCount(int encodedStringLength)
        {
            int groupCount;

            if (encodedStringLength % SYMBOLS_PER_GROUP == 0)
            {
                groupCount = encodedStringLength / SYMBOLS_PER_GROUP;
            }
            else
            {
                groupCount = (int)Math.Ceiling((double)encodedStringLength / SYMBOLS_PER_GROUP);
            }

            return groupCount * BYTES_PER_GROUP;
        }

        private abstract class SymbolGroupEncoder : Disposable
        {
            private const ulong SYMBOL_BIT_MASK = (1 << BITS_PER_SYMBOL) - 1;

            private readonly char[] m_symbols;

            private readonly char? m_paddingChar;

            private readonly char[] m_encodeBuffer;

            protected SymbolGroupEncoder(char[] symbols, char? paddingChar)
            {
                this.m_symbols = symbols;
                this.m_paddingChar = paddingChar;

                this.m_encodeBuffer = ArrayPool<char>.Shared.Rent(SYMBOLS_PER_GROUP);
            }

            /// <inheritdoc />
            protected override void DisposeManagedResources()
            {
                ArrayPool<char>.Shared.Return(this.m_encodeBuffer);

                base.DisposeManagedResources();
            }

            [MustUseReturnValue]
            protected ArraySegment<char> EncodeGroup(ReadOnlySpan<byte> readBytes)
            {
                ulong allBits = 0;

                for (int i = 0; i < readBytes.Length; i++)
                {
                    allBits |= (ulong)readBytes[i] << ((BYTES_PER_GROUP - i - 1) * BITS_PER_BYTE);
                }

                // Byte layout:
                //
                //      4       3        2        1        0
                // +--------+--------+--------+--------+--------+
                // |< 1 >< 2| >< 3 ><|.4 >< 5.|>< 6 ><.|7 >< 8 >|
                // +--------+--------+--------+--------+--------+
                //

                int symbolCount;

                Span<byte> values = stackalloc byte[SYMBOLS_PER_GROUP];

                values[0] = (byte)((allBits >> ((SYMBOLS_PER_GROUP - 1) * BITS_PER_SYMBOL)) & SYMBOL_BIT_MASK);
                values[1] = (byte)((allBits >> ((SYMBOLS_PER_GROUP - 2) * BITS_PER_SYMBOL)) & SYMBOL_BIT_MASK);

                if (readBytes.Length > 1)
                {
                    values[2] = (byte)((allBits >> ((SYMBOLS_PER_GROUP - 3) * BITS_PER_SYMBOL)) & SYMBOL_BIT_MASK);
                    values[3] = (byte)((allBits >> ((SYMBOLS_PER_GROUP - 4) * BITS_PER_SYMBOL)) & SYMBOL_BIT_MASK);

                    if (readBytes.Length > 2)
                    {
                        values[4] = (byte)((allBits >> ((SYMBOLS_PER_GROUP - 5) * BITS_PER_SYMBOL)) & SYMBOL_BIT_MASK);

                        if (readBytes.Length > 3)
                        {
                            values[5] = (byte)((allBits >> ((SYMBOLS_PER_GROUP - 6) * BITS_PER_SYMBOL)) & SYMBOL_BIT_MASK);
                            values[6] = (byte)((allBits >> ((SYMBOLS_PER_GROUP - 7) * BITS_PER_SYMBOL)) & SYMBOL_BIT_MASK);

                            if (readBytes.Length == 5)
                            {
                                values[7] = (byte)((allBits >> ((SYMBOLS_PER_GROUP - 8) * BITS_PER_SYMBOL)) & SYMBOL_BIT_MASK);
                                symbolCount = 8;
                            }
                            else
                            {
                                symbolCount = 7;
                            }
                        }
                        else
                        {
                            symbolCount = 5;
                        }
                    }
                    else
                    {
                        symbolCount = 4;
                    }
                }
                else
                {
                    symbolCount = 2;
                }

                for (int i = 0; i < symbolCount; i++)
                {
                    this.m_encodeBuffer[i] = this.m_symbols[values[i]];
                }

                if (symbolCount < SYMBOLS_PER_GROUP && this.m_paddingChar != null)
                {
                    for (int i = symbolCount; i < SYMBOLS_PER_GROUP; i++)
                    {
                        this.m_encodeBuffer[i] = this.m_paddingChar.Value;
                    }

                    return this.m_encodeBuffer.Slice(0, SYMBOLS_PER_GROUP);
                }

                return this.m_encodeBuffer.Slice(0, symbolCount);
            }
        }

        private sealed class MemoryBasedSymbolGroupEncoder : SymbolGroupEncoder
        {
            private readonly Memory<byte> m_data;

            private int m_offset;

            private int m_count;

            public MemoryBasedSymbolGroupEncoder(Memory<byte> data, char[] symbols, char? paddingChar)
                : base(symbols, paddingChar)
            {
                this.m_data = data;
                this.m_count = data.Length;
            }

            [MustUseReturnValue]
            public ArraySegment<char> EncodeNextGroup()
            {
                if (this.m_count == 0)
                {
                    return ArraySegment<char>.Empty;
                }

                int bytesToRead = this.m_count < BYTES_PER_GROUP ? this.m_count : BYTES_PER_GROUP;

                var symbols = EncodeGroup(this.m_data.Slice(this.m_offset, bytesToRead).Span);

                this.m_offset += bytesToRead;
                this.m_count -= bytesToRead;

                return symbols;
            }
        }

        private sealed class StreamBasedSymbolGroupEncoder : SymbolGroupEncoder
        {
            private readonly IReadOnlyStream m_dataStream;

            public StreamBasedSymbolGroupEncoder(IReadOnlyStream dataStream, char[] symbols, char? paddingChar)
                : base(symbols, paddingChar)
            {
                this.m_dataStream = dataStream;
            }

            [MustUseReturnValue]
            public ArraySegment<char> EncodeNextGroup()
            {
                Span<byte> readBuffer = stackalloc byte[BYTES_PER_GROUP];

                int readBytes = this.m_dataStream.ReadUntilFull(readBuffer);
                if (readBytes == 0)
                {
                    return ArraySegment<char>.Empty;
                }

                return EncodeGroup(readBuffer.Slice(0, readBytes));
            }

            [MustUseReturnValue]
            public async Task<ArraySegment<char>> EncodeNextGroupAsync()
            {
                var readBuffer = ArrayPool<byte>.Shared.Rent(BYTES_PER_GROUP);
                try
                {
                    int readBytes = await this.m_dataStream.ReadUntilFullAsync(readBuffer.AsMemory(0, BYTES_PER_GROUP)).ConfigureAwait(false);
                    if (readBytes == 0)
                    {
                        return ArraySegment<char>.Empty;
                    }

                    return EncodeGroup(readBuffer.AsSpan(0, readBytes));
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(readBuffer);
                }
            }
        }

        private abstract class SymbolGroupDecoder : Disposable
        {
            private const int BYTE_BIT_MASK = 0xFF;

            private readonly Dictionary<char, byte> m_reverseSymbols;

            private readonly char? m_paddingChar;

            private readonly byte[] m_decoderBuffer;

            protected SymbolGroupDecoder(Dictionary<char, byte> reverseSymbols, char? paddingChar)
            {
                this.m_reverseSymbols = reverseSymbols;
                this.m_paddingChar = paddingChar;

                this.m_decoderBuffer = ArrayPool<byte>.Shared.Rent(BYTES_PER_GROUP);
            }

            /// <inheritdoc />
            protected override void DisposeManagedResources()
            {
                ArrayPool<byte>.Shared.Return(this.m_decoderBuffer);

                base.DisposeManagedResources();
            }

            protected ArraySegment<byte> DecodeGroup(ReadOnlySpan<char> symbolGroup)
            {
                ulong allBits = 0;

                int symbolCount;

                for (symbolCount = 0; symbolCount < symbolGroup.Length; symbolCount++)
                {
                    var symbol = symbolGroup[symbolCount];

                    if (symbol == this.m_paddingChar)
                    {
                        break;
                    }

                    if (!this.m_reverseSymbols.TryGetValue(symbol, out var value))
                    {
                        throw new FormatException($"The symbol '{symbol}' is not a valid Base32 symbol.");
                    }

                    allBits |= (ulong)value << ((SYMBOLS_PER_GROUP - symbolCount - 1) * BITS_PER_SYMBOL);
                }

                int byteCountToWrite;

                switch (symbolCount)
                {
                    case 2:
                        byteCountToWrite = 1;
                        break;
                    case 4:
                        byteCountToWrite = 2;
                        break;
                    case 5:
                        byteCountToWrite = 3;
                        break;
                    case 7:
                        byteCountToWrite = 4;
                        break;
                    case 8:
                        byteCountToWrite = 5;
                        break;

                    default:
                        throw new FormatException($"This is not a valid Base32 string. (invalid symbol group size: {symbolGroup.Length})");
                }

                for (int i = 0; i < byteCountToWrite; i++)
                {
                    byte byteToWrite = (byte)((allBits >> ((BYTES_PER_GROUP - i - 1) * BITS_PER_BYTE)) & BYTE_BIT_MASK);
                    this.m_decoderBuffer[i] = byteToWrite;
                }

                return this.m_decoderBuffer.Slice(0, byteCountToWrite);
            }
        }

        private sealed class StringBasedSymbolGroupDecoder : SymbolGroupDecoder
        {
            private readonly string m_encodedString;

            private int m_offset;

            private int m_count;

            /// <inheritdoc />
            public StringBasedSymbolGroupDecoder(string encodedString, Dictionary<char, byte> reverseSymbols, char? paddingChar)
                : base(reverseSymbols, paddingChar)
            {
                this.m_encodedString = encodedString;
                this.m_count = encodedString.Length;
            }

            [MustUseReturnValue]
            public ArraySegment<byte> DecodeNextGroup()
            {
                if (this.m_count == 0)
                {
                    return ArraySegment<byte>.Empty;
                }

                int charsToRead = this.m_count < SYMBOLS_PER_GROUP ? this.m_count : SYMBOLS_PER_GROUP;

                var nextEncodedGroup = this.m_encodedString.AsSpan(this.m_offset, charsToRead);

                var decodedGroup = DecodeGroup(nextEncodedGroup);

                this.m_offset += charsToRead;
                this.m_count -= charsToRead;

                return decodedGroup;
            }
        }

        private sealed class StringReaderBasedSymbolGroupDecoder : SymbolGroupDecoder
        {
            private readonly TextReader m_stringReader;

            /// <inheritdoc />
            public StringReaderBasedSymbolGroupDecoder(TextReader stringReader, Dictionary<char, byte> reverseSymbols, char? paddingChar)
                : base(reverseSymbols, paddingChar)
            {
                this.m_stringReader = stringReader;
            }

            [MustUseReturnValue]
            public ArraySegment<byte> DecodeNextGroup()
            {
                Span<char> readBuffer = stackalloc char[SYMBOLS_PER_GROUP];

                int readChars = this.m_stringReader.Read(readBuffer);
                if (readChars == 0)
                {
                    return ArraySegment<byte>.Empty;
                }

                return DecodeGroup(readBuffer.Slice(0, readChars));
            }

            [MustUseReturnValue]
            public async Task<ArraySegment<byte>> DecodeNextGroupAsync()
            {
                var readBuffer = ArrayPool<char>.Shared.Rent(SYMBOLS_PER_GROUP);
                try
                {
                    int readChars = await this.m_stringReader.ReadAsync(readBuffer.AsMemory(0, SYMBOLS_PER_GROUP)).ConfigureAwait(false);
                    if (readChars == 0)
                    {
                        return ArraySegment<byte>.Empty;
                    }

                    return DecodeGroup(readBuffer.AsSpan(0, readChars));
                }
                finally
                {
                    ArrayPool<char>.Shared.Return(readBuffer);
                }
            }
        }
    }
}

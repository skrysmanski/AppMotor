﻿#region License
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
using System.IO;
using System.Threading.Tasks;

using AppMotor.Core.IO;

using JetBrains.Annotations;

namespace AppMotor.Core.Utils
{
    /// <summary>
    /// Represents an RFC 4648 encoding - i.e. Base16, Base32, or Base64.
    ///
    /// <para>See: https://tools.ietf.org/html/rfc4648 </para>
    /// </summary>
    public abstract class Rfc4648Encoding
    {
        /// <summary>
        /// The padding character. If <c>null</c>, no padding will be used.
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
        public abstract void Encode(IReadOnlyStream data, StringWriter outputWriter);

        /// <summary>
        /// Encodes the specified data as BaseX string.
        /// </summary>
        [PublicAPI]
        public abstract Task EncodeAsync(IReadOnlyStream data, StringWriter outputWriter);

        /// <summary>
        /// Decodes the specified BaseX string.
        /// </summary>
        [PublicAPI, Pure]
        public abstract byte[] Decode(string encodedString);

        /// <summary>
        /// Decodes the specified BaseX string.
        /// </summary>
        [PublicAPI]
        public abstract void Decode(StringReader encodedString, Stream destination);

        /// <summary>
        /// Decodes the specified BaseX string.
        /// </summary>
        [PublicAPI]
        public abstract Task DecodeAsync(StringReader encodedString, Stream destination);
    }
}
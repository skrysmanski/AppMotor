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
using System.IO;
using System.Threading.Tasks;

using AppMotor.Core.IO;

using JetBrains.Annotations;

namespace AppMotor.Core.Utils
{
    public class Base64Encoding : Rfc4648Encoding
    {
        /// <summary>
        /// The Base64 converter with the default symbols and default padding character
        /// (see <see cref="Rfc4648Encoding.DEFAULT_PADDING_CHAR"/>).
        /// </summary>
        [PublicAPI]
        public static Base64Encoding DefaultWithPadding { get; } = new Base64Encoding();

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
}

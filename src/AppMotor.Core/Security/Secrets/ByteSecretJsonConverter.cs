#region License
// Copyright 2021 AppMotor Framework (https://github.com/skrysmanski/AppMotor)
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
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AppMotor.Core.Security.Secrets
{
    internal sealed class ByteSecretJsonConverter : JsonConverter<SecretBytes>
    {
        private readonly List<SecretBytes> _createdSecrets = new();

        public IReadOnlyList<SecretBytes> CreatedSecrets => this._createdSecrets;

        /// <inheritdoc />
        public override SecretBytes? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.HasValueSequence)
            {
                throw new NotSupportedException("ValueSequence is not supported.");
            }

            ReadOnlySpan<byte> span = reader.ValueSpan;
            using var base64EncodedSecret = SecretBytes.FromInMemorySource(span);
            using var base64EncodedStringSecret = base64EncodedSecret.ToStringSecret(SecretString.SupportedEncodings.Ascii);

            var byteSecret = new SecretBytes(span.Length);
            var underlyingArray = byteSecret.GetUnderlyingArray();

            if (!Convert.TryFromBase64Chars(base64EncodedStringSecret.Span, underlyingArray.UnderlyingArray, out int bytesWritten))
            {
                byteSecret.Dispose();
                throw new JsonException("Could not decode Base64 string.");
            }

            underlyingArray.SetLength(bytesWritten);

            this._createdSecrets.Add(byteSecret);

            return byteSecret;
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, SecretBytes value, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }
    }
}

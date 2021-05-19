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
using System.Text;

using AppMotor.Core.Exceptions;
using AppMotor.Core.Utils;

namespace AppMotor.Core.Security.Secrets
{
    /// <summary>
    /// An immutable secret consisting of characters.
    /// </summary>
    /// <seealso cref="SecretBytes"/>
    public sealed class SecretString : Disposable
    {
        private static readonly UTF8Encoding UTF8_ENCODING = new();

        private readonly SecretsArray<char> _secretData;

        public ReadOnlySpan<char> Span => this._secretData.Span;

        public SecretString(SecretBytes byteSecret, SupportedEncodings encoding)
        {
            switch (encoding)
            {
                case SupportedEncodings.Ascii:
                    this._secretData = DecodeAscii(byteSecret);
                    break;

                case SupportedEncodings.Utf16:
                    this._secretData = DecodeUtf16(byteSecret);
                    break;

                case SupportedEncodings.Utf8:
                    this._secretData = DecodeUtf8(byteSecret);
                    break;

                default:
                    throw new UnexpectedSwitchValueException(nameof(encoding), encoding);
            }
        }

        private static SecretsArray<char> DecodeAscii(SecretBytes byteSecret)
        {
            var source = byteSecret.Span;

            var secretData = new SecretsArray<char>(source.Length);

            try
            {
                var target = secretData.UnderlyingArray;

                for (int i = 0; i < target.Length; i++)
                {
                    // TODO: Verify conversion
                    target[i] = (char)(source[i] & 0x7F);
                }

                return secretData;
            }
            catch (Exception)
            {
                secretData.Dispose();
                throw;
            }
        }

        private static SecretsArray<char> DecodeUtf16(SecretBytes byteSecret)
        {
            var source = byteSecret.Span;

            if (source.Length % 2 != 0)
            {
                throw new ArgumentException("The input's length is not dividable by two.");
            }

            var secretData = new SecretsArray<char>(source.Length / 2);

            try
            {
                var target = secretData.UnderlyingArray;

                for (int i = 0; i < target.Length; i++)
                {
                    // TODO: Verify conversion
                    target[i] = (char)((source[i * 2] << 8) | source[i * 2 + 1]);
                }

                return secretData;
            }
            catch (Exception)
            {
                secretData.Dispose();
                throw;
            }
        }

        private static SecretsArray<char> DecodeUtf8(SecretBytes byteSecret)
        {
            var source = byteSecret.GetUnderlyingArray();

            var secretData = new SecretsArray<char>(UTF8_ENCODING.GetMaxCharCount(source.Length));

            try
            {
                int actualChars = UTF8_ENCODING.GetChars(source.UnderlyingArray, 0, source.Length, secretData.UnderlyingArray, 0);
                secretData.SetLength(actualChars);

                return secretData;
            }
            catch (Exception)
            {
                secretData.Dispose();
                throw;
            }
        }

        /// <inheritdoc />
        protected override void DisposeManagedResources()
        {
            this._secretData.Dispose();
        }

        public enum SupportedEncodings
        {
            Ascii,
            Utf8,
            Utf16,
        }
    }
}

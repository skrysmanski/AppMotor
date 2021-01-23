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
using System.Security.Cryptography;

using AppMotor.Core.Utils;

namespace AppMotor.Core.Certificates
{
    public sealed class AsnEncodedDataComparer : SimpleRefTypeEqualityComparer<AsnEncodedData>
    {
        public static AsnEncodedDataComparer Instance { get; } = new();

        private AsnEncodedDataComparer()
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
}

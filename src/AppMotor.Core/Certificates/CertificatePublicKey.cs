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
using System.Security.Cryptography.X509Certificates;

namespace AppMotor.Core.Certificates
{
    public sealed class CertificatePublicKey : IEquatable<CertificatePublicKey>
    {
        /// <summary>
        /// The OID of the public key's algorithm (e.g. RSA). See <see cref="KeyAlgorithm"/> for the translation.
        /// </summary>
        public CertificateOid Oid { get; }

        /// <summary>
        /// The key algorithm used to create this public key.
        /// </summary>
        public CertificateKeyAlgorithms KeyAlgorithm { get; }

        private PublicKey UnderlyingValue { get; }

        public CertificatePublicKey(PublicKey source)
        {
            this.UnderlyingValue = source;
            this.Oid = source.Oid;
            this.KeyAlgorithm = CertificateKeyAlgorithmOids.GetAlgorithmFromOid(source.Oid);
        }

        /// <inheritdoc />
        public bool Equals(CertificatePublicKey? other)
        {
            if (other is null)
            {
                return false;
            }
            else if (ReferenceEquals(this, other))
            {
                return true;
            }
            else
            {
                return this.Oid == other.Oid
                    && AsnEncodedDataComparer.Instance.Equals(this.UnderlyingValue.EncodedKeyValue, other.UnderlyingValue.EncodedKeyValue)
                    && AsnEncodedDataComparer.Instance.Equals(this.UnderlyingValue.EncodedParameters, other.UnderlyingValue.EncodedParameters);
            }
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is CertificatePublicKey other && Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(this.Oid, AsnEncodedDataComparer.Instance.GetHashCode(this.UnderlyingValue.EncodedKeyValue));
        }

        public static bool operator ==(CertificatePublicKey? left, CertificatePublicKey? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CertificatePublicKey? left, CertificatePublicKey? right)
        {
            return !Equals(left, right);
        }
    }
}

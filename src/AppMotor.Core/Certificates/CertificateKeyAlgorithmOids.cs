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

using JetBrains.Annotations;

namespace AppMotor.Core.Certificates
{
    public static class CertificateKeyAlgorithmOids
    {
        // ReSharper disable InconsistentNaming

        /// <summary>
        /// The raw value for <see cref="RSA"/>.
        /// </summary>
        [PublicAPI]
        public const string RSA_VALUE = "1.2.840.113549.1.1.1";

        /// <summary>
        /// The OID for RSA.
        /// </summary>
        [PublicAPI]
        public static CertificateOid RSA { get; } = new(RSA_VALUE);

        /// <summary>
        /// The raw value for <see cref="DSA"/>.
        /// </summary>
        [PublicAPI]
        public const string DSA_VALUE = "1.2.840.10040.4.1";

        /// <summary>
        /// The OID for RSA.
        /// </summary>
        [PublicAPI]
        public static CertificateOid DSA { get; } = new(DSA_VALUE);

        /// <summary>
        /// The raw value for <see cref="DSA"/>.
        /// </summary>
        [PublicAPI]
        public const string ECDSA_VALUE = "1.2.840.10045.2.1";

        /// <summary>
        /// The OID for RSA.
        /// </summary>
        [PublicAPI]
        public static CertificateOid ECDsa { get; } = new(ECDSA_VALUE);

        // ReSharper restore InconsistentNaming

        /// <summary>
        /// Returns the <see cref="CertificateKeyAlgorithms"/> for the specified OID. Returns
        /// <see cref="CertificateKeyAlgorithms.Other"/> if the OID is unknown.
        /// </summary>
        [PublicAPI, MustUseReturnValue]
        public static CertificateKeyAlgorithms GetAlgorithmFromOid(CertificateOid oid)
        {
            switch (oid.Value)
            {
                case RSA_VALUE:
                    return CertificateKeyAlgorithms.RSA;

                case DSA_VALUE:
                    return CertificateKeyAlgorithms.DSA;

                case ECDSA_VALUE:
                    return CertificateKeyAlgorithms.ECDSA;

                default:
                    return CertificateKeyAlgorithms.Other;
            }
        }
    }
}

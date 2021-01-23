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

namespace AppMotor.Core.Certificates
{
    public enum CertificateKeyAlgorithms
    {
        Other,

        // ReSharper disable InconsistentNaming

        /// <summary>
        /// RSA (Rivest–Shamir–Adleman) is the default key algorithm for certificates as of 2021.
        /// It's widely supported. The minimum recommended key length for RSA keys is 2048.
        /// </summary>
        RSA,

        /// <summary>
        /// DSA (Digital Signature Algorithm) is the key algorithm primarily used for digital
        /// signature. As such, it is not widely supported for certificates.
        /// </summary>
        /// <seealso cref="ECDSA"/>
        DSA,

        /// <summary>
        /// ECDSA (Elliptic Curve Digital Signature Algorithm) is related to DSA and uses ECC (Elliptic Curve Cryptography).
        /// ECDSA requires a smaller key size than RSA. Because of this, performance is better.
        /// </summary>
        ECDSA,

        // ReSharper restore InconsistentNaming
    }
}

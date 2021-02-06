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
using System.Security.Cryptography.X509Certificates;

using AppMotor.Core.Utils;

using JetBrains.Annotations;

namespace AppMotor.Core.Certificates
{
    public sealed class TlsCertificateRequest : Disposable
    {
        private readonly CertificateRequest _underlyingCertificateRequest;

        private readonly RSA _rsaAlgorithm;

        public TlsCertificateRequest(string hostname)
        {
            Validate.ArgumentWithName(nameof(hostname)).IsNotNullOrWhiteSpace(hostname);

            this._rsaAlgorithm = RSA.Create();

            // TODO: Add support multiple hostnames (SAN)
            this._underlyingCertificateRequest = new CertificateRequest(
                $"cn={hostname}",
                this._rsaAlgorithm,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1
            );
        }

        /// <inheritdoc />
        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();

            this._rsaAlgorithm.Dispose();
        }

        /// <summary>
        /// Creates a self signed certificate from this certificate request.
        /// </summary>
        /// <param name="certificateLifetime">for how long this certificate is to be valid - from now.</param>
        [PublicAPI, MustUseReturnValue]
        public TlsCertificate CreateSelfSignedCertificate(TimeSpan certificateLifetime)
        {
            var cert = this._underlyingCertificateRequest.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now + certificateLifetime);

            // A self signed certificate is automatically exportable.
            return new TlsCertificate(cert, allowPrivateKeyExport: true);
        }
    }
}

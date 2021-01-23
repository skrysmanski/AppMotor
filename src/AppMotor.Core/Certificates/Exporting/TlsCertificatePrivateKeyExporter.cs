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
using System.Text;

using JetBrains.Annotations;

namespace AppMotor.Core.Certificates.Exporting
{
    public sealed class TlsCertificatePrivateKeyExporter
    {
        private readonly TlsCertificate _certificate;

        internal TlsCertificatePrivateKeyExporter(TlsCertificate certificate)
        {
            this._certificate = certificate;
        }

        public SingleBlobExporter AsPfx()
        {
            return new(this._certificate.UnderlyingCertificate.Export(X509ContentType.Pfx));
        }

        public DoubleBlobExporter AsDer()
        {
            return new(
                publicKeyBytes: this._certificate.UnderlyingCertificate.Export(X509ContentType.Cert),
                privateKeyBytes: ExportPrivateKey()
            );
        }

        public DoubleBlobExporter AsPem()
        {
            byte[] privateKeyBytes;

            if (this._certificate.KeyAlgorithm == CertificateKeyAlgorithms.RSA)
            {
                privateKeyBytes = ConvertToPem(ExportPrivateKey(), "RSA PRIVATE KEY");
            }
            else
            {
                throw new NotSupportedException($"Exporting private key of type '{this._certificate.KeyAlgorithm}' is not supported.");
            }

            return new DoubleBlobExporter(
                publicKeyBytes: ConvertToPem(this._certificate.UnderlyingCertificate.Export(X509ContentType.Cert), "CERTIFICATE"),
                privateKeyBytes: privateKeyBytes
            );
        }

        private static byte[] ConvertToPem(byte[] bytes, string type)
        {
            var outputBuilder = new StringBuilder();

            outputBuilder.Append($"-----BEGIN {type}-----\r\n");
            outputBuilder.Append(Convert.ToBase64String(bytes, Base64FormattingOptions.InsertLineBreaks));
            outputBuilder.Append($"\r\n-----END {type}-----");

            return Encoding.ASCII.GetBytes(outputBuilder.ToString());
        }

        [MustUseReturnValue]
        internal byte[] ExportPrivateKey()
        {
            AsymmetricAlgorithm? privateKey = this._certificate.UnderlyingCertificate.PrivateKey;

            if (privateKey is null)
            {
                throw new InvalidOperationException("This certificate has no private key.");
            }

            if (privateKey is RSA rsaPrivateKey)
            {
                return ExportRsaPrivateKey(rsaPrivateKey);
            }
            else
            {
                throw new NotSupportedException($"Exporting private keys of type '{privateKey.GetType().Name}' is not supported.");
            }
        }

        /// <summary>
        /// Exports the private key into a byte array.
        /// </summary>
        [MustUseReturnValue]
        private static byte[] ExportRsaPrivateKey(RSA privateKey)
        {
            const string NOT_SECRET_PASSWORD = "passwd";

            //
            // IMPORTANT: At least on Windows, we can't just use "privateKey.ExportRSAPrivateKey()"
            //   because this method does not always work (it throws a "NotSupportedException" in
            //   this case). This is why we have this workaround where we export the private key
            //   encrypted and then reimport it afterwards.
            //

            byte[] encryptedPrivateKey = privateKey.ExportEncryptedPkcs8PrivateKey(
                NOT_SECRET_PASSWORD,
                new PbeParameters(
                    PbeEncryptionAlgorithm.Aes128Cbc,
                    HashAlgorithmName.SHA256,
                    // Only 1 iteration because we don't need to secure the exported value
                    // since it's immediately re-imported.
                    iterationCount: 1
                )
            );

            // ReSharper disable once IdentifierTypo
            using var rsaReexporter = RSA.Create();

            rsaReexporter.ImportEncryptedPkcs8PrivateKey(NOT_SECRET_PASSWORD, encryptedPrivateKey, bytesRead: out _);

            return rsaReexporter.ExportRSAPrivateKey();
        }
    }
}

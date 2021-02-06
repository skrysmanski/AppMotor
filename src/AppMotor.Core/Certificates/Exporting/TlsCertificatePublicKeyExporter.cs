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
using System.Text;

using AppMotor.Core.Exceptions;

using JetBrains.Annotations;

namespace AppMotor.Core.Certificates.Exporting
{
    public sealed class TlsCertificatePublicKeyExporter
    {
        private readonly TlsCertificate _certificate;

        internal TlsCertificatePublicKeyExporter(TlsCertificate certificate)
        {
            this._certificate = certificate;
        }

        [PublicAPI, MustUseReturnValue]
        public SingleBlobExporter AsPfx()
        {
            return new(() =>
                {
                    using var tempCert = new X509Certificate2(this._certificate.UnderlyingCertificate.Export(X509ContentType.Cert));
                    return tempCert.Export(X509ContentType.Pfx);
                }
            );
        }

        [PublicAPI, MustUseReturnValue]
        public SingleBlobExporter AsPem()
        {
            return new(
                () =>
                {
                    var outputBuilder = new StringBuilder();

                    outputBuilder.Append("-----BEGIN CERTIFICATE-----\r\n");
                    outputBuilder.Append(
                        Convert.ToBase64String(
                            this._certificate.UnderlyingCertificate.Export(X509ContentType.Cert),
                            Base64FormattingOptions.InsertLineBreaks
                        )
                    );
                    outputBuilder.Append("\r\n-----END CERTIFICATE-----");

                    return Encoding.UTF8.GetBytes(outputBuilder.ToString());
                }
            );
        }

        [PublicAPI, MustUseReturnValue]
        public SingleBlobExporter As(CertificateFileFormats exportFormat)
        {
            switch (exportFormat)
            {
                case CertificateFileFormats.PFX:
                    return AsPfx();

                case CertificateFileFormats.PEM:
                    return AsPem();

                default:
                    throw new UnexpectedSwitchValueException(nameof(exportFormat), exportFormat);
            }
        }
    }
}

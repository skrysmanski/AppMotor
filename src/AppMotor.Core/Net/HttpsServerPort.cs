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

using AppMotor.Core.Certificates;
using AppMotor.Core.Utils;

using JetBrains.Annotations;

namespace AppMotor.Core.Net
{
    /// <summary>
    /// Represents an HTTPS server port.
    /// </summary>
    public class HttpsServerPort : HttpServerPort
    {
        /// <summary>
        /// The default HTTPS port.
        /// </summary>
        [PublicAPI]
        public new const int DEFAULT_PORT = 443;

        /// <summary>
        /// This function returns the HTTPS certificate to be used. Note that the certificate
        /// must contain a private key.
        /// </summary>
        /// <remarks>
        /// For security reasons, this property is a function rather than storing the certificate itself. This way
        /// it doesn't keep the certificate in memory.
        /// </remarks>
        [PublicAPI]
        public Func<TlsCertificate> CertificateProvider { get; }

        /// <summary>
        /// Constructor. Uses <see cref="DEFAULT_PORT"/> as port.
        /// </summary>
        /// <param name="listenAddress">From where to accept connections.</param>
        /// <param name="certificateProvider">Provides the certificate to be used; see <see cref="CertificateProvider"/> for more details.</param>
        [PublicAPI]
        public HttpsServerPort(SocketListenAddresses listenAddress, Func<TlsCertificate> certificateProvider) : this(listenAddress, port: DEFAULT_PORT, certificateProvider)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="listenAddress">From where to accept connections.</param>
        /// <param name="port">The port to use</param>
        /// <param name="certificateProvider">Provides the certificate to be used; see <see cref="CertificateProvider"/> for more details.</param>
        [PublicAPI]
        public HttpsServerPort(SocketListenAddresses listenAddress, int port, Func<TlsCertificate> certificateProvider) : base(listenAddress, port)
        {
            Validate.ArgumentWithName(nameof(certificateProvider)).IsNotNull(certificateProvider);

            this.CertificateProvider = certificateProvider;
        }
    }
}

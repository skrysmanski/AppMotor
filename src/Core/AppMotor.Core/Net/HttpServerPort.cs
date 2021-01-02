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

namespace AppMotor.Core.Net
{
    /// <summary>
    /// Represents an HTTP server port. For HTTPS, use <see cref="HttpsServerPort"/> instead.
    /// </summary>
    public class HttpServerPort : ServerPort
    {
        /// <summary>
        /// The default HTTP port.
        /// </summary>
        [PublicAPI]
        public const int DEFAULT_PORT = 80;

        /// <summary>
        /// Constructor. Uses <see cref="DEFAULT_PORT"/> as port.
        /// </summary>
        [PublicAPI]
        public HttpServerPort(SocketListenAddresses listenAddress) : this(listenAddress, port: DEFAULT_PORT)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        [PublicAPI]
        public HttpServerPort(SocketListenAddresses listenAddress, int port) : base(listenAddress, port)
        {
        }
    }
}

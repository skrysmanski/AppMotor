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
    /// The address a socket (server) can listen on (i.e. from where to accept connections).
    /// </summary>
    /// <seealso cref="ServerPort"/>
    [PublicAPI]
    public enum SocketListenAddresses
    {
        /// <summary>
        /// Accept connections from anywhere.
        /// </summary>
        Any,

        /// <summary>
        /// Accept connections only from localhost.
        /// </summary>
        Loopback,
    }
}

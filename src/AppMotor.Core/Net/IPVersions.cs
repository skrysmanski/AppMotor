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

using System.Net.Http;

using JetBrains.Annotations;

namespace AppMotor.Core.Net;

/// <summary>
/// Represents the IP version (IPv4, IPv6) to use.
///
/// <para>Unless absolutely necessary, you should always use <see cref="DualStack"/>.</para>
/// </summary>
/// <seealso cref="ServerPort"/>
[PublicAPI]
public enum IPVersions
{
    /// <summary>
    /// Both <see cref="IPv4"/> and <see cref="IPv6"/> will be used (commonly referred to as "dual-stack").
    ///
    /// <para>Note that it's usually recommended to use dual-stack because with this a dual-stack capable
    /// client doesn't need to guess the IP stack when initiating a connection. For example, if DNS returns
    /// both an IPv4 and an IPv6 address for the server, <see cref="HttpClient"/> first tries to connect
    /// via IPv6 for 5 seconds before retrying with IPv4. This leads to an unnecessary delay if the server
    /// only serves an IPv4 endpoint.</para>
    /// </summary>
    DualStack,

    /// <summary>
    /// Represents the "legacy" IP version, IPv4. Ip addresses have a format like "192.168.0.1".
    /// </summary>
    /// <seealso cref="IPv6"/>
    // ReSharper disable once InconsistentNaming
    IPv4,

    /// <summary>
    /// Represents the "modern" IP version, IPv6. Ip addresses have a format like "2001:0db8:85a3:0000:0000:8a2e:0370:7334".
    /// </summary>
    /// <see cref="IPv4"/>
    // ReSharper disable once InconsistentNaming
    IPv6,
}
// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using JetBrains.Annotations;

namespace AppMotor.Core.Net;

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

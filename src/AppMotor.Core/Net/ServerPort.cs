// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using JetBrains.Annotations;

namespace AppMotor.Core.Net;

/// <summary>
/// Represents information about a generic server port (i.e. the information required by a socket listen operation).
///
/// <para>For a more specialized variant, see <see cref="HttpServerPort"/> and <see cref="HttpsServerPort"/>.</para>
/// </summary>
public class ServerPort
{
    /// <summary>
    /// From where to accept connections.
    /// </summary>
    [PublicAPI]
    public SocketListenAddresses ListenAddress { get; }

    /// <summary>
    /// The port to use.
    /// </summary>
    [PublicAPI]
    public int Port { get; }

    /// <summary>
    /// The IP version (IPv4, IPv6) to use.
    /// </summary>
    [PublicAPI]
    public IPVersions IPVersion { get; init; } = IPVersions.DualStack;

    /// <summary>
    /// Constructor.
    /// </summary>
    [PublicAPI]
    public ServerPort(SocketListenAddresses listenAddress, int port)
    {
        if (port < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(port));
        }

        this.ListenAddress = listenAddress;
        this.Port = port;
    }
}

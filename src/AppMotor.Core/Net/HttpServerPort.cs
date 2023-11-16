// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using JetBrains.Annotations;

namespace AppMotor.Core.Net;

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

// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

namespace AppMotor.TestCore.Networking;

/// <summary>
/// Provides each test with its own port. This is necessary because tests
/// run in parallel.
/// </summary>
public static class ServerPortProvider
{
    private static readonly object s_lock = new();

    private static int s_nextPort = 1234;

    /// <summary>
    /// Provides a port to be used for testing.
    /// </summary>
    [MustUseReturnValue]
    [SuppressMessage("Design", "CA1024:Use properties where appropriate")]
    public static int GetNextTestPort()
    {
        lock (s_lock)
        {
            int port = s_nextPort;
            s_nextPort++;
            return port;
        }
    }
}

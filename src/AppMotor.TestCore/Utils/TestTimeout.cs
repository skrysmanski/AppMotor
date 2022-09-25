// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Diagnostics;

using AppMotor.TestCore.Extensions;

namespace AppMotor.TestCore.Utils;

/// <summary>
/// Helper class for handling timeouts in tests.
/// </summary>
public static class TestTimeout
{
    /// <summary>
    /// Throws a <see cref="TimeoutException"/> if the specified task doesn't finish within the specified timeout.
    /// Note that the timeout is ignored if a debugger is attached (since you usually don't want a tests you're
    /// currently debugger to fail after 10 seconds or so).
    /// </summary>
    public static async Task TimeoutAfter(Task task, TimeSpan timeout)
    {
        if (!Debugger.IsAttached)
        {
            await task.OrTimeoutAfter(timeout).ConfigureAwait(false);
        }
        else
        {
            await task.ConfigureAwait(false);
        }
    }
}

// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

namespace AppMotor.Core.Utils;

/// <summary>
/// Helper methods for <see cref="Task"/>.
/// </summary>
public static class TaskUtils
{
    /// <summary>
    /// Just like <see cref="Task.Delay(TimeSpan,CancellationToken)"/> but returns <c>false</c>
    /// instead of throwing a <see cref="TaskCanceledException"/> if the cancellation token
    /// is canceled.
    /// </summary>
    /// <param name="delay">The amount of time to wait.</param>
    /// <param name="cancellationToken">A cancellation token to interrupt the wait
    /// before it's finished.</param>
    /// <returns>Returns <c>true</c> if the timeout was not interrupted (i.e. the method
    /// waited for the whole timespan); returns <c>false</c> if the cancellation token
    /// has been cancelled.</returns>
    public static async Task<bool> DelaySafe(TimeSpan delay, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return false;
        }

        try
        {
            await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
            return true;
        }
        catch (TaskCanceledException)
        {
            return false;
        }
    }
}

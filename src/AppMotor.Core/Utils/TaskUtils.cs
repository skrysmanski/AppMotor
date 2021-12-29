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
// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using JetBrains.Annotations;

using Shouldly;

namespace AppMotor.TestCore.Extensions;

/// <summary>
/// Contains extension methods for <see cref="Task"/> to throw a <see cref="TimeoutException"/>
/// if a task didn't finish in time.
/// <see cref="Should"/>ly extensions for tasks that should finish within a certain amount of time.
/// Basically replaces <see cref="Should.CompleteIn(Action,TimeSpan,Func{string?}?)"/> so that
/// it works (reliably) with XUnit's parallelized test execution.
/// </summary>
/// <remarks>
/// This class is only available in "AppMotor.TestCore" but not in "AppMotor.Core". I thought a lot
/// about moving it to the production code but ultimately decided against it. Why? The biggest problem
/// with the methods in this class is that the actual task - in case of a timeout - continues running
/// in the background (basically as <c>async void</c>). This may be okay in tests but for production
/// code this could be very bad. So, to not encourage this style, this class remains for tests only.
/// </remarks>
/// <remarks>
/// Shouldly has <see cref="Should.CompleteIn(Action,TimeSpan,Func{string?}?)"/> that does something
/// similar. However, this method leads to deadlocks with XUnit's parallel test execution (if the processor
/// count is low like in free CI systems).
/// </remarks>
[ShouldlyMethods]
public static class TaskTimeoutExtensions
{
    //
    // NOTE: These methods do not create proper Shouldly "... should finish within" messages because these
    //   can't be produced for "async" methods (because the stacktrace doesn't provide the necessary
    //   information). Unfortunately, we can't actually use synchronous methods here because this
    //   leads to deadlocks with XUnit's parallel test execution.
    //
    //   See also: https://github.com/skrysmanski/AppMotor/pull/31
    //

    /// <summary>
    /// Throws a <see cref="TimeoutException"/> if the <paramref name="task"/> did not finish
    /// within the the specified <paramref name="timeout"/>.
    /// </summary>
    [PublicAPI]
    public static async Task OrTimeoutAfter(this Task task, TimeSpan timeout)
    {
        await Task.WhenAny(task, Task.Delay(timeout)).ConfigureAwait(false);

        if (!task.IsCompleted)
        {
            throw new TimeoutException($"The task did not finished within {timeout}.");
        }
    }

    /// <summary>
    /// Throws a <see cref="TimeoutException"/> if the <paramref name="task"/> did not finish
    /// within the the specified <paramref name="timeout"/>.
    /// </summary>
    [PublicAPI]
    public static async Task<T> OrTimeoutAfter<T>(this Task<T> task, TimeSpan timeout)
    {
        await Task.WhenAny(task, Task.Delay(timeout)).ConfigureAwait(false);

        if (!task.IsCompleted)
        {
            throw new TimeoutException($"The task did not finished within {timeout}.");
        }

        return await task.ConfigureAwait(false);
    }
}

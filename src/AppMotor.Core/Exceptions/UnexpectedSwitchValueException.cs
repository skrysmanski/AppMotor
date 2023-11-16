// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using JetBrains.Annotations;

namespace AppMotor.Core.Exceptions;

/// <summary>
/// This exception is for the <c>default</c> case in <c>switch</c> blocks where all other case blocks
/// should already cover all possible values. In most cases, this is used for switch blocks over enums
/// where each enum member is already covered by a case block but the compiler require a default block.
/// </summary>
/// <remarks>
/// This exception is basically a specialization of <see cref="UnexpectedBehaviorException"/>.
/// </remarks>
[PublicAPI]
public class UnexpectedSwitchValueException : UnexpectedBehaviorException
{
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="what">What was "switched" on; you should use <c>nameof(...)</c>, if possible</param>
    /// <param name="value">The value that was not expected</param>
    [PublicAPI]
    public UnexpectedSwitchValueException(string what, object? value)
        : base($"Unexpected {what}: {value ?? "null"}")
    {
    }
}

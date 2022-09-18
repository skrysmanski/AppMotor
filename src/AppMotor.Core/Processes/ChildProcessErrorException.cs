// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

namespace AppMotor.Core.Processes;

/// <summary>
/// Thrown by <see cref="ChildProcess"/> when the child process signals an error.
/// </summary>
public sealed class ChildProcessErrorException : Exception
{
    /// <inheritdoc />
    public ChildProcessErrorException(string message) : base(message)
    {
    }
}

// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.Core.Utils;

using JetBrains.Annotations;

namespace AppMotor.Core.Processes;

/// <summary>
/// The result of executing a child process through <see cref="ChildProcess"/>.
/// </summary>
public sealed class ChildProcessResult
{
    /// <summary>
    /// The exit code of the child process.
    /// </summary>
    [PublicAPI]
    public int ExitCode { get; }

    /// <summary>
    /// The output (stdout) of the child process. Never <c>null</c> but may be empty.
    /// </summary>
    [PublicAPI]
    public string Output { get; }

    /// <summary>
    /// The error output (stderr) of the child process. Is <c>null</c> if no
    /// error output was produced by the child process.
    /// </summary>
    [PublicAPI]
    public string? ErrorOutput { get; }

    /// <summary>
    /// Constructor.
    /// </summary>
    public ChildProcessResult(int exitCode, string output, string? errorOutput)
    {
        Validate.ArgumentWithName(nameof(output)).IsNotNull(output);

        this.ExitCode = exitCode;
        this.Output = output;
        this.ErrorOutput = errorOutput;
    }
}

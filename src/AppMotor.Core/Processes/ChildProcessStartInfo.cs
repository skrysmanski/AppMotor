// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Diagnostics.CodeAnalysis;

using AppMotor.Core.Utils;

using JetBrains.Annotations;

namespace AppMotor.Core.Processes;

/// <summary>
/// Allows some more advanced options for <see cref="ChildProcess"/> to be specified.
/// </summary>
[PublicAPI]
public sealed class ChildProcessStartInfo
{
    /// <summary>
    /// The default value for <see cref="ProcessTimeout"/>.
    /// </summary>
    public static readonly TimeSpan DEFAULT_PROCESS_TIMEOUT = TimeSpan.FromSeconds(30);

    /// <summary>
    /// The path to the file to execute.
    /// </summary>
    public string ProcessFileName { get; }

    /// <summary>
    /// The arguments to pass to <see cref="ProcessFileName"/>.
    /// </summary>
    [SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "By design")]
    public ProcessArguments Arguments
    {
        get => this._arguments;
        set
        {
            Validate.ArgumentWithName(nameof(value)).IsNotNull(value);
            this._arguments = value;
        }
    }

    private ProcessArguments _arguments;

    /// <summary>
    /// The working directory. Can be <c>null</c> in which case the runtime picks
    /// a working directory.
    /// </summary>
    public string? WorkingDirectory { get; set; }

    /// <summary>
    /// Determines for how long the process can run before triggering a <see cref="TimeoutException"/>.
    /// Can be <c>null</c> in which case <see cref="ChildProcess"/> will wait indefinitely for the process
    /// to finish (not recommended).
    ///
    /// <para>Defaults to 30 seconds (see <see cref="DEFAULT_PROCESS_TIMEOUT"/>).</para>
    /// </summary>
    public TimeSpan? ProcessTimeout { get; set; } = DEFAULT_PROCESS_TIMEOUT;

    /// <summary>
    /// The exit code that represents success. If the child process reports any other exit code,
    /// <see cref="ChildProcess"/> will throw a <see cref="ChildProcessErrorException"/> exception.
    /// If set to <c>null</c>, the exit code won't be checked (and thus no exception will occur).
    ///
    /// <para>Defaults to 0.</para>
    /// </summary>
    public int? SuccessExitCode { get; set; } = 0;

    /// <summary>
    /// Whether any (non-whitespace) output to stderr should be considered an error. If <c>true</c>,
    /// <see cref="ChildProcess"/> will throw a <see cref="ChildProcessErrorException"/> in this case.
    /// If <c>false</c>, the error output will be returned via <see cref="ChildProcessResult.ErrorOutput"/>.
    ///
    /// <para>Defaults to <c>true</c>.</para>
    /// </summary>
    public bool TreatErrorOutputAsFailure { get; set; } = true;

    /// <summary>
    /// Constructor without arguments.
    /// </summary>
    /// <param name="processFileName">The path to the file to execute.</param>
    public ChildProcessStartInfo(string processFileName)
    {
        Validate.ArgumentWithName(nameof(processFileName)).IsNotNullOrWhiteSpace(processFileName);

        this.ProcessFileName = processFileName;
        this._arguments = [];
    }

    /// <summary>
    /// Constructor with arguments.
    /// </summary>
    /// <param name="processFileName">The path to the file to execute.</param>
    /// <param name="processArguments">The argument to pass to the process.</param>
    public ChildProcessStartInfo(string processFileName, ProcessArguments? processArguments)
    {
        Validate.ArgumentWithName(nameof(processFileName)).IsNotNullOrWhiteSpace(processFileName);

        this.ProcessFileName = processFileName;
        this._arguments = processArguments ?? [];
    }
}

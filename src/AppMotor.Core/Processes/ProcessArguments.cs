// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Diagnostics;

using AppMotor.Core.Utils;

namespace AppMotor.Core.Processes;

/// <summary>
/// Represents the arguments for a process as a list (rather than a single string). Provides
/// convenience methods to create the arguments list as well as apply it to an instance
/// of <see cref="ProcessStartInfo"/>.
/// </summary>
public class ProcessArguments : List<string>
{
    /// <summary>
    /// Creates an instance of this class and populates it with the specified arguments.
    /// </summary>
    public ProcessArguments(params string[]? args)
        : base(args ?? [])
    {
    }

    /// <summary>
    /// Creates an instance of this class and populates it with a copy of the specified collection.
    /// </summary>
    public ProcessArguments(IEnumerable<string>? args)
        : base(args ?? [])
    {
    }

    /// <summary>
    /// Copies the arguments in this instance to the specified instance of <see cref="ProcessStartInfo"/>.
    /// </summary>
    public void CopyTo(ProcessStartInfo processStartInfo)
    {
        Validate.ArgumentWithName(nameof(processStartInfo)).IsNotNull(processStartInfo);

        // Must be set to empty when using "ArgumentsList".
        processStartInfo.Arguments = "";

        var startInfoArguments = processStartInfo.ArgumentList;

        // Make sure it's empty.
        startInfoArguments.Clear();

        foreach (var processArgument in this)
        {
            startInfoArguments.Add(processArgument);
        }
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return string.Join(" ", this.Select(arg => arg.Contains(' ') ? '"' + arg + '"' : arg));
    }
}

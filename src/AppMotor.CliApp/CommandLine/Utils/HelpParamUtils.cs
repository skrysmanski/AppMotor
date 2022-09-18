// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.Core.Utils;

using JetBrains.Annotations;

namespace AppMotor.CliApp.CommandLine.Utils;

/// <summary>
/// Provides utility methods for the help parameter/command.
/// </summary>
public static class HelpParamUtils
{
    /// <summary>
    /// The default variant of the help param names (see <see cref="IsHelpParamName"/>).
    /// </summary>
    public static string DefaultHelpParamName => "--help";

    /// <summary>
    /// The name of the (virtual) help command.
    /// </summary>
    public static string HelpCommandName => "help";

    /// <summary>
    /// Checks whether the specified parameter/argument represents the help param.
    /// </summary>
    [MustUseReturnValue]
    public static bool IsHelpParamName(string arg)
    {
        Validate.ArgumentWithName(nameof(arg)).IsNotNull(arg);

        switch (arg.ToLowerInvariant())
        {
            case "-h":
            case "/h":
            case "--help":
            case "-?":
            case "/?":
                return true;

            default:
                return false;
        }
    }
}

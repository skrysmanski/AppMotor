// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.Core.Exceptions;
using AppMotor.Core.Utils;

using JetBrains.Annotations;

using NotNullOnExitAttribute = System.Diagnostics.CodeAnalysis.NotNullAttribute;

namespace AppMotor.CliApp.CommandLine.Utils;

/// <summary>
/// Provides validation methods for parameter names (as used in <see cref="CliParam{T}"/>).
/// </summary>
public static class CliParamNameValidation
{
    /// <summary>
    /// Validation method to be used with <see cref="Validate"/>. See <see cref="CheckIfNameIsValid"/> for more details.
    /// </summary>
    public static void IsValidParameterName(this NamedValidator validator, [NotNullOnExit] string? paramName, CliParamTypes paramType, bool allowReservedParamName = false)
    {
        validator.IsNotNullOrWhiteSpace(paramName);

        var checkResult = CheckIfNameIsValid(paramName, paramType, allowReservedParamName: allowReservedParamName);

        switch (checkResult)
        {
            case CliParamNameValidityCheckResults.Valid:
                // ok
                break;

            case CliParamNameValidityCheckResults.Invalid:
                if (paramType == CliParamTypes.Named)
                {
                    throw validator.CreateRootException($"The parameter name '{paramName}' is invalid. Names of named parameters must either be '-x' or '--some-name'.");
                }
                else
                {
                    throw validator.CreateRootException($"The parameter name '{paramName}' is invalid. Names of positional parameters must not start with a prefix.");
                }

            case CliParamNameValidityCheckResults.ContainsSpaces:
                throw validator.CreateRootException($"The parameter name '{paramName}' is invalid because it contains spaces.");

            case CliParamNameValidityCheckResults.ReservedName:
                throw validator.CreateRootException($"The name '{paramName}' is reserved and can't be used.");

            default:
                // NOTE: Can't be reached by code coverage.
                throw new UnexpectedSwitchValueException(nameof(checkResult), checkResult);
        }
    }

    /// <summary>
    /// Checks if the given parameter name (<paramref name="paramName"/>) is a valid parameter name for the given parameter
    /// type (<paramref name="paramType"/>) and returns the result. See <see cref="CliParamNameValidityCheckResults"/> for
    /// details on name rules.
    /// </summary>
    /// <param name="paramName">The name to check</param>
    /// <param name="paramType">The parameter type this name is to be used for.</param>
    /// <param name="allowReservedParamName">Whether reserved names are allowed for <paramref name="paramName"/>. Is <c>false</c>
    /// by default.</param>
    /// <returns>The result of the check.</returns>
    /// <seealso cref="IsValidParameterName"/>
    [MustUseReturnValue]
    public static CliParamNameValidityCheckResults CheckIfNameIsValid(string? paramName, CliParamTypes paramType, bool allowReservedParamName = false)
    {
        if (string.IsNullOrWhiteSpace(paramName))
        {
            return CliParamNameValidityCheckResults.Invalid;
        }

        if (paramType == CliParamTypes.Named)
        {
            char firstCharAfterPrefix;

            if (paramName.Length == 2)
            {
                if (!paramName.StartsWith('-'))
                {
                    return CliParamNameValidityCheckResults.Invalid;
                }

                firstCharAfterPrefix = paramName[1];
            }
            else if (paramName.Length >= 4)
            {
                if (!paramName.StartsWith("--", StringComparison.Ordinal))
                {
                    return CliParamNameValidityCheckResults.Invalid;
                }

                firstCharAfterPrefix = paramName[2];
            }
            else
            {
                return CliParamNameValidityCheckResults.Invalid;
            }

            if (firstCharAfterPrefix == '-')
            {
                return CliParamNameValidityCheckResults.Invalid;
            }
        }
        else
        {
            var firstChar = paramName[0];

            switch (firstChar)
            {
                case '-':
                case '/':
                    return CliParamNameValidityCheckResults.Invalid;
            }
        }

        if (paramName.Contains(' ', StringComparison.Ordinal))
        {
            return CliParamNameValidityCheckResults.ContainsSpaces;
        }

        if (!allowReservedParamName && HelpParamUtils.IsHelpParamName(paramName))
        {
            return CliParamNameValidityCheckResults.ReservedName;
        }

        return CliParamNameValidityCheckResults.Valid;
    }
}

/// <summary>
/// The result of <see cref="CliParamNameValidation.CheckIfNameIsValid"/>. See each enum member
/// for details on the rules.
/// </summary>
public enum CliParamNameValidityCheckResults
{
    /// <summary>
    /// The name is valid.
    /// </summary>
    Valid,

    /// <summary>
    /// The name contains spaces which is not allowed.
    /// </summary>
    ContainsSpaces,

    /// <summary>
    /// The name is a reserved name that must not be used.
    /// </summary>
    ReservedName,

    /// <summary>
    /// The name is invalid for some other reason. The primary problems are:
    ///
    /// <list type="bullet">
    /// <item>
    /// <description>The name is null or empty or just white space characters.</description>
    /// </item>
    /// <item>
    /// <description>The name is for a named parameter and does not start with the "-" or "--" prefix.</description>
    /// </item>
    /// <item>
    /// <description>The name is for a named parameter, starts with a single "-" but has more than one character after that.
    /// For example, allowed are <c>-m</c>, <c>-t</c> but not <c>-abc</c>.</description>
    /// </item>
    /// <item>
    /// <description>The name is for a named parameter, starts with "--" but only has one character after that.
    /// For example, allowed are <c>--my-value</c>, <c>--temp</c> but not <c>--a</c>.</description>
    /// </item>
    /// <item>
    /// <description>The name is for a positional parameter but starts with "-" or "/". Names for positional
    /// parameters must not have a prefix.</description>
    /// </item>
    /// </list>
    /// </summary>
    Invalid,
}

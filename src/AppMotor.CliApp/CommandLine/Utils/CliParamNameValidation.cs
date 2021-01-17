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

using System;

using AppMotor.Core.Exceptions;
using AppMotor.Core.Utils;

using JetBrains.Annotations;

using NotNullOnExitAttribute = System.Diagnostics.CodeAnalysis.NotNullAttribute;

namespace AppMotor.CliApp.CommandLine.Utils
{
    public static class CliParamNameValidation
    {
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
                    throw new UnexpectedSwitchValueException(nameof(checkResult), checkResult);
            }
        }

        [PublicAPI, MustUseReturnValue]
        public static CliParamNameValidityCheckResults CheckIfNameIsValid(string paramName, CliParamTypes paramType, bool allowReservedParamName = false)
        {
            Validate.ArgumentWithName(nameof(paramName)).IsNotNull(paramName);

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

    public enum CliParamNameValidityCheckResults
    {
        Valid,
        Invalid,
        ContainsSpaces,
        ReservedName,
    }

}

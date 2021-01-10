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
        public static void IsValidParameterName(this NamedValidator validator, [NotNullOnExit] string? value, CliParamTypes paramType)
        {
            validator.IsNotNullOrWhiteSpace(value);

            var checkResult = CheckIfNameIsValid(value, paramType);

            switch (checkResult)
            {
                case CliParamNameValidityCheckResults.Valid:
                    // ok
                    break;

                case CliParamNameValidityCheckResults.Invalid:
                    if (paramType == CliParamTypes.Named)
                    {
                        throw validator.CreateRootException($"The parameter name '{value}' is invalid. Names of named parameters must either be '-x' or '--some-name'.");
                    }
                    else
                    {
                        throw validator.CreateRootException($"The parameter name '{value}' is invalid. Names of positional parameters must not start with a prefix.");
                    }

                case CliParamNameValidityCheckResults.ContainsSpaces:
                    throw validator.CreateRootException($"The parameter name '{value}' is invalid because it contains spaces.");

                default:
                    throw new UnexpectedSwitchValueException(nameof(checkResult), checkResult);
            }
        }

        [PublicAPI, MustUseReturnValue]
        public static CliParamNameValidityCheckResults CheckIfNameIsValid(string value, CliParamTypes paramType)
        {
            Validate.ArgumentWithName(nameof(value)).IsNotNull(value);

            if (string.IsNullOrWhiteSpace(value))
            {
                return CliParamNameValidityCheckResults.Invalid;
            }

            if (paramType == CliParamTypes.Named)
            {
                char firstCharAfterPrefix;

                if (value.Length == 2)
                {
                    if (!value.StartsWith('-'))
                    {
                        return CliParamNameValidityCheckResults.Invalid;
                    }

                    firstCharAfterPrefix = value[1];
                }
                else if (value.Length >= 4)
                {
                    if (!value.StartsWith("--", StringComparison.Ordinal))
                    {
                        return CliParamNameValidityCheckResults.Invalid;
                    }

                    firstCharAfterPrefix = value[2];
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
                var firstChar = value[0];

                switch (firstChar)
                {
                    case '-':
                    case '/':
                        return CliParamNameValidityCheckResults.Invalid;
                }
            }

            if (value.Contains(' ', StringComparison.Ordinal))
            {
                return CliParamNameValidityCheckResults.ContainsSpaces;
            }

            return CliParamNameValidityCheckResults.Valid;
        }


    }

    public enum CliParamNameValidityCheckResults
    {
        Valid,
        Invalid,
        ContainsSpaces,
    }

}

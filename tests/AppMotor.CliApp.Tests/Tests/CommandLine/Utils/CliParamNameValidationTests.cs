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

using System.Collections.Generic;

using AppMotor.CliApp.CommandLine;
using AppMotor.CliApp.CommandLine.Utils;
using AppMotor.Core.Exceptions;
using AppMotor.Core.Utils;

using Shouldly;

using Xunit;

namespace AppMotor.CliApp.Tests.CommandLine.Utils
{
    public sealed class CliParamNameValidationTests
    {
        [Theory]
        [MemberData(nameof(TestData))]
        public void Test_CheckIfNameIsValid(string? paramName, CliParamTypes paramType, bool allowReservedParamName, CliParamNameValidityCheckResults expectedResult)
        {
            CliParamNameValidation.CheckIfNameIsValid(paramName, paramType, allowReservedParamName: allowReservedParamName).ShouldBe(expectedResult);
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void Test_IsValidParameterName(string? paramName, CliParamTypes paramType, bool allowReservedParamName, CliParamNameValidityCheckResults expectedResult)
        {
            if (expectedResult == CliParamNameValidityCheckResults.Valid)
            {
                Should.NotThrow(() => Validate.ValueWithName("abc").IsValidParameterName(paramName, paramType, allowReservedParamName: allowReservedParamName));
            }
            else
            {
                var ex = Should.Throw<ValueException>(() => Validate.ValueWithName("abc").IsValidParameterName(paramName, paramType, allowReservedParamName: allowReservedParamName));
                if (string.IsNullOrWhiteSpace(paramName))
                {
                    var emptyStringException = Should.Throw<ValueException>(() => Validate.ValueWithName("abc").IsNotNullOrWhiteSpace(paramName));
                    ex.Message.ShouldBe(emptyStringException.Message);
                }
                else
                {
                    switch (expectedResult)
                    {
                        case CliParamNameValidityCheckResults.Invalid:
                            if (paramType == CliParamTypes.Named)
                            {
                                ex.Message.ShouldStartWith($"The parameter name '{paramName}' is invalid. Names of named parameters must either be '-x' or '--some-name'.");
                            }
                            else
                            {
                                ex.Message.ShouldStartWith($"The parameter name '{paramName}' is invalid. Names of positional parameters must not start with a prefix.");
                            }
                            break;

                        case CliParamNameValidityCheckResults.ContainsSpaces:
                            ex.Message.ShouldStartWith($"The parameter name '{paramName}' is invalid because it contains spaces.");
                            break;

                        case CliParamNameValidityCheckResults.ReservedName:
                            ex.Message.ShouldStartWith($"The name '{paramName}' is reserved and can't be used.");
                            break;

                        default:
                            throw new UnexpectedSwitchValueException(nameof(expectedResult), expectedResult);
                    }
                }

            }
        }

        public static IEnumerable<object?[]> TestData
        {
            get
            {
                foreach (var enumerable in UnflattedTestData)
                {
                    foreach (var testData in enumerable)
                    {
                        yield return testData;
                    }
                }
            }
        }

        // ReSharper disable once IdentifierTypo
        private static IEnumerable<IEnumerable<object?[]>> UnflattedTestData
        {
            get
            {
                yield return CreateTestDataWith(paramName: null, paramType: null, allowReservedParamName: null, expectedResult: CliParamNameValidityCheckResults.Invalid);
                yield return CreateTestDataWith(paramName: "", paramType: null, allowReservedParamName: null, expectedResult: CliParamNameValidityCheckResults.Invalid);
                yield return CreateTestDataWith(paramName: "  ", paramType: null, allowReservedParamName: null, expectedResult: CliParamNameValidityCheckResults.Invalid);

                yield return CreateTestDataWith(paramName: "-h", paramType: CliParamTypes.Named, allowReservedParamName: false, expectedResult: CliParamNameValidityCheckResults.ReservedName);
                yield return CreateTestDataWith(paramName: "--help", paramType: CliParamTypes.Named, allowReservedParamName: false, expectedResult: CliParamNameValidityCheckResults.ReservedName);
                yield return CreateTestDataWith(paramName: "help", paramType: CliParamTypes.Positional, allowReservedParamName: false, expectedResult: CliParamNameValidityCheckResults.Valid); // this is indeed valid
                yield return CreateTestDataWith(paramName: "-h", paramType: CliParamTypes.Named, allowReservedParamName: true, expectedResult: CliParamNameValidityCheckResults.Valid);
                yield return CreateTestDataWith(paramName: "--help", paramType: CliParamTypes.Named, allowReservedParamName: true, expectedResult: CliParamNameValidityCheckResults.Valid);
                yield return CreateTestDataWith(paramName: "help", paramType: CliParamTypes.Positional, allowReservedParamName: true, expectedResult: CliParamNameValidityCheckResults.Valid);

                yield return CreateTestDataWith(paramName: "-abc", paramType: CliParamTypes.Named, allowReservedParamName: null, expectedResult: CliParamNameValidityCheckResults.Invalid);
                yield return CreateTestDataWith(paramName: "--a", paramType: CliParamTypes.Named, allowReservedParamName: null, expectedResult: CliParamNameValidityCheckResults.Invalid);
                yield return CreateTestDataWith(paramName: "-a", paramType: CliParamTypes.Named, allowReservedParamName: null, expectedResult: CliParamNameValidityCheckResults.Valid);
                yield return CreateTestDataWith(paramName: "--abc", paramType: CliParamTypes.Named, allowReservedParamName: null, expectedResult: CliParamNameValidityCheckResults.Valid);

                yield return CreateTestDataWith(paramName: "--", paramType: CliParamTypes.Named, allowReservedParamName: null, expectedResult: CliParamNameValidityCheckResults.Invalid);
                yield return CreateTestDataWith(paramName: "-", paramType: CliParamTypes.Named, allowReservedParamName: null, expectedResult: CliParamNameValidityCheckResults.Invalid);

                yield return CreateTestDataWith(paramName: "-- a", paramType: CliParamTypes.Named, allowReservedParamName: null, expectedResult: CliParamNameValidityCheckResults.ContainsSpaces);
                yield return CreateTestDataWith(paramName: "- ", paramType: CliParamTypes.Named, allowReservedParamName: null, expectedResult: CliParamNameValidityCheckResults.ContainsSpaces);
                yield return CreateTestDataWith(paramName: "a b", paramType: CliParamTypes.Positional, allowReservedParamName: null, expectedResult: CliParamNameValidityCheckResults.ContainsSpaces);
            }
        }

        private static IEnumerable<object?[]> CreateTestDataWith(string? paramName, CliParamTypes? paramType, bool? allowReservedParamName, CliParamNameValidityCheckResults expectedResult)
        {
            if (paramType is null)
            {
                foreach (var testData in CreateTestDataWith(paramName, CliParamTypes.Named, allowReservedParamName, expectedResult))
                {
                    yield return testData;
                }
                foreach (var testData in CreateTestDataWith(paramName, CliParamTypes.Positional, allowReservedParamName, expectedResult))
                {
                    yield return testData;
                }
            }
            else
            {
                foreach (var testData in CreateTestDataWith(paramName, paramType.Value, allowReservedParamName, expectedResult))
                {
                    yield return testData;
                }
            }
        }

        private static IEnumerable<object?[]> CreateTestDataWith(string? paramName, CliParamTypes paramType, bool? allowReservedParamName, CliParamNameValidityCheckResults expectedResult)
        {
            if (allowReservedParamName is null)
            {
                foreach (var testData in CreateTestDataWith(paramName, paramType, allowReservedParamName: true, expectedResult))
                {
                    yield return testData;
                }

                foreach (var testData in CreateTestDataWith(paramName, paramType, allowReservedParamName: false, expectedResult))
                {
                    yield return testData;
                }
            }
            else
            {
                foreach (var testData in CreateTestDataWith(paramName, paramType, allowReservedParamName.Value, expectedResult))
                {
                    yield return testData;
                }
            }
        }

        private static IEnumerable<object?[]> CreateTestDataWith(string? paramName, CliParamTypes paramType, bool allowReservedParamName, CliParamNameValidityCheckResults expectedResult)
        {
            yield return new object?[] { paramName, paramType, allowReservedParamName, expectedResult };
        }
    }
}

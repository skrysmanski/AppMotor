#region License
// Copyright 2020 AppMotor Framework (https://github.com/skrysmanski/AppMotor)
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
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Builder;

using AppMotor.CliApp.CommandLine;
using AppMotor.CliApp.TestUtils;
using AppMotor.Core.Exceptions;

using Shouldly;

using Xunit;

namespace AppMotor.CliApp.Tests.CommandLine
{
    public sealed class CliParamTests
    {
        [Fact]
        public void TestNamedParameter()
        {
            var param = new CliParam<int>("--value", "-v")
            {
                HelpText = "abc",
            };

            param.Names.ShouldBe(new[] { "--value", "-v" });
            param.PrimaryName.ShouldBe("--value");

            param.IsNamedParameter.ShouldBe(true);
            param.IsPositionalParameter.ShouldBe(false);
            param.PositionIndex.ShouldBe(null);

            param.HelpText.ShouldBe("abc");

            param.ToString().ShouldBe("--value");

            var underlyingImplementation = (Option<int>)param.UnderlyingImplementation;
            param.UnderlyingImplementation.ShouldBeSameAs(underlyingImplementation); // no new creation

            underlyingImplementation.Aliases.ShouldBe(new[] { "--value", "-v" });
            underlyingImplementation.Description.ShouldBe("abc");
        }

        [Fact]
        public void TestPositionalParameter()
        {
            var param = new CliParam<int>("index", positionIndex: 42)
            {
                HelpText = "abc",
            };

            param.Names.ShouldBe(new[] { "index" });
            param.PrimaryName.ShouldBe("index");

            param.IsNamedParameter.ShouldBe(false);
            param.IsPositionalParameter.ShouldBe(true);
            param.PositionIndex.ShouldBe(42);

            var underlyingImplementation = (Argument<int>)param.UnderlyingImplementation;
            param.UnderlyingImplementation.ShouldBeSameAs(underlyingImplementation); // no new creation

            underlyingImplementation.Name.ShouldBe("index");
            underlyingImplementation.Description.ShouldBe("abc");
        }

        [Theory]
        [InlineData(null)]
        [InlineData(CollectionDefaultValues.Null)]
        [InlineData(CollectionDefaultValues.EmptyArray)]
        [InlineData(CollectionDefaultValues.EmptyList)]
        [InlineData(CollectionDefaultValues.EmptyReadOnlyList)]
        public void TestNamedParameter_Collection(CollectionDefaultValues? collectionDefaultValue)
        {
            // Setup
            CliParam<ICollection<string>?> param;
            switch (collectionDefaultValue)
            {
                case null:
                    param = new("--value")
                    {
                        HelpText = "A collection of values.",
                    };
                    break;

                case CollectionDefaultValues.Null:
                    param = new("--value")
                    {
                        HelpText = "A collection of values.",
                        DefaultValue = null,
                    };
                    break;

                case CollectionDefaultValues.EmptyArray:
                    param = new("--value")
                    {
                        HelpText = "A collection of values.",
                        DefaultValue = Array.Empty<string>(),
                    };
                    break;

                case CollectionDefaultValues.EmptyList:
                    param = new("--value")
                    {
                        HelpText = "A collection of values.",
                        DefaultValue = new List<string>(),
                    };
                    break;

                case CollectionDefaultValues.EmptyReadOnlyList:
                    param = new("--value")
                    {
                        HelpText = "A collection of values.",
                        DefaultValue = new List<string>().AsReadOnly(),
                    };
                    break;

                default:
                    throw new UnexpectedSwitchValueException(nameof(collectionDefaultValue), collectionDefaultValue);
            }

            // Assumptions
            param.IsNamedParameter.ShouldBe(true);
            param.DefaultValue.IsSet.ShouldBe(collectionDefaultValue is not null);

            // Tests

            TestApplicationWithParams app;

            // Test - without params
            if (collectionDefaultValue is null)
            {
                app = new TestApplicationWithParams(
                    () => throw new InvalidOperationException("We should not get here."),
                    param
                );
                app.RunWithExpectedExitCode(1);
            }
            else
            {
                app = new TestApplicationWithParams(
                    () =>
                    {
                        switch (collectionDefaultValue)
                        {
                            case CollectionDefaultValues.Null:
                                param.Value.ShouldBe(null);
                                break;

                            case CollectionDefaultValues.EmptyArray:
                            case CollectionDefaultValues.EmptyList:
                            case CollectionDefaultValues.EmptyReadOnlyList:
                                param.Value.ShouldNotBeNull();
                                param.Value.Count.ShouldBe(0);
                                break;

                            default:
                                throw new UnexpectedSwitchValueException(nameof(collectionDefaultValue), collectionDefaultValue);
                        }
                    },
                    param
                );
                app.Run();
                app.ShouldHaveNoOutput();

                app = new TestApplicationWithParams(
                    () => throw new InvalidOperationException("We should not get here."),
                    param
                );
                app.Run("--help");
                app.TerminalOutput.ShouldContain("--value", Case.Sensitive, app.TerminalOutput);
                app.TerminalOutput.ShouldNotContain("[default:", Case.Sensitive, app.TerminalOutput);
            }

            // Test - with one param
            app = new TestApplicationWithParams(
                () => param.Value.ShouldBe(new[] { "abc" }),
                param
            );
            app.Run("--value", "abc");
            app.ShouldHaveNoOutput();

            // Test - with two params
            app = new TestApplicationWithParams(
                () => param.Value.ShouldBe(new[] { "abc", "def" }),
                param
            );
            app.Run("--value", "abc", "--value", "def");
            app.ShouldHaveNoOutput();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(CollectionDefaultValues.Null)]
        [InlineData(CollectionDefaultValues.EmptyArray)]
        [InlineData(CollectionDefaultValues.EmptyList)]
        [InlineData(CollectionDefaultValues.EmptyReadOnlyList)]
        public void TestPositionalParameter_Collection(CollectionDefaultValues? collectionDefaultValue)
        {
            // Setup
            CliParam<ICollection<string>?> param;
            switch (collectionDefaultValue)
            {
                case null:
                    param = new("value", positionIndex: 42)
                    {
                        HelpText = "A collection of values.",
                    };
                    break;

                case CollectionDefaultValues.Null:
                    param = new("value", positionIndex: 42)
                    {
                        HelpText = "A collection of values.",
                        DefaultValue = null,
                    };
                    break;

                case CollectionDefaultValues.EmptyArray:
                    param = new("value", positionIndex: 42)
                    {
                        HelpText = "A collection of values.",
                        DefaultValue = Array.Empty<string>(),
                    };
                    break;

                case CollectionDefaultValues.EmptyList:
                    param = new ("value", positionIndex: 42)
                    {
                        HelpText = "A collection of values.",
                        DefaultValue = new List<string>(),
                    };
                    break;

                case CollectionDefaultValues.EmptyReadOnlyList:
                    param = new("value", positionIndex: 42)
                    {
                        HelpText = "A collection of values.",
                        DefaultValue = new List<string>().AsReadOnly(),
                    };
                    break;

                default:
                    throw new UnexpectedSwitchValueException(nameof(collectionDefaultValue), collectionDefaultValue);
            }

            // Assumptions
            param.IsPositionalParameter.ShouldBe(true);
            param.DefaultValue.IsSet.ShouldBe(collectionDefaultValue is not null);

            // Tests

            TestApplicationWithParams app;

            // Test - without params
            if (collectionDefaultValue is null)
            {
                app = new TestApplicationWithParams(
                    () => throw new InvalidOperationException("We should not get here."),
                    param
                );
                app.RunWithExpectedExitCode(1);
            }
            else
            {
                app = new TestApplicationWithParams(
                    () =>
                    {
                        switch (collectionDefaultValue)
                        {
                            case CollectionDefaultValues.Null:
                                param.Value.ShouldBe(null);
                                break;

                            case CollectionDefaultValues.EmptyArray:
                            case CollectionDefaultValues.EmptyList:
                            case CollectionDefaultValues.EmptyReadOnlyList:
                                param.Value.ShouldNotBeNull();
                                param.Value.Count.ShouldBe(0);
                                break;

                            default:
                                throw new UnexpectedSwitchValueException(nameof(collectionDefaultValue), collectionDefaultValue);
                        }
                    },
                    param
                );
                app.Run();
                app.ShouldHaveNoOutput();

                app = new TestApplicationWithParams(
                    () => throw new InvalidOperationException("We should not get here."),
                    param
                );
                app.Run("--help");
                app.TerminalOutput.ShouldContain("value", Case.Sensitive, app.TerminalOutput);
                app.TerminalOutput.ShouldNotContain("[default:", Case.Sensitive, app.TerminalOutput);
            }

            // Test - with one param
            app = new TestApplicationWithParams(
                () => param.Value.ShouldBe(new[] { "abc" }),
                param
            );
            app.Run("abc");
            app.ShouldHaveNoOutput();

            // Test - with two params
            app = new TestApplicationWithParams(
                () => param.Value.ShouldBe(new[] { "abc", "def" }),
                param
            );
            app.Run("abc", "def");
            app.ShouldHaveNoOutput();
        }

        public enum CollectionDefaultValues
        {
            Null,
            EmptyArray,
            EmptyList,
            EmptyReadOnlyList,
        }

        [Fact]
        public void TestNamedParameter_Collection_NonEmptyDefaultValue()
        {
            // Setup
            CliParam<List<string>> param = new("--value")
            {
                HelpText = "A collection of values.",
                DefaultValue = new List<string>() { "abc", "def" },
            };

            // Assumptions
            param.IsNamedParameter.ShouldBe(true);
            param.DefaultValue.IsSet.ShouldBe(true);

            // Tests
            TestApplicationWithParams app = new(
                () =>
                {
                    param.Value.ShouldNotBeNull();
                    param.Value.Count.ShouldBe(2);
                },
                param
            );
            app.Run();
            app.ShouldHaveNoOutput();

            app = new TestApplicationWithParams(
                () => throw new InvalidOperationException("We should not get here."),
                param
            );
            app.Run("--help");
            app.TerminalOutput.ShouldContain("--value", Case.Sensitive, app.TerminalOutput);
            app.TerminalOutput.ShouldContain("[default:", Case.Sensitive, app.TerminalOutput);
        }

        [Fact]
        public void TestPositionalParameter_Collection_NonEmptyDefaultValue()
        {
            // Setup
            CliParam<ICollection<string>?> param = new("value", positionIndex: 42)
            {
                HelpText = "A collection of values.",
                DefaultValue = new List<string>() { "abc", "def" },
            };

            // Assumptions
            param.IsPositionalParameter.ShouldBe(true);
            param.DefaultValue.IsSet.ShouldBe(true);

            // Tests
            TestApplicationWithParams app = new(
                () =>
                {
                    param.Value.ShouldNotBeNull();
                    param.Value.Count.ShouldBe(2);
                },
                param
            );
            app.Run();
            app.ShouldHaveNoOutput();

            app = new TestApplicationWithParams(
                () => throw new InvalidOperationException("We should not get here."),
                param
            );
            app.Run("--help");
            app.TerminalOutput.ShouldContain("value", Case.Sensitive, app.TerminalOutput);
            app.TerminalOutput.ShouldContain("[default:", Case.Sensitive, app.TerminalOutput);
        }

        [Fact]
        public void TestNamedParameter_WithDefaultValue()
        {
            // Setup
            var param = new CliParam<int>("--value")
            {
                DefaultValue = 42,
            };
            param.IsPositionalParameter.ShouldBe(false);

            // Test
            param.DefaultValue.IsSet.ShouldBe(true);
            param.DefaultValue.Value.ShouldBe(42);

            var underlyingImplementation = (Option<int>)param.UnderlyingImplementation;
            underlyingImplementation.Argument.HasDefaultValue.ShouldBe(true);
            underlyingImplementation.IsRequired.ShouldBe(false);

            // Test without specifying parameter
            var app = new TestApplicationWithParams(
                () => param.Value.ShouldBe(42),
                param
            );
            app.Run();
            app.ShouldHaveNoOutput();

            // Test with specifying parameter
            app = new TestApplicationWithParams(
                () => param.Value.ShouldBe(44),
                param
            );
            app.Run("--value", "44");
            app.ShouldHaveNoOutput();
        }

        /// <summary>
        /// Tests that named parameters can have <c>null</c> as default value.
        /// </summary>
        [Fact]
        public void TestNamedParameter_WithNullDefaultValue_ValueType()
        {
            // Setup
            var param = new CliParam<int?>("--value")
            {
                DefaultValue = null,
            };
            param.IsPositionalParameter.ShouldBe(false);

            // Test
            param.DefaultValue.IsSet.ShouldBe(true);
            param.DefaultValue.Value.ShouldBeNull();

            var underlyingImplementation = (Option<int?>)param.UnderlyingImplementation;
            underlyingImplementation.Argument.HasDefaultValue.ShouldBe(false);
            underlyingImplementation.IsRequired.ShouldBe(false);

            // Test without specifying parameter
            var app = new TestApplicationWithParams(
                () => param.Value.ShouldBeNull(),
                param
            );
            app.Run();
            app.ShouldHaveNoOutput();

            // Test with specifying parameter
            app = new TestApplicationWithParams(
                () => param.Value.ShouldBe(44),
                param
            );
            app.Run("--value", "44");
            app.ShouldHaveNoOutput();
        }

        /// <summary>
        /// Tests that named parameters can have <c>null</c> as default value.
        /// </summary>
        [Fact]
        public void TestNamedParameter_WithNullDefaultValue_RefType()
        {
            // Setup
            var param = new CliParam<string?>("--value")
            {
                DefaultValue = null,
            };
            param.IsPositionalParameter.ShouldBe(false);

            // Test
            param.DefaultValue.IsSet.ShouldBe(true);
            param.DefaultValue.Value.ShouldBeNull();

            var underlyingImplementation = (Option<string?>)param.UnderlyingImplementation;
            underlyingImplementation.Argument.HasDefaultValue.ShouldBe(false);
            underlyingImplementation.IsRequired.ShouldBe(false);

            // Test without specifying parameter
            var app = new TestApplicationWithParams(
                () => param.Value.ShouldBeNull(),
                param
            );
            app.Run();
            app.ShouldHaveNoOutput();

            // Test with specifying parameter
            app = new TestApplicationWithParams(
                () => param.Value.ShouldBe("abc"),
                param
            );
            app.Run("--value", "abc");
            app.ShouldHaveNoOutput();
        }

        [Fact]
        public void TestNamedParameter_WithoutDefaultValue()
        {
            // Setup
            var param = new CliParam<int>("--value");
            param.IsPositionalParameter.ShouldBe(false);

            // Test
            param.DefaultValue.IsSet.ShouldBe(false);

            var underlyingImplementation = (Option<int>)param.UnderlyingImplementation;
            underlyingImplementation.Argument.HasDefaultValue.ShouldBe(false);
            underlyingImplementation.IsRequired.ShouldBe(true);

            // Test without specifying parameter
            var app = new TestApplicationWithParams(
                () => throw new Exception("We should not get here."),
                param
            );
            app.RunWithExpectedExitCode(1);
            app.TerminalOutput.ShouldContain("Option '--value' is required.");

            // Test with specifying parameter
            app = new TestApplicationWithParams(
                () => param.Value.ShouldBe(44),
                param
            );
            app.Run("--value", "44");
            app.ShouldHaveNoOutput();
        }

        /// <summary>
        /// Tests that bool named params (flags) have an automatic default value of "false".
        /// </summary>
        [Fact]
        public void TestNamedBoolParameter_WithoutDefaultValue()
        {
            // Setup
            var param = new CliParam<bool>("--verbose");
            param.IsPositionalParameter.ShouldBe(false);

            // Test
            param.DefaultValue.IsSet.ShouldBe(true);

            var underlyingImplementation = (Option<bool>)param.UnderlyingImplementation;
            underlyingImplementation.Argument.HasDefaultValue.ShouldBe(false);
            underlyingImplementation.IsRequired.ShouldBe(false);

            // Test without specifying parameter
            var app = new TestApplicationWithParams(
                () => param.Value.ShouldBe(false),
                param
            );
            app.Run();
            app.ShouldHaveNoOutput();

            // Test with specifying parameter without value
            app = new TestApplicationWithParams(
                () => param.Value.ShouldBe(true),
                param
            );
            app.Run("--verbose");
            app.ShouldHaveNoOutput();

            // Test with specifying parameter with value=true
            app = new TestApplicationWithParams(
                () => param.Value.ShouldBe(true),
                param
            );
            app.Run("--verbose", "true");
            app.ShouldHaveNoOutput();

            // Test with specifying parameter with value=false
            app = new TestApplicationWithParams(
                () => param.Value.ShouldBe(false),
                param
            );
            app.Run("--verbose", "false");
            app.ShouldHaveNoOutput();
        }

        [Fact]
        public void TestPositionalParameter_WithDefaultValue()
        {
            // Setup
            var param = new CliParam<string>("value", positionIndex: 42)
            {
                DefaultValue = "abc",
            };
            param.IsPositionalParameter.ShouldBe(true);

            // Test
            param.DefaultValue.IsSet.ShouldBe(true);
            param.DefaultValue.Value.ShouldBe("abc");

            var underlyingImplementation = (Argument<string>)param.UnderlyingImplementation;
            underlyingImplementation.HasDefaultValue.ShouldBe(true);

            // Test without specifying parameter
            var app = new TestApplicationWithParams(
                () => param.Value.ShouldBe("abc"),
                param
            );
            app.Run();
            app.ShouldHaveNoOutput();

            // Test with specifying parameter
            app = new TestApplicationWithParams(
                () => param.Value.ShouldBe("def"),
                param
            );
            app.Run("def");
            app.ShouldHaveNoOutput();
        }

        [Fact]
        public void TestPositionalParameter_WithoutDefaultValue()
        {
            // Setup
            var param = new CliParam<string>("value", positionIndex: 42);
            param.IsPositionalParameter.ShouldBe(true);

            // Test
            param.DefaultValue.IsSet.ShouldBe(false);

            var underlyingImplementation = (Argument<string>)param.UnderlyingImplementation;
            underlyingImplementation.HasDefaultValue.ShouldBe(false);

            // Test without specifying parameter
            var app = new TestApplicationWithParams(
                () => throw new Exception("We should not get here"),
                param
            );
            app.RunWithExpectedExitCode(1);
            app.TerminalOutput.ShouldContain("Required argument missing for command");

            // Test with specifying parameter
            app = new TestApplicationWithParams(
                () => param.Value.ShouldBe("def"),
                param
            );
            app.Run("def");
            app.ShouldHaveNoOutput();
        }

        [Fact]
        public void TestValue_NamedParam_WithoutDefaultValue()
        {
            var valueTypeParam = new CliParam<int>("--value", "-v");
            var refTypeParam = new CliParam<string>("--value", "-v");

            Should.Throw<InvalidOperationException>(() => valueTypeParam.Value);
            Should.Throw<InvalidOperationException>(() => refTypeParam.Value);
        }

        [Fact]
        public void TestValue_PositionalParam_WithoutDefaultValue()
        {
            var valueTypeParam = new CliParam<int>("value", positionIndex: 1);
            var refTypeParam = new CliParam<string>("value", positionIndex: 2);

            Should.Throw<InvalidOperationException>(() => valueTypeParam.Value);
            Should.Throw<InvalidOperationException>(() => refTypeParam.Value);
        }

        [Fact]
        public void TestValue_NamedParam_WithDefaultValue()
        {
            var valueTypeParam = new CliParam<int>("--value", "-v")
            {
                DefaultValue = 42,
            };
            var refTypeParam = new CliParam<string>("--value", "-v")
            {
                DefaultValue = "abc",
            };

            Should.Throw<InvalidOperationException>(() => valueTypeParam.Value);
            Should.Throw<InvalidOperationException>(() => refTypeParam.Value);
        }

        [Fact]
        public void TestValue_PositionalParam_WithDefaultValue()
        {
            var valueTypeParam = new CliParam<int>("value", positionIndex: 1)
            {
                DefaultValue = 42,
            };
            var refTypeParam = new CliParam<string>("value", positionIndex: 2)
            {
                DefaultValue = "abc",
            };

            Should.Throw<InvalidOperationException>(() => valueTypeParam.Value);
            Should.Throw<InvalidOperationException>(() => refTypeParam.Value);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void TestNullableParameter_ValueType_WithoutDefaultValue(bool named)
        {
            // Setup
            var param = named ? new CliParam<int?>("--value")
                              : new CliParam<int?>("value", positionIndex: 0);
            param.IsNamedParameter.ShouldBe(named);

            // Test
            param.DefaultValue.IsSet.ShouldBe(true);
            param.DefaultValue.Value.ShouldBe(null);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void TestNonNullableParameter_ValueType_WithoutDefaultValue(bool named)
        {
            // Setup
            var param = named ? new CliParam<int>("--value")
                      : new CliParam<int>("value", positionIndex: 0);
            param.IsNamedParameter.ShouldBe(named);

            // Test
            param.DefaultValue.IsSet.ShouldBe(false);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void TestNullableParameter_RefType_WithoutDefaultValue(bool named)
        {
            // Setup
            var param = named ? new CliParam<string?>("--value")
                              : new CliParam<string?>("value", positionIndex: 0);
            param.IsNamedParameter.ShouldBe(named);

            // Test
            // NOTE: Ideally we would the default value to be set for nullable reference types
            //   (like for nullable value types). Unfortunately, .NET's type system doesn't
            //   record the nullability of reference types. So there's no way for the CliParam
            //   class to differentiate between "string" and "string?".
            param.DefaultValue.IsSet.ShouldBe(false);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void TestNonNullableParameter_RefType_WithoutDefaultValue(bool named)
        {
            // Setup
            var param = named ? new CliParam<string>("--value")
                              : new CliParam<string>("value", positionIndex: 0);
            param.IsNamedParameter.ShouldBe(named);

            // Test
            param.DefaultValue.IsSet.ShouldBe(false);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void TestNamedParameter_UnderlyingDefaultValue_SpecialCases_NullableValueType(bool explicitSet)
        {
            // Setup
            CliParam<int?> param;
            if (explicitSet)
            {
                param = new CliParam<int?>("--value")
                {
                    DefaultValue = null,
                };
            }
            else
            {
                param = new CliParam<int?>("--value");
            }
            param.IsPositionalParameter.ShouldBe(false);

            // Test
            param.DefaultValue.IsSet.ShouldBe(true);

            var underlyingImplementation = (Option<int?>)param.UnderlyingImplementation;
            underlyingImplementation.IsRequired.ShouldBe(false);
            underlyingImplementation.Argument.HasDefaultValue.ShouldBe(false);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void TestPositionalParameter_UnderlyingDefaultValue_SpecialCases_NullableValueType(bool explicitSet)
        {
            // Setup
            CliParam<int?> param;
            if (explicitSet)
            {
                param = new CliParam<int?>("value", positionIndex: 42)
                {
                    DefaultValue = null,
                };
            }
            else
            {
                param = new CliParam<int?>("value", positionIndex: 42);
            }
            param.IsPositionalParameter.ShouldBe(true);

            // Test
            param.DefaultValue.IsSet.ShouldBe(true);

            var underlyingImplementation = (Argument<int?>)param.UnderlyingImplementation;
            underlyingImplementation.HasDefaultValue.ShouldBe(false);

            var app = new TestApplicationWithParams(
                () => param.Value.ShouldBe(null),
                param
            );
            app.Run();
            app.ShouldHaveNoOutput();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void TestNamedParameter_UnderlyingDefaultValue_SpecialCases_NullableRefType(bool explicitSet)
        {
            // Setup
            CliParam<string?> param;
            if (explicitSet)
            {
                param = new CliParam<string?>("--value")
                {
                    DefaultValue = null,
                };
            }
            else
            {
                param = new CliParam<string?>("--value");
            }
            param.IsPositionalParameter.ShouldBe(false);

            // Test
            param.DefaultValue.IsSet.ShouldBe(explicitSet);

            var underlyingImplementation = (Option<string?>)param.UnderlyingImplementation;
            underlyingImplementation.IsRequired.ShouldBe(!explicitSet);
            underlyingImplementation.Argument.HasDefaultValue.ShouldBe(false);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void TestPositionalParameter_UnderlyingDefaultValue_SpecialCases_NullableRefType(bool explicitSet)
        {
            // Setup
            CliParam<string?> param;
            if (explicitSet)
            {
                param = new CliParam<string?>("value", positionIndex: 42)
                {
                    DefaultValue = null,
                };
            }
            else
            {
                param = new CliParam<string?>("value", positionIndex: 42);
            }
            param.IsPositionalParameter.ShouldBe(true);

            // Test
            param.DefaultValue.IsSet.ShouldBe(explicitSet);

            var underlyingImplementation = (Argument<string?>)param.UnderlyingImplementation;
            underlyingImplementation.HasDefaultValue.ShouldBe(false);

            // Unfortunately, we can't differentiate between "CliParam<string?>" and "CliParam<string>". So,
            // the "DefaultValue" property is the only way for reference type parameters to differentiate
            // between optional and required parameter (unlike with value types where nullable value types
            // are optional by default).
            if (explicitSet)
            {
                var app = new TestApplicationWithParams(
                    () => param.Value.ShouldBe(null),
                    param
                );
                app.Run();
                app.ShouldHaveNoOutput();
            }
            else
            {
                var app = new TestApplicationWithParams(
                    () => throw new InvalidOperationException("We should not get here."),
                    param
                );
                app.RunWithExpectedExitCode(1);
                app.TerminalOutput.ShouldContain("Required argument missing");
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void TestNamedParameter_UnderlyingDefaultValue_SpecialCases_Bool(bool explicitSet)
        {
            // Setup
            CliParam<bool> param;
            if (explicitSet)
            {
                param = new CliParam<bool>("--value")
                {
                    DefaultValue = false,
                };
            }
            else
            {
                param = new CliParam<bool>("--value");
            }
            param.IsPositionalParameter.ShouldBe(false);

            // Test
            param.DefaultValue.IsSet.ShouldBe(true);

            var underlyingImplementation = (Option<bool>)param.UnderlyingImplementation;
            underlyingImplementation.IsRequired.ShouldBe(false);
            underlyingImplementation.Argument.HasDefaultValue.ShouldBe(false);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void TestPositionalParameter_UnderlyingDefaultValue_SpecialCases_Bool(bool explicitSet)
        {
            // Setup
            CliParam<bool> param;
            if (explicitSet)
            {
                param = new CliParam<bool>("value", positionIndex: 42)
                {
                    DefaultValue = false,
                };
            }
            else
            {
                param = new CliParam<bool>("value", positionIndex: 42);
            }
            param.IsPositionalParameter.ShouldBe(true);

            // Test
            param.DefaultValue.IsSet.ShouldBe(explicitSet);

            var underlyingImplementation = (Argument<bool>)param.UnderlyingImplementation;
            underlyingImplementation.HasDefaultValue.ShouldBe(explicitSet);

            if (explicitSet)
            {
                var app = new TestApplicationWithParams(
                    () => param.Value.ShouldBe(false),
                    param
                );
                app.Run();
                app.ShouldHaveNoOutput();
            }
            else
            {
                var app = new TestApplicationWithParams(
                    () => throw new InvalidOperationException("We should not get here."),
                    param
                );
                app.RunWithExpectedExitCode(1);
                app.TerminalOutput.ShouldContain("Required argument missing");
            }
        }

        [Fact]
        public void TestConstructor_EmptyNamesList()
        {
            Should.Throw<ArgumentException>(() => new CliParam<int>(Array.Empty<string>()));
        }

        [Fact]
        public void TestConstructor_DuplicateNameForNamedParam()
        {
            var ex1 = Should.Throw<ArgumentException>(() => new CliParam<int>("--value", "--value"));
            ex1.Message.ShouldContain("--value");
            var ex2 = Should.Throw<ArgumentException>(() => new CliParam<int>("--value", "-v", "--value"));
            ex2.Message.ShouldContain("--value");
            var ex3 = Should.Throw<ArgumentException>(() => new CliParam<int>("--value", "--value", "-v"));
            ex3.Message.ShouldContain("--value");
            var ex4 = Should.Throw<ArgumentException>(() => new CliParam<int>("--value", "-v", "-v"));
            ex4.Message.ShouldContain("-v");
            var ex5 = Should.Throw<ArgumentException>(() => new CliParam<int>("--value", "--vv", "-v", "-v"));
            ex5.Message.ShouldContain("-v");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void TestInvalidName_NamedParameter(string? name)
        {
            Should.Throw<ArgumentException>(() => new CliParam<int>(name!));
            Should.Throw<ArgumentException>(() => new CliParam<int>("--value", name!));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void TestInvalidName_PositionalParameter(string? name)
        {
            Should.Throw<ArgumentException>(() => new CliParam<int>(name!, positionIndex: 5));
        }

        [Theory]
        [InlineData("--help")]
        [InlineData("--Help")]
        [InlineData("-h")]
        [InlineData("-?")]
        [InlineData("/h")]
        [InlineData("/?")]
        public void TestHelpNames_NamedParameter(string name)
        {
            Should.Throw<ArgumentException>(() => new CliParam<bool>(name));
        }

        [Theory]
        [InlineData("--help")]
        [InlineData("--Help")]
        [InlineData("-h")]
        [InlineData("-?")]
        [InlineData("/h")]
        [InlineData("/?")]
        public void TestHelpNames_PositionalParameter(string name)
        {
            Should.Throw<ArgumentException>(() => new CliParam<bool>(name, positionIndex: 0));
        }

        /// <summary>
        /// This test primarily exists to get code coverage to 100% for <see cref="CliParam{T}"/>.
        /// </summary>
        [Fact]
        public void TestInvalidStates()
        {
            // Setup
            var parser = new CommandLineBuilder()
                             .UseDefaults()
                             .Build();

            var parseResult = parser.Parse(new string[] { });

            var optionParam = new CliParam<int>("--value");
            var argumentParam = new CliParam<int>("value", positionIndex: 0);
            var invalidTypeParam = new InvalidParamTypeParam();

            // Test
            Should.Throw<InvalidOperationException>(() => optionParam.SetValueFromParseResult(parseResult));
            Should.Throw<InvalidOperationException>(() => argumentParam.SetValueFromParseResult(parseResult));
            Should.Throw<UnexpectedSwitchValueException>(() => invalidTypeParam.SetValueFromParseResult(parseResult));
        }

        private sealed class InvalidParamTypeParam : CliParam<int>
        {
            public InvalidParamTypeParam() : base("invalid", positionIndex: 0)
            {
            }

            /// <inheritdoc />
            internal override Symbol UnderlyingImplementation { get; } = new Argument();
        }
    }
}

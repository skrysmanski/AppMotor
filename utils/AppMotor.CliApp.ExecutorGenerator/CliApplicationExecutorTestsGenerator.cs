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

using System.Text;

using AppMotor.Core.Exceptions;
using AppMotor.Core.Utils;

using JetBrains.Annotations;

namespace AppMotor.CliApp.ExecutorGenerator
{
    internal sealed class CliApplicationExecutorTestsGenerator : ExecutorTestsGeneratorBase
    {
        /// <inheritdoc />
        protected override void GenerateClassContentCore()
        {
            AppendTestMethods(async: false);
            AppendTestMethods(async: true);
        }

        private void AppendTestMethods(bool async)
        {
            foreach (var returnType in EnumUtils.GetValues<ReturnTypes>())
            {
                AppendLines(CreateTestMethod(new TestMethodDescriptor(returnType, async: async, withArgs: false)));
                AppendLine();

                AppendLines(CreateTestMethod(new TestMethodDescriptor(returnType, async: async, withArgs: true)));
                AppendLine();
            }
        }

        [MustUseReturnValue]
        private static string CreateTestMethod(TestMethodDescriptor descriptor)
        {
            string testMethodName = CreateTestMethodName(descriptor);

            return FixMultiLineText($@"
{CreateTestAttributes(descriptor)}
public void {testMethodName}({CreateTestMethodParameterList(descriptor)})
{{
    // Setup
    bool called = false;

{CreateExecuteMethod(descriptor)}

    var testApplication = new TestApplication(new CliApplicationExecutor(Execute));

    // Test
    testApplication.{CreateRunMethodCall(descriptor)};

    // Verify
    called.ShouldBe(true);
    testApplication.ShouldHaveNoOutput();
}}
");
        }

        [MustUseReturnValue]
        private static string CreateTestAttributes(TestMethodDescriptor descriptor)
        {
            switch (descriptor.ReturnType)
            {
                case ReturnTypes.Void:
                    return "[Fact]";

                case ReturnTypes.Bool:
                    return FixMultiLineText(@"
[Theory]
[InlineData(true)]
[InlineData(false)]
");

                case ReturnTypes.Int:
                    return FixMultiLineText(@"
[Theory]
[InlineData(0)]
[InlineData(1)]
[InlineData(42)]
");

                default:
                    throw new UnexpectedSwitchValueException(nameof(descriptor.ReturnType), descriptor.ReturnType);
            }
        }

        [MustUseReturnValue]
        private static string CreateTestMethodParameterList(TestMethodDescriptor descriptor)
        {
            if (descriptor.ReturnType == ReturnTypes.Void)
            {
                return "";
            }
            else
            {
                return $"{descriptor.ReturnType.ToString().ToLowerInvariant()} retVal";
            }
        }

        [MustUseReturnValue]
        private static string CreateTestMethodName(TestMethodDescriptor descriptor)
        {
            var nameBuilder = new StringBuilder();

            nameBuilder.Append("Test_");

            if (descriptor.Async)
            {
                nameBuilder.Append("Async_");
            }
            else
            {
                nameBuilder.Append("Sync_");
            }

            nameBuilder.Append($"{descriptor.ReturnType}_");

            if (descriptor.WithArgs)
            {
                nameBuilder.Append("WithArgs");
            }
            else
            {
                nameBuilder.Append("NoArgs");
            }

            return nameBuilder.ToString();
        }

        [MustUseReturnValue]
        private static string CreateRunMethodCall(TestMethodDescriptor descriptor)
        {
            switch (descriptor.ReturnType)
            {
                case ReturnTypes.Void:
                    return "Run(TEST_ARGS)";

                case ReturnTypes.Bool:
                    return "RunWithExpectedExitCode(expectedExitCode: retVal ? 0 : 1, TEST_ARGS)";

                case ReturnTypes.Int:
                    return "RunWithExpectedExitCode(expectedExitCode: retVal, TEST_ARGS)";

                default:
                    throw new UnexpectedSwitchValueException(nameof(descriptor.ReturnType), descriptor.ReturnType);
            }
        }

        [MustUseReturnValue]
        private static string CreateExecuteMethod(TestMethodDescriptor descriptor)
        {
            return FixMultiLineText($@"
    {CreateExecuteMethodSignature(descriptor)}
    {{
{CreateExecuteMethodBody(descriptor)}
    }}
");
        }

        private static string CreateExecuteMethodSignature(TestMethodDescriptor descriptor)
        {
            string returnTypeForSignature;

            if (descriptor.Async)
            {
                if (descriptor.ReturnType == ReturnTypes.Void)
                {
                    returnTypeForSignature = "async Task";
                }
                else
                {
                    returnTypeForSignature = $"async Task<{descriptor.ReturnType.ToString().ToLowerInvariant()}>";
                }
            }
            else
            {
                returnTypeForSignature = descriptor.ReturnType.ToString().ToLowerInvariant();
            }

            string parameterList = descriptor.WithArgs ? "string[] args" : "";

            return $"{returnTypeForSignature} Execute({parameterList})";
        }

        private static string CreateExecuteMethodBody(TestMethodDescriptor descriptor)
        {
            var bodyBuilder = new StringBuilder();

            if (descriptor.Async)
            {
                bodyBuilder.AppendLine($"{INDENTATION_LEVEL}{INDENTATION_LEVEL}await Task.Delay(1);");
            }

            bodyBuilder.AppendLine($"{INDENTATION_LEVEL}{INDENTATION_LEVEL}called = true;");

            if (descriptor.WithArgs)
            {
                bodyBuilder.AppendLine($"{INDENTATION_LEVEL}{INDENTATION_LEVEL}args.ShouldBe(TEST_ARGS);");
            }

            if (descriptor.ReturnType != ReturnTypes.Void)
            {
                bodyBuilder.AppendLine($"{INDENTATION_LEVEL}{INDENTATION_LEVEL}return retVal;");
            }

            return bodyBuilder.ToString().TrimEnd();
        }

        private sealed class TestMethodDescriptor
        {
            public ReturnTypes ReturnType { get; }

            public bool Async { get; }

            public bool WithArgs { get; }

            public TestMethodDescriptor(ReturnTypes returnType, bool async, bool withArgs)
            {
                this.ReturnType = returnType;
                this.Async = async;
                this.WithArgs = withArgs;
            }
        }
    }
}

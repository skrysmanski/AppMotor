﻿#region License
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
using System.Text;

using AppMotor.Core.Exceptions;
using AppMotor.Core.Utils;

using JetBrains.Annotations;

namespace AppMotor.CliApp.ExecutorGenerator
{
    internal abstract class ExecutorTestsGeneratorBase : SourceCodeGeneratorBase
    {
        protected enum ReturnTypes
        {
            Void,
            Bool,
            Int,
        }

        /// <inheritdoc />
        protected sealed override void GenerateClassContentCore()
        {
            AppendTestMethods(async: false);
            AppendTestMethods(async: true);
        }

        private void AppendTestMethods(bool async)
        {
            foreach (var returnType in EnumUtils.GetValues<ReturnTypes>())
            {
                foreach (var testMethodCode in CreateTestMethods(async, returnType))
                {
                    AppendLines(testMethodCode);
                    AppendLine();
                }
            }
        }

        protected abstract IEnumerable<string> CreateTestMethods(bool async, ReturnTypes returnType);

        [MustUseReturnValue]
        protected string CreateTestMethod(TestMethodDescriptor descriptor)
        {
            string testMethodName = CreateTestMethodName(descriptor);

            return FixMultiLineText($@"
{CreateTestAttributes(descriptor)}
public void {testMethodName}({CreateTestMethodParameterList(descriptor)})
{{
    // Setup
{CreateSetupCode(descriptor)}

{CreateExecuteMethod(descriptor)}

    var testApplication = new TestApplication({CreateTestApplicationParam(descriptor)});

    // Test
    testApplication.AppHelper.{CreateRunMethodCall(descriptor)};

    // Verify
    called.ShouldBe(true);
    testApplication.ShouldHaveNoOutput();
}}
");
        }

        [MustUseReturnValue]
        protected abstract string CreateTestMethodName(TestMethodDescriptor descriptor);

        [MustUseReturnValue]
        protected abstract string CreateTestApplicationParam(TestMethodDescriptor descriptor);

        [MustUseReturnValue]
        protected static string CreateTestAttributes(TestMethodDescriptor descriptor)
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
        protected static string CreateTestMethodParameterList(TestMethodDescriptor descriptor)
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
        protected static string CreateSetupCode(TestMethodDescriptor descriptor)
        {
            var builder = new StringBuilder();

            builder.AppendLine($"{INDENTATION_LEVEL}bool called = false;");

            if (descriptor.WithCancellationToken)
            {
                builder.AppendLine();
                builder.AppendLine($"{INDENTATION_LEVEL}using var cts = new CancellationTokenSource();");
            }

            return builder.ToString().TrimEnd();
        }

        [MustUseReturnValue]
        protected abstract string CreateAppRunArgsArgument();

        [MustUseReturnValue]
        protected string CreateRunMethodCall(TestMethodDescriptor descriptor)
        {
            var builder = new StringBuilder();

            builder.Append($"Run({CreateAppRunArgsArgument()}, expectedExitCode: ");

            switch (descriptor.ReturnType)
            {
                case ReturnTypes.Void:
                    builder.Append("0");
                    break;

                case ReturnTypes.Bool:
                    builder.Append("retVal ? 0 : 1");
                    break;

                case ReturnTypes.Int:
                    builder.Append("retVal");
                    break;

                default:
                    throw new UnexpectedSwitchValueException(nameof(descriptor.ReturnType), descriptor.ReturnType);
            }

            if (descriptor.WithCancellationToken)
            {
                builder.Append(", cts.Token");
            }

            builder.Append(")");

            return builder.ToString();
        }

        [MustUseReturnValue]
        protected static string CreateExecuteMethod(TestMethodDescriptor descriptor)
        {
            return FixMultiLineText($@"
    {CreateExecuteMethodSignature(descriptor)}
    {{
{CreateExecuteMethodBody(descriptor)}
    }}
");
        }

        [MustUseReturnValue]
        protected static string CreateExecuteMethodSignature(TestMethodDescriptor descriptor)
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

            var parameterList = new List<string>();
            if (descriptor.WithArgs)
            {
                parameterList.Add("string[] args");
            }
            if (descriptor.WithCancellationToken)
            {
                parameterList.Add("CancellationToken cancellationToken");
            }

            return $"{returnTypeForSignature} Execute({string.Join(", ", parameterList)})";
        }

        [MustUseReturnValue]
        protected static string CreateExecuteMethodBody(TestMethodDescriptor descriptor)
        {
            var bodyBuilder = new StringBuilder();

            if (descriptor.Async)
            {
                if (descriptor.WithCancellationToken)
                {
                    bodyBuilder.AppendLine($"{INDENTATION_LEVEL}{INDENTATION_LEVEL}// ReSharper disable once MethodSupportsCancellation");
                }

                bodyBuilder.AppendLine($"{INDENTATION_LEVEL}{INDENTATION_LEVEL}await Task.Delay(1);");
            }

            bodyBuilder.AppendLine($"{INDENTATION_LEVEL}{INDENTATION_LEVEL}called = true;");

            if (descriptor.WithArgs)
            {
                bodyBuilder.AppendLine($"{INDENTATION_LEVEL}{INDENTATION_LEVEL}args.ShouldBe(TEST_ARGS);");
            }

            if (descriptor.WithCancellationToken)
            {
                bodyBuilder.AppendLine($"{INDENTATION_LEVEL}{INDENTATION_LEVEL}cancellationToken.IsCancellationRequested.ShouldBe(false);");
                bodyBuilder.AppendLine($"{INDENTATION_LEVEL}{INDENTATION_LEVEL}cts.Cancel();");
                bodyBuilder.AppendLine($"{INDENTATION_LEVEL}{INDENTATION_LEVEL}cancellationToken.IsCancellationRequested.ShouldBe(true); // Validates we actually got the token from \"cts\"");
            }

            if (descriptor.ReturnType != ReturnTypes.Void)
            {
                bodyBuilder.AppendLine($"{INDENTATION_LEVEL}{INDENTATION_LEVEL}return retVal;");
            }

            return bodyBuilder.ToString().TrimEnd();
        }

        protected sealed class TestMethodDescriptor
        {
            public ReturnTypes ReturnType { get; }

            public bool Async { get; }

            public bool WithArgs { get; }

            public bool WithCancellationToken { get; }

            public TestMethodDescriptor(ReturnTypes returnType, bool async, bool withArgs, bool withCancellationToken)
            {
                this.ReturnType = returnType;
                this.Async = async;
                this.WithArgs = withArgs;
                this.WithCancellationToken = withCancellationToken;
            }
        }
    }
}

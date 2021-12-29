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
using System.Text;

namespace AppMotor.CliApp.ExecutorGenerator;

internal sealed class CliCommandExecutorTestsGenerator : ExecutorTestsGeneratorBase
{
    /// <inheritdoc />
    protected override IEnumerable<string> CreateTestMethods(bool async, ReturnTypes returnType)
    {
        yield return CreateTestMethod(new TestMethodDescriptor(returnType, async: async, withCancellationToken: false, withArgs: false));

        yield return CreateTestMethod(new TestMethodDescriptor(returnType, async: async, withCancellationToken: true, withArgs: false));
    }

    protected override string CreateTestMethodName(TestMethodDescriptor descriptor)
    {
        var nameBuilder = new StringBuilder();

        nameBuilder.Append("Test");

        if (descriptor.Async)
        {
            nameBuilder.Append("_Async");
        }
        else
        {
            nameBuilder.Append("_Sync");
        }

        nameBuilder.Append($"_{descriptor.ReturnType}");

        if (descriptor.WithCancellationToken)
        {
            nameBuilder.Append("_WithCancellationToken");
        }
        else
        {
            nameBuilder.Append("_NoCancellationToken");
        }

        return nameBuilder.ToString();
    }

    /// <inheritdoc />
    protected override string CreateTestApplicationParam(TestMethodDescriptor descriptor)
    {
        return "new CliCommandExecutor(Execute)";
    }

    /// <inheritdoc />
    protected override string CreateAppRunArgsArgument()
    {
        return "new[] { COMMAND_NAME }";
    }
}
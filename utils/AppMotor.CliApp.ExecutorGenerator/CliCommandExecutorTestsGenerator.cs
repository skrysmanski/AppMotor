// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

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
    protected override string CreateTestApplicationParam()
    {
        return "new CliCommandExecutor(Execute)";
    }

    /// <inheritdoc />
    protected override string CreateAppRunArgsArgument()
    {
        return "new[] { COMMAND_NAME }";
    }
}

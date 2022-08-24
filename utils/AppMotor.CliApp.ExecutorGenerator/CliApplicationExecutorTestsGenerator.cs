// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Text;

namespace AppMotor.CliApp.ExecutorGenerator;

internal sealed class CliApplicationExecutorTestsGenerator : ExecutorTestsGeneratorBase
{
    /// <inheritdoc />
    protected override IEnumerable<string> CreateTestMethods(bool async, ReturnTypes returnType)
    {
        yield return CreateTestMethod(new TestMethodDescriptor(returnType, async: async, withArgs: false, withCancellationToken: false));

        yield return CreateTestMethod(new TestMethodDescriptor(returnType, async: async, withArgs: true, withCancellationToken: false));

        yield return CreateTestMethod(new TestMethodDescriptor(returnType, async: async, withArgs: false, withCancellationToken: true));

        yield return CreateTestMethod(new TestMethodDescriptor(returnType, async: async, withArgs: true, withCancellationToken: true));
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

        if (descriptor.WithArgs)
        {
            nameBuilder.Append("_WithArgs");
        }
        else
        {
            nameBuilder.Append("_NoArgs");
        }

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
        return "new CliApplicationExecutor(Execute)";
    }

    /// <inheritdoc />
    protected override string CreateAppRunArgsArgument()
    {
        return "TEST_ARGS";
    }
}

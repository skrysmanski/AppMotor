// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.CliApp.CommandLine;

namespace AppMotor.CliApp.TestUtils;

internal class TestApplicationWithParams : TestApplicationWithParamsBase
{
    private readonly Action _mainAction;

    private readonly List<CliParamBase> _params = new();

    /// <inheritdoc />
    protected override CliCommandExecutor Executor => new(Execute);

    public TestApplicationWithParams(Action mainAction, params CliParamBase[] cliParams)
    {
        this._mainAction = mainAction;

        this._params.AddRange(cliParams);
    }

    /// <inheritdoc />
    protected override IEnumerable<CliParamBase> GetAllParams()
    {
        return this._params;
    }

    private void Execute()
    {
        this._mainAction();
    }
}
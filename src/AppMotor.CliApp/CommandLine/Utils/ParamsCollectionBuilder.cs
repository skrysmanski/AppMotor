// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Collections.Immutable;

using AppMotor.Core.Extensions;

using JetBrains.Annotations;

namespace AppMotor.CliApp.CommandLine.Utils;

internal sealed class ParamsCollectionBuilder
{
    private readonly List<CliParamBase> _params = [];

    private readonly HashSet<string> _aliases = [];

    public IReadOnlySet<string> RegisteredAliases => this._aliases;

    public void AddParam(CliParamBase param)
    {
        foreach (var paramName in param.Names)
        {
            if (this._aliases.Contains(paramName))
            {
                throw new InvalidOperationException($"The alias '{paramName}' is already used by another parameter.");
            }
        }

        this._aliases.AddRange(param.Names);
        this._params.Add(param);
    }

    [MustUseReturnValue]
    public ImmutableArray<CliParamBase> Build()
    {
        return this._params.ToImmutableArray();
    }
}

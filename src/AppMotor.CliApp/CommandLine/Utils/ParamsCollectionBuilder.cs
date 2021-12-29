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

using System.Collections.Immutable;

using AppMotor.Core.Extensions;

using JetBrains.Annotations;

namespace AppMotor.CliApp.CommandLine.Utils;

internal sealed class ParamsCollectionBuilder
{
    private readonly List<CliParamBase> _params = new();

    private readonly HashSet<string> _aliases = new();

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
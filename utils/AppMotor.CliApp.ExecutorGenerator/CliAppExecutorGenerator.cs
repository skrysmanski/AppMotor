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

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using JetBrains.Annotations;

namespace AppMotor.CliApp.ExecutorGenerator;

internal sealed class CliAppExecutorGenerator : SourceCodeGeneratorBase
{
    private readonly string _className;

    private readonly ImmutableArray<string> _optionalInParamTypes;

    private readonly Dictionary<string, string> _optionalInParamType2DescriptionMap = new();

    private readonly Dictionary<string, string> _optionalInParamType2NameMap = new();

    public CliAppExecutorGenerator(string className, params ExecutorParameterDescriptor[] optionInParameters)
    {
        if (optionInParameters.Select(desc => desc.ParameterType).ToHashSet().Count != optionInParameters.Length)
        {
            // NOTE: This is not supported because it would result in multiple constructors with the same signature.
            throw new ArgumentException("The in param types are not distinct. That's not supported.");
        }
        if (optionInParameters.Select(desc => desc.ParameterName).ToHashSet().Count != optionInParameters.Length)
        {
            throw new ArgumentException("The in param names are not distinct. That's not supported.");
        }

        this._className = className;
        this._optionalInParamTypes = optionInParameters.Select(desc => desc.ParameterType).ToImmutableArray();

        foreach (var optionInParameter in optionInParameters)
        {
            this._optionalInParamType2NameMap[optionInParameter.ParameterType] = optionInParameter.ParameterName;
            this._optionalInParamType2DescriptionMap[optionInParameter.ParameterType] = optionInParameter.ParameterDescription;
        }
    }

    /// <inheritdoc />
    protected override void GenerateClassContentCore()
    {
        if (this._optionalInParamTypes.Length != 0)
        {
            AppendLine($"private readonly Func<{string.Join(", ", this._optionalInParamTypes)}, Task<int>> _action;");
        }
        else
        {
            AppendLine("private readonly Func<Task<int>> _action;");
        }

        AppendLine();

        foreach (var isAsync in new[] { false, true })
        {
            foreach (var returnType in new[] { null, "int", "bool" })
            {
                foreach (var inParameters in GetInParameterPermutations())
                {
                    var constructorCode = GenerateConstructor(inParameters, returnType, isAsync);
                    AppendLines(constructorCode);
                    AppendLine();
                }
            }
        }

        var executeMethodCode = GenerateExecuteMethod();
        AppendLines(executeMethodCode);
    }

    private IEnumerable<IReadOnlyList<string>> GetInParameterPermutations()
    {
        if (this._optionalInParamTypes.Length > 2)
        {
            throw new NotSupportedException($"Support for {this._optionalInParamTypes.Length} parameter types has not yet been implemented.");
        }

        // No parameters
        yield return Array.Empty<string>();

        // One parameter
        foreach (var optionalInParamType in this._optionalInParamTypes)
        {
            yield return new[] { optionalInParamType };
        }

        if (this._optionalInParamTypes.Length == 2)
        {
            yield return this._optionalInParamTypes;
        }
    }

    [MustUseReturnValue]
    private string GenerateConstructor(IReadOnlyList<string> inParameterTypes, string? returnType, bool isAsync)
    {
        var documentation = GenerateConstructorDocumentation(inParameterTypes, returnType, isAsync);

        var paramType = GenerateConstructorParamType(inParameterTypes, returnType, isAsync);

        var constructorBody = GenerateConstructorBody(inParameterTypes, returnType, isAsync);

        return FixMultiLineText($@"
{documentation}
[PublicAPI]
public {this._className}({paramType} action)
{{
{constructorBody}
}}
");
    }

    [MustUseReturnValue]
    private string GenerateConstructorDocumentation(IReadOnlyList<string> inParameterTypes, string? returnType, bool isAsync)
    {
        string isSynchronousText = isAsync ? "is asynchronous" : "is synchronous";

        string exitCodeText;
        string exitCodeFullText;

        if (returnType is null)
        {
            if (isAsync)
            {
                exitCodeText = "no exit code (<c>Task</c>/<c>void</c>)";
            }
            else
            {
                exitCodeText = "no exit code (<c>void</c>)";
            }

            exitCodeFullText = "The exit code is always 0.";
        }
        else if (returnType == "bool")
        {
            exitCodeText = "a success <c>bool</c>";
            exitCodeFullText = "The return value of <c>true</c> is translated into the exit code 0; <c>false</c> is translated into 1.";
        }
        else
        {
            exitCodeText = "an exit code";
            exitCodeFullText = "The return value is directly taken as exit code.";
        }

        string inParamText;

        if (inParameterTypes.Count == 0)
        {
            inParamText = "";
        }
        else if (inParameterTypes.Count == 1)
        {
            inParamText = $", takes in {this._optionalInParamType2DescriptionMap[inParameterTypes[0]]}";
        }
        else
        {
            var allButLastDescriptions = inParameterTypes.SkipLast(1).Select(type => this._optionalInParamType2DescriptionMap[type]);
            inParamText = $", takes in {string.Join(", ", allButLastDescriptions)}, and {this._optionalInParamType2DescriptionMap[inParameterTypes[^1]]}";
        }

        return FixMultiLineText($@"
/// <summary>
/// Creates an executor for a method that: {isSynchronousText}{inParamText}, and returns {exitCodeText}.
///
/// <para>{exitCodeFullText}</para>
/// </summary>
");
    }

    [MustUseReturnValue]
    private static string GenerateConstructorParamType(IReadOnlyList<string> inParameterTypes, string? returnType, bool isAsync)
    {
        if (isAsync)
        {
            if (returnType is null)
            {
                returnType = "Task";
            }
            else
            {
                returnType = $"Task<{returnType}>";
            }
        }

        if (returnType is null)
        {
            if (inParameterTypes.Count != 0)
            {
                return $"Action<{string.Join(", ", inParameterTypes)}>";
            }
            else
            {
                return "Action";
            }
        }
        else
        {
            if (inParameterTypes.Count != 0)
            {
                return $"Func<{string.Join(", ", inParameterTypes)}, {returnType}>";
            }
            else
            {
                return $"Func<{returnType}>";
            }
        }
    }

    [MustUseReturnValue]
    private string GenerateConstructorBody(IReadOnlyList<string> inParameterTypes, string? returnType, bool isAsync)
    {
        if (returnType == "int" && isAsync && inParameterTypes.Count == this._optionalInParamTypes.Length)
        {
            return $"{INDENTATION_LEVEL}this._action = action;";
        }

        string lambdaParamList;

        if (this._optionalInParamTypes.Length == 0)
        {
            lambdaParamList = "()";
        }
        else if (this._optionalInParamTypes.Length == 1)
        {
            if (inParameterTypes.Count == 0)
            {
                lambdaParamList = "_";
            }
            else
            {
                lambdaParamList = this._optionalInParamType2NameMap[inParameterTypes[0]];
            }
        }
        else
        {
            List<string> lambdaParamNames = new();

            foreach (var optionalInParamType in this._optionalInParamTypes)
            {
                if (inParameterTypes.Contains(optionalInParamType))
                {
                    lambdaParamNames.Add(this._optionalInParamType2NameMap[optionalInParamType]);
                }
                else
                {
                    lambdaParamNames.Add("_");
                }
            }

            lambdaParamList = $"({string.Join(", ", lambdaParamNames)})";
        }

        var asyncLambdaPrefix = isAsync ? "async " : "";

        string actionCallPrefix = returnType is not null ? $"{returnType} retVal = " : "";

        if (isAsync)
        {
            actionCallPrefix += "await ";
        }

        string actionCallPostfix = isAsync ? ".ConfigureAwait(continueOnCapturedContext: false)" : "";

        string actionCallParamList = string.Join(", ", inParameterTypes.Select(type => this._optionalInParamType2NameMap[type]));

        string retValExpression;

        if (returnType is null)
        {
            retValExpression = "0";
        }
        else if (returnType == "bool")
        {
            retValExpression = "retVal ? 0 : 1";
        }
        else
        {
            retValExpression = "retVal";
        }

        string returnStatementContent = isAsync ? retValExpression : $"Task.FromResult({retValExpression})";

        return FixMultiLineText($@"
    this._action = {asyncLambdaPrefix}{lambdaParamList} =>
    {{
        {actionCallPrefix}action({actionCallParamList}){actionCallPostfix};
        return {returnStatementContent};
    }};
");
    }

    [MustUseReturnValue]
    private string GenerateExecuteMethod()
    {
        string parameters;
        string actionArgs;

        if (this._optionalInParamTypes.Length == 0)
        {
            parameters = "";
            actionArgs = "";
        }
        else
        {
            var parametersList = new List<string>();
            var actionArgsList = new List<string>();

            foreach (var paramType in this._optionalInParamTypes)
            {
                var paramName = this._optionalInParamType2NameMap[paramType];
                parametersList.Add($"{paramType} {paramName}");
                actionArgsList.Add(paramName);
            }

            parameters = string.Join(", ", parametersList);
            actionArgs = string.Join(", ", actionArgsList);
        }

        return FixMultiLineText($@"
/// <summary>
/// Executes this executor and returns the exit code.
/// </summary>
public async Task<int> Execute({parameters})
{{
    return await this._action({actionArgs}).ConfigureAwait(continueOnCapturedContext: false);
}}
");
    }
}
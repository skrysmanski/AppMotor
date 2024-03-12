// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Reflection;

using AppMotor.Core.Extensions;
using AppMotor.Core.Utils;

using JetBrains.Annotations;

namespace AppMotor.CliApp.CommandLine.Utils;

/// <summary>
/// Provides utility methods for <see cref="CliParam{T}"/> - mainly <see cref="GetAllParamsFor(object)"/>.
/// </summary>
public static class CliParamUtils
{
    /// <summary>
    /// Uses reflection to obtain all <see cref="CliParamBase"/> instances from the container. Note that only
    /// instance (i.e. non-static) properties and fields are checked. Base classes are also checked.
    /// Positional parameters (<see cref="CliParamBase.ParameterType"/>) are returned first and in order
    /// of <see cref="CliParamBase.PositionIndex"/>.
    /// </summary>
    /// <param name="container">The object that holds the parameters.</param>
    [MustUseReturnValue]
#pragma warning disable CA1002 // Do not expose generic lists // BUG: https://github.com/dotnet/roslyn-analyzers/issues/4508
    public static List<CliParamBase> GetAllParamsFor(object container)
#pragma warning restore CA1002 // Do not expose generic lists
    {
        Validate.ArgumentWithName(nameof(container)).IsNotNull(container);

        var allParams = new List<CliParamBase>();
        var alreadyFoundCliParams = new HashSet<CliParamBase>(ReferenceEqualityComparer.Instance);
        var allParamNames = new HashSet<string>();

        Type? containerType = container.GetType();

        while (containerType != null)
        {
            foreach (var cliParam in GetAllParamsFor(container, containerType, alreadyFoundCliParams, allParamNames))
            {
                allParams.Add(cliParam);
            }

            // NOTE: We need to do this because "Type.GetProperties()" and "Type.GetFields()" doesn't return the private
            //   properties and fields of base types.
            containerType = containerType.BaseType;
        }

        allParams.Sort(ParamComparer.INSTANCE);

        return allParams;
    }

    [MustUseReturnValue]
    private static IEnumerable<CliParamBase> GetAllParamsFor(object container, Type type, HashSet<CliParamBase> alreadyFoundCliParams, HashSet<string> allParamNames)
    {
        foreach (var propertyInfo in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            if (!propertyInfo.PropertyType.Is<CliParamBase>())
            {
                continue;
            }

            if (!propertyInfo.CanRead)
            {
                continue;
            }

            var cliParam = (CliParamBase?)propertyInfo.GetValue(container);
            if (cliParam == null)
            {
                continue;
            }

            if (AddIfNeeded(cliParam, alreadyFoundCliParams, allParamNames))
            {
                yield return cliParam;
            }
        }

        foreach (var fieldInfo in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            if (!fieldInfo.FieldType.Is<CliParamBase>())
            {
                continue;
            }

            var cliParam = (CliParamBase?)fieldInfo.GetValue(container);
            if (cliParam == null)
            {
                continue;
            }

            if (AddIfNeeded(cliParam, alreadyFoundCliParams, allParamNames))
            {
                yield return cliParam;
            }
        }
    }

    private static bool AddIfNeeded(CliParamBase cliParam, HashSet<CliParamBase> alreadyFoundCliParams, HashSet<string> allParamNames)
    {
        if (alreadyFoundCliParams.Contains(cliParam))
        {
            return false;
        }

        foreach (var cliParamName in cliParam.Names)
        {
            if (!allParamNames.Add(cliParamName))
            {
                throw new InvalidOperationException($"There are multiple parameters with name '{cliParamName}'.");
            }
        }

        alreadyFoundCliParams.Add(cliParam);

        return true;
    }

    /// <summary>
    /// Sorts positional parameters by their index; sorts positional parameters before
    /// named parameters.
    /// </summary>
    private sealed class ParamComparer : IComparer<CliParamBase>
    {
        public static readonly ParamComparer INSTANCE = new();

        /// <inheritdoc />
        public int Compare(CliParamBase? x, CliParamBase? y)
        {
            if (x?.PositionIndex == null && y?.PositionIndex == null)
            {
                return 0;
            }
            else if (x?.PositionIndex == null)
            {
                return 1;
            }
            else if (y?.PositionIndex == null)
            {
                return -1;
            }
            else
            {
                return x.PositionIndex.Value - y.PositionIndex.Value;
            }
        }
    }
}

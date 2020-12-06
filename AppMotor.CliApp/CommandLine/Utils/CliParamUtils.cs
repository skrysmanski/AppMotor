#region License
// Copyright 2020 AppMotor Framework (https://github.com/skrysmanski/AppMotor)
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
using System.Reflection;

using AppMotor.Core.Extensions;
using AppMotor.Core.Utils;

using JetBrains.Annotations;

namespace AppMotor.CliApp.CommandLine.Utils
{
    /// <summary>
    /// Provides utility methods for <see cref="CliParam{T}"/> - mainly <see cref="GetAllParamsFor(object)"/>.
    /// </summary>
    public static class CliParamUtils
    {
        /// <summary>
        /// Uses reflection to obtain all <see cref="CliParam"/> instances from the container. Note that only
        /// instance (i.e. non-static) properties and fields are checked. Base classes are also checked.
        /// Positional parameters (<see cref="CliParam.IsPositionalParameter"/>) are returned first and in order
        /// of <see cref="CliParam.PositionIndex"/>.
        /// </summary>
        /// <param name="container">The object that holds the parameters.</param>
        [MustUseReturnValue]
#pragma warning disable CA1002 // Do not expose generic lists // BUG: https://github.com/dotnet/roslyn-analyzers/issues/4508
        public static List<CliParam> GetAllParamsFor(object container)
#pragma warning restore CA1002 // Do not expose generic lists
        {
            Validate.Argument.IsNotNull(container, nameof(container));

            var allParams = new List<CliParam>();
            var alreadyFoundCliParams = new HashSet<CliParam>(ReferenceEqualityComparer.Instance);
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
        private static IEnumerable<CliParam> GetAllParamsFor(object container, Type type, HashSet<CliParam> alreadyFoundCliParams, HashSet<string> allParamNames)
        {
            foreach (var propertyInfo in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (!propertyInfo.PropertyType.Is<CliParam>())
                {
                    continue;
                }

                if (!propertyInfo.CanRead)
                {
                    continue;
                }

                var cliParam = (CliParam?)propertyInfo.GetValue(container);
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
                if (!fieldInfo.FieldType.Is<CliParam>())
                {
                    continue;
                }

                var cliParam = (CliParam?)fieldInfo.GetValue(container);
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

        private static bool AddIfNeeded(CliParam cliParam, HashSet<CliParam> alreadyFoundCliParams, HashSet<string> allParamNames)
        {
            if (alreadyFoundCliParams.Contains(cliParam))
            {
                return false;
            }

            foreach (var cliParamName in cliParam.Names)
            {
                if (allParamNames.Contains(cliParamName))
                {
                    throw new InvalidOperationException($"There are multiple parameters with name '{cliParamName}'.");
                }

                allParamNames.Add(cliParamName);
            }

            alreadyFoundCliParams.Add(cliParam);

            return true;
        }

        /// <summary>
        /// Sorts positional parameters by their index; sorts positional parameters before
        /// named parameters.
        /// </summary>
        private sealed class ParamComparer : IComparer<CliParam>
        {
            public static readonly ParamComparer INSTANCE = new();

            /// <inheritdoc />
            public int Compare(CliParam? x, CliParam? y)
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
}

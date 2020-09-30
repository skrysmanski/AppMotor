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
using System.Diagnostics.CodeAnalysis;

namespace AppMotor.Core.Utils
{
    // TODO: Remove, when https://github.com/dotnet/roslyn-analyzers/issues/4248 got implemented
    /// <summary>
    /// Tells the FxCop code analyzers (rule CA1062) that the parameter will be checked for null.
    ///
    /// <para>Note: This attribute should always be specified together with <see cref="NotNullAttribute"/>.</para>
    /// </summary>
    /// <remarks>
    /// The rule CA1062 checks for an attribute with this exact name (namespace doesn't matter).
    /// So the attribute MOST NOT BE RENAMED.
    /// </remarks>
    /// <seealso cref="Validate"/>
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class ValidatedNotNullAttribute : Attribute
    {
    }
}

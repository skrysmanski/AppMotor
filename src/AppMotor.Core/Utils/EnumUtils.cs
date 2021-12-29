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

using JetBrains.Annotations;

namespace AppMotor.Core.Utils;

/// <summary>
/// Utility method for enums.
/// </summary>
public static class EnumUtils
{
    /// <summary>
    /// Returns the enum values of the specified enum.
    /// </summary>
    /// <remarks>
    /// This is the typed version of <see cref="Enum.GetValues"/>.
    /// </remarks>
    [PublicAPI, Pure]
    public static TEnum[] GetValues<TEnum>() where TEnum : Enum
    {
        return (TEnum[])Enum.GetValues(typeof(TEnum));
    }
}
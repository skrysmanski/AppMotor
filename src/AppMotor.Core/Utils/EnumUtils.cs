// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

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

// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Security;

using AppMotor.Core.Extensions;
using AppMotor.Core.Utils;

using JetBrains.Annotations;

using NotNullOnExitAttribute = System.Diagnostics.CodeAnalysis.NotNullAttribute;

namespace AppMotor.Core.DataModel;

/// <summary>
/// Contains methods related to sensitive values. See <see cref="ISensitiveValue"/>
/// for more details.
/// </summary>
public static class SensitiveValues
{
    /// <summary>
    /// Returns whether the specified value is a sensitive value (see <see cref="ISensitiveValue"/>).
    /// Respects <see cref="SecureString"/>, <see cref="ISensitiveValue"/>, and <see cref="SensitiveValueMarker"/>.
    /// </summary>
    /// <seealso cref="IsSensitiveValueType"/>
    [PublicAPI, Pure]
    public static bool IsSensitiveValue<T>([NotNullOnExit] this T value) where T : notnull
    {
        Validate.ArgumentWithName(nameof(value)).IsNotNullUnconstrained(value);

        if (value is SecureString || value is ISensitiveValue)
        {
            return true;
        }

        if (value.GetType().IsMarkedWith<SensitiveValueMarker>())
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Returns whether the specified type is a type that may contain sensitive information (see <see cref="ISensitiveValue"/>).
    /// Respects <see cref="SecureString"/>, <see cref="ISensitiveValue"/>, and <see cref="SensitiveValueMarker"/>.
    /// </summary>
    /// <seealso cref="IsSensitiveValue{T}"/>
    [PublicAPI, Pure]
    public static bool IsSensitiveValueType(Type typeToCheck)
    {
        if (typeToCheck == typeof(SecureString) || typeToCheck.Is<ISensitiveValue>())
        {
            return true;
        }

        if (typeToCheck.IsMarkedWith<SensitiveValueMarker>())
        {
            return true;
        }

        return false;
    }
}
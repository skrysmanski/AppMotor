// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

namespace AppMotor.Core.Utils;

/// <summary>
/// Helper methods for <c>params</c> parameters.
/// </summary>
public static class ParamsUtils
{
    /// <summary>
    /// Combines <paramref name="first"/> and <paramref name="others"/> into a single enumerable.
    /// The main use case for this method is to have a method with <c>params</c> parameter where
    /// you want to make sure that the user provides at least one parameter.
    /// </summary>
    public static IEnumerable<T> Combine<T>(T first, T[] others)
    {
        Validate.ArgumentWithName(nameof(others)).IsNotNull(others);

        yield return first;

        foreach (var other in others)
        {
            yield return other;
        }
    }

    /// <summary>
    /// Combines <paramref name="first"/>, <paramref name="second"/>, and
    /// <paramref name="others"/> into a single enumerable. The main use case
    /// for this method is to have a method with <c>params</c> parameter where
    /// you want to make sure that the user provides at least two parameter.
    /// </summary>
    public static IEnumerable<T> Combine<T>(T first, T second, T[] others)
    {
        Validate.ArgumentWithName(nameof(others)).IsNotNull(others);

        yield return first;
        yield return second;

        foreach (var other in others)
        {
            yield return other;
        }
    }
}
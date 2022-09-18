// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using JetBrains.Annotations;

namespace AppMotor.Core.Extensions;

/// <summary>
/// Extension methods for array types.
/// </summary>
public static class ArrayExtensions
{
    /// <summary>
    /// Creates a slice as <see cref="ArraySegment{T}"/> out of this array.
    ///
    /// <para>Note: The range indexer on arrays creates new array (i.e. creates a copy of
    /// the slice) while this method creates a view on the original array (i.e. no copy
    /// is created).</para>
    /// </summary>
    [PublicAPI, Pure]
    public static ArraySegment<T> Slice<T>(this T[] array, int offset, int count)
    {
        return new ArraySegment<T>(array, offset, count);
    }
}

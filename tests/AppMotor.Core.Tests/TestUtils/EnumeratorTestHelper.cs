// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Collections;

namespace AppMotor.Core.TestUtils;

internal static class EnumeratorTestHelper
{
    /// <summary>
    /// Obtains the generic enumerator (<see cref="IEnumerator{T}"/>) from this collection, iterates it and
    /// returns the result as a list.
    /// </summary>
    public static List<TItem> ExecuteGenericEnumerator<TItem>(this IEnumerable<TItem> enumerable)
    {
        return enumerable.ToList();
    }

    /// <summary>
    /// Obtains the non-generic enumerator (<see cref="IEnumerator"/>) from this collection, iterates it and
    /// returns the result as a list.
    /// </summary>
    /// <typeparam name="TItem">The type of the items in this collection.</typeparam>
    public static List<TItem> ExecuteNonGenericEnumerator<TItem>(this IEnumerable enumerable)
    {
        var resultList = new List<TItem>();

        var enumerator = enumerable.GetEnumerator();

        try
        {
            while (enumerator.MoveNext())
            {
#pragma warning disable 8605
#pragma warning disable CS8604 // Possible null reference argument.
                resultList.Add((TItem)enumerator.Current);
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore 8605
            }
        }
        finally
        {
            (enumerator as IDisposable)?.Dispose();
        }

        return resultList;
    }
}

// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.Core.Exceptions;
using AppMotor.Core.Utils;

using JetBrains.Annotations;

namespace AppMotor.Core.Extensions;

/// <summary>
/// Extension methods for <see cref="ICollection{T}"/>.
/// </summary>
public static class CollectionExtensions
{
    /// <summary>
    /// Adds the specified items to the collection.
    /// </summary>
    /// <exception cref="CollectionIsReadOnlyException">Thrown if the collection is read-only.</exception>
    [PublicAPI]
    public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> itemsToAdd)
    {
        Validate.ArgumentWithName(nameof(collection)).IsNotNull(collection);
        Validate.ArgumentWithName(nameof(collection)).IsNotReadOnly(collection);
        Validate.ArgumentWithName(nameof(itemsToAdd)).IsNotNull(itemsToAdd);

        foreach (var item in itemsToAdd)
        {
            collection.Add(item);
        }
    }

    /// <summary>
    /// Removes the items from the collection where the <paramref name="predicate"/> returns true.
    /// </summary>
    /// <returns>Returns the items that were removed from the collection. If no items were
    /// removed, the returned list will be empty (but never <c>null</c>).</returns>
    /// <exception cref="CollectionIsReadOnlyException">Thrown if the collection is read-only.</exception>
    [PublicAPI]
#pragma warning disable CA1002 // Do not expose generic lists // BUG: https://github.com/dotnet/roslyn-analyzers/issues/4508
    public static List<T> RemoveWhere<T>(this ICollection<T> collection, Predicate<T> predicate)
#pragma warning restore CA1002 // Do not expose generic lists
    {
        Validate.ArgumentWithName(nameof(collection)).IsNotNull(collection);
        Validate.ArgumentWithName(nameof(collection)).IsNotReadOnly(collection);
        Validate.ArgumentWithName(nameof(predicate)).IsNotNull(predicate);

        var leftOver = new List<T>(collection.Count);
        var removed = new List<T>(collection.Count);

        foreach (var item in collection)
        {
            if (predicate(item))
            {
                removed.Add(item);
            }
            else
            {
                leftOver.Add(item);
            }
        }

        collection.Clear();
        collection.AddRange(leftOver);

        return removed;
    }

}

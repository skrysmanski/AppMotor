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

using AppMotor.Core.Exceptions;
using AppMotor.Core.Utils;

using JetBrains.Annotations;

namespace AppMotor.Core.Extensions
{
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
            Validate.Argument.IsNotNull(collection, nameof(collection));
            Validate.Argument.IsNotNull(itemsToAdd, nameof(itemsToAdd));
            Validate.Argument.IsNotReadOnly(collection, nameof(collection));

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
            Validate.Argument.IsNotNull(collection, nameof(collection));
            Validate.Argument.IsNotNull(predicate, nameof(predicate));
            Validate.Argument.IsNotReadOnly(collection, nameof(collection));

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
}

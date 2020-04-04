#region License
// Copyright 2020 - 2020 AppWeave.Core (https://github.com/skrysmanski/AppWeave.Core)
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

using AppWeave.Core.Exceptions;
using AppWeave.Core.Utils;

using JetBrains.Annotations;

namespace AppWeave.Core.Extensions
{
    public static class CollectionExtensions
    {
        /// <summary>
        /// Adds the specified items to the collection.
        /// </summary>
        /// <exception cref="ReadOnlyCollectionModificationException">Thrown if the collection is read-only.</exception>
        [PublicAPI]
        public static void AddRange<T>([NotNull] this ICollection<T> collection, [NotNull] IEnumerable<T> itemsToAdd)
        {
            Verify.ParamNotNull(collection, nameof(collection));
            Verify.ParamNotNull(itemsToAdd, nameof(itemsToAdd));
            Verify.ValueNotReadOnly(collection, nameof(collection));

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
        /// <exception cref="ReadOnlyCollectionModificationException">Thrown if the collection is read-only.</exception>
        [PublicAPI, NotNull]
        public static List<T> RemoveWhere<T>([NotNull] this ICollection<T> collection, [NotNull] Predicate<T> predicate)
        {
            Verify.ParamNotNull(collection, nameof(collection));
            Verify.ParamNotNull(predicate, nameof(predicate));
            Verify.ValueNotReadOnly(collection, nameof(collection));

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

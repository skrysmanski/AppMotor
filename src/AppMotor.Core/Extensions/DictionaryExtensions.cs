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

using AppMotor.Core.Exceptions;
using AppMotor.Core.Utils;

using JetBrains.Annotations;

namespace AppMotor.Core.Extensions;

/// <summary>
/// Extension methods for <see cref="IDictionary{TKey,TValue}"/>.
/// </summary>
public static class DictionaryExtensions
{
    /// <summary>
    /// Like <see cref="CollectionExtensions.AddRange{T}"/> but uses
    /// the indexer of the dictionary rather than <see cref="IDictionary{TKey,TValue}.Add(TKey,TValue)"/>.
    /// Thus this method will not result in an exception if the key already exists.
    /// </summary>
    /// <exception cref="CollectionIsReadOnlyException">Thrown if the collection is read-only.</exception>
    [PublicAPI]
    public static void PutRange<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary,
            IEnumerable<KeyValuePair<TKey, TValue>> itemsToAdd
        )
        where TKey : notnull
    {
        Validate.ArgumentWithName(nameof(dictionary)).IsNotNull(dictionary);
        Validate.ArgumentWithName(nameof(dictionary)).IsNotReadOnly(dictionary);
        Validate.ArgumentWithName(nameof(itemsToAdd)).IsNotNull(itemsToAdd);

        foreach (var item in itemsToAdd)
        {
            dictionary[item.Key] = item.Value;
        }
    }
}
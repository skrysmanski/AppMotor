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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

namespace AppMotor.Core.TestUtils
{
    internal static class EnumeratorTestHelper
    {
        /// <summary>
        /// Obtains the generic enumerator (<see cref="IEnumerator{T}"/>) from this collection, iterates it and
        /// returns the result as a list.
        /// </summary>
        [NotNull]
        public static List<TItem> ExecuteGenericEnumerator<TItem>([NotNull] this IEnumerable<TItem> enumerable)
        {
            return enumerable.ToList();
        }

        /// <summary>
        /// Obtains the non-generic enumerator (<see cref="IEnumerator"/>) from this collection, iterates it and
        /// returns the result as a list.
        /// </summary>
        /// <typeparam name="TItem">The type of the items in this collection.</typeparam>
        [NotNull]
        public static List<TItem> ExecuteNonGenericEnumerator<TItem>([NotNull] this IEnumerable enumerable)
        {
            var resultList = new List<TItem>();

            var enumerator = enumerable.GetEnumerator();

            try
            {
                while (enumerator.MoveNext())
                {
                    resultList.Add((TItem)enumerator.Current);
                }
            }
            finally
            {
                (enumerator as IDisposable)?.Dispose();
            }

            return resultList;
        }
    }
}

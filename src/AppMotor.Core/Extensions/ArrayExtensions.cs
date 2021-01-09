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

using JetBrains.Annotations;

namespace AppMotor.Core.Extensions
{
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
}

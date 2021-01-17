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

using System.Collections.Generic;

namespace AppMotor.Core.Utils
{
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
}

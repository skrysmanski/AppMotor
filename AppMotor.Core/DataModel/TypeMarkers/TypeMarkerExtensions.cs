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

using JetBrains.Annotations;

namespace AppWeave.Core.DataModel
{
    public static class TypeMarkerExtensions
    {
        /// <summary>
        /// Marks this type with the specified type marker. If it's already marked with this marker,
        /// the method simply returns. No exception will be thrown.
        ///
        /// <para>This method is thread-safe.</para>
        /// </summary>
        [PublicAPI]
        public static void MarkWith<TTypeMarker>([NotNull] this Type typeToMark) where TTypeMarker : TypeMarker
        {
            TypeMarkers.RegisterTypeMaker(typeToMark, typeof(TTypeMarker));
        }

        /// <summary>
        /// Returns whether this type has been with the specified type marker (via <see cref="MarkWith{TTypeMarker}"/>).
        ///
        /// <para>This method is thread-safe.</para>
        /// </summary>
        [PublicAPI, Pure]
        public static bool IsMarkedWith<TTypeMarker>([NotNull] this Type typeToCheck) where TTypeMarker : TypeMarker
        {
            return TypeMarkers.IsTypeMarkerRegistered(typeToCheck, typeof(TTypeMarker));
        }
    }
}

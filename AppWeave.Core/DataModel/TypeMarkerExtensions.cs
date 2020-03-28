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
using System.Collections.Concurrent;
using System.Collections.Generic;

using JetBrains.Annotations;

namespace AppWeave.Core.DataModel
{
    public static class TypeMarkerExtensions
    {
        [NotNull]
        private static readonly ConcurrentDictionary<Type, TypeMarkerCollection> s_markers = new ConcurrentDictionary<Type, TypeMarkerCollection>();

        /// <summary>
        /// Marks this type with the specified type marker. If it's already marked with this marker,
        /// the method simply returns. No exception will be thrown.
        ///
        /// <para>This method is thread-safe.</para>
        /// </summary>
        [PublicAPI]
        public static void MarkWith<TTypeMarker>([NotNull] this Type typeToMark) where TTypeMarker : TypeMarker
        {
            var typeMarkerCollection = s_markers.GetOrAdd(typeToMark, CreateTypeMarkerCollection);
            typeMarkerCollection.Add(typeof(TTypeMarker));
        }

        /// <summary>
        /// Returns whether this type has been with the specified type marker (via <see cref="MarkWith{TTypeMarker}"/>).
        ///
        /// <para>This method is thread-safe.</para>
        /// </summary>
        [PublicAPI, Pure]
        public static bool IsMarkedWith<TTypeMarker>([NotNull] this Type typeToMark) where TTypeMarker : TypeMarker
        {
            if (s_markers.TryGetValue(typeToMark, out var typeMarkerCollection))
            {
                return typeMarkerCollection.Contains(typeof(TTypeMarker));
            }

            return false;
        }

        [NotNull]
        private static TypeMarkerCollection CreateTypeMarkerCollection(Type arg)
        {
            return new TypeMarkerCollection();
        }

        private sealed class TypeMarkerCollection
        {
            [NotNull]
            private readonly object m_updateLock = new object();

            /// <summary>
            /// The underlying set. Note this set is managed as "copy-on-write". This way we
            /// don't need a lock in <see cref="Contains"/>.
            /// </summary>
            [CanBeNull]
            private HashSet<Type> m_underlyingCollection;

            public void Add([NotNull] Type markerType)
            {
                lock (this.m_updateLock)
                {
                    if (Contains(markerType))
                    {
                        // Already present. We don't need to update the set.
                        return;
                    }

                    HashSet<Type> newCollection;

                    if (this.m_underlyingCollection != null)
                    {
                        newCollection = new HashSet<Type>(this.m_underlyingCollection);
                    }
                    else
                    {
                        newCollection = new HashSet<Type>();
                    }

                    newCollection.Add(markerType);

                    this.m_underlyingCollection = newCollection;
                }
            }

            [Pure]
            public bool Contains([NotNull] Type markerType)
            {
                return this.m_underlyingCollection?.Contains(markerType) == true;
            }
        }
    }
}

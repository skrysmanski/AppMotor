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
using System.Collections.Concurrent;
using System.Collections.Generic;

using JetBrains.Annotations;

namespace AppMotor.Core.DataModel
{
    /// <summary>
    /// Companion class to provide static properties and events for the <see cref="TypeMarker"/> feature.
    /// </summary>
    public static class TypeMarkers
    {
        private static readonly ConcurrentDictionary<Type, TypeMarkerCollection> s_markers = new ConcurrentDictionary<Type, TypeMarkerCollection>();

        /// <summary>
        /// This event is raised whenever a type is marked with a new type marker (via
        /// <see cref="TypeMarkerExtensions.MarkWith{TTypeMarker}"/>).
        /// </summary>
        public static event EventHandler<TypeMarkerAddedEventArgs>? TypeMarkerAdded;

        /// <summary>
        /// See <see cref="TypeMarkerExtensions.MarkWith{TTypeMarker}"/> for documentation.
        /// </summary>
        internal static void RegisterTypeMaker(Type typeToMark, Type markerType)
        {
            var typeMarkerCollection = s_markers.GetOrAdd(typeToMark, CreateTypeMarkerCollection);
            bool markerAdded = typeMarkerCollection.Add(markerType);
            if (markerAdded)
            {
                TypeMarkerAdded?.Invoke(null, new TypeMarkerAddedEventArgs(typeToMark, markerType));
            }
        }

        /// <summary>
        /// See <see cref="TypeMarkerExtensions.IsMarkedWith{TTypeMarker}"/> for documentation.
        /// </summary>
        [Pure]
        internal static bool IsTypeMarkerRegistered(Type typeToCheck, Type markerType)
        {
            if (s_markers.TryGetValue(typeToCheck, out var typeMarkerCollection))
            {
                return typeMarkerCollection.Contains(markerType);
            }

            return false;
        }

        private static TypeMarkerCollection CreateTypeMarkerCollection(Type arg)
        {
            return new TypeMarkerCollection();
        }

        private sealed class TypeMarkerCollection
        {
            private readonly object m_updateLock = new object();

            /// <summary>
            /// The underlying set. Note this set is managed as "copy-on-write". This way we
            /// don't need a lock in <see cref="Contains"/>.
            /// </summary>
            private HashSet<Type>? m_underlyingCollection;

            public bool Add(Type markerType)
            {
                lock (this.m_updateLock)
                {
                    if (Contains(markerType))
                    {
                        // Already present. We don't need to update the set.
                        return false;
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

                return true;
            }

            [Pure]
            public bool Contains(Type markerType)
            {
                return this.m_underlyingCollection?.Contains(markerType) == true;
            }
        }

    }
}

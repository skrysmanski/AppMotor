// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Collections.Concurrent;

using JetBrains.Annotations;

namespace AppMotor.Core.DataModel;

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
        private readonly object _updateLock = new();

        /// <summary>
        /// The underlying set. Note this set is managed as "copy-on-write". This way we
        /// don't need a lock in <see cref="Contains"/>.
        /// </summary>
        private HashSet<Type>? _underlyingCollection;

        public bool Add(Type markerType)
        {
            lock (this._updateLock)
            {
                if (Contains(markerType))
                {
                    // Already present. We don't need to update the set.
                    return false;
                }

                HashSet<Type> newCollection;

                if (this._underlyingCollection != null)
                {
                    newCollection = [..this._underlyingCollection];
                }
                else
                {
                    newCollection = [];
                }

                newCollection.Add(markerType);

                this._underlyingCollection = newCollection;
            }

            return true;
        }

        [Pure]
        public bool Contains(Type markerType)
        {
            return this._underlyingCollection?.Contains(markerType) == true;
        }
    }

}

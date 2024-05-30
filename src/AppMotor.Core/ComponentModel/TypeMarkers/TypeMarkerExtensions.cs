// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using JetBrains.Annotations;

namespace AppMotor.Core.ComponentModel;

/// <summary>
/// Extension methods to mark arbitrary types with <see cref="TypeMarker"/>s.
/// </summary>
public static class TypeMarkerExtensions
{
    /// <summary>
    /// Marks this type with the specified type marker. If it's already marked with this marker,
    /// the method simply returns. No exception will be thrown.
    ///
    /// <para>This method is thread-safe.</para>
    /// </summary>
    [PublicAPI]
    public static void MarkWith<TTypeMarker>(this Type typeToMark) where TTypeMarker : TypeMarker
    {
        TypeMarkers.RegisterTypeMaker(typeToMark, typeof(TTypeMarker));
    }

    /// <summary>
    /// Returns whether this type has been with the specified type marker (via <see cref="MarkWith{TTypeMarker}"/>).
    ///
    /// <para>This method is thread-safe.</para>
    /// </summary>
    [PublicAPI, Pure]
    public static bool IsMarkedWith<TTypeMarker>(this Type typeToCheck) where TTypeMarker : TypeMarker
    {
        return TypeMarkers.IsTypeMarkerRegistered(typeToCheck, typeof(TTypeMarker));
    }
}

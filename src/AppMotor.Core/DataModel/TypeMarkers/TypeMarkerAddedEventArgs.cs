// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.Core.Utils;

namespace AppMotor.Core.DataModel;

/// <summary>
/// Used by <see cref="TypeMarkers.TypeMarkerAdded"/>.
/// </summary>
public sealed class TypeMarkerAddedEventArgs : EventArgs
{
    /// <summary>
    /// The type that has been marked with <see cref="TypeMarkerType"/>.
    /// </summary>
    public Type MarkedType { get; }

    /// <summary>
    /// The type marker type; i.e. a sub class of <see cref="TypeMarker"/>.
    /// </summary>
    public Type TypeMarkerType { get; }

    /// <inheritdoc />
    public TypeMarkerAddedEventArgs(Type markedType, Type typeMarkerType)
    {
        Validate.ArgumentWithName(nameof(typeMarkerType)).IsNotNull(typeMarkerType);
        Validate.ArgumentWithName(nameof(markedType)).IsNotNull(markedType);

        this.MarkedType = markedType;
        this.TypeMarkerType = typeMarkerType;
    }
}

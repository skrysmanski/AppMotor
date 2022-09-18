// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

namespace AppMotor.Core.DataModel;

/// <summary>
/// Represents a type marker; that is a way of marking certain .NET <see cref="Type"/>s in a
/// generic way. The main use case here is provide an alternative to marker interfaces when
/// you can't change the implementation of an existing type you wish to mark. For example,
/// if you can't implement the marker interface <see cref="ISensitiveValue"/>, you can instead
/// mark the type with <see cref="SensitiveValueMarker"/> (via <see cref="TypeMarkerExtensions.MarkWith{TTypeMarker}"/>).
/// </summary>
public abstract class TypeMarker
{
}

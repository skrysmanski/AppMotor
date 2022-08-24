// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using JetBrains.Annotations;

namespace AppMotor.Core.DataModel;

/// <summary>
/// Represents a type that can create clones (copies) of itself.
///
/// <para>Only use this interface if for <typeparamref name="T"/> there's no
/// distinction between deep and shallow clones (for example, for strings).
/// Otherwise prefer <see cref="IShallowCloneable{T}"/> or <see cref="IDeepCloneable{T}"/>.
/// </para>
/// </summary>
/// <remarks>
/// For types that are mutable but only consist of immutable items it makes not sense
/// to differentiate between shallow and deep clones.
/// </remarks>
public interface ICloneable<out T> where T : ICloneable<T>
{
    /// <summary>
    /// Creates a copy of this object.
    /// </summary>
    [PublicAPI, Pure]
    T Clone();
}
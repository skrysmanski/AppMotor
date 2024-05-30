// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using JetBrains.Annotations;

namespace AppMotor.Core.ComponentModel;

/// <summary>
/// Represents a type that can create shallow clones (copies) of itself.
/// </summary>
/// <remarks>
/// This interface does not inherit <see cref="ICloneable"/> on purpose because
/// <see cref="ICloneable.Clone"/> makes no distinction of whether it's
/// a deep or a shallow clone.
/// </remarks>
/// <seealso cref="IDeepCloneable{T}"/>
/// <seealso cref="ICloneable{T}"/>
public interface IShallowCloneable<out T> where T : IShallowCloneable<T>
{
    /// <summary>
    /// Creates a shallow copy of this object.
    /// </summary>
    [PublicAPI, Pure]
    T CloneShallow();
}

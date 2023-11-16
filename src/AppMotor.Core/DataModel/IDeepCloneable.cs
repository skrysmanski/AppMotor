// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using JetBrains.Annotations;

namespace AppMotor.Core.DataModel;

/// <summary>
/// Represents a type that can create deep clones (copies) of itself.
/// </summary>
/// <remarks>
/// This interface does not inherit <see cref="ICloneable"/> on purpose because
/// <see cref="ICloneable.Clone"/> makes no distinction of whether it's
/// a deep or a shallow clone.
/// </remarks>
/// <seealso cref="IShallowCloneable{T}"/>
/// <seealso cref="ICloneable{T}"/>
public interface IDeepCloneable<out T> where T : IDeepCloneable<T>
{
    /// <summary>
    /// Creates a deep copy of this object.
    /// </summary>
    [PublicAPI, Pure]
    T CloneDeep();
}

#region License
// Copyright 2021 AppMotor Framework (https://github.com/skrysmanski/AppMotor)
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
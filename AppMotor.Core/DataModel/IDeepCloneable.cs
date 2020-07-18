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

using JetBrains.Annotations;

namespace AppMotor.Core.DataModel
{
    /// <summary>
    /// Represents a type that can create deep clones (copies) of itself.
    /// </summary>
    /// <remarks>
    /// This interface does not inherit <see cref="ICloneable"/> on purpose because
    /// <see cref="ICloneable.Clone"/> makes no distinction of whether it's
    /// a deep or a shallow clone.
    /// </remarks>
    /// <seealso cref="IShallowCloneable{T}"/>
    public interface IDeepCloneable<out T> where T : IDeepCloneable<T>
    {
        /// <summary>
        /// Creates a deep copy of this object.
        /// </summary>
        [PublicAPI, NotNull, Pure]
        T CloneDeep();
    }
}

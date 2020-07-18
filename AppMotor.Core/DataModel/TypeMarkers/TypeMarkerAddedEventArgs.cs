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

using AppMotor.Core.Utils;

using JetBrains.Annotations;

namespace AppMotor.Core.DataModel
{
    /// <summary>
    /// Used by <see cref="TypeMarkers.TypeMarkerAdded"/>.
    /// </summary>
    public sealed class TypeMarkerAddedEventArgs : EventArgs
    {
        /// <summary>
        /// The type that has been marked with <see cref="TypeMarkerType"/>.
        /// </summary>
        [NotNull]
        public Type MarkedType { get; }

        /// <summary>
        /// The type marker type; i.e. a sub class of <see cref="TypeMarker"/>.
        /// </summary>
        [NotNull]
        public Type TypeMarkerType { get; }

        /// <inheritdoc />
        public TypeMarkerAddedEventArgs([NotNull] Type markedType, [NotNull] Type typeMarkerType)
        {
            Verify.Argument.NotNull(typeMarkerType, nameof(typeMarkerType));
            Verify.Argument.NotNull(markedType, nameof(markedType));

            this.MarkedType = markedType;
            this.TypeMarkerType = typeMarkerType;
        }
    }
}

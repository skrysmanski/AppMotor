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

using AppMotor.Core.Utils;

using JetBrains.Annotations;

namespace AppMotor.Core.Logging
{
    /// <summary>
    /// Used by <see cref="LoggableValues.LoggabilityChanged"/>.
    /// </summary>
    public sealed class LoggabilityChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The type for which the loggability has changed. You may use the methods
        /// of <see cref="LoggableValues"/> to determine the new loggability.
        /// </summary>
        [PublicAPI, NotNull]
        public Type Type { get; }

        public LoggabilityChangedEventArgs([NotNull] Type type)
        {
            Verify.ParamNotNull(type, nameof(type));

            this.Type = type;
        }
    }
}

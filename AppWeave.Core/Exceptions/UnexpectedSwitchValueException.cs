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

using JetBrains.Annotations;

namespace AppWeave.Core.Exceptions
{
    /// <summary>
    /// This exception is for the <c>default</c> case in <c>switch</c> blocks where all other case blocks
    /// should already cover all possible values. In most cases, this is used for switch blocks over enums
    /// where each enum member is already covered by a case block but the compiler require a default block.
    /// </summary>
    /// <remarks>
    /// This exception is basically a specialization of <see cref="UnexpectedBehaviorException"/>.
    /// </remarks>
    [PublicAPI]
    public class UnexpectedSwitchValueException : UnexpectedBehaviorException
    {
        [PublicAPI]
        public UnexpectedSwitchValueException([NotNull] string what, [CanBeNull] object value)
            : base($"Unexpected {what}: {value ?? "null"}")
        {
        }
    }
}

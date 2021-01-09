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
using System.ComponentModel;

using JetBrains.Annotations;

namespace AppMotor.Core.Exceptions
{
    /// <summary>
    /// An exception whose message can/should be displayed to the user - but not the stacktrace.
    /// </summary>
    public class ErrorMessageException : Exception
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">The message to display to the user.</param>
        [PublicAPI]
        public ErrorMessageException([Localizable(true)] string message)
            : base(message)
        {
        }

        /// <summary>
        /// Only writes the message; no type nor stacktrace.
        /// </summary>
        public override string ToString()
        {
            return this.Message;
        }
    }
}

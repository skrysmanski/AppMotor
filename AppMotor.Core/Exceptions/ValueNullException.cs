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

namespace AppMotor.Core.Exceptions
{
    /// <summary>
    /// Equivalent to <see cref="ArgumentNullException"/> - just for values that are not arguments/parameters.
    /// </summary>
    [PublicAPI]
    public class ValueNullException : ValueException
    {
        public ValueNullException()
            : this(message: null, valueName: null, innerException: null)
        {
        }

        public ValueNullException(string? message)
            : this(message: message, valueName: null, innerException: null)
        {
        }

        public ValueNullException(string? message, string? valueName)
            : this(message: message, valueName: valueName, innerException: null)
        {
        }

        public ValueNullException(string? message, Exception? innerException)
            : this(message: message, valueName: null, innerException: innerException)
        {
        }

        public ValueNullException(string? message, string? valueName, Exception? innerException)
            : base(message ?? Validate.ExceptionMessages.VALUE_IS_NULL, valueName, innerException)
        {
        }
    }
}

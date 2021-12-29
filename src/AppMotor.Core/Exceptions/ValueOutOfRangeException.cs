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

using JetBrains.Annotations;

namespace AppMotor.Core.Exceptions;

/// <summary>
/// Equivalent to <see cref="ArgumentOutOfRangeException"/> - just for values that are not arguments/parameters.
/// </summary>
[PublicAPI]
public class ValueOutOfRangeException : ValueException
{
    /// <summary>
    /// Constructor.
    /// </summary>
    public ValueOutOfRangeException()
        : this(message: null, valueName: null, innerException: null)
    {
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ValueOutOfRangeException(string? message)
        : this(message: message, valueName: null, innerException: null)
    {
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="valueName">The name of the value this exception applies to.</param>
    public ValueOutOfRangeException(string? message, string? valueName)
        : this(message: message, valueName: valueName, innerException: null)
    {
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The inner exception</param>
    public ValueOutOfRangeException(string? message, Exception? innerException)
        : this(message: message, valueName: null, innerException: innerException)
    {
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="valueName">The name of the value this exception applies to.</param>
    /// <param name="innerException">The inner exception</param>
    public ValueOutOfRangeException(string? message, string? valueName, Exception? innerException)
        : base(message ?? "The specified value is outside of the range of valid values.", valueName, innerException)
    {
    }
}
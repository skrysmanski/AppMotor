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

namespace AppMotor.Core.Utils;

/// <summary>
/// Provides the current time and date. This interface primarily exists for (unit) testing
/// code that relies on the current time and date (thus allowing test code to mock the current
/// time and date). For the default implementation, see <see cref="DefaultDateTimeProvider"/>.
/// </summary>
public interface IDateTimeProvider
{
    /// <summary>
    /// The current date and time in the local timezone (i.e. <see cref="DateTimeKind.Local"/>).
    /// </summary>
    /// <seealso cref="UtcNow"/>
    [PublicAPI]
    DateTime LocalNow { get; }

    /// <summary>
    /// The current date and time in the UTC timezone (i.e. <see cref="DateTimeKind.Utc"/>).
    /// </summary>
    /// <seealso cref="LocalNow"/>
    [PublicAPI]
    DateTimeUtc UtcNow { get; }
}
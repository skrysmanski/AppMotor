﻿#region License
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

namespace AppMotor.Core.DataModel
{
    /// <summary>
    /// Represents a sensitive value (e.g. a password or access token). Users of
    /// this class can make certain that the value is never logged or displayed
    /// by accident.
    /// </summary>
    /// <seealso cref="SensitiveValueMarker"/>
    public interface ISensitiveValue
    {
    }

    /// <summary>
    /// A <see cref="TypeMarker"/> alternative to <see cref="ISensitiveValue"/>.
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed class SensitiveValueMarker : TypeMarker
    {
    }
}
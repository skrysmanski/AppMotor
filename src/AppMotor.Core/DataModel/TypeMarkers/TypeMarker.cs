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

namespace AppMotor.Core.DataModel;

/// <summary>
/// Represents a type marker; that is a way of marking certain .NET <see cref="Type"/>s in a
/// generic way. The main use case here is provide an alternative to marker interfaces when
/// you can't change the implementation of an existing type you wish to mark. For example,
/// if you can't implement the marker interface <see cref="ISensitiveValue"/>, you can instead
/// mark the type with <see cref="SensitiveValueMarker"/> (via <see cref="TypeMarkerExtensions.MarkWith{TTypeMarker}"/>).
/// </summary>
public abstract class TypeMarker
{
}
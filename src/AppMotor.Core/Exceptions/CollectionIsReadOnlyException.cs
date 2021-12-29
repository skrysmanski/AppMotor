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
using System.Collections.Generic;

namespace AppMotor.Core.Exceptions;

/// <summary>
/// Thrown on the attempt to modify a read-only collection.
/// </summary>
/// <remarks>
/// It's not very intuitive whether modifying a read-only collection should result
/// in a <see cref="InvalidOperationException"/> or a <see cref="NotSupportedException"/>.
/// The .NET framework itself uses <see cref="NotSupportedException"/> while I personally find
/// <see cref="InvalidOperationException"/> more fitting. This why this exception
/// exists: so that developer don't need to care.
///
/// <para>Note: The base exception is <see cref="NotSupportedException"/> so that this
/// exception can honor the contract of <see cref="ICollection{T}.Add"/> and the like
/// which require a <see cref="NotSupportedException"/> in case of read-only collections.</para>
/// </remarks>
public class CollectionIsReadOnlyException : NotSupportedException, ICollectionIsReadOnlyException
{
    /// <summary>
    /// The message used as text for this exception - and its "cousins" <see cref="CollectionIsReadOnlyArgumentException"/>
    /// and <see cref="CollectionIsReadOnlyValueException"/>.
    /// </summary>
    public const string DEFAULT_MESSAGE = "This collection is read-only.";

    /// <summary>
    /// Constructor.
    /// </summary>
    public CollectionIsReadOnlyException() : base(DEFAULT_MESSAGE)
    {
    }
}
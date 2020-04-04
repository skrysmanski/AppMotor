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

namespace AppWeave.Core.Exceptions
{
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
    /// <para>Note: The base exception is not part of the contract and may change.</para>
    /// </remarks>
    public class ReadOnlyCollectionModificationException : ValueException
    {
        public ReadOnlyCollectionModificationException() : base("This collection is read-only.")
        {
        }

        public ReadOnlyCollectionModificationException(string valueName)
            : base("This collection is read-only.", valueName: valueName)
        {
        }

    }
}

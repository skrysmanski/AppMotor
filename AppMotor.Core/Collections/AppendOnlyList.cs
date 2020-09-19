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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using AppMotor.Core.DataModel;
using AppMotor.Core.Exceptions;
using AppMotor.Core.Utils;

using JetBrains.Annotations;

using ExcludeFromCodeCoverageAttribute = System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute;

namespace AppMotor.Core.Collections
{
    /// <summary>
    /// Represents a list that can only be append to (but not removed from or changed in any other way).
    ///
    /// <para>This design allows for the list to have very cheap clones (via <see cref="CloneShallow"/>)
    /// when there's always/most of the time only one clone that will be appended to. (If more than one
    /// clone is created and then more than one clone is appended to, every clone but the first that
    /// was appended to will create its own full copy of the underlying list.)</para>
    ///
    /// <para>The primary use case for this list is when a <c>+ operator</c> overload appends to
    /// some internal list - which in this case should be an instance of this class. In this case, you can
    /// simply use <c>myInternalList.CloneShallow().Append(...)</c> and store the result in the return
    /// value. The trick here is that <see cref="CloneShallow"/> is very efficient in this case.</para>
    ///
    /// <para>Note: This class is not thread-safe.</para>
    /// </summary>
    public sealed class AppendOnlyList<T> : IReadOnlyList<T>, IShallowCloneable<AppendOnlyList<T>>
    {
        private List<T> m_underlyingList;

        /// <inheritdoc />
        public int Count { get; private set; }

        /// <inheritdoc />
        public T this[int index]
        {
            get
            {
                if (index >= this.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                return this.m_underlyingList[index];
            }
        }

        /// <summary>
        /// Creates a new, empty list.
        /// </summary>
        [PublicAPI]
        public AppendOnlyList()
        {
            this.m_underlyingList = new List<T>();
        }

        /// <summary>
        /// Creates a new list with a copy of <paramref name="source"/>.
        /// </summary>
        [PublicAPI]
        public AppendOnlyList(IEnumerable<T> source)
        {
            Verify.Argument.IsNotNull(source, nameof(source));

            if (source is AppendOnlyList<T> appendOnlyList)
            {
                this.m_underlyingList = appendOnlyList.m_underlyingList;
                this.Count = appendOnlyList.Count;
            }
            else
            {
                this.m_underlyingList = source.ToList();
                this.Count = this.m_underlyingList.Count;
            }
        }

        /// <summary>
        /// Creates a shallow clone of the specified AppendOnlyList. Has the
        /// same effect as <see cref="CloneShallow"/>.
        /// </summary>
        [PublicAPI]
        public AppendOnlyList(AppendOnlyList<T> source)
        {
            Verify.Argument.IsNotNull(source, nameof(source));

            this.m_underlyingList = source.m_underlyingList;
            this.Count = source.Count;
        }

        /// <summary>
        /// Appends the specified item to the end of the list.
        /// </summary>
        [PublicAPI]
        public void Append(T item)
        {
            EnsureUnderlyingListCanBeAppended();

            this.m_underlyingList.Add(item);
            this.Count = this.m_underlyingList.Count;
        }

        /// <summary>
        /// Appends the specified items to the end of the list.
        /// </summary>
        [PublicAPI]
        public void AppendRange(IEnumerable<T> items)
        {
            Verify.Argument.IsNotNull(items, nameof(items));

            EnsureUnderlyingListCanBeAppended();

            if (items is AppendOnlyList<T> otherAppendOnlyList)
            {
                this.m_underlyingList.AddRange(otherAppendOnlyList.CreateListRange());
            }
            else
            {
                this.m_underlyingList.AddRange(items);
            }

            this.Count = this.m_underlyingList.Count;
        }

        private void EnsureUnderlyingListCanBeAppended()
        {
            if (this.m_underlyingList.Count != this.Count)
            {
                // Another user of the list has already appended to it. Thus,
                // we need to create a copy of the list.

                var originalUnderlyingList = CreateListRange();

                // NOTE: We reserve 5 more items for future append calls.
                this.m_underlyingList = new List<T>(this.Count + 5);

                // NOTE: The ListRange class implements "ICollection" which makes
                //   the copy process more efficient than any collection type that
                //   doesn't implement "ICollection" (incl. "IReadOnlyCollection").
                this.m_underlyingList.AddRange(originalUnderlyingList);
            }
        }

        /// <summary>
        /// Creates an instance of <see cref="ListRange"/> so that a call
        /// to <see cref="List{T}.AddRange"/> is more efficient.
        /// </summary>
        [Pure]
        private ListRange CreateListRange()
        {
            return new ListRange(this.m_underlyingList, this.Count);
        }

        /// <inheritdoc />
        public AppendOnlyList<T> CloneShallow()
        {
            return new AppendOnlyList<T>(this);
        }

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator()
        {
            for (var i = 0; i < this.Count; i++)
            {
                yield return this.m_underlyingList[i];
            }
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Implementation of <see cref="ICollection{T}"/> to allow for
        /// more efficient copying in <see cref="List{T}.AddRange"/> (because
        /// of the existence of <see cref="CopyTo"/>).
        /// </summary>
        private sealed class ListRange : ICollection<T>
        {
            private readonly List<T> m_underlyingList;

            /// <summary>
            /// The length of this range.
            /// </summary>
            public int Count { get; }

            /// <inheritdoc />
            [ExcludeFromCodeCoverage]
            public bool IsReadOnly => true;

            [ExcludeFromCodeCoverage]
            public ListRange(List<T> underlyingList, int rangeLength)
            {
                if (underlyingList.Count < rangeLength)
                {
                    throw new InvalidOperationException(
                        $"The range is expected to have {rangeLength} items but the underlying list only has {underlyingList.Count} items."
                    );
                }

                this.m_underlyingList = underlyingList;
                this.Count = rangeLength;
            }

            /// <inheritdoc />
            [ExcludeFromCodeCoverage]
            public IEnumerator<T> GetEnumerator()
            {
                for (var i = 0; i < this.Count; i++)
                {
                    yield return this.m_underlyingList[i];
                }
            }

            /// <inheritdoc />
            [ExcludeFromCodeCoverage]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            /// <inheritdoc />
            [ExcludeFromCodeCoverage]
            public void Add(T item)
            {
                throw new CollectionIsReadOnlyException();
            }

            /// <inheritdoc />
            [ExcludeFromCodeCoverage]
            public void Clear()
            {
                throw new CollectionIsReadOnlyException();
            }

            /// <inheritdoc />
            [ExcludeFromCodeCoverage]
            public bool Contains(T item)
            {
                if (this.Count == 0)
                {
                    return false;
                }

                var index = this.m_underlyingList.IndexOf(item);
                if (index == -1 || index >= this.Count)
                {
                    return false;
                }

                return true;
            }

            /// <inheritdoc />
            public void CopyTo(T[] array, int destArrayIndex)
            {
                this.m_underlyingList.CopyTo(0, array, destArrayIndex, this.Count);
            }

            /// <inheritdoc />
            [ExcludeFromCodeCoverage]
            public bool Remove(T item)
            {
                throw new CollectionIsReadOnlyException();
            }
        }
    }
}

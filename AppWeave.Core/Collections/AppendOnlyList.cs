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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using AppWeave.Core.DataModel;
using AppWeave.Core.Exceptions;
using AppWeave.Core.Utils;

using JetBrains.Annotations;

namespace AppWeave.Core.Collections
{
    /// <summary>
    /// Represents a list that can only be append to (but not removed from or changed in any other way).
    /// This design allows for the same list instance to be shared and appended to in a very efficient way.
    /// The primary use case for this list is to be used in <c>+ operator</c> overloads that append to
    /// an internal list. In this case, since neither operand of the operator should be modified by
    /// the implementation, a copy of the internal list would need to be created for the return value.
    /// By using this class, no copy is created most of the time while the operands remain unchanged.
    ///
    /// <para>Note: This class is not thread-safe.</para>
    /// </summary>
    public sealed class AppendOnlyList<T> : IReadOnlyList<T>, IShallowCloneable<AppendOnlyList<T>>
    {
        [NotNull]
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
        public AppendOnlyList([NotNull] IEnumerable<T> source)
        {
            Verify.ParamNotNull(source, nameof(source));

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
        public AppendOnlyList([NotNull] AppendOnlyList<T> source)
        {
            Verify.ParamNotNull(source, nameof(source));

            this.m_underlyingList = source.m_underlyingList;
            this.Count = source.Count;
        }

        /// <summary>
        /// Appends the specified item to the end of the list.
        /// </summary>
        [PublicAPI]
        public void Append([CanBeNull] T item)
        {
            EnsureUnderlyingListCanBeAppended();

            this.m_underlyingList.Add(item);
            this.Count = this.m_underlyingList.Count;
        }

        /// <summary>
        /// Appends the specified items to the end of the list.
        /// </summary>
        [PublicAPI]
        public void AppendRange([NotNull] IEnumerable<T> items)
        {
            Verify.ParamNotNull(items, nameof(items));

            EnsureUnderlyingListCanBeAppended();

            this.m_underlyingList.AddRange(items);
            this.Count = this.m_underlyingList.Count;
        }

        private void EnsureUnderlyingListCanBeAppended()
        {
            if (this.m_underlyingList.Count != this.Count)
            {
                // Another user of the list has already appended to it. Thus,
                // we need to create a copy of the list.

                var oldUnderlyingList = this.m_underlyingList;

                // NOTE: We reserve 5 more items for future append calls.
                this.m_underlyingList = new List<T>(this.Count + 5);

                // NOTE: The ListRange class implements "ICollection" which makes
                //   the copy process more efficient than any collection type that
                //   doesn't implement "ICollection" (incl. "IReadOnlyCollection").
                this.m_underlyingList.AddRange(new ListRange(oldUnderlyingList, this.Count));
            }
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
            [NotNull]
            private readonly List<T> m_underlyingList;

            /// <summary>
            /// The length of this range.
            /// </summary>
            public int Count { get; }

            /// <inheritdoc />
            public bool IsReadOnly => true;

            public ListRange([NotNull] List<T> underlyingList, int rangeLength)
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

            /// <inheritdoc />
            public void Add(T item)
            {
                throw new ReadOnlyCollectionModificationException();
            }

            /// <inheritdoc />
            public void Clear()
            {
                throw new ReadOnlyCollectionModificationException();
            }

            /// <inheritdoc />
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
            public bool Remove(T item)
            {
                throw new ReadOnlyCollectionModificationException();
            }
        }
    }
}

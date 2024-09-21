// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Collections;

using AppMotor.Core.ComponentModel;
using AppMotor.Core.Exceptions;
using AppMotor.Core.Utils;

using JetBrains.Annotations;

using ExcludeFromCodeCoverageAttribute = System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute;

namespace AppMotor.Core.Collections;

/// <summary>
/// Represents a list that can only be appended to (but not removed from or changed in any other way).
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
    private List<T> _underlyingList;

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

            return this._underlyingList[index];
        }
    }

    /// <summary>
    /// Creates a new, empty list.
    /// </summary>
    [PublicAPI]
    public AppendOnlyList()
    {
        this._underlyingList = [];
    }

    /// <summary>
    /// Creates a new list with a copy of <paramref name="source"/>.
    /// </summary>
    [PublicAPI]
    public AppendOnlyList(IEnumerable<T> source)
    {
        Validate.ArgumentWithName(nameof(source)).IsNotNull(source);

        if (source is AppendOnlyList<T> appendOnlyList)
        {
            this._underlyingList = appendOnlyList._underlyingList;
            this.Count = appendOnlyList.Count;
        }
        else
        {
            this._underlyingList = source.ToList();
            this.Count = this._underlyingList.Count;
        }
    }

    /// <summary>
    /// Creates a shallow clone of the specified AppendOnlyList. Has the
    /// same effect as <see cref="CloneShallow"/>.
    /// </summary>
    [PublicAPI]
    public AppendOnlyList(AppendOnlyList<T> source)
    {
        Validate.ArgumentWithName(nameof(source)).IsNotNull(source);

        this._underlyingList = source._underlyingList;
        this.Count = source.Count;
    }

    /// <summary>
    /// Appends the specified item to the end of the list.
    /// </summary>
    [PublicAPI]
    public void Append(T item)
    {
        EnsureUnderlyingListCanBeAppended();

        this._underlyingList.Add(item);
        this.Count = this._underlyingList.Count;
    }

    /// <summary>
    /// Appends the specified items to the end of the list.
    /// </summary>
    [PublicAPI]
    public void AppendRange(IEnumerable<T> items)
    {
        Validate.ArgumentWithName(nameof(items)).IsNotNull(items);

        EnsureUnderlyingListCanBeAppended();

        if (items is AppendOnlyList<T> otherAppendOnlyList)
        {
            this._underlyingList.AddRange(otherAppendOnlyList.CreateListRange());
        }
        else
        {
            this._underlyingList.AddRange(items);
        }

        this.Count = this._underlyingList.Count;
    }

    private void EnsureUnderlyingListCanBeAppended()
    {
        if (this._underlyingList.Count != this.Count)
        {
            // Another user of the list has already appended to it. Thus,
            // we need to create a copy of the list.

            var originalUnderlyingList = CreateListRange();

            // NOTE: We reserve 5 more items for future append calls.
            this._underlyingList = new List<T>(this.Count + 5);

            // NOTE: The ListRange class implements "ICollection" which makes
            //   the copy process more efficient than any collection type that
            //   doesn't implement "ICollection" (incl. "IReadOnlyCollection").
            this._underlyingList.AddRange(originalUnderlyingList);
        }
    }

    /// <summary>
    /// Creates an instance of <see cref="ListRange"/> so that a call
    /// to <see cref="List{T}.AddRange"/> is more efficient.
    /// </summary>
    [Pure]
    private ListRange CreateListRange()
    {
        return new ListRange(this._underlyingList, this.Count);
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
            yield return this._underlyingList[i];
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
        private readonly List<T> _underlyingList;

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

            this._underlyingList = underlyingList;
            this.Count = rangeLength;
        }

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public IEnumerator<T> GetEnumerator()
        {
            for (var i = 0; i < this.Count; i++)
            {
                yield return this._underlyingList[i];
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

            var index = this._underlyingList.IndexOf(item);
            if (index == -1 || index >= this.Count)
            {
                return false;
            }

            return true;
        }

        /// <inheritdoc />
        public void CopyTo(T[] array, int destArrayIndex)
        {
            this._underlyingList.CopyTo(0, array, destArrayIndex, this.Count);
        }

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public bool Remove(T item)
        {
            throw new CollectionIsReadOnlyException();
        }
    }
}

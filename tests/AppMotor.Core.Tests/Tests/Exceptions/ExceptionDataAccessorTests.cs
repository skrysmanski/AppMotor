// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Collections;
using System.Collections.ObjectModel;

using AppMotor.Core.Exceptions;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.Exceptions;

public sealed class ExceptionDataAccessorTests
{
    [Fact]
    public void TestRegularData()
    {
        // setup
        var exception = new Exception();
        var accessor = new ExceptionDataAccessor(exception);

        // test
        accessor.IsReadOnly.ShouldBe(false);
        accessor.Count.ShouldBe(0);
        accessor["abc"].ShouldBe(null);
        accessor.ContainsKey("abc").ShouldBe(false);

        // check that dictionary is writable
        accessor["abc"] = 42;
        accessor.Remove("abc");
        accessor.Clear();

        accessor["def"] = 43;

        using (var enumerator = accessor.GetEnumerator())
        {
            enumerator.MoveNext().ShouldBe(true);
            enumerator.Current.Key.ShouldBe("def");
            enumerator.Current.Value.ShouldBe(43);
            enumerator.MoveNext().ShouldBe(false);
        }

        // check that data is written through to the exception
        exception.Data["abc"].ShouldBe(null);
        exception.Data["def"].ShouldBe(43);
    }

    [Fact]
    public void TestNullData()
    {
        // setup
        var exception = new ExceptionWithNullData();
        var accessor = new ExceptionDataAccessor(exception);

        // test our assumptions
        exception.Data.ShouldBeNull();

        // test
        accessor.IsReadOnly.ShouldBe(true);
        accessor.Count.ShouldBe(0);
        accessor["abc"].ShouldBe(null);
        accessor.ContainsKey("abc").ShouldBe(false);

        Should.NotThrow(() => accessor["abc"] = 42);
        accessor["abc"].ShouldBe(null);
        Should.NotThrow(() => accessor.Remove("abc"));
        Should.NotThrow(() => accessor.Clear());

        // Enumeration is empty.
        accessor.GetEnumerator().MoveNext().ShouldBe(false);
    }

    [Fact]
    public void TestReadOnlyData()
    {
        // setup
        var exception = new ExceptionWithReadOnlyData();
        var accessor = new ExceptionDataAccessor(exception);

        // test our assumptions
        exception.Data.IsReadOnly.ShouldBe(true);

        // test
        accessor.IsReadOnly.ShouldBe(true);
        accessor.Count.ShouldBe(0);
        accessor["abc"].ShouldBe(null);
        accessor.ContainsKey("abc").ShouldBe(false);

        Should.NotThrow(() => accessor["abc"] = 42);
        accessor["abc"].ShouldBe(null);
        Should.NotThrow(() => accessor.Remove("abc"));
        Should.NotThrow(() => accessor.Clear());

        // Enumeration is empty.
        accessor.GetEnumerator().MoveNext().ShouldBe(false);
    }

    [Fact]
    public void TestIndexerGetterWithNotExistingKey_GenericsDictionary()
    {
        // setup
        var exception = new ExceptionWithActualDictionaryData();
        var accessor = new ExceptionDataAccessor(exception);

        // test
        accessor["abc"].ShouldBe(null);
    }

    /// <summary>
    /// This test exists to verify the behavior if some custom dictionary implementation
    /// accidentally implements <see cref="IDictionary.this"/> like <see cref="IDictionary{TKey,TValue}.this"/>
    /// - i.e. it throws an exception if the key is not in the dictionary (instead of returning
    /// <c>null</c> like it's correct for <see cref="IDictionary"/>).
    ///
    /// <para>While this mistake is a broken contract, even seasoned C# developers may not know that
    /// these two properties should behave differently (even though they seem to be the same). Since
    /// this mistake can happen rather easily, <see cref="ExceptionDataAccessor"/> has safeguards
    /// in place to align the behavior again with <see cref="IDictionary"/>.</para>
    /// </summary>
    [Fact]
    public void TestIndexerGetterWithNotExistingKey_CustomDictionary()
    {
        // setup
        var exception = new ExceptionWithCustomDictionaryData();
        var accessor = new ExceptionDataAccessor(exception);

        // test our assumption
        Should.Throw<KeyNotFoundException>(() => exception.Data["abc"]);

        // test
        accessor["abc"].ShouldBe(null);
    }

    private sealed class ExceptionWithNullData : Exception
    {
#pragma warning disable 8764
        public override IDictionary? Data => null;
#pragma warning restore 8764
    }

    private sealed class ExceptionWithReadOnlyData : Exception
    {
        /// <inheritdoc />
        public override IDictionary Data { get; } = new ReadOnlyDictionary<object, object>(new Dictionary<object, object>());
    }

    private sealed class ExceptionWithActualDictionaryData : Exception
    {
        /// <inheritdoc />
        public override IDictionary Data { get; } = new Dictionary<object, object>();
    }

    private sealed class ExceptionWithCustomDictionaryData : Exception
    {
        /// <inheritdoc />
        public override IDictionary Data { get; } = new MyCustomDictionary();

        private sealed class MyCustomDictionary : IDictionary<object, object>, IDictionary
        {
            // ReSharper disable once CollectionNeverUpdated.Local
            private readonly Dictionary<object, object> _underlyingDictionary = new();

            public int Count => throw new NotSupportedException();

            public bool IsReadOnly => false;

            /// <inheritdoc />
            public ICollection<object> Keys => throw new NotSupportedException();

            /// <inheritdoc />
            public ICollection<object> Values => throw new NotSupportedException();

            /// <inheritdoc />
            ICollection IDictionary.Keys => throw new NotSupportedException();

            /// <inheritdoc />
            ICollection IDictionary.Values => throw new NotSupportedException();

            /// <inheritdoc />
            public bool IsSynchronized => throw new NotSupportedException();

            /// <inheritdoc />
            public object SyncRoot => throw new NotSupportedException();

            /// <inheritdoc />
            public bool IsFixedSize => throw new NotSupportedException();

            public object this[object key]
            {
                get => this._underlyingDictionary[key];
#pragma warning disable 8767
                set => throw new NotSupportedException();
#pragma warning restore 8767
            }

            /// <inheritdoc />
            public bool TryGetValue(object key, out object value)
            {
                throw new NotSupportedException("No yet implemented");
            }

            /// <inheritdoc />
            public bool Contains(object key)
            {
                throw new NotSupportedException();
            }

            /// <inheritdoc />
            IDictionaryEnumerator IDictionary.GetEnumerator()
            {
                throw new NotSupportedException();
            }

            /// <inheritdoc />
            void IDictionary.Remove(object key)
            {
                throw new NotSupportedException();
            }

            /// <inheritdoc />
            public void CopyTo(Array array, int index)
            {
                throw new NotSupportedException();
            }


            /// <inheritdoc />
            public IEnumerator<KeyValuePair<object, object>> GetEnumerator()
            {
                throw new NotSupportedException();
            }

            /// <inheritdoc />
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            /// <inheritdoc />
            public void Add(KeyValuePair<object, object> item)
            {
                throw new NotSupportedException();
            }

            public void Clear()
            {
                throw new NotSupportedException();
            }

            /// <inheritdoc />
            public bool Contains(KeyValuePair<object, object> item)
            {
                throw new NotSupportedException();
            }

            /// <inheritdoc />
            public void CopyTo(KeyValuePair<object, object>[] array, int arrayIndex)
            {
                throw new NotSupportedException();
            }

            /// <inheritdoc />
            public bool Remove(KeyValuePair<object, object> item)
            {
                throw new NotSupportedException();
            }

            public void Add(object key, object? value)
            {
                throw new NotSupportedException();
            }

            /// <inheritdoc />
            public bool ContainsKey(object key)
            {
                throw new NotSupportedException();
            }

            /// <inheritdoc />
            public bool Remove(object key)
            {
                throw new NotSupportedException();
            }
        }
    }
}

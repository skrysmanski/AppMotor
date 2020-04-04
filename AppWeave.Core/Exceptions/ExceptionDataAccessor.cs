#region License

// Copyright  - 2020 AppWeave.Core (https://github.com/skrysmanski/AppWeave.Core)
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

using AppWeave.Core.Extensions;
using AppWeave.Core.Utils;

using JetBrains.Annotations;

namespace AppWeave.Core.Exceptions
{
    /// <summary>
    /// Provides safe access to the <see cref="Exception.Data"/> property. Usually obtained through
    /// <see cref="ExceptionExtensions.GetData"/>.
    ///
    /// <para>The safety of this struct is two-fold:</para>
    ///
    /// <para>First, there are certain situations where <see cref="Exception.Data"/> can be <c>null</c>.
    /// For one, an older version of the .NET Framework documentation for this property noted that for
    /// certain exception types this property is <c>null</c>. Also, since the property is <c>virtual</c>,
    /// we can't be sure that child classes will always return a non-null value. This struct internally
    /// treats <c>null</c> as empty, read-only dictionary. So for write operations, you simply have
    /// to check <see cref="IsReadOnly"/>.</para>
    ///
    /// <para>Secondly, it makes the data dictionary "type-safe" for <c>foreach</c>. So you can
    /// use <c>foreach (var entry : ...)</c> where entry will be a proper key value pair.</para>
    /// </summary>
    public readonly struct ExceptionDataAccessor : IReadOnlyCollection<KeyValuePair<object, object>>
    {
        [CanBeNull]
        private readonly IDictionary m_data;

        /// <inheritdoc />
        public int Count => this.m_data?.Count ?? 0;

        /// <summary>
        /// Whether the exception data dictionary is read only.
        /// </summary>
        [PublicAPI]
        public bool IsReadOnly => this.m_data?.IsReadOnly ?? true;

        /// <summary>
        /// Indexer for retrieving or adding items.
        ///
        /// <para>Note: If using the getter and the specified key does not exist in the
        /// dictionary, <c>null</c> is returned (instead of throwing an exception).</para>
        /// </summary>
        [PublicAPI, CanBeNull]
        public object this[[NotNull] object key]
        {
            get
            {
                try
                {
                    return this.m_data?[key];
                }
                catch (KeyNotFoundException)
                {
                    // NOTE: We only get here if the "IDictionary" implementation
                    //   uses accidentally the indexer contract/implementation of
                    //   IDictionary<T,V> (which throws an exception if the specified
                    //   key is not in the dictionary).
                    return null;
                }
            }
            set
            {
                VerifyNotReadOnly();

                // ReSharper disable once PossibleNullReferenceException
                this.m_data[key] = value;
            }
        }

        /// <summary>
        /// Constructor. Alternatively, you can use <see cref="ExceptionExtensions.GetData"/>.
        /// </summary>
        public ExceptionDataAccessor([NotNull] Exception exception)
        {
            Verify.ParamNotNull(exception, nameof(exception));

            // For nullability of "Data", see: https://github.com/dotnet/dotnet-api-docs/issues/4045
            this.m_data = exception.Data;
        }

        private void VerifyNotReadOnly()
        {
            if (this.IsReadOnly)
            {
                throw new ReadOnlyCollectionModificationException();
            }
        }

        /// <summary>
        /// Removes all entries from this dictionary.
        /// </summary>
        [PublicAPI]
        public void Clear()
        {
            VerifyNotReadOnly();
            this.m_data?.Clear();
        }

        /// <summary>
        /// Returns whether the specified key is contained in the dictionary.
        /// </summary>
        [PublicAPI, Pure]
        public bool ContainsKey([NotNull] object key)
        {
            return this.m_data?.Contains(key) ?? false;
        }

        /// <summary>
        /// Removes the specified key from the dictionary. If it's not
        /// in the dictionary, nothing happens (especially not exception).
        /// </summary>
        [PublicAPI]
        public void Remove([NotNull] object key)
        {
            VerifyNotReadOnly();
            this.m_data?.Remove(key);
        }

        /// <inheritdoc />
        public IEnumerator<KeyValuePair<object, object>> GetEnumerator()
        {
            if (this.m_data != null)
            {
                foreach (DictionaryEntry entry in this.m_data)
                {
                    yield return new KeyValuePair<object, object>(entry.Key, entry.Value);
                }
            }
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

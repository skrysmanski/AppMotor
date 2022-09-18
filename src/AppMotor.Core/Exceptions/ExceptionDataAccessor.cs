// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Collections;

using AppMotor.Core.Extensions;

using JetBrains.Annotations;

namespace AppMotor.Core.Exceptions;

/// <summary>
/// Provides safe access to the <see cref="Exception.Data"/> property. Usually obtained through
/// <see cref="ExceptionExtensions.GetData"/>.
///
/// <para>The safety of this type is as follows:</para>
///
/// <para>First, there are certain situations where <see cref="Exception.Data"/> can be <c>null</c>.
/// For one, an older version of the .NET Framework documentation for this property noted that for
/// certain exception types this property is <c>null</c>. Also, since the property is <c>virtual</c>,
/// we can't be sure that child classes will always return a non-null value. This struct internally
/// treats <c>null</c> as empty, read-only dictionary.</para>
///
/// <para>Secondly, if the data dictionary is read-only, write operations just wont do anything.
/// They especially won't throw any exceptions.</para>
///
/// <para>Thirdly, it makes the data dictionary "type-safe" for <c>foreach</c>. So you can
/// use <c>foreach (var entry : ...)</c> where entry will be a proper key value pair.</para>
/// </summary>
#pragma warning disable CA1815 // Override equals and operator equals on value types
public readonly struct ExceptionDataAccessor : IReadOnlyCollection<KeyValuePair<object, object?>>
#pragma warning restore CA1815 // Override equals and operator equals on value types
{
    private readonly IDictionary? _data;

    /// <inheritdoc />
    public int Count => this._data?.Count ?? 0;

    /// <summary>
    /// Whether the exception data dictionary is read only.
    /// </summary>
    [PublicAPI]
    public bool IsReadOnly => this._data?.IsReadOnly ?? true;

    /// <summary>
    /// Indexer for retrieving or adding items.
    ///
    /// <para>Note: If using the getter and the specified key does not exist in the
    /// dictionary, <c>null</c> is returned (instead of throwing an exception).</para>
    /// </summary>
    [PublicAPI]
    public object? this[object key]
    {
        get
        {
            try
            {
                return this._data?[key];
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
            if (!this.IsReadOnly)
            {
                this._data![key] = value;
            }
        }
    }

    /// <summary>
    /// Constructor. Users should use <see cref="ExceptionExtensions.GetData"/>.
    /// </summary>
    /// <remarks>
    /// To save some performance here we don't enforce that <paramref name="exception"/> is
    /// not null. Instead we treat <c>null</c> as if the data was empty and read-only.
    /// </remarks>
    internal ExceptionDataAccessor(Exception? exception)
    {
        // For nullability of "Data", see: https://github.com/dotnet/dotnet-api-docs/issues/4045
        this._data = exception?.Data;
    }

    /// <summary>
    /// Removes all entries from this dictionary.
    /// </summary>
    [PublicAPI]
    public void Clear()
    {
        if (!this.IsReadOnly)
        {
            this._data!.Clear();
        }
    }

    /// <summary>
    /// Returns whether the specified key is contained in the dictionary.
    /// </summary>
    [PublicAPI, Pure]
    public bool ContainsKey(object key)
    {
        return this._data?.Contains(key) ?? false;
    }

    /// <summary>
    /// Removes the specified key from the dictionary. If it's not
    /// in the dictionary, nothing happens (especially not exception).
    /// </summary>
    [PublicAPI]
    public void Remove(object key)
    {
        if (!this.IsReadOnly)
        {
            this._data!.Remove(key);
        }
    }

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<object, object?>> GetEnumerator()
    {
        if (this._data != null)
        {
            foreach (var entry in this._data)
            {
                if (entry is DictionaryEntry dictionaryEntry)
                {
                    yield return new KeyValuePair<object, object?>(dictionaryEntry.Key, dictionaryEntry.Value);
                }
            }
        }
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

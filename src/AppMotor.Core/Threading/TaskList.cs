﻿// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Collections;

using AppMotor.Core.Collections;

using JetBrains.Annotations;

namespace AppMotor.Core.Threading;

/// <summary>
/// A (very simple) list of tasks that - for convenience purposes - has a "+=" operator.
///
/// <para>Note: This class is not thread-safe.</para>
/// </summary>
/// <remarks>
/// For efficiency reasons, this type is a struct rather than a class - because we need to create
/// new instances within the + operator but usually only the last instance is used.
/// </remarks>
/// <seealso cref="TaskList{T}"/>
#pragma warning disable CA1815 // Override equals and operator equals on value types
public readonly struct TaskList : IReadOnlyList<Task>
#pragma warning restore CA1815 // Override equals and operator equals on value types
{
    private readonly AppendOnlyList<Task>? _underlyingList;

    /// <inheritdoc />
    public int Count => this._underlyingList?.Count ?? 0;

    /// <inheritdoc />
    public Task this[int index]
    {
        get
        {
            if (this._underlyingList == null)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return this._underlyingList[index];
        }
    }

    private TaskList(AppendOnlyList<Task> list)
    {
        this._underlyingList = list;
    }

    /// <summary>
    /// Adds a task to this list.
    /// </summary>
    public static TaskList operator +(TaskList taskList, Task task)
    {
        var newList = taskList._underlyingList?.CloneShallow() ?? new AppendOnlyList<Task>();
        newList.Append(task);

        return new TaskList(newList);
    }

    /// <summary>
    /// Calls <see cref="Task.WhenAll(IEnumerable{Task})"/> for this list.
    /// </summary>
    [PublicAPI]
    public Task WhenAll()
    {
        if (this._underlyingList is null || this._underlyingList.Count == 0)
        {
            return Task.CompletedTask;
        }

        return Task.WhenAll(this._underlyingList);
    }

    /// <summary>
    /// Calls <see cref="Task.WhenAny(IEnumerable{Task})"/> for this list. Returns the completed task.
    /// </summary>
    [PublicAPI]
    public Task<Task> WhenAny()
    {
        if (this._underlyingList is null || this._underlyingList.Count == 0)
        {
            throw new InvalidOperationException("The task list is empty.");
        }

        return Task.WhenAny(this._underlyingList);
    }

    /// <inheritdoc />
    [MustDisposeResource]
    public IEnumerator<Task> GetEnumerator()
    {
        if (this._underlyingList != null)
        {
            return this._underlyingList.GetEnumerator();
        }
        else
        {
            return Enumerable.Empty<Task>().GetEnumerator();
        }
    }

    /// <inheritdoc />
    [MustDisposeResource]
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

/// <summary>
/// A (very simple) list of tasks that - for convenience purposes - has a "+=" operator.
///
/// <para>Note: This class is not thread-safe.</para>
/// </summary>
/// <remarks>
/// For efficiency reasons, this type is a struct rather than a class - because we need to create
/// new instances within the + operator but usually only the last instance is used.
/// </remarks>
/// <seealso cref="TaskList"/>
#pragma warning disable CA1815 // Override equals and operator equals on value types
public readonly struct TaskList<T> : IReadOnlyList<Task<T>>
#pragma warning restore CA1815 // Override equals and operator equals on value types
{
    private readonly AppendOnlyList<Task<T>>? _underlyingList;

    /// <inheritdoc />
    public int Count => this._underlyingList?.Count ?? 0;

    /// <inheritdoc />
    public Task<T> this[int index]
    {
        get
        {
            if (this._underlyingList == null)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return this._underlyingList[index];
        }
    }

    private TaskList(AppendOnlyList<Task<T>> list)
    {
        this._underlyingList = list;
    }

    /// <summary>
    /// Adds a task to this list.
    /// </summary>
    public static TaskList<T> operator +(TaskList<T> taskList, Task<T> task)
    {
        var newList = taskList._underlyingList?.CloneShallow() ?? new AppendOnlyList<Task<T>>();
        newList.Append(task);

        return new TaskList<T>(newList);
    }

    /// <summary>
    /// Calls <see cref="Task.WhenAll(IEnumerable{Task})"/> for this list.
    /// </summary>
    [PublicAPI]
    public Task<T[]> WhenAll()
    {
        if (this._underlyingList is null || this._underlyingList.Count == 0)
        {
            return Task.FromResult(Array.Empty<T>());
        }

        return Task.WhenAll(this._underlyingList);
    }

    /// <summary>
    /// Calls <see cref="Task.WhenAny(IEnumerable{Task})"/> for this list. Returns the completed task.
    /// </summary>
    [PublicAPI]
    public Task<Task<T>> WhenAny()
    {
        if (this._underlyingList is null || this._underlyingList.Count == 0)
        {
            throw new InvalidOperationException("The task list is empty.");
        }

        return Task.WhenAny(this._underlyingList);
    }

    /// <inheritdoc />
    [MustDisposeResource]
    public IEnumerator<Task<T>> GetEnumerator()
    {
        if (this._underlyingList != null)
        {
            return this._underlyingList.GetEnumerator();
        }
        else
        {
            return Enumerable.Empty<Task<T>>().GetEnumerator();
        }
    }

    /// <inheritdoc />
    [MustDisposeResource]
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

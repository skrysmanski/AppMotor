#region License

// Copyright 2021 AppMotor Framework (https://github.com/skrysmanski/AppMotor)
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

using JetBrains.Annotations;

namespace AppMotor.Core.Utils;

/// <summary>
/// Simplifies implementation of <see cref="IEqualityComparer{T}"/> for reference types by already implementing
/// the null checks that implementers would otherwise do themselves.
///
/// <para>Note: This class implements <c>IEqualityComparer&lt;T?&gt;</c> - not <c>IEqualityComparer&lt;T&gt;</c>
/// (note the missing ?). If you need to implement <c>IEqualityComparer&lt;T&gt;</c>, there's no need for this
/// class.</para>
/// </summary>
/// <typeparam name="T">the type to compare; must be non-null</typeparam>
/// <seealso cref="SimpleValueTypeEqualityComparer{T}"/>
public abstract class SimpleRefTypeEqualityComparer<T> : IEqualityComparer<T?>, IEqualityComparer where T : class
{
    /// <inheritdoc />
    public bool Equals(T? x, T? y)
    {
        if (x is null && y is null)
        {
            return true;
        }
        else if (x is null || y is null)
        {
            return false;
        }
        else if (ReferenceEquals(x, y))
        {
            return true;
        }
        else
        {
            return EqualsCore(x, y);
        }
    }

    /// <inheritdoc />
    bool IEqualityComparer.Equals(object? x, object? y)
    {
        if (x is null && y is null)
        {
            return true;
        }
        else if (x is null || y is null)
        {
            return false;
        }
        else if (ReferenceEquals(x, y))
        {
            // NOTE: This if branch is a bit odd as it's not 100% clear what should happen
            //   if x and y are not of type T but still the same reference. I'd argue that
            //   it's better (and a little bit more intuitive) that we return "true" (as
            //   the two value are the same). Returning "false" here would be kind of weird.
            return true;
        }
        else if (x is T xAsT && y is T yAsT)
        {
            return EqualsCore(xAsT, yAsT);
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Determines whether the specified values are equal. Note that both values
    /// are guaranteed to be not-null and guaranteed not to be the same (i.e. reference
    /// equal) object.
    /// </summary>
    [PublicAPI]
    protected abstract bool EqualsCore(T x, T y);

    /// <inheritdoc />
    public int GetHashCode(T? obj)
    {
        if (obj is null)
        {
            // As per discussion in: https://github.com/dotnet/runtime/issues/47367
            return 0;
        }

        return GetHashCodeCore(obj);
    }

    /// <inheritdoc />
    int IEqualityComparer.GetHashCode(object? obj)
    {
        if (obj is null)
        {
            // As per discussion in: https://github.com/dotnet/runtime/issues/47367
            return 0;
        }

        if (obj is T value)
        {
            return GetHashCodeCore(value);
        }

        throw new ArgumentException($"The value is not of type '{typeof(T).FullName}'.", nameof(obj));
    }

    /// <summary>
    /// Returns the hash code for the specified value. Note that the value is
    /// guaranteed to be not-null.
    /// </summary>
    [PublicAPI]
    protected abstract int GetHashCodeCore(T value);
}

/// <summary>
/// Simplifies implementation of <see cref="IEqualityComparer{T}"/> for value types by already implementing
/// the null checks that implementers would otherwise do themselves.
///
/// <para>Note: This class implements <c>IEqualityComparer&lt;T?&gt;</c> - not <c>IEqualityComparer&lt;T&gt;</c>
/// (note the missing ?). If you need to implement <c>IEqualityComparer&lt;T&gt;</c>, there's no need for this
/// class.</para>
/// </summary>
/// <typeparam name="T">the type to compare; must be non-null</typeparam>
/// <seealso cref="SimpleRefTypeEqualityComparer{T}"/>
public abstract class SimpleValueTypeEqualityComparer<T> : IEqualityComparer<T?>, IEqualityComparer where T : struct
{
    /// <inheritdoc />
    public bool Equals(T? x, T? y)
    {
        if (x is null && y is null)
        {
            return true;
        }
        else if (x is null || y is null)
        {
            return false;
        }
        else
        {
            return EqualsCore(x.Value, y.Value);
        }
    }

    /// <inheritdoc />
    bool IEqualityComparer.Equals(object? x, object? y)
    {
        if (x is null && y is null)
        {
            return true;
        }
        else if (x is null || y is null)
        {
            return false;
        }
        else if (x is T xAsT && y is T yAsT)
        {
            return EqualsCore(xAsT, yAsT);
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Determines whether the specified values are equal. Note that both values
    /// are guaranteed to be not-null.
    /// </summary>
    [PublicAPI]
    protected abstract bool EqualsCore(T x, T y);

    /// <inheritdoc />
    public int GetHashCode(T? obj)
    {
        if (obj is null)
        {
            // As per discussion in: https://github.com/dotnet/runtime/issues/47367
            return 0;
        }

        return GetHashCodeCore(obj.Value);
    }

    /// <inheritdoc />
    int IEqualityComparer.GetHashCode(object? obj)
    {
        if (obj is null)
        {
            // As per discussion in: https://github.com/dotnet/runtime/issues/47367
            return 0;
        }

        if (obj is T value)
        {
            return GetHashCodeCore(value);
        }

        throw new ArgumentException($"The value is not of type '{typeof(T).FullName}'.", nameof(obj));
    }

    /// <summary>
    /// Returns the hash code for the specified value. Note that the value is
    /// guaranteed to be not-null.
    /// </summary>
    [PublicAPI]
    protected abstract int GetHashCodeCore(T value);
}
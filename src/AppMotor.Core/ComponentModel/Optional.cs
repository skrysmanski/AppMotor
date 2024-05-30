// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using JetBrains.Annotations;

namespace AppMotor.Core.ComponentModel;

/// <summary>
/// Represents a value that may or may not be set.
///
/// <para>The primary use case for this type is to be with a nullable type where
/// <c>null</c> is a valid value and thus can't be used to signal whether the value
/// is set or not.
/// </para>
/// </summary>
public readonly struct Optional<T> : IEquatable<Optional<T>>, IEquatable<T>
{
    /// <summary>
    /// You may use this to unset an optional value.
    /// </summary>
    [PublicAPI]
    public static readonly Optional<T> UNSET;

    private readonly T _value;

    /// <summary>
    /// The value. Can only be obtained if <see cref="HasValue"/> is <c>true</c>;
    /// otherwise an exception will be thrown.
    /// </summary>
    [PublicAPI]
    public T Value
    {
        get
        {
            if (!this.HasValue)
            {
                throw new InvalidOperationException("This value is not set.");
            }

            return this._value;
        }
    }

    /// <summary>
    /// Whether this value is set.
    /// </summary>
    [PublicAPI]
    public bool HasValue { get; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="value">The value. Can be <c>null</c> which is treated as a set value (unlike in <see cref="Optional{T}"/>).</param>
    public Optional(T value) : this()
    {
        this._value = value;
        this.HasValue = true;
    }

    /// <summary>
    /// Implicit conversion operator from <typeparamref name="T"/> to <see cref="Optional{T}"/>.
    /// </summary>
    public static implicit operator Optional<T>(T value)
    {
        return new(value);
    }

    /// <inheritdoc />
    public bool Equals(Optional<T> other)
    {
        if (this.HasValue != other.HasValue)
        {
            return false;
        }

        if (!this.HasValue) // && !other.HasValue
        {
            // If both sides are "null", we don't need to compare "_value".
            return true;
        }

        return EqualityComparer<T>.Default.Equals(this._value, other._value);
    }

    /// <inheritdoc />
    public bool Equals(T? other)
    {
        return this.HasValue && EqualityComparer<T>.Default.Equals(this._value, other);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (obj is Optional<T> otherOption)
        {
            return Equals(otherOption);
        }
        else if (obj is T other)
        {
            return Equals(other);
        }

        return false;
    }

    /// <summary>
    /// Compares the two optionals for equality.
    /// </summary>
    public static bool operator ==(Optional<T> left, Optional<T> right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Compares the two optionals for in-equality.
    /// </summary>
    public static bool operator !=(Optional<T> left, Optional<T> right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Compares the two optionals for equality.
    /// </summary>
    public static bool operator ==(T? left, Optional<T> right)
    {
        return right.Equals(left);
    }

    /// <summary>
    /// Compares the two optionals for in-equality.
    /// </summary>
    public static bool operator !=(T? left, Optional<T> right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Compares the two optionals for equality.
    /// </summary>
    public static bool operator ==(Optional<T> left, T? right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Compares the two optionals for in-equality.
    /// </summary>
    public static bool operator !=(Optional<T> left, T? right)
    {
        return !(left == right);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        if (!this.HasValue)
        {
            return 0;
        }

        return this._value?.GetHashCode() ?? 1;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        if (this.HasValue)
        {
            return this._value?.ToString() ?? "";
        }
        else
        {
            return "<not set>";
        }
    }
}

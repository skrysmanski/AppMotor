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
using System.Collections.Generic;

using JetBrains.Annotations;

namespace AppMotor.Core.DataModel
{
    /// <summary>
    /// Represents a value that may or may not be set.
    ///
    /// <para>The primary use case for this type is to be with a nullable type where
    /// <c>null</c> is a valid value and thus can't be used to signal whether the value
    /// is set or not.
    /// </para>
    /// </summary>
    public readonly struct Optional<T> : IEquatable<Optional<T>>
    {
        /// <summary>
        /// You may use this to unset an optional value.
        /// </summary>
        [PublicAPI]
        public static readonly Optional<T> UNSET = new Optional<T>();

        private readonly T m_value;

        /// <summary>
        /// The value. Can only be obtained if <see cref="IsSet"/> is <c>true</c>;
        /// otherwise an exception will be thrown.
        /// </summary>
        [PublicAPI]
        public T Value
        {
            get
            {
                if (!this.IsSet)
                {
                    throw new InvalidOperationException("This value is not set.");
                }

                return this.m_value;
            }
        }

        /// <summary>
        /// Whether this value is set.
        /// </summary>
        [PublicAPI]
        public bool IsSet { get; }

        /// <inheritdoc />
        public Optional(T value) : this()
        {
            this.m_value = value;
            this.IsSet = true;
        }

        /// <summary>
        /// Implicit coversion operator from <typeparamref name="T"/> to <see cref="Optional{T}"/>.
        /// </summary>
        public static implicit operator Optional<T>(T value)
        {
            return new(value);
        }

        /// <inheritdoc />
        public bool Equals(Optional<T> other)
        {
            if (this.IsSet != other.IsSet)
            {
                return false;
            }

            if (!this.IsSet) // && !other.IsSet
            {
                return true;
            }

            return EqualityComparer<T>.Default.Equals(this.m_value, other.m_value);
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is Optional<T> other && Equals(other);
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

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(this.m_value, this.IsSet);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            if (this.IsSet)
            {
#pragma warning disable CA1508 // Avoid dead conditional code // BUG: https://github.com/dotnet/roslyn-analyzers/issues/4509
                return this.m_value?.ToString() ?? "";
#pragma warning restore CA1508 // Avoid dead conditional code
            }
            else
            {
                return "<not set>";
            }
        }
    }
}

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

using AppMotor.Core.Collections;
using AppMotor.Core.DataModel;

using JetBrains.Annotations;

namespace AppMotor.Core.System
{
    /// <summary>
    /// Represents a string that contains (potentially) colored substrings. Each colored substring is represented
    /// by a <see cref="Substring"/> instance - created through one of the <c>TextIn...</c> classes. The primary
    /// goal of this class is to be able to make color console output (via <see cref="Terminal"/>).
    /// </summary>
    public sealed class ColoredString : IReadOnlyList<ColoredString.Substring>, IShallowCloneable<ColoredString>
    {
        [NotNull, ItemNotNull]
        private readonly AppendOnlyList<Substring> m_substrings;

        /// <inheritdoc />
        public int Count => this.m_substrings.Count;

        /// <inheritdoc />
        [NotNull]
        public Substring this[int index] => this.m_substrings[index];

        private ColoredString([CanBeNull] ColoredString other)
        {
            if (other != null)
            {
                this.m_substrings = other.m_substrings.CloneShallow();
            }
            else
            {
                this.m_substrings = new AppendOnlyList<Substring>();
            }
        }

        /// <summary>
        /// Creates a new empty <see cref="ColoredString"/>.
        /// </summary>
        [PublicAPI, NotNull, Pure]
        public static ColoredString New()
        {
            return new ColoredString(null);
        }

        /// <summary>
        /// Appends the specified colored string (<paramref name="value"/>) to this colored
        /// string.
        /// </summary>
        /// <param name="color">The color to be used for uncolored substrings in <paramref name="value"/>.
        /// If <c>null</c>, they will remain uncolored.</param>
        /// <param name="value">The color string to append.</param>
        /// <returns>Returns this instance (useful for chaining calls).</returns>
        [PublicAPI, NotNull]
        public ColoredString Append(ConsoleColor? color, [CanBeNull] ColoredString value)
        {
            if (value == null || value.Count == 0)
            {
                return this;
            }

            if (color == null)
            {
                //
                // No change in color. We can use the more efficient "AppendRange()" method.
                //
                this.m_substrings.AppendRange(value.m_substrings);
            }
            else
            {
                //
                // Change color of uncolored substrings.
                //
                IReadOnlyList<Substring> substringsToAdd;

                if (ReferenceEquals(this.m_substrings, value.m_substrings))
                {
                    // Create copy because we can't iterate and modify the
                    // same collection at the same time.
                    substringsToAdd = value.m_substrings.ToList();
                }
                else
                {
                    substringsToAdd = value.m_substrings;
                }

                foreach (var coloredSubstring in substringsToAdd)
                {
                    if (coloredSubstring.Color == null)
                    {
                        // Set color.
                        this.m_substrings.Append(new Substring(color, coloredSubstring.Text));
                    }
                    else
                    {
                        this.m_substrings.Append(coloredSubstring);
                    }
                }
            }

            return this;
        }

        /// <summary>
        /// Appends the string value with the specified color.
        /// </summary>
        /// <param name="color">The color to use for the string. Can be <c>null</c>. See
        /// <see cref="Substring.Color"/> for more details.</param>
        /// <param name="value">The value to append</param>
        /// <returns>Returns this instance (useful for chaining calls).</returns>
        /// <remarks>
        /// This overload exists because when appending Substrings, the compiler would first
        /// convert them into a <see cref="ColoredString"/> instance and then use
        /// <see cref="Append(ConsoleColor?,ColoredString)"/> instead of
        /// <see cref="Append(ConsoleColor?,object)"/> (resulting in unnecessary conversions).
        /// </remarks>
        [PublicAPI, NotNull]
        public ColoredString Append(ConsoleColor? color, [CanBeNull] Substring value)
        {
            if (value == null)
            {
                return this;
            }

            if (color == null)
            {
                return Append(value);
            }
            else
            {
                return Append(color, value.Text);
            }
        }

        [PublicAPI, NotNull]
        public ColoredString Append([CanBeNull] Substring substring)
        {
            if (substring == null || string.IsNullOrEmpty(substring.Text))
            {
                // If it's empty, don't add it (wouldn't be visible anyways).
                return this;
            }

            this.m_substrings.Append(substring);

            return this;
        }

        /// <summary>
        /// Appends the string value with the specified color.
        /// </summary>
        /// <param name="color">The color to use for the string. Can be <c>null</c>. See
        /// <see cref="Substring.Color"/> for more details.</param>
        /// <param name="value">The value to append</param>
        /// <returns>Returns this instance (useful for chaining calls).</returns>
        /// <remarks>
        /// This overload exists because when appending strings, the compiler would first
        /// convert them into a <see cref="ColoredString"/> instance and then use
        /// <see cref="Append(ConsoleColor?,ColoredString)"/> instead of
        /// <see cref="Append(ConsoleColor?,object)"/> (resulting in unnecessary conversions).
        /// </remarks>
        [PublicAPI, NotNull]
        public ColoredString Append(ConsoleColor? color, [CanBeNull] string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                // If it's null or empty, don't add it (wouldn't be visible anyways).
                return this;
            }

            this.m_substrings.Append(new Substring(color, value));

            return this;
        }

        /// <summary>
        /// Appends the value (converted to a string via <c>ToString()</c>) with the specified color.
        /// </summary>
        /// <param name="color">The color to use for the string. Can be <c>null</c>. See
        /// <see cref="Substring.Color"/> for more details.</param>
        /// <param name="value">The value to append</param>
        /// <returns>Returns this instance (useful for chaining calls).</returns>
        [PublicAPI, NotNull]
        public ColoredString Append(ConsoleColor? color, [CanBeNull] object value)
        {
            switch (value)
            {
                case ColoredString coloredString:
                    return Append(color, coloredString);

                case Substring substring:
                    return Append(color, substring);

                case null:
                    // If it's null, don't add it (wouldn't be visible anyways).
                    return this;
            }

            var valueAsString = value.ToString();
            if (valueAsString == "")
            {
                // If it's empty, don't add it (wouldn't be visible anyways).
                return this;
            }

            this.m_substrings.Append(new Substring(color, valueAsString));

            return this;
        }

        [NotNull]
        public static ColoredString operator +([NotNull] ColoredString left, [NotNull] string right)
        {
            return left.CloneShallow().Append(color: null, right);
        }

        [NotNull]
        public static ColoredString operator +([NotNull] ColoredString left, [NotNull] Substring right)
        {
            return left.CloneShallow().Append(right);
        }

        [NotNull]
        public static ColoredString operator +([NotNull] ColoredString left, [NotNull] ColoredString right)
        {
            return left.CloneShallow().Append(color: null, right);
        }

        [NotNull]
        public static ColoredString operator +([NotNull] string left, [NotNull] ColoredString right)
        {
            return New().Append(color: null, left).Append(color: null, right);
        }

        [NotNull]
        public static implicit operator ColoredString([NotNull] Substring text)
        {
            return New().Append(text);
        }

        [NotNull]
        public static implicit operator ColoredString([NotNull] string text)
        {
            return New().Append(color: null, (object)text);
        }

        #region ICollection<T>

        public IEnumerator<Substring> GetEnumerator()
        {
            return this.m_substrings.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion ICollection<T>

        /// <inheritdoc />
        public ColoredString CloneShallow()
        {
            return new ColoredString(this);
        }

        [NotNull]
        public override string ToString()
        {
            return string.Join("", this.m_substrings.Select(sub => sub.Text));
        }

        /// <summary>
        /// Represents a (potentially) colored string. Note that unlike <see cref="ColoredString"/>,
        /// instances of this class represent strings where the whole(!) string has the same color.
        ///
        /// <para>Note: To create instances of this class, use the various <c>TextIn...</c> classes.</para>
        /// </summary>
        public class Substring : IEquatable<Substring>
        {
            /// <summary>
            /// The text value of this colored string. Is never null but may be empty (when
            /// created via one of the <c>TextIn...</c> classes).
            /// </summary>
            [PublicAPI, NotNull]
            public string Text { get; }

            /// <summary>
            /// The color of this string. Can be <c>null</c> in which case the color is
            /// "inherited" (e.g. it's the console's default color).
            /// </summary>
            [PublicAPI]
            public ConsoleColor? Color { get; }

            internal Substring(ConsoleColor? color, [CanBeNull] string text)
            {
                this.Text = text ?? "";
                this.Color = color;
            }

            [NotNull]
            public static ColoredString operator +([NotNull] Substring left, [CanBeNull] string right)
            {
                return New().Append(left).Append(null, right);
            }

            [NotNull]
            public static ColoredString operator +([CanBeNull] string left, [NotNull] Substring right)
            {
                return New().Append(null, left).Append(right);
            }

            [NotNull]
            public static ColoredString operator +([NotNull] Substring left, [NotNull] Substring right)
            {
                return New().Append(left).Append(right);
            }

            /// <inheritdoc />
            public bool Equals(Substring other)
            {
                if (other is null)
                {
                    return false;
                }

                if (ReferenceEquals(this, other))
                {
                    return true;
                }

                return this.Text == other.Text && this.Color == other.Color;
            }

            /// <inheritdoc />
            public override bool Equals(object obj)
            {
                return Equals(obj as Substring);
            }

            /// <inheritdoc />
            public override int GetHashCode()
            {
                return HashCode.Combine(this.Text, this.Color);
            }

            [NotNull]
            public override string ToString()
            {
                return this.Text;
            }
        }
    }

    /// <summary>
    /// Represents a text with text color <see cref="ConsoleColor.Black"/>.
    ///
    /// <para>Use it like this: <c>(TextInBlack)"some text here"</c></para>
    /// </summary>
    [PublicAPI]
    public sealed class TextInBlack : ColoredString.Substring
    {
        private TextInBlack([NotNull] string text)
            : base(ConsoleColor.Black, text)
        {
        }

        [NotNull]
        public static explicit operator TextInBlack([CanBeNull] string text)
        {
            return new TextInBlack(text ?? "");
        }
    }

    /// <summary>
    /// Represents a text with text color <see cref="ConsoleColor.DarkGray"/>.
    ///
    /// <para>Use it like this: <c>(TextInDarkGray)"some text here"</c></para>
    /// </summary>
    [PublicAPI]
    public sealed class TextInDarkGray : ColoredString.Substring
    {
        private TextInDarkGray([NotNull] string text)
            : base(ConsoleColor.DarkGray, text)
        {
        }

        [NotNull]
        public static explicit operator TextInDarkGray([CanBeNull] string text)
        {
            return new TextInDarkGray(text ?? "");
        }
    }

    /// <summary>
    /// Represents a text with text color <see cref="ConsoleColor.Gray"/>.
    ///
    /// <para>Use it like this: <c>(TextInGray)"some text here"</c></para>
    /// </summary>
    [PublicAPI]
    public sealed class TextInGray : ColoredString.Substring
    {
        private TextInGray([NotNull] string text)
            : base(ConsoleColor.Gray, text)
        {
        }

        [NotNull]
        public static explicit operator TextInGray([CanBeNull] string text)
        {
            return new TextInGray(text ?? "");
        }
    }


    /// <summary>
    /// Represents a text with text color <see cref="ConsoleColor.White"/>.
    ///
    /// <para>Use it like this: <c>(TextInWhite)"some text here"</c></para>
    /// </summary>
    [PublicAPI]
    public sealed class TextInWhite : ColoredString.Substring
    {
        private TextInWhite([NotNull] string text)
            : base(ConsoleColor.White, text)
        {
        }

        [NotNull]
        public static explicit operator TextInWhite([CanBeNull] string text)
        {
            return new TextInWhite(text ?? "");
        }
    }

    /// <summary>
    /// Represents a text with text color <see cref="ConsoleColor.DarkBlue"/>.
    ///
    /// <para>Use it like this: <c>(TextInDarkBlue)"some text here"</c></para>
    /// </summary>
    [PublicAPI]
    public sealed class TextInDarkBlue : ColoredString.Substring
    {
        private TextInDarkBlue([NotNull] string text)
            : base(ConsoleColor.DarkBlue, text)
        {
        }

        [NotNull]
        public static explicit operator TextInDarkBlue([CanBeNull] string text)
        {
            return new TextInDarkBlue(text ?? "");
        }
    }

    /// <summary>
    /// Represents a text with text color <see cref="ConsoleColor.Blue"/>.
    ///
    /// <para>Use it like this: <c>(TextInBlue)"some text here"</c></para>
    /// </summary>
    [PublicAPI]
    public sealed class TextInBlue : ColoredString.Substring
    {
        private TextInBlue([NotNull] string text)
            : base(ConsoleColor.Blue, text)
        {
        }

        [NotNull]
        public static explicit operator TextInBlue([CanBeNull] string text)
        {
            return new TextInBlue(text ?? "");
        }
    }

    /// <summary>
    /// Represents a text with text color <see cref="ConsoleColor.DarkGreen"/>.
    ///
    /// <para>Use it like this: <c>(TextInDarkGreen)"some text here"</c></para>
    /// </summary>
    [PublicAPI]
    public sealed class TextInDarkGreen : ColoredString.Substring
    {
        private TextInDarkGreen([NotNull] string text)
            : base(ConsoleColor.DarkGreen, text)
        {
        }

        [NotNull]
        public static explicit operator TextInDarkGreen([CanBeNull] string text)
        {
            return new TextInDarkGreen(text ?? "");
        }
    }

    /// <summary>
    /// Represents a text with text color <see cref="ConsoleColor.Green"/>.
    ///
    /// <para>Use it like this: <c>(TextInGreen)"some text here"</c></para>
    /// </summary>
    [PublicAPI]
    public sealed class TextInGreen : ColoredString.Substring
    {
        private TextInGreen([NotNull] string text)
            : base(ConsoleColor.Green, text)
        {
        }

        [NotNull]
        public static explicit operator TextInGreen([CanBeNull] string text)
        {
            return new TextInGreen(text ?? "");
        }
    }

    /// <summary>
    /// Represents a text with text color <see cref="ConsoleColor.DarkCyan"/>.
    ///
    /// <para>Use it like this: <c>(TextInDarkCyan)"some text here"</c></para>
    /// </summary>
    [PublicAPI]
    public sealed class TextInDarkCyan : ColoredString.Substring
    {
        private TextInDarkCyan([NotNull] string text)
            : base(ConsoleColor.DarkCyan, text)
        {
        }

        [NotNull]
        public static explicit operator TextInDarkCyan([CanBeNull] string text)
        {
            return new TextInDarkCyan(text ?? "");
        }
    }

    /// <summary>
    /// Represents a text with text color <see cref="ConsoleColor.Cyan"/>.
    ///
    /// <para>Use it like this: <c>(TextInCyan)"some text here"</c></para>
    /// </summary>
    [PublicAPI]
    public sealed class TextInCyan : ColoredString.Substring
    {
        private TextInCyan([NotNull] string text)
            : base(ConsoleColor.Cyan, text)
        {
        }

        [NotNull]
        public static explicit operator TextInCyan([CanBeNull] string text)
        {
            return new TextInCyan(text ?? "");
        }
    }

    /// <summary>
    /// Represents a text with text color <see cref="ConsoleColor.DarkRed"/>.
    ///
    /// <para>Use it like this: <c>(TextInDarkRed)"some text here"</c></para>
    /// </summary>
    [PublicAPI]
    public sealed class TextInDarkRed : ColoredString.Substring
    {
        private TextInDarkRed([NotNull] string text)
            : base(ConsoleColor.DarkRed, text)
        {
        }

        [NotNull]
        public static explicit operator TextInDarkRed([CanBeNull] string text)
        {
            return new TextInDarkRed(text ?? "");
        }
    }

    /// <summary>
    /// Represents a text with text color <see cref="ConsoleColor.Red"/>.
    ///
    /// <para>Use it like this: <c>(TextInRed)"some text here"</c></para>
    /// </summary>
    [PublicAPI]
    public sealed class TextInRed : ColoredString.Substring
    {
        private TextInRed([NotNull] string text)
            : base(ConsoleColor.Red, text)
        {
        }

        [NotNull]
        public static explicit operator TextInRed([CanBeNull] string text)
        {
            return new TextInRed(text ?? "");
        }
    }

    /// <summary>
    /// Represents a text with text color <see cref="ConsoleColor.DarkMagenta"/>.
    ///
    /// <para>Use it like this: <c>(TextInDarkMagenta)"some text here"</c></para>
    /// </summary>
    [PublicAPI]
    public sealed class TextInDarkMagenta : ColoredString.Substring
    {
        private TextInDarkMagenta([NotNull] string text)
            : base(ConsoleColor.DarkMagenta, text)
        {
        }

        [NotNull]
        public static explicit operator TextInDarkMagenta([CanBeNull] string text)
        {
            return new TextInDarkMagenta(text ?? "");
        }
    }

    /// <summary>
    /// Represents a text with text color <see cref="ConsoleColor.Magenta"/>.
    ///
    /// <para>Use it like this: <c>(TextInMagenta)"some text here"</c></para>
    /// </summary>
    [PublicAPI]
    public sealed class TextInMagenta : ColoredString.Substring
    {
        private TextInMagenta([NotNull] string text)
            : base(ConsoleColor.Magenta, text)
        {
        }

        [NotNull]
        public static explicit operator TextInMagenta([CanBeNull] string text)
        {
            return new TextInMagenta(text ?? "");
        }
    }

    /// <summary>
    /// Represents a text with text color <see cref="ConsoleColor.DarkYellow"/>.
    ///
    /// <para>Use it like this: <c>(TextInDarkYellow)"some text here"</c></para>
    /// </summary>
    [PublicAPI]
    public sealed class TextInDarkYellow : ColoredString.Substring
    {
        private TextInDarkYellow([NotNull] string text)
            : base(ConsoleColor.DarkYellow, text)
        {
        }

        [NotNull]
        public static explicit operator TextInDarkYellow([CanBeNull] string text)
        {
            return new TextInDarkYellow(text ?? "");
        }
    }

    /// <summary>
    /// Represents a text with text color <see cref="ConsoleColor.Yellow"/>.
    ///
    /// <para>Use it like this: <c>(TextInYellow)"some text here"</c></para>
    /// </summary>
    [PublicAPI]
    public sealed class TextInYellow : ColoredString.Substring
    {
        private TextInYellow([NotNull] string text)
            : base(ConsoleColor.Yellow, text)
        {
        }

        [NotNull]
        public static explicit operator TextInYellow([CanBeNull] string text)
        {
            return new TextInYellow(text ?? "");
        }
    }

}

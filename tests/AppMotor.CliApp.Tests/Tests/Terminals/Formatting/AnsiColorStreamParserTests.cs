// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.CliApp.Terminals.Formatting;
using AppMotor.Core.Colors;

using Shouldly;

using Xunit;

namespace AppMotor.CliApp.Tests.Terminals.Formatting;

public sealed class AnsiColorStreamParserTests
{
    [Theory]
    // Start: text color
    [InlineData("\u001B[30m", true, ConsoleColor.Black)]
    [InlineData("\u001B[31m", true, ConsoleColor.DarkRed)]
    [InlineData("\u001B[32m", true, ConsoleColor.DarkGreen)]
    [InlineData("\u001B[33m", true, ConsoleColor.DarkYellow)]
    [InlineData("\u001B[34m", true, ConsoleColor.DarkBlue)]
    [InlineData("\u001B[35m", true, ConsoleColor.DarkMagenta)]
    [InlineData("\u001B[36m", true, ConsoleColor.DarkCyan)]
    [InlineData("\u001B[37m", true, ConsoleColor.Gray)]
    [InlineData("\u001B[90m", true, ConsoleColor.DarkGray)]
    [InlineData("\u001B[91m", true, ConsoleColor.Red)]
    [InlineData("\u001B[92m", true, ConsoleColor.Green)]
    [InlineData("\u001B[93m", true, ConsoleColor.Yellow)]
    [InlineData("\u001B[94m", true, ConsoleColor.Blue)]
    [InlineData("\u001B[95m", true, ConsoleColor.Magenta)]
    [InlineData("\u001B[96m", true, ConsoleColor.Cyan)]
    [InlineData("\u001B[97m", true, ConsoleColor.White)]
    [InlineData("\u001B[1;30m", true, ConsoleColor.DarkGray)]
    [InlineData("\u001B[1;31m", true, ConsoleColor.Red)]
    [InlineData("\u001B[1;32m", true, ConsoleColor.Green)]
    [InlineData("\u001B[1;33m", true, ConsoleColor.Yellow)]
    [InlineData("\u001B[1;34m", true, ConsoleColor.Blue)]
    [InlineData("\u001B[1;35m", true, ConsoleColor.Magenta)]
    [InlineData("\u001B[1;36m", true, ConsoleColor.Cyan)]
    [InlineData("\u001B[1;37m", true, ConsoleColor.White)]
    [InlineData("\u001B[1m\u001B[30m", true, ConsoleColor.DarkGray)]
    [InlineData("\u001B[1m\u001B[31m", true, ConsoleColor.Red)]
    [InlineData("\u001B[1m\u001B[32m", true, ConsoleColor.Green)]
    [InlineData("\u001B[1m\u001B[33m", true, ConsoleColor.Yellow)]
    [InlineData("\u001B[1m\u001B[34m", true, ConsoleColor.Blue)]
    [InlineData("\u001B[1m\u001B[35m", true, ConsoleColor.Magenta)]
    [InlineData("\u001B[1m\u001B[36m", true, ConsoleColor.Cyan)]
    [InlineData("\u001B[1m\u001B[37m", true, ConsoleColor.White)]
    // Start: background
    [InlineData("\u001B[40m", false, ConsoleColor.Black)]
    [InlineData("\u001B[41m", false, ConsoleColor.DarkRed)]
    [InlineData("\u001B[42m", false, ConsoleColor.DarkGreen)]
    [InlineData("\u001B[43m", false, ConsoleColor.DarkYellow)]
    [InlineData("\u001B[44m", false, ConsoleColor.DarkBlue)]
    [InlineData("\u001B[45m", false, ConsoleColor.DarkMagenta)]
    [InlineData("\u001B[46m", false, ConsoleColor.DarkCyan)]
    [InlineData("\u001B[47m", false, ConsoleColor.Gray)]
    [InlineData("\u001B[100m", false, ConsoleColor.DarkGray)]
    [InlineData("\u001B[101m", false, ConsoleColor.Red)]
    [InlineData("\u001B[102m", false, ConsoleColor.Green)]
    [InlineData("\u001B[103m", false, ConsoleColor.Yellow)]
    [InlineData("\u001B[104m", false, ConsoleColor.Blue)]
    [InlineData("\u001B[105m", false, ConsoleColor.Magenta)]
    [InlineData("\u001B[106m", false, ConsoleColor.Cyan)]
    [InlineData("\u001B[107m", false, ConsoleColor.White)]
    [InlineData("\u001B[1;40m", false, ConsoleColor.Black)]
    [InlineData("\u001B[1;41m", false, ConsoleColor.DarkRed)]
    [InlineData("\u001B[1;42m", false, ConsoleColor.DarkGreen)]
    [InlineData("\u001B[1;43m", false, ConsoleColor.DarkYellow)]
    [InlineData("\u001B[1;44m", false, ConsoleColor.DarkBlue)]
    [InlineData("\u001B[1;45m", false, ConsoleColor.DarkMagenta)]
    [InlineData("\u001B[1;46m", false, ConsoleColor.DarkCyan)]
    [InlineData("\u001B[1;47m", false, ConsoleColor.Gray)]
    [InlineData("\u001B[1m\u001B[40m", false, ConsoleColor.Black)]
    [InlineData("\u001B[1m\u001B[41m", false, ConsoleColor.DarkRed)]
    [InlineData("\u001B[1m\u001B[42m", false, ConsoleColor.DarkGreen)]
    [InlineData("\u001B[1m\u001B[43m", false, ConsoleColor.DarkYellow)]
    [InlineData("\u001B[1m\u001B[44m", false, ConsoleColor.DarkBlue)]
    [InlineData("\u001B[1m\u001B[45m", false, ConsoleColor.DarkMagenta)]
    [InlineData("\u001B[1m\u001B[46m", false, ConsoleColor.DarkCyan)]
    [InlineData("\u001B[1m\u001B[47m", false, ConsoleColor.Gray)]
    public void Test_Parsing_Color_4Bit(string escapeSequence, bool expectForegroundColor, ConsoleColor color)
    {
        // Setup
        var streamParser = new TestStreamParser();

        var expectedParsedItems = new List<IParsedItem>();
        expectedParsedItems.Add(new ParsedTextItem(isEscapeSequence: false, "abc"));
        expectedParsedItems.Add(new ParsedColorItem(foregroundColor: expectForegroundColor, color));
        expectedParsedItems.Add(new ParsedTextItem(isEscapeSequence: false, "def"));

        // Test
        streamParser.ParseNext("abc" + escapeSequence + "def");

        // Verify
        streamParser.ParsedItems.ShouldBe(expectedParsedItems);
    }

    [Theory]
    [InlineData("\u001B[38;5;0m", true, ConsoleColor.Black)]
    [InlineData("\u001B[38;5;1m", true, ConsoleColor.DarkRed)]
    [InlineData("\u001B[38;5;2m", true, ConsoleColor.DarkGreen)]
    [InlineData("\u001B[38;5;3m", true, ConsoleColor.DarkYellow)]
    [InlineData("\u001B[38;5;4m", true, ConsoleColor.DarkBlue)]
    [InlineData("\u001B[38;5;5m", true, ConsoleColor.DarkMagenta)]
    [InlineData("\u001B[38;5;6m", true, ConsoleColor.DarkCyan)]
    [InlineData("\u001B[38;5;7m", true, ConsoleColor.Gray)]
    [InlineData("\u001B[38;5;8m", true, ConsoleColor.DarkGray)]
    [InlineData("\u001B[38;5;9m", true, ConsoleColor.Red)]
    [InlineData("\u001B[38;5;10m", true, ConsoleColor.Green)]
    [InlineData("\u001B[38;5;11m", true, ConsoleColor.Yellow)]
    [InlineData("\u001B[38;5;12m", true, ConsoleColor.Blue)]
    [InlineData("\u001B[38;5;13m", true, ConsoleColor.Magenta)]
    [InlineData("\u001B[38;5;14m", true, ConsoleColor.Cyan)]
    [InlineData("\u001B[38;5;15m", true, ConsoleColor.White)]
    [InlineData("\u001B[48;5;0m", false, ConsoleColor.Black)]
    [InlineData("\u001B[48;5;1m", false, ConsoleColor.DarkRed)]
    [InlineData("\u001B[48;5;2m", false, ConsoleColor.DarkGreen)]
    [InlineData("\u001B[48;5;3m", false, ConsoleColor.DarkYellow)]
    [InlineData("\u001B[48;5;4m", false, ConsoleColor.DarkBlue)]
    [InlineData("\u001B[48;5;5m", false, ConsoleColor.DarkMagenta)]
    [InlineData("\u001B[48;5;6m", false, ConsoleColor.DarkCyan)]
    [InlineData("\u001B[48;5;7m", false, ConsoleColor.Gray)]
    [InlineData("\u001B[48;5;8m", false, ConsoleColor.DarkGray)]
    [InlineData("\u001B[48;5;9m", false, ConsoleColor.Red)]
    [InlineData("\u001B[48;5;10m", false, ConsoleColor.Green)]
    [InlineData("\u001B[48;5;11m", false, ConsoleColor.Yellow)]
    [InlineData("\u001B[48;5;12m", false, ConsoleColor.Blue)]
    [InlineData("\u001B[48;5;13m", false, ConsoleColor.Magenta)]
    [InlineData("\u001B[48;5;14m", false, ConsoleColor.Cyan)]
    [InlineData("\u001B[48;5;15m", false, ConsoleColor.White)]
    public void Test_Parsing_Color_8Bit_Below16(string escapeSequence, bool expectForegroundColor, ConsoleColor color)
    {
        // Setup
        var streamParser = new TestStreamParser();

        var expectedParsedItems = new List<IParsedItem>();
        expectedParsedItems.Add(new ParsedTextItem(isEscapeSequence: false, "abc"));
        expectedParsedItems.Add(new ParsedColorItem(foregroundColor: expectForegroundColor, color));
        expectedParsedItems.Add(new ParsedTextItem(isEscapeSequence: false, "def"));

        // Test
        streamParser.ParseNext("abc" + escapeSequence + "def");

        // Verify
        streamParser.ParsedItems.ShouldBe(expectedParsedItems);
    }

    [Theory]
    [InlineData("\u001B[38;5;16m", true, 16)]
    [InlineData("\u001B[38;5;99m", true, 99)]
    [InlineData("\u001B[38;5;100m", true, 100)]
    [InlineData("\u001B[38;5;255m", true, 255)]
    [InlineData("\u001B[48;5;16m", false, 16)]
    [InlineData("\u001B[48;5;99m", false, 99)]
    [InlineData("\u001B[48;5;100m", false, 100)]
    [InlineData("\u001B[48;5;255m", false, 255)]
    public void Test_Parsing_Color_8Bit_HigherThan15(string escapeSequence, bool expectForegroundColor, int colorIndex)
    {
        // Setup
        var streamParser = new TestStreamParser();

        var expectedParsedItems = new List<IParsedItem>();
        expectedParsedItems.Add(new ParsedTextItem(isEscapeSequence: false, "abc"));
        expectedParsedItems.Add(new ParsedColorItem(foregroundColor: expectForegroundColor, colorIndex));
        expectedParsedItems.Add(new ParsedTextItem(isEscapeSequence: false, "def"));

        // Test
        streamParser.ParseNext("abc" + escapeSequence + "def");

        // Verify
        streamParser.ParsedItems.ShouldBe(expectedParsedItems);
    }

    /// <summary>
    /// Verifies that invalid color sequences are simply dropped.
    /// </summary>
    [Theory]
    [InlineData("\u001B[38;5;-1m")]
    [InlineData("\u001B[38;5;256m")]
    [InlineData("\u001B[38;5;2.0m")]
    [InlineData("\u001B[48;5;-1m")]
    [InlineData("\u001B[48;5;256m")]
    [InlineData("\u001B[48;5;2.0m")]
    public void Test_Parsing_Color_8Bit_InvalidColor(string escapeSequence)
    {
        // Setup
        var streamParser = new TestStreamParser();

        var expectedParsedItems = new List<IParsedItem>();
        expectedParsedItems.Add(new ParsedTextItem(isEscapeSequence: false, "abc"));
        expectedParsedItems.Add(new ParsedTextItem(isEscapeSequence: false, "def"));

        // Test
        streamParser.ParseNext("abc" + escapeSequence + "def");

        // Verify
        streamParser.ParsedItems.ShouldBe(expectedParsedItems);
    }
    [Theory]
    [InlineData("\u001B[38;2;0;0;0m",       true,   0,   0,   0)]
    [InlineData("\u001B[38;2;255;0;0m",     true, 255,   0,   0)]
    [InlineData("\u001B[38;2;0;255;0m",     true,   0, 255,   0)]
    [InlineData("\u001B[38;2;0;0;255m",     true,   0,   0, 255)]
    [InlineData("\u001B[38;2;255;255;255m", true, 255, 255, 255)]
    [InlineData("\u001B[48;2;0;0;0m",       false,   0,   0,   0)]
    [InlineData("\u001B[48;2;255;0;0m",     false, 255,   0,   0)]
    [InlineData("\u001B[48;2;0;255;0m",     false,   0, 255,   0)]
    [InlineData("\u001B[48;2;0;0;255m",     false,   0,   0, 255)]
    [InlineData("\u001B[48;2;255;255;255m", false, 255, 255, 255)]
    public void Test_Parsing_Color_24Bit(string escapeSequence, bool expectForegroundColor, byte expectedRed, byte expectedGreen, byte expectedBlue)
    {
        // Setup
        var streamParser = new TestStreamParser();

        var expectedParsedItems = new List<IParsedItem>();
        expectedParsedItems.Add(new ParsedTextItem(isEscapeSequence: false, "abc"));
        expectedParsedItems.Add(new ParsedColorItem(foregroundColor: expectForegroundColor, new RgbColor(expectedRed, expectedGreen, expectedBlue)));
        expectedParsedItems.Add(new ParsedTextItem(isEscapeSequence: false, "def"));

        // Test
        streamParser.ParseNext("abc" + escapeSequence + "def");

        // Verify
        streamParser.ParsedItems.ShouldBe(expectedParsedItems);
    }

    /// <summary>
    /// Verifies that invalid color sequences are simply dropped.
    /// </summary>
    [Theory]
    [InlineData("\u001B[38;2;0;0;256m")]
    [InlineData("\u001B[38;2;-1;0;0m")]
    [InlineData("\u001B[38;2;0;a;0m")]
    [InlineData("\u001B[38;2;0;5.5;0m")]
    [InlineData("\u001B[38;2;0m")]
    [InlineData("\u001B[38;2;0;m")]
    [InlineData("\u001B[38;2;0;0m")]
    [InlineData("\u001B[38;2;0;0;m")]
    [InlineData("\u001B[48;2;0;0;256m")]
    [InlineData("\u001B[48;2;-1;0;0m")]
    [InlineData("\u001B[48;2;0;a;0m")]
    [InlineData("\u001B[48;2;0;5.5;0m")]
    [InlineData("\u001B[48;2;0m")]
    [InlineData("\u001B[48;2;0;m")]
    [InlineData("\u001B[48;2;0;0m")]
    [InlineData("\u001B[48;2;0;0;m")]
    public void Test_Parsing_Color_24Bit_InvalidColor(string escapeSequence)
    {
        // Setup
        var streamParser = new TestStreamParser();

        var expectedParsedItems = new List<IParsedItem>();
        expectedParsedItems.Add(new ParsedTextItem(isEscapeSequence: false, "abc"));
        expectedParsedItems.Add(new ParsedTextItem(isEscapeSequence: false, "def"));

        // Test
        streamParser.ParseNext("abc" + escapeSequence + "def");

        // Verify
        streamParser.ParsedItems.ShouldBe(expectedParsedItems);
    }

    /// <summary>
    /// Verifies that invalid color sequences are simply dropped.
    /// </summary>
    [Theory]
    [InlineData("\u001B[38;3;0m")]
    [InlineData("\u001B[38;-1;0m")]
    [InlineData("\u001B[38;2.0;0;a;0m")]
    [InlineData("\u001B[38;2m")]
    [InlineData("\u001B[38;5m")]
    [InlineData("\u001B[38m")]
    [InlineData("\u001B[1;38m")] // longer than 2 characters
    [InlineData("\u001B[48;3;0m")]
    [InlineData("\u001B[48;-1;0m")]
    [InlineData("\u001B[48;2.0;0;a;0m")]
    [InlineData("\u001B[48;2m")]
    [InlineData("\u001B[48;5m")]
    [InlineData("\u001B[48m")]
    [InlineData("\u001B[1;48m")] // longer than 2 characters
    public void Test_Parsing_Color_InvalidColorType(string escapeSequence)
    {
        // Setup
        var streamParser = new TestStreamParser();

        var expectedParsedItems = new List<IParsedItem>();
        expectedParsedItems.Add(new ParsedTextItem(isEscapeSequence: false, "abc"));
        expectedParsedItems.Add(new ParsedTextItem(isEscapeSequence: false, "def"));

        // Test
        streamParser.ParseNext("abc" + escapeSequence + "def");

        // Verify
        streamParser.ParsedItems.ShouldBe(expectedParsedItems);
    }

    [Theory]
    [InlineData("\u001B[7m", "7")]
    [InlineData("\u001B[7;12m", "7", "12")]
    [InlineData("\u001B[7m\u001B[12m", "7", "12")]
    public void Test_NonColorEscapeSequence(string escapeSequence, params string[] expectedSequenceNumbers)
    {
        // Setup
        var streamParser = new TestStreamParser();

        var expectedParsedItems = new List<IParsedItem>();
        expectedParsedItems.Add(new ParsedTextItem(isEscapeSequence: false, "abc"));
        foreach (var expectedSequenceNumber in expectedSequenceNumbers)
        {
            expectedParsedItems.Add(new ParsedTextItem(isEscapeSequence: true, expectedSequenceNumber));
        }
        expectedParsedItems.Add(new ParsedTextItem(isEscapeSequence: false, "def"));

        // Test
        streamParser.ParseNext("abc" + escapeSequence + "def");

        // Verify
        streamParser.ParsedItems.ShouldBe(expectedParsedItems);
    }

    [Theory]
    [InlineData("\u001B[0m", true, true)]
    [InlineData("\u001B[39m", true, false)]
    [InlineData("\u001B[49m", false, true)]
    public void Test_ResetColorEscapeSequence(string escapeSequence, bool expectedForegroundColorReset, bool expectedBackgroundColorReset)
    {
        // Setup
        var streamParser = new TestStreamParser();

        var expectedParsedItems = new List<IParsedItem>();
        expectedParsedItems.Add(new ParsedTextItem(isEscapeSequence: false, "abc"));
        expectedParsedItems.Add(new ParsedResetColorsItem(expectedForegroundColorReset, expectedBackgroundColorReset));
        expectedParsedItems.Add(new ParsedTextItem(isEscapeSequence: false, "def"));

        // Test
        streamParser.ParseNext("abc" + escapeSequence + "def");

        // Verify
        streamParser.ParsedItems.ShouldBe(expectedParsedItems);
    }

    /// <summary>
    /// This test primarily exists to increase code coverage for <c>TryParseNumber()</c>.
    /// </summary>
    /// <param name="escapeSequence"></param>
    /// <param name="expectedSequenceNumber"></param>
    [Theory]
    [InlineData("\u001B[7m", "7")]
    [InlineData("\u001B[a2m", "a2")]
    [InlineData("\u001B[2am", "2a")]
    [InlineData("\u001B[23m", "23")]
    [InlineData("\u001B[a23m", "a23")]
    [InlineData("\u001B[2a3m", "2a3")]
    [InlineData("\u001B[23am", "23a")]
    [InlineData("\u001B[234m", "234")]
    [InlineData("\u001B[aaaam", "aaaa")]
    [InlineData("\u001B[1234m", "1234")]
    public void Test_TryParseNumber(string escapeSequence, string expectedSequenceNumber)
    {
        // Setup
        var streamParser = new TestStreamParser();

        // Test
        streamParser.ParseNext(escapeSequence);

        // Verify
        streamParser.ParsedItems.ShouldBe(new[] { new ParsedTextItem(isEscapeSequence: true, expectedSequenceNumber) });
    }

    private sealed class TestStreamParser : AnsiColorStreamParser
    {
        public List<IParsedItem> ParsedItems { get; } = new();

        /// <inheritdoc />
        protected override void OnTextColor(ConsoleColor color)
        {
            this.ParsedItems.Add(new ParsedColorItem(foregroundColor: true, color));
        }

        /// <inheritdoc />
        protected override void OnTextColor(int colorIndex)
        {
            this.ParsedItems.Add(new ParsedColorItem(foregroundColor: true, colorIndex));
        }

        /// <inheritdoc />
        protected override void OnTextColor(RgbColor color)
        {
            this.ParsedItems.Add(new ParsedColorItem(foregroundColor: true, color));
        }

        /// <inheritdoc />
        protected override void OnBackgroundColor(ConsoleColor color)
        {
            this.ParsedItems.Add(new ParsedColorItem(foregroundColor: false, color));
        }

        /// <inheritdoc />
        protected override void OnBackgroundColor(int colorIndex)
        {
            this.ParsedItems.Add(new ParsedColorItem(foregroundColor: false, colorIndex));
        }

        /// <inheritdoc />
        protected override void OnBackgroundColor(RgbColor color)
        {
            this.ParsedItems.Add(new ParsedColorItem(foregroundColor: false, color));
        }

        /// <inheritdoc />
        protected override void OnResetColors(bool resetForegroundColor, bool resetBackgroundColor)
        {
            this.ParsedItems.Add(new ParsedResetColorsItem(resetForegroundColor, resetBackgroundColor));
        }

        /// <inheritdoc />
        protected override void OnText(ReadOnlySpan<char> text)
        {
            this.ParsedItems.Add(new ParsedTextItem(isEscapeSequence: false, text));
        }

        /// <inheritdoc />
        protected override void OnNonColorAnsiEscapeSequence(ReadOnlySpan<char> escapeSequenceContents)
        {
            this.ParsedItems.Add(new ParsedTextItem(isEscapeSequence: true, escapeSequenceContents));
        }
    }

    private interface IParsedItem
    {

    }

    private sealed class ParsedTextItem : IParsedItem, IEquatable<ParsedTextItem>
    {
        public bool IsEscapeSequence { get; }

        public string Value { get; }

        public ParsedTextItem(bool isEscapeSequence, ReadOnlySpan<char> value)
        {
            this.IsEscapeSequence = isEscapeSequence;
            this.Value = value.ToString();
        }

        /// <inheritdoc />
        public bool Equals(ParsedTextItem? other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this.IsEscapeSequence == other.IsEscapeSequence && this.Value == other.Value;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || (obj is ParsedTextItem other && Equals(other));
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(this.IsEscapeSequence, this.Value);
        }

        public static bool operator ==(ParsedTextItem? left, ParsedTextItem? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ParsedTextItem? left, ParsedTextItem? right)
        {
            return !Equals(left, right);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"ParsedTextItem[IsEscapeSequence={this.IsEscapeSequence}; Value={this.Value}]";
        }
    }

    private sealed class ParsedColorItem : IParsedItem, IEquatable<ParsedColorItem>
    {
        public bool ForegroundColor { get; }

        public object Color { get; }

        public ParsedColorItem(bool foregroundColor, object color)
        {
            this.ForegroundColor = foregroundColor;
            this.Color = color;
        }

        /// <inheritdoc />
        public bool Equals(ParsedColorItem? other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this.ForegroundColor == other.ForegroundColor && this.Color.Equals(other.Color);
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || (obj is ParsedColorItem other && Equals(other));
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(this.ForegroundColor, this.Color);
        }

        public static bool operator ==(ParsedColorItem? left, ParsedColorItem? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ParsedColorItem? left, ParsedColorItem? right)
        {
            return !Equals(left, right);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"ParsedColorItem[Foreground={this.ForegroundColor}; Color={this.Color}]";
        }
    }

    private sealed class ParsedResetColorsItem : IParsedItem, IEquatable<ParsedResetColorsItem>
    {
        public bool ResetForegroundColor { get; }

        public bool ResetBackgroundColor { get; }

        public ParsedResetColorsItem(bool resetForegroundColor, bool resetBackgroundColor)
        {
            this.ResetForegroundColor = resetForegroundColor;
            this.ResetBackgroundColor = resetBackgroundColor;
        }

        /// <inheritdoc />
        public bool Equals(ParsedResetColorsItem? other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this.ResetForegroundColor == other.ResetForegroundColor && this.ResetBackgroundColor == other.ResetBackgroundColor;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || (obj is ParsedResetColorsItem other && Equals(other));
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(this.ResetForegroundColor, this.ResetBackgroundColor);
        }

        public static bool operator ==(ParsedResetColorsItem? left, ParsedResetColorsItem? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ParsedResetColorsItem? left, ParsedResetColorsItem? right)
        {
            return !Equals(left, right);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"ResetColors[Foreground={this.ResetForegroundColor}; Background={this.ResetBackgroundColor}]";
        }
    }
}

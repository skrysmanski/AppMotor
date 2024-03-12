// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.CliApp.Terminals.Formatting;

using Shouldly;

using Xunit;

namespace AppMotor.CliApp.Tests.Terminals.Formatting;

public sealed class AnsiStreamParserTests
{
    [Theory]
    [InlineData("abc", false, "abc")]
    [InlineData("\u001B[1m", true, "\u001B[1m")]
    [InlineData("\u001B[1mabcm", true, "\u001B[1m", false, "abcm")]
    [InlineData("\u001B[1mabcm\u001B[0m", true, "\u001B[1m", false, "abcm", true, "\u001B[0m")]
    [InlineData("abc\u001B[1mabcm", false, "abc", true, "\u001B[1m", false, "abcm")]
    public void Test_RegularParsing(string input, params object[] expectedParsedItemsAsObjects)
    {
        // Setup
        var ansiStreamParser = new TestStreamParser();

        var expectedParsedItems = new List<ParsedItem>();

        for (var index = 0; index < expectedParsedItemsAsObjects.Length; index += 2)
        {
            var isEscapeSequence = (bool)expectedParsedItemsAsObjects[index];
            var value = (string)expectedParsedItemsAsObjects[index + 1];

            expectedParsedItems.Add(new ParsedItem(isEscapeSequence, value));
        }

        // Test
        ansiStreamParser.ParseNext(input);

        // Verify
        ansiStreamParser.ParseResults.ShouldBe(expectedParsedItems);
    }

    [Theory]
    [InlineData("\u001Babc")]
    [InlineData("a\u001Bbc")]
    [InlineData("a\u001Bbc\u001Bdef")]
    public void Test_InvalidEscapeSequence(string input)
    {
        // Setup
        var ansiStreamParser = new TestStreamParser();

        // Test
        ansiStreamParser.ParseNext(input);

        // Verify
        ansiStreamParser.ParseResults.All(pi => !pi.IsEscapeSequence).ShouldBe(true);
        string.Join("", ansiStreamParser.ParseResults.Select(pi => pi.Value)).ShouldBe(input);
    }

    [Theory]
    [InlineData("\u001B[1;31;39m", "", "")]
    [InlineData("\u001B[1;31;39m", "abc", "")]
    [InlineData("\u001B[1;31;39m", "abc", "def")]
    [InlineData("\u001B[1;31;39m", "", "def")]
    public void Test_PartialEscapeSequence(string escapeSequence, string prefix, string postfix)
    {
        // Setup
        var ansiStreamParser = new TestStreamParser();

        var expectedParsedItems = new List<ParsedItem>();
        if (prefix.Length != 0)
        {
            expectedParsedItems.Add(new ParsedItem(isEscapeSequence: false, prefix));
        }
        expectedParsedItems.Add(new ParsedItem(isEscapeSequence: true, escapeSequence));
        if (postfix.Length != 0)
        {
            expectedParsedItems.Add(new ParsedItem(isEscapeSequence: false, postfix));
        }

        // Test
        for (var i = 0; i < escapeSequence.Length; i++)
        {
            var ch = escapeSequence[i..(i + 1)];
            if (i == 0 && prefix.Length != 0)
            {
                ansiStreamParser.ParseNext(prefix + ch);
            }
            else if (i == escapeSequence.Length - 1 && postfix.Length != 0)
            {
                ansiStreamParser.ParseNext(ch + postfix);
            }
            else
            {
                ansiStreamParser.ParseNext(ch);
            }
        }

        // Verify
        ansiStreamParser.ParseResults.ShouldBe(expectedParsedItems);
    }

    [Theory]
    [InlineData("\u001B[1;31;39m", 1, "")]
    [InlineData("\u001B[1;31;39m", 2, "")]
    [InlineData("\u001B[1;31;39m", 3, "")]
    [InlineData("\u001B[1;31;39m", 4, "")]
    [InlineData("\u001B[1;31;39m", 5, "")]
    [InlineData("\u001B[1;31;39m", 1, "abc")]
    [InlineData("\u001B[1;31;39m", 2, "abc")]
    [InlineData("\u001B[1;31;39m", 3, "abc")]
    [InlineData("\u001B[1;31;39m", 4, "abc")]
    [InlineData("\u001B[1;31;39m", 5, "abc")]
    public void Test_PartialStartOfEscapeSequence(string escapeSequence, int partialLength, string postfix)
    {
        // Setup
        var ansiStreamParser = new TestStreamParser();

        var expectedParsedItems = new List<ParsedItem>();
        expectedParsedItems.Add(new ParsedItem(isEscapeSequence: true, escapeSequence));
        if (postfix.Length != 0)
        {
            expectedParsedItems.Add(new ParsedItem(isEscapeSequence: false, postfix));
        }

        // Test
        ansiStreamParser.ParseNext(escapeSequence[0..partialLength]);
        ansiStreamParser.ParseNext(escapeSequence[partialLength..] + postfix);

        // Verify
        ansiStreamParser.ParseResults.ShouldBe(expectedParsedItems);
    }

    [Theory]
    [InlineData("\u001B1;31;39m", 1)]
    [InlineData("\u001B1;31;39m", 2)]
    [InlineData("\u001B1;31;39m", 3)]
    public void Test_PartialStartOfInvalidEscapeSequence(string input, int partialLength)
    {
        // Setup
        var ansiStreamParser = new TestStreamParser();

        // Test
        ansiStreamParser.ParseNext(input[0..partialLength]);
        ansiStreamParser.ParseNext(input[partialLength..]);

        // Verify
        ansiStreamParser.ParseResults.All(pi => !pi.IsEscapeSequence).ShouldBe(true);
        string.Join("", ansiStreamParser.ParseResults.Select(pi => pi.Value)).ShouldBe(input);
    }

    [Fact]
    public void Test_EmptyValue()
    {
        // Setup
        var ansiStreamParser = new TestStreamParser();

        // Test
        ansiStreamParser.ParseNext("");

        // Verify
        ansiStreamParser.ParseResults.Count.ShouldBe(0);
    }

    private sealed class TestStreamParser : AnsiStreamParser
    {
        public List<ParsedItem> ParseResults { get; } = [];

        /// <inheritdoc />
        protected override void OnAnsiEscapeSequence(ReadOnlySpan<char> escapeSequence)
        {
            this.ParseResults.Add(new ParsedItem(isEscapeSequence: true, escapeSequence));
        }

        /// <inheritdoc />
        protected override void OnText(ReadOnlySpan<char> text)
        {
            this.ParseResults.Add(new ParsedItem(isEscapeSequence: false, text));
        }
    }

    private sealed class ParsedItem : IEquatable<ParsedItem>
    {
        public bool IsEscapeSequence { get; }

        public string Value { get; }

        public ParsedItem(bool isEscapeSequence, ReadOnlySpan<char> value)
        {
            this.IsEscapeSequence = isEscapeSequence;
            this.Value = value.ToString();
        }

        /// <inheritdoc />
        public bool Equals(ParsedItem? other)
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
            return ReferenceEquals(this, obj) || (obj is ParsedItem other && Equals(other));
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(this.IsEscapeSequence, this.Value);
        }

        public static bool operator ==(ParsedItem? left, ParsedItem? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ParsedItem? left, ParsedItem? right)
        {
            return !Equals(left, right);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{this.Value} (escape sequence: {this.IsEscapeSequence})";
        }
    }
}

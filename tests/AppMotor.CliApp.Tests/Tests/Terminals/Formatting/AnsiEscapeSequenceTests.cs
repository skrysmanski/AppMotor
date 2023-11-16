// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.CliApp.Terminals.Formatting;

using Shouldly;

using Xunit;

namespace AppMotor.CliApp.Tests.Terminals.Formatting;

/// <summary>
/// Tests for <see cref="AnsiEscapeSequence"/>.
/// </summary>
public sealed class AnsiEscapeSequenceTests
{
    [Fact]
    public void Test_Create_Number()
    {
        AnsiEscapeSequence.Create(4).ShouldBe("\u001b[4m");
    }

    [Fact]
    public void Test_Create_String()
    {
        AnsiEscapeSequence.Create("1234").ShouldBe("\u001b[1234m");
    }

    [Fact]
    public void Test_Create_Span()
    {
        AnsiEscapeSequence.Create("1234".AsSpan()).ShouldBe("\u001b[1234m");
    }

    [Fact]
    public void Test_ExtractSequenceContent()
    {
        AnsiEscapeSequence.ExtractSequenceContent("\u001b[1234m".AsSpan()).ToString().ShouldBe("1234");
    }

    [Fact]
    public void Test_ExtractSequenceContent_EmptyContent()
    {
        AnsiEscapeSequence.ExtractSequenceContent("\u001b[m".AsSpan()).Length.ShouldBe(0);
    }

    [Theory]
    [InlineData("")]
    [InlineData("\u001b")]
    [InlineData("\u001b[")]
    public void Test_ExtractSequenceContent_InvalidContent(string content)
    {
        // ReSharper disable once MustUseReturnValue
        Should.Throw<ArgumentOutOfRangeException>(() => AnsiEscapeSequence.ExtractSequenceContent(content.AsSpan()));
    }
}

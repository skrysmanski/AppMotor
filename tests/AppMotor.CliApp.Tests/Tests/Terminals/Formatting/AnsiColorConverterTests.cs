// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.CliApp.Terminals.Formatting;

using Shouldly;

using Xunit;

namespace AppMotor.CliApp.Tests.Terminals.Formatting;

/// <summary>
/// Tests for <see cref="AnsiColorConverter"/>.
/// </summary>
public sealed class AnsiColorConverterTests
{
    [Theory]
    [InlineData(ConsoleColor.Black, 30)]
    [InlineData(ConsoleColor.DarkBlue, 34)]
    [InlineData(ConsoleColor.DarkGreen, 32)]
    [InlineData(ConsoleColor.DarkCyan, 36)]
    [InlineData(ConsoleColor.DarkRed, 31)]
    [InlineData(ConsoleColor.DarkMagenta, 35)]
    [InlineData(ConsoleColor.DarkYellow, 33)]
    [InlineData(ConsoleColor.Gray, 37)]
    [InlineData(ConsoleColor.DarkGray, 90)]
    [InlineData(ConsoleColor.Blue, 94)]
    [InlineData(ConsoleColor.Green, 92)]
    [InlineData(ConsoleColor.Cyan, 96)]
    [InlineData(ConsoleColor.Red, 91)]
    [InlineData(ConsoleColor.Magenta, 95)]
    [InlineData(ConsoleColor.Yellow, 93)]
    [InlineData(ConsoleColor.White, 97)]
    public void Test_ConvertToAnsiColor(ConsoleColor color, int expectedResult)
    {
        AnsiColorConverter.ConvertToAnsiColor(color).ShouldBe(expectedResult);
    }

    [Fact]
    public void Test_ConvertToAnsiColor_InvalidColor()
    {
        Should.Throw<ArgumentOutOfRangeException>(() => AnsiColorConverter.ConvertToAnsiColor((ConsoleColor)5000));
    }

    [Theory]
    [InlineData(30, ConsoleColor.Black)]
    [InlineData(34, ConsoleColor.DarkBlue)]
    [InlineData(32, ConsoleColor.DarkGreen)]
    [InlineData(36, ConsoleColor.DarkCyan)]
    [InlineData(31, ConsoleColor.DarkRed)]
    [InlineData(35, ConsoleColor.DarkMagenta)]
    [InlineData(33, ConsoleColor.DarkYellow)]
    [InlineData(37, ConsoleColor.Gray)]
    [InlineData(90, ConsoleColor.DarkGray)]
    [InlineData(94, ConsoleColor.Blue)]
    [InlineData(92, ConsoleColor.Green)]
    [InlineData(96, ConsoleColor.Cyan)]
    [InlineData(91, ConsoleColor.Red)]
    [InlineData(95, ConsoleColor.Magenta)]
    [InlineData(93, ConsoleColor.Yellow)]
    [InlineData(97, ConsoleColor.White)]
    public void Test_ConvertToConsoleColor_NonBright(int value, ConsoleColor expectedColor)
    {
        AnsiColorConverter.ConvertToConsoleColor(value, bright: false).ShouldBe(expectedColor);
    }

    [Theory]
    [InlineData(30, ConsoleColor.DarkGray)]
    [InlineData(34, ConsoleColor.Blue)]
    [InlineData(32, ConsoleColor.Green)]
    [InlineData(36, ConsoleColor.Cyan)]
    [InlineData(31, ConsoleColor.Red)]
    [InlineData(35, ConsoleColor.Magenta)]
    [InlineData(33, ConsoleColor.Yellow)]
    [InlineData(37, ConsoleColor.White)]
    [InlineData(90, ConsoleColor.DarkGray)]
    [InlineData(94, ConsoleColor.Blue)]
    [InlineData(92, ConsoleColor.Green)]
    [InlineData(96, ConsoleColor.Cyan)]
    [InlineData(91, ConsoleColor.Red)]
    [InlineData(95, ConsoleColor.Magenta)]
    [InlineData(93, ConsoleColor.Yellow)]
    [InlineData(97, ConsoleColor.White)]
    public void Test_ConvertToConsoleColor_Bright(int value, ConsoleColor expectedColor)
    {
        AnsiColorConverter.ConvertToConsoleColor(value, bright: true).ShouldBe(expectedColor);
    }

    [Fact]
    public void Test_ConvertToConsoleColor_InvalidColor()
    {
        Should.Throw<ArgumentOutOfRangeException>(() => AnsiColorConverter.ConvertToConsoleColor(5000, bright: true));
        Should.Throw<ArgumentOutOfRangeException>(() => AnsiColorConverter.ConvertToConsoleColor(5000, bright: false));
    }

    [Theory]
    [InlineData(29, false)]
    [InlineData(30, true)]
    [InlineData(31, true)]
    [InlineData(32, true)]
    [InlineData(33, true)]
    [InlineData(34, true)]
    [InlineData(35, true)]
    [InlineData(36, true)]
    [InlineData(37, true)]
    [InlineData(38, false)]
    [InlineData(89, false)]
    [InlineData(90, true)]
    [InlineData(91, true)]
    [InlineData(92, true)]
    [InlineData(93, true)]
    [InlineData(94, true)]
    [InlineData(95, true)]
    [InlineData(96, true)]
    [InlineData(97, true)]
    [InlineData(98, false)]
    [InlineData(40, false)] // background colors
    [InlineData(100, false)]
    public void Test_Is4BitForegroundColor(int value, bool isForegroundColor)
    {
        AnsiColorConverter.Is4BitForegroundColor(value).ShouldBe(isForegroundColor);
    }

    [Theory]
    [InlineData(39, false)]
    [InlineData(40, true)]
    [InlineData(41, true)]
    [InlineData(42, true)]
    [InlineData(43, true)]
    [InlineData(44, true)]
    [InlineData(45, true)]
    [InlineData(46, true)]
    [InlineData(47, true)]
    [InlineData(48, false)]
    [InlineData(99, false)]
    [InlineData(100, true)]
    [InlineData(101, true)]
    [InlineData(102, true)]
    [InlineData(103, true)]
    [InlineData(104, true)]
    [InlineData(105, true)]
    [InlineData(106, true)]
    [InlineData(107, true)]
    [InlineData(108, false)]
    [InlineData(30, false)] // foreground colors
    [InlineData(90, false)]
    public void Test_Is4BitBackgroundColor(int value, bool isBackgroundColor)
    {
        AnsiColorConverter.Is4BitBackgroundColor(value).ShouldBe(isBackgroundColor);
    }

    [Theory]
    [InlineData(0, ConsoleColor.Black)]
    [InlineData(1, ConsoleColor.DarkRed)]
    [InlineData(2, ConsoleColor.DarkGreen)]
    [InlineData(3, ConsoleColor.DarkYellow)]
    [InlineData(4, ConsoleColor.DarkBlue)]
    [InlineData(5, ConsoleColor.DarkMagenta)]
    [InlineData(6, ConsoleColor.DarkCyan)]
    [InlineData(7, ConsoleColor.Gray)]
    [InlineData(8, ConsoleColor.DarkGray)]
    [InlineData(9, ConsoleColor.Red)]
    [InlineData(10, ConsoleColor.Green)]
    [InlineData(11, ConsoleColor.Yellow)]
    [InlineData(12, ConsoleColor.Blue)]
    [InlineData(13, ConsoleColor.Magenta)]
    [InlineData(14, ConsoleColor.Cyan)]
    [InlineData(15, ConsoleColor.White)]
    public void Test_Convert8BitColorToConsoleColor(int value, ConsoleColor expectedColor)
    {
        AnsiColorConverter.Convert8BitColorToConsoleColor(value).ShouldBe(expectedColor);
    }

    [Fact]
    public void Test_Convert8BitColorToConsoleColor_OtherColor()
    {
        AnsiColorConverter.Convert8BitColorToConsoleColor(16).ShouldBe(null);
        AnsiColorConverter.Convert8BitColorToConsoleColor(255).ShouldBe(null);
        AnsiColorConverter.Convert8BitColorToConsoleColor(-10).ShouldBe(null);
        AnsiColorConverter.Convert8BitColorToConsoleColor(1000).ShouldBe(null);
    }
}

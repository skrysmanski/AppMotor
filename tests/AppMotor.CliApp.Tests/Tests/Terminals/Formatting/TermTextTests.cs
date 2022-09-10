// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.CliApp.Terminals.Formatting;
using AppMotor.Core.Colors;

using Shouldly;

using Xunit;

namespace AppMotor.CliApp.Tests.Terminals.Formatting;

/// <summary>
/// Tests for <see cref="TermText"/> and related tests for <see cref="AnsiTextFormatter"/>.
/// </summary>
public sealed class TermTextTests
{
    [Fact]
    public void Test_Underline()
    {
        const string SEQUENCE_CONTENT = "4";
        const string EXPECTED_TEXT = $"\u001b[{SEQUENCE_CONTENT}mmy_text\u001b[0m";

        TermText.Underline().Text("my_text").ShouldBe(EXPECTED_TEXT);
        TermText.Underline("my_text").ShouldBe(EXPECTED_TEXT);

        new AnsiTextFormatter().Underline().Text("my_text").ShouldBe(EXPECTED_TEXT);
        new AnsiTextFormatter().Underline("my_text").ShouldBe(EXPECTED_TEXT);
    }

    [Fact]
    public void Test_Inverse()
    {
        const string SEQUENCE_CONTENT = "7";
        const string EXPECTED_TEXT = $"\u001b[{SEQUENCE_CONTENT}mmy_text\u001b[0m";

        TermText.Inverse().Text("my_text").ShouldBe(EXPECTED_TEXT);
        TermText.Inverse("my_text").ShouldBe(EXPECTED_TEXT);

        new AnsiTextFormatter().Inverse().Text("my_text").ShouldBe(EXPECTED_TEXT);
        new AnsiTextFormatter().Inverse("my_text").ShouldBe(EXPECTED_TEXT);
    }

    #region RGB colors

    [Fact]
    public void Test_Rgb_Components()
    {
        const string SEQUENCE_CONTENT = "38;2;12;34;56";
        const string EXPECTED_TEXT = $"\u001b[{SEQUENCE_CONTENT}mmy_text\u001b[0m";

        TermText.Rgb(12, 34, 56).Text("my_text").ShouldBe(EXPECTED_TEXT);
        new AnsiTextFormatter().Rgb(12, 34, 56).Text("my_text").ShouldBe(EXPECTED_TEXT);
    }

    [Fact]
    public void Test_Rgb_Color()
    {
        const string SEQUENCE_CONTENT = "38;2;12;34;56";
        const string EXPECTED_TEXT = $"\u001b[{SEQUENCE_CONTENT}mmy_text\u001b[0m";

        TermText.Rgb(new RgbColor(12, 34, 56)).Text("my_text").ShouldBe(EXPECTED_TEXT);
        new AnsiTextFormatter().Rgb(new RgbColor(12, 34, 56)).Text("my_text").ShouldBe(EXPECTED_TEXT);
    }

    [Fact]
    public void Test_Rgb_GenericColor()
    {
        const string SEQUENCE_CONTENT = "38;2;12;34;56";
        const string EXPECTED_TEXT = $"\u001b[{SEQUENCE_CONTENT}mmy_text\u001b[0m";

        TermText.Rgb(new HexColor("#0C2238")).Text("my_text").ShouldBe(EXPECTED_TEXT);
        new AnsiTextFormatter().Rgb(new HexColor("#0C2238")).Text("my_text").ShouldBe(EXPECTED_TEXT);
    }

    [Fact]
    public void Test_Rgb_HexColor()
    {
        const string SEQUENCE_CONTENT = "38;2;12;34;56";
        const string EXPECTED_TEXT = $"\u001b[{SEQUENCE_CONTENT}mmy_text\u001b[0m";

        TermText.Hex("#0C2238").Text("my_text").ShouldBe(EXPECTED_TEXT);
        new AnsiTextFormatter().Hex("#0C2238").Text("my_text").ShouldBe(EXPECTED_TEXT);
    }

    [Fact]
    public void Test_BgRgb_Components()
    {
        const string SEQUENCE_CONTENT = "48;2;12;34;56";
        const string EXPECTED_TEXT = $"\u001b[{SEQUENCE_CONTENT}mmy_text\u001b[0m";

        TermText.BgRgb(12, 34, 56).Text("my_text").ShouldBe(EXPECTED_TEXT);
        new AnsiTextFormatter().BgRgb(12, 34, 56).Text("my_text").ShouldBe(EXPECTED_TEXT);
    }

    [Fact]
    public void Test_BgRgb_Color()
    {
        const string SEQUENCE_CONTENT = "48;2;12;34;56";
        const string EXPECTED_TEXT = $"\u001b[{SEQUENCE_CONTENT}mmy_text\u001b[0m";

        TermText.BgRgb(new RgbColor(12, 34, 56)).Text("my_text").ShouldBe(EXPECTED_TEXT);
        new AnsiTextFormatter().BgRgb(new RgbColor(12, 34, 56)).Text("my_text").ShouldBe(EXPECTED_TEXT);
    }

    [Fact]
    public void Test_BgRgb_GenericColor()
    {
        const string SEQUENCE_CONTENT = "48;2;12;34;56";
        const string EXPECTED_TEXT = $"\u001b[{SEQUENCE_CONTENT}mmy_text\u001b[0m";

        TermText.BgRgb(new HexColor("#0C2238")).Text("my_text").ShouldBe(EXPECTED_TEXT);
        new AnsiTextFormatter().BgRgb(new HexColor("#0C2238")).Text("my_text").ShouldBe(EXPECTED_TEXT);
    }

    [Fact]
    public void Test_BgRgb_HexColor()
    {
        const string SEQUENCE_CONTENT = "48;2;12;34;56";
        const string EXPECTED_TEXT = $"\u001b[{SEQUENCE_CONTENT}mmy_text\u001b[0m";

        TermText.BgHex("#0C2238").Text("my_text").ShouldBe(EXPECTED_TEXT);
        new AnsiTextFormatter().BgHex("#0C2238").Text("my_text").ShouldBe(EXPECTED_TEXT);
    }

    #endregion RGB colors

    #region 4 bit colors

    [Fact]
    public void Test_Black()
    {
        const int ANSI_NUMBER = 30;
        string expectedText = $"\u001b[{ANSI_NUMBER}mmy_text\u001b[0m";

        TermText.Black().Text("my_text").ShouldBe(expectedText);
        TermText.Black("my_text").ShouldBe(expectedText);
        TermText.In(ConsoleColor.Black).Text("my_text").ShouldBe(expectedText);

        new AnsiTextFormatter().Black().Text("my_text").ShouldBe(expectedText);
        new AnsiTextFormatter().Black("my_text").ShouldBe(expectedText);
        new AnsiTextFormatter().In(ConsoleColor.Black).Text("my_text").ShouldBe(expectedText);
    }

    [Fact]
    public void Test_BgBlack()
    {
        const int ANSI_NUMBER = 40;
        string expectedText = $"\u001b[{ANSI_NUMBER}mmy_text\u001b[0m";

        TermText.BgBlack().Text("my_text").ShouldBe(expectedText);
        TermText.Bg(ConsoleColor.Black).Text("my_text").ShouldBe(expectedText);

        new AnsiTextFormatter().BgBlack().Text("my_text").ShouldBe(expectedText);
        new AnsiTextFormatter().Bg(ConsoleColor.Black).Text("my_text").ShouldBe(expectedText);
    }

    [Fact]
    public void Test_Gray()
    {
        const int ANSI_NUMBER = 37;
        string expectedText = $"\u001b[{ANSI_NUMBER}mmy_text\u001b[0m";

        TermText.Gray().Text("my_text").ShouldBe(expectedText);
        TermText.Gray("my_text").ShouldBe(expectedText);
        TermText.In(ConsoleColor.Gray).Text("my_text").ShouldBe(expectedText);

        new AnsiTextFormatter().Gray().Text("my_text").ShouldBe(expectedText);
        new AnsiTextFormatter().Gray("my_text").ShouldBe(expectedText);
        new AnsiTextFormatter().In(ConsoleColor.Gray).Text("my_text").ShouldBe(expectedText);
    }

    [Fact]
    public void Test_BgGray()
    {
        const int ANSI_NUMBER = 47;
        string expectedText = $"\u001b[{ANSI_NUMBER}mmy_text\u001b[0m";

        TermText.BgGray().Text("my_text").ShouldBe(expectedText);
        TermText.Bg(ConsoleColor.Gray).Text("my_text").ShouldBe(expectedText);

        new AnsiTextFormatter().BgGray().Text("my_text").ShouldBe(expectedText);
        new AnsiTextFormatter().Bg(ConsoleColor.Gray).Text("my_text").ShouldBe(expectedText);
    }

    [Fact]
    public void Test_DarkGray()
    {
        const int ANSI_NUMBER = 90;
        string expectedText = $"\u001b[{ANSI_NUMBER}mmy_text\u001b[0m";

        TermText.DarkGray().Text("my_text").ShouldBe(expectedText);
        TermText.DarkGray("my_text").ShouldBe(expectedText);
        TermText.In(ConsoleColor.DarkGray).Text("my_text").ShouldBe(expectedText);

        new AnsiTextFormatter().DarkGray().Text("my_text").ShouldBe(expectedText);
        new AnsiTextFormatter().DarkGray("my_text").ShouldBe(expectedText);
        new AnsiTextFormatter().In(ConsoleColor.DarkGray).Text("my_text").ShouldBe(expectedText);
    }

    [Fact]
    public void Test_BgDarkGray()
    {
        const int ANSI_NUMBER = 100;
        string expectedText = $"\u001b[{ANSI_NUMBER}mmy_text\u001b[0m";

        TermText.BgDarkGray().Text("my_text").ShouldBe(expectedText);
        TermText.Bg(ConsoleColor.DarkGray).Text("my_text").ShouldBe(expectedText);

        new AnsiTextFormatter().BgDarkGray().Text("my_text").ShouldBe(expectedText);
        new AnsiTextFormatter().Bg(ConsoleColor.DarkGray).Text("my_text").ShouldBe(expectedText);
    }

    [Fact]
    public void Test_Blue()
    {
        const int ANSI_NUMBER = 94;
        string expectedText = $"\u001b[{ANSI_NUMBER}mmy_text\u001b[0m";

        TermText.Blue().Text("my_text").ShouldBe(expectedText);
        TermText.Blue("my_text").ShouldBe(expectedText);
        TermText.In(ConsoleColor.Blue).Text("my_text").ShouldBe(expectedText);

        new AnsiTextFormatter().Blue().Text("my_text").ShouldBe(expectedText);
        new AnsiTextFormatter().Blue("my_text").ShouldBe(expectedText);
        new AnsiTextFormatter().In(ConsoleColor.Blue).Text("my_text").ShouldBe(expectedText);
    }

    [Fact]
    public void Test_BgBlue()
    {
        const int ANSI_NUMBER = 104;
        string expectedText = $"\u001b[{ANSI_NUMBER}mmy_text\u001b[0m";

        TermText.BgBlue().Text("my_text").ShouldBe(expectedText);
        TermText.Bg(ConsoleColor.Blue).Text("my_text").ShouldBe(expectedText);

        new AnsiTextFormatter().BgBlue().Text("my_text").ShouldBe(expectedText);
        new AnsiTextFormatter().Bg(ConsoleColor.Blue).Text("my_text").ShouldBe(expectedText);
    }

    [Fact]
    public void Test_DarkBlue()
    {
        const int ANSI_NUMBER = 34;
        string expectedText = $"\u001b[{ANSI_NUMBER}mmy_text\u001b[0m";

        TermText.DarkBlue().Text("my_text").ShouldBe(expectedText);
        TermText.DarkBlue("my_text").ShouldBe(expectedText);
        TermText.In(ConsoleColor.DarkBlue).Text("my_text").ShouldBe(expectedText);

        new AnsiTextFormatter().DarkBlue().Text("my_text").ShouldBe(expectedText);
        new AnsiTextFormatter().DarkBlue("my_text").ShouldBe(expectedText);
        new AnsiTextFormatter().In(ConsoleColor.DarkBlue).Text("my_text").ShouldBe(expectedText);
    }

    [Fact]
    public void Test_BgDarkBlue()
    {
        const int ANSI_NUMBER = 44;
        string expectedText = $"\u001b[{ANSI_NUMBER}mmy_text\u001b[0m";

        TermText.BgDarkBlue().Text("my_text").ShouldBe(expectedText);
        TermText.Bg(ConsoleColor.DarkBlue).Text("my_text").ShouldBe(expectedText);

        new AnsiTextFormatter().BgDarkBlue().Text("my_text").ShouldBe(expectedText);
        new AnsiTextFormatter().Bg(ConsoleColor.DarkBlue).Text("my_text").ShouldBe(expectedText);
    }

    [Fact]
    public void Test_Green()
    {
        const int ANSI_NUMBER = 92;
        string expectedText = $"\u001b[{ANSI_NUMBER}mmy_text\u001b[0m";

        TermText.Green().Text("my_text").ShouldBe(expectedText);
        TermText.Green("my_text").ShouldBe(expectedText);
        TermText.In(ConsoleColor.Green).Text("my_text").ShouldBe(expectedText);

        new AnsiTextFormatter().Green().Text("my_text").ShouldBe(expectedText);
        new AnsiTextFormatter().Green("my_text").ShouldBe(expectedText);
        new AnsiTextFormatter().In(ConsoleColor.Green).Text("my_text").ShouldBe(expectedText);
    }

    [Fact]
    public void Test_BgGreen()
    {
        const int ANSI_NUMBER = 102;
        string expectedText = $"\u001b[{ANSI_NUMBER}mmy_text\u001b[0m";

        TermText.BgGreen().Text("my_text").ShouldBe(expectedText);
        TermText.Bg(ConsoleColor.Green).Text("my_text").ShouldBe(expectedText);

        new AnsiTextFormatter().BgGreen().Text("my_text").ShouldBe(expectedText);
        new AnsiTextFormatter().Bg(ConsoleColor.Green).Text("my_text").ShouldBe(expectedText);
    }

    [Fact]
    public void Test_DarkGreen()
    {
        const int ANSI_NUMBER = 32;
        string expectedText = $"\u001b[{ANSI_NUMBER}mmy_text\u001b[0m";

        TermText.DarkGreen().Text("my_text").ShouldBe(expectedText);
        TermText.DarkGreen("my_text").ShouldBe(expectedText);
        TermText.In(ConsoleColor.DarkGreen).Text("my_text").ShouldBe(expectedText);

        new AnsiTextFormatter().DarkGreen().Text("my_text").ShouldBe(expectedText);
        new AnsiTextFormatter().DarkGreen("my_text").ShouldBe(expectedText);
        new AnsiTextFormatter().In(ConsoleColor.DarkGreen).Text("my_text").ShouldBe(expectedText);
    }

    [Fact]
    public void Test_BgDarkGreen()
    {
        const int ANSI_NUMBER = 42;
        string expectedText = $"\u001b[{ANSI_NUMBER}mmy_text\u001b[0m";

        TermText.BgDarkGreen().Text("my_text").ShouldBe(expectedText);
        TermText.Bg(ConsoleColor.DarkGreen).Text("my_text").ShouldBe(expectedText);

        new AnsiTextFormatter().BgDarkGreen().Text("my_text").ShouldBe(expectedText);
        new AnsiTextFormatter().Bg(ConsoleColor.DarkGreen).Text("my_text").ShouldBe(expectedText);
    }

    [Fact]
    public void Test_Cyan()
    {
        const int ANSI_NUMBER = 96;
        string expectedText = $"\u001b[{ANSI_NUMBER}mmy_text\u001b[0m";

        TermText.Cyan().Text("my_text").ShouldBe(expectedText);
        TermText.Cyan("my_text").ShouldBe(expectedText);
        TermText.In(ConsoleColor.Cyan).Text("my_text").ShouldBe(expectedText);

        new AnsiTextFormatter().Cyan().Text("my_text").ShouldBe(expectedText);
        new AnsiTextFormatter().Cyan("my_text").ShouldBe(expectedText);
        new AnsiTextFormatter().In(ConsoleColor.Cyan).Text("my_text").ShouldBe(expectedText);
    }

    [Fact]
    public void Test_BgCyan()
    {
        const int ANSI_NUMBER = 106;
        string expectedText = $"\u001b[{ANSI_NUMBER}mmy_text\u001b[0m";

        TermText.BgCyan().Text("my_text").ShouldBe(expectedText);
        TermText.Bg(ConsoleColor.Cyan).Text("my_text").ShouldBe(expectedText);

        new AnsiTextFormatter().BgCyan().Text("my_text").ShouldBe(expectedText);
        new AnsiTextFormatter().Bg(ConsoleColor.Cyan).Text("my_text").ShouldBe(expectedText);
    }

    [Fact]
    public void Test_DarkCyan()
    {
        const int ANSI_NUMBER = 36;
        string expectedText = $"\u001b[{ANSI_NUMBER}mmy_text\u001b[0m";

        TermText.DarkCyan().Text("my_text").ShouldBe(expectedText);
        TermText.DarkCyan("my_text").ShouldBe(expectedText);
        TermText.In(ConsoleColor.DarkCyan).Text("my_text").ShouldBe(expectedText);

        new AnsiTextFormatter().DarkCyan().Text("my_text").ShouldBe(expectedText);
        new AnsiTextFormatter().DarkCyan("my_text").ShouldBe(expectedText);
        new AnsiTextFormatter().In(ConsoleColor.DarkCyan).Text("my_text").ShouldBe(expectedText);
    }

    [Fact]
    public void Test_BgDarkCyan()
    {
        const int ANSI_NUMBER = 46;
        string expectedText = $"\u001b[{ANSI_NUMBER}mmy_text\u001b[0m";

        TermText.BgDarkCyan().Text("my_text").ShouldBe(expectedText);
        TermText.Bg(ConsoleColor.DarkCyan).Text("my_text").ShouldBe(expectedText);

        new AnsiTextFormatter().BgDarkCyan().Text("my_text").ShouldBe(expectedText);
        new AnsiTextFormatter().Bg(ConsoleColor.DarkCyan).Text("my_text").ShouldBe(expectedText);
    }

    [Fact]
    public void Test_Red()
    {
        const int ANSI_NUMBER = 91;
        string expectedText = $"\u001b[{ANSI_NUMBER}mmy_text\u001b[0m";

        TermText.Red().Text("my_text").ShouldBe(expectedText);
        TermText.Red("my_text").ShouldBe(expectedText);
        TermText.In(ConsoleColor.Red).Text("my_text").ShouldBe(expectedText);

        new AnsiTextFormatter().Red().Text("my_text").ShouldBe(expectedText);
        new AnsiTextFormatter().Red("my_text").ShouldBe(expectedText);
        new AnsiTextFormatter().In(ConsoleColor.Red).Text("my_text").ShouldBe(expectedText);
    }

    [Fact]
    public void Test_BgRed()
    {
        const int ANSI_NUMBER = 101;
        string expectedText = $"\u001b[{ANSI_NUMBER}mmy_text\u001b[0m";

        TermText.BgRed().Text("my_text").ShouldBe(expectedText);
        TermText.Bg(ConsoleColor.Red).Text("my_text").ShouldBe(expectedText);

        new AnsiTextFormatter().BgRed().Text("my_text").ShouldBe(expectedText);
        new AnsiTextFormatter().Bg(ConsoleColor.Red).Text("my_text").ShouldBe(expectedText);
    }

    [Fact]
    public void Test_DarkRed()
    {
        const int ANSI_NUMBER = 31;
        string expectedText = $"\u001b[{ANSI_NUMBER}mmy_text\u001b[0m";

        TermText.DarkRed().Text("my_text").ShouldBe(expectedText);
        TermText.DarkRed("my_text").ShouldBe(expectedText);
        TermText.In(ConsoleColor.DarkRed).Text("my_text").ShouldBe(expectedText);

        new AnsiTextFormatter().DarkRed().Text("my_text").ShouldBe(expectedText);
        new AnsiTextFormatter().DarkRed("my_text").ShouldBe(expectedText);
        new AnsiTextFormatter().In(ConsoleColor.DarkRed).Text("my_text").ShouldBe(expectedText);
    }

    [Fact]
    public void Test_BgDarkRed()
    {
        const int ANSI_NUMBER = 41;
        string expectedText = $"\u001b[{ANSI_NUMBER}mmy_text\u001b[0m";

        TermText.BgDarkRed().Text("my_text").ShouldBe(expectedText);
        TermText.Bg(ConsoleColor.DarkRed).Text("my_text").ShouldBe(expectedText);

        new AnsiTextFormatter().BgDarkRed().Text("my_text").ShouldBe(expectedText);
        new AnsiTextFormatter().Bg(ConsoleColor.DarkRed).Text("my_text").ShouldBe(expectedText);
    }

    [Fact]
    public void Test_Magenta()
    {
        const int ANSI_NUMBER = 95;
        string expectedText = $"\u001b[{ANSI_NUMBER}mmy_text\u001b[0m";

        TermText.Magenta().Text("my_text").ShouldBe(expectedText);
        TermText.Magenta("my_text").ShouldBe(expectedText);
        TermText.In(ConsoleColor.Magenta).Text("my_text").ShouldBe(expectedText);

        new AnsiTextFormatter().Magenta().Text("my_text").ShouldBe(expectedText);
        new AnsiTextFormatter().Magenta("my_text").ShouldBe(expectedText);
        new AnsiTextFormatter().In(ConsoleColor.Magenta).Text("my_text").ShouldBe(expectedText);
    }

    [Fact]
    public void Test_BgMagenta()
    {
        const int ANSI_NUMBER = 105;
        string expectedText = $"\u001b[{ANSI_NUMBER}mmy_text\u001b[0m";

        TermText.BgMagenta().Text("my_text").ShouldBe(expectedText);
        TermText.Bg(ConsoleColor.Magenta).Text("my_text").ShouldBe(expectedText);

        new AnsiTextFormatter().BgMagenta().Text("my_text").ShouldBe(expectedText);
        new AnsiTextFormatter().Bg(ConsoleColor.Magenta).Text("my_text").ShouldBe(expectedText);
    }

    [Fact]
    public void Test_DarkMagenta()
    {
        const int ANSI_NUMBER = 35;
        string expectedText = $"\u001b[{ANSI_NUMBER}mmy_text\u001b[0m";

        TermText.DarkMagenta().Text("my_text").ShouldBe(expectedText);
        TermText.DarkMagenta("my_text").ShouldBe(expectedText);
        TermText.In(ConsoleColor.DarkMagenta).Text("my_text").ShouldBe(expectedText);

        new AnsiTextFormatter().DarkMagenta().Text("my_text").ShouldBe(expectedText);
        new AnsiTextFormatter().DarkMagenta("my_text").ShouldBe(expectedText);
        new AnsiTextFormatter().In(ConsoleColor.DarkMagenta).Text("my_text").ShouldBe(expectedText);
    }

    [Fact]
    public void Test_BgDarkMagenta()
    {
        const int ANSI_NUMBER = 45;
        string expectedText = $"\u001b[{ANSI_NUMBER}mmy_text\u001b[0m";

        TermText.BgDarkMagenta().Text("my_text").ShouldBe(expectedText);
        TermText.Bg(ConsoleColor.DarkMagenta).Text("my_text").ShouldBe(expectedText);

        new AnsiTextFormatter().BgDarkMagenta().Text("my_text").ShouldBe(expectedText);
        new AnsiTextFormatter().Bg(ConsoleColor.DarkMagenta).Text("my_text").ShouldBe(expectedText);
    }

    [Fact]
    public void Test_Yellow()
    {
        const int ANSI_NUMBER = 93;
        string expectedText = $"\u001b[{ANSI_NUMBER}mmy_text\u001b[0m";

        TermText.Yellow().Text("my_text").ShouldBe(expectedText);
        TermText.Yellow("my_text").ShouldBe(expectedText);
        TermText.In(ConsoleColor.Yellow).Text("my_text").ShouldBe(expectedText);

        new AnsiTextFormatter().Yellow().Text("my_text").ShouldBe(expectedText);
        new AnsiTextFormatter().Yellow("my_text").ShouldBe(expectedText);
        new AnsiTextFormatter().In(ConsoleColor.Yellow).Text("my_text").ShouldBe(expectedText);
    }

    [Fact]
    public void Test_BgYellow()
    {
        const int ANSI_NUMBER = 103;
        string expectedText = $"\u001b[{ANSI_NUMBER}mmy_text\u001b[0m";

        TermText.BgYellow().Text("my_text").ShouldBe(expectedText);
        TermText.Bg(ConsoleColor.Yellow).Text("my_text").ShouldBe(expectedText);

        new AnsiTextFormatter().BgYellow().Text("my_text").ShouldBe(expectedText);
        new AnsiTextFormatter().Bg(ConsoleColor.Yellow).Text("my_text").ShouldBe(expectedText);
    }

    [Fact]
    public void Test_DarkYellow()
    {
        const int ANSI_NUMBER = 33;
        string expectedText = $"\u001b[{ANSI_NUMBER}mmy_text\u001b[0m";

        TermText.DarkYellow().Text("my_text").ShouldBe(expectedText);
        TermText.DarkYellow("my_text").ShouldBe(expectedText);
        TermText.In(ConsoleColor.DarkYellow).Text("my_text").ShouldBe(expectedText);

        new AnsiTextFormatter().DarkYellow().Text("my_text").ShouldBe(expectedText);
        new AnsiTextFormatter().DarkYellow("my_text").ShouldBe(expectedText);
        new AnsiTextFormatter().In(ConsoleColor.DarkYellow).Text("my_text").ShouldBe(expectedText);
    }

    [Fact]
    public void Test_BgDarkYellow()
    {
        const int ANSI_NUMBER = 43;
        string expectedText = $"\u001b[{ANSI_NUMBER}mmy_text\u001b[0m";

        TermText.BgDarkYellow().Text("my_text").ShouldBe(expectedText);
        TermText.Bg(ConsoleColor.DarkYellow).Text("my_text").ShouldBe(expectedText);

        new AnsiTextFormatter().BgDarkYellow().Text("my_text").ShouldBe(expectedText);
        new AnsiTextFormatter().Bg(ConsoleColor.DarkYellow).Text("my_text").ShouldBe(expectedText);
    }

    [Fact]
    public void Test_White()
    {
        const int ANSI_NUMBER = 97;
        string expectedText = $"\u001b[{ANSI_NUMBER}mmy_text\u001b[0m";

        TermText.White().Text("my_text").ShouldBe(expectedText);
        TermText.White("my_text").ShouldBe(expectedText);
        TermText.In(ConsoleColor.White).Text("my_text").ShouldBe(expectedText);

        new AnsiTextFormatter().White().Text("my_text").ShouldBe(expectedText);
        new AnsiTextFormatter().White("my_text").ShouldBe(expectedText);
        new AnsiTextFormatter().In(ConsoleColor.White).Text("my_text").ShouldBe(expectedText);
    }

    [Fact]
    public void Test_BgWhite()
    {
        const int ANSI_NUMBER = 107;
        string expectedText = $"\u001b[{ANSI_NUMBER}mmy_text\u001b[0m";

        TermText.BgWhite().Text("my_text").ShouldBe(expectedText);
        TermText.Bg(ConsoleColor.White).Text("my_text").ShouldBe(expectedText);

        new AnsiTextFormatter().BgWhite().Text("my_text").ShouldBe(expectedText);
        new AnsiTextFormatter().Bg(ConsoleColor.White).Text("my_text").ShouldBe(expectedText);
    }

    #endregion 4 bit colors
}

//
// NOTE: This file has been AUTOMATICALLY GENERATED from 'AnsiTextFormatter.4BitColors.cs.mustache'. Any changes made to
//   this file will be LOST on the next build.
//

// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using JetBrains.Annotations;

namespace AppMotor.CliApp.Terminals.Formatting;

public readonly partial struct AnsiTextFormatter
{
    #region Black

    /// <summary>
    /// Chains style: text color black
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    /// <remarks>
    /// The exact color used is up to the terminal and can often be customized by the user.
    /// </remarks>
    [MustUseReturnValue]
    public AnsiTextFormatter Black() => In(ConsoleColor.Black);

    /// <summary>
    /// Returns the specified text with the current style plus: text color black
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    /// <remarks>
    /// The exact color used is up to the terminal and can often be customized by the user.
    /// </remarks>
    [MustUseReturnValue]
    public string Black(string text) => In(ConsoleColor.Black).Text(text);

    /// <summary>
    /// Chains style: background color black
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    /// <remarks>
    /// The exact color used is up to the terminal and can often be customized by the user.
    /// </remarks>
    [MustUseReturnValue]
    public AnsiTextFormatter BgBlack() => Bg(ConsoleColor.Black);

    #endregion Black

    #region Gray

    /// <summary>
    /// Chains style: text color gray
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    /// <remarks>
    /// The exact color used is up to the terminal and can often be customized by the user.
    /// </remarks>
    [MustUseReturnValue]
    public AnsiTextFormatter Gray() => In(ConsoleColor.Gray);

    /// <summary>
    /// Returns the specified text with the current style plus: text color gray
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    /// <remarks>
    /// The exact color used is up to the terminal and can often be customized by the user.
    /// </remarks>
    [MustUseReturnValue]
    public string Gray(string text) => In(ConsoleColor.Gray).Text(text);

    /// <summary>
    /// Chains style: background color gray
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    /// <remarks>
    /// The exact color used is up to the terminal and can often be customized by the user.
    /// </remarks>
    [MustUseReturnValue]
    public AnsiTextFormatter BgGray() => Bg(ConsoleColor.Gray);

    #endregion Gray

    #region DarkGray

    /// <summary>
    /// Chains style: text color dark gray
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    /// <remarks>
    /// The exact color used is up to the terminal and can often be customized by the user.
    /// </remarks>
    [MustUseReturnValue]
    public AnsiTextFormatter DarkGray() => In(ConsoleColor.DarkGray);

    /// <summary>
    /// Returns the specified text with the current style plus: text color dark gray
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    /// <remarks>
    /// The exact color used is up to the terminal and can often be customized by the user.
    /// </remarks>
    [MustUseReturnValue]
    public string DarkGray(string text) => In(ConsoleColor.DarkGray).Text(text);

    /// <summary>
    /// Chains style: background color dark gray
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    /// <remarks>
    /// The exact color used is up to the terminal and can often be customized by the user.
    /// </remarks>
    [MustUseReturnValue]
    public AnsiTextFormatter BgDarkGray() => Bg(ConsoleColor.DarkGray);

    #endregion DarkGray

    #region Blue

    /// <summary>
    /// Chains style: text color blue
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    /// <remarks>
    /// The exact color used is up to the terminal and can often be customized by the user.
    /// </remarks>
    [MustUseReturnValue]
    public AnsiTextFormatter Blue() => In(ConsoleColor.Blue);

    /// <summary>
    /// Returns the specified text with the current style plus: text color blue
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    /// <remarks>
    /// The exact color used is up to the terminal and can often be customized by the user.
    /// </remarks>
    [MustUseReturnValue]
    public string Blue(string text) => In(ConsoleColor.Blue).Text(text);

    /// <summary>
    /// Chains style: background color blue
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    /// <remarks>
    /// The exact color used is up to the terminal and can often be customized by the user.
    /// </remarks>
    [MustUseReturnValue]
    public AnsiTextFormatter BgBlue() => Bg(ConsoleColor.Blue);

    #endregion Blue

    #region DarkBlue

    /// <summary>
    /// Chains style: text color dark blue
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    /// <remarks>
    /// The exact color used is up to the terminal and can often be customized by the user.
    /// </remarks>
    [MustUseReturnValue]
    public AnsiTextFormatter DarkBlue() => In(ConsoleColor.DarkBlue);

    /// <summary>
    /// Returns the specified text with the current style plus: text color dark blue
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    /// <remarks>
    /// The exact color used is up to the terminal and can often be customized by the user.
    /// </remarks>
    [MustUseReturnValue]
    public string DarkBlue(string text) => In(ConsoleColor.DarkBlue).Text(text);

    /// <summary>
    /// Chains style: background color dark blue
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    /// <remarks>
    /// The exact color used is up to the terminal and can often be customized by the user.
    /// </remarks>
    [MustUseReturnValue]
    public AnsiTextFormatter BgDarkBlue() => Bg(ConsoleColor.DarkBlue);

    #endregion DarkBlue

    #region Green

    /// <summary>
    /// Chains style: text color green
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    /// <remarks>
    /// The exact color used is up to the terminal and can often be customized by the user.
    /// </remarks>
    [MustUseReturnValue]
    public AnsiTextFormatter Green() => In(ConsoleColor.Green);

    /// <summary>
    /// Returns the specified text with the current style plus: text color green
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    /// <remarks>
    /// The exact color used is up to the terminal and can often be customized by the user.
    /// </remarks>
    [MustUseReturnValue]
    public string Green(string text) => In(ConsoleColor.Green).Text(text);

    /// <summary>
    /// Chains style: background color green
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    /// <remarks>
    /// The exact color used is up to the terminal and can often be customized by the user.
    /// </remarks>
    [MustUseReturnValue]
    public AnsiTextFormatter BgGreen() => Bg(ConsoleColor.Green);

    #endregion Green

    #region DarkGreen

    /// <summary>
    /// Chains style: text color dark green
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    /// <remarks>
    /// The exact color used is up to the terminal and can often be customized by the user.
    /// </remarks>
    [MustUseReturnValue]
    public AnsiTextFormatter DarkGreen() => In(ConsoleColor.DarkGreen);

    /// <summary>
    /// Returns the specified text with the current style plus: text color dark green
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    /// <remarks>
    /// The exact color used is up to the terminal and can often be customized by the user.
    /// </remarks>
    [MustUseReturnValue]
    public string DarkGreen(string text) => In(ConsoleColor.DarkGreen).Text(text);

    /// <summary>
    /// Chains style: background color dark green
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    /// <remarks>
    /// The exact color used is up to the terminal and can often be customized by the user.
    /// </remarks>
    [MustUseReturnValue]
    public AnsiTextFormatter BgDarkGreen() => Bg(ConsoleColor.DarkGreen);

    #endregion DarkGreen

    #region Cyan

    /// <summary>
    /// Chains style: text color cyan
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    /// <remarks>
    /// The exact color used is up to the terminal and can often be customized by the user.
    /// </remarks>
    [MustUseReturnValue]
    public AnsiTextFormatter Cyan() => In(ConsoleColor.Cyan);

    /// <summary>
    /// Returns the specified text with the current style plus: text color cyan
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    /// <remarks>
    /// The exact color used is up to the terminal and can often be customized by the user.
    /// </remarks>
    [MustUseReturnValue]
    public string Cyan(string text) => In(ConsoleColor.Cyan).Text(text);

    /// <summary>
    /// Chains style: background color cyan
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    /// <remarks>
    /// The exact color used is up to the terminal and can often be customized by the user.
    /// </remarks>
    [MustUseReturnValue]
    public AnsiTextFormatter BgCyan() => Bg(ConsoleColor.Cyan);

    #endregion Cyan

    #region DarkCyan

    /// <summary>
    /// Chains style: text color dark cyan
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    /// <remarks>
    /// The exact color used is up to the terminal and can often be customized by the user.
    /// </remarks>
    [MustUseReturnValue]
    public AnsiTextFormatter DarkCyan() => In(ConsoleColor.DarkCyan);

    /// <summary>
    /// Returns the specified text with the current style plus: text color dark cyan
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    /// <remarks>
    /// The exact color used is up to the terminal and can often be customized by the user.
    /// </remarks>
    [MustUseReturnValue]
    public string DarkCyan(string text) => In(ConsoleColor.DarkCyan).Text(text);

    /// <summary>
    /// Chains style: background color dark cyan
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    /// <remarks>
    /// The exact color used is up to the terminal and can often be customized by the user.
    /// </remarks>
    [MustUseReturnValue]
    public AnsiTextFormatter BgDarkCyan() => Bg(ConsoleColor.DarkCyan);

    #endregion DarkCyan

    #region Red

    /// <summary>
    /// Chains style: text color red
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    /// <remarks>
    /// The exact color used is up to the terminal and can often be customized by the user.
    /// </remarks>
    [MustUseReturnValue]
    public AnsiTextFormatter Red() => In(ConsoleColor.Red);

    /// <summary>
    /// Returns the specified text with the current style plus: text color red
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    /// <remarks>
    /// The exact color used is up to the terminal and can often be customized by the user.
    /// </remarks>
    [MustUseReturnValue]
    public string Red(string text) => In(ConsoleColor.Red).Text(text);

    /// <summary>
    /// Chains style: background color red
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    /// <remarks>
    /// The exact color used is up to the terminal and can often be customized by the user.
    /// </remarks>
    [MustUseReturnValue]
    public AnsiTextFormatter BgRed() => Bg(ConsoleColor.Red);

    #endregion Red

    #region DarkRed

    /// <summary>
    /// Chains style: text color dark red
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    /// <remarks>
    /// The exact color used is up to the terminal and can often be customized by the user.
    /// </remarks>
    [MustUseReturnValue]
    public AnsiTextFormatter DarkRed() => In(ConsoleColor.DarkRed);

    /// <summary>
    /// Returns the specified text with the current style plus: text color dark red
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    /// <remarks>
    /// The exact color used is up to the terminal and can often be customized by the user.
    /// </remarks>
    [MustUseReturnValue]
    public string DarkRed(string text) => In(ConsoleColor.DarkRed).Text(text);

    /// <summary>
    /// Chains style: background color dark red
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    /// <remarks>
    /// The exact color used is up to the terminal and can often be customized by the user.
    /// </remarks>
    [MustUseReturnValue]
    public AnsiTextFormatter BgDarkRed() => Bg(ConsoleColor.DarkRed);

    #endregion DarkRed

    #region Magenta

    /// <summary>
    /// Chains style: text color magenta
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    /// <remarks>
    /// The exact color used is up to the terminal and can often be customized by the user.
    /// </remarks>
    [MustUseReturnValue]
    public AnsiTextFormatter Magenta() => In(ConsoleColor.Magenta);

    /// <summary>
    /// Returns the specified text with the current style plus: text color magenta
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    /// <remarks>
    /// The exact color used is up to the terminal and can often be customized by the user.
    /// </remarks>
    [MustUseReturnValue]
    public string Magenta(string text) => In(ConsoleColor.Magenta).Text(text);

    /// <summary>
    /// Chains style: background color magenta
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    /// <remarks>
    /// The exact color used is up to the terminal and can often be customized by the user.
    /// </remarks>
    [MustUseReturnValue]
    public AnsiTextFormatter BgMagenta() => Bg(ConsoleColor.Magenta);

    #endregion Magenta

    #region DarkMagenta

    /// <summary>
    /// Chains style: text color dark magenta
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    /// <remarks>
    /// The exact color used is up to the terminal and can often be customized by the user.
    /// </remarks>
    [MustUseReturnValue]
    public AnsiTextFormatter DarkMagenta() => In(ConsoleColor.DarkMagenta);

    /// <summary>
    /// Returns the specified text with the current style plus: text color dark magenta
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    /// <remarks>
    /// The exact color used is up to the terminal and can often be customized by the user.
    /// </remarks>
    [MustUseReturnValue]
    public string DarkMagenta(string text) => In(ConsoleColor.DarkMagenta).Text(text);

    /// <summary>
    /// Chains style: background color dark magenta
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    /// <remarks>
    /// The exact color used is up to the terminal and can often be customized by the user.
    /// </remarks>
    [MustUseReturnValue]
    public AnsiTextFormatter BgDarkMagenta() => Bg(ConsoleColor.DarkMagenta);

    #endregion DarkMagenta

    #region Yellow

    /// <summary>
    /// Chains style: text color yellow
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    /// <remarks>
    /// The exact color used is up to the terminal and can often be customized by the user.
    /// </remarks>
    [MustUseReturnValue]
    public AnsiTextFormatter Yellow() => In(ConsoleColor.Yellow);

    /// <summary>
    /// Returns the specified text with the current style plus: text color yellow
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    /// <remarks>
    /// The exact color used is up to the terminal and can often be customized by the user.
    /// </remarks>
    [MustUseReturnValue]
    public string Yellow(string text) => In(ConsoleColor.Yellow).Text(text);

    /// <summary>
    /// Chains style: background color yellow
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    /// <remarks>
    /// The exact color used is up to the terminal and can often be customized by the user.
    /// </remarks>
    [MustUseReturnValue]
    public AnsiTextFormatter BgYellow() => Bg(ConsoleColor.Yellow);

    #endregion Yellow

    #region DarkYellow

    /// <summary>
    /// Chains style: text color dark yellow
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    /// <remarks>
    /// The exact color used is up to the terminal and can often be customized by the user.
    /// </remarks>
    [MustUseReturnValue]
    public AnsiTextFormatter DarkYellow() => In(ConsoleColor.DarkYellow);

    /// <summary>
    /// Returns the specified text with the current style plus: text color dark yellow
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    /// <remarks>
    /// The exact color used is up to the terminal and can often be customized by the user.
    /// </remarks>
    [MustUseReturnValue]
    public string DarkYellow(string text) => In(ConsoleColor.DarkYellow).Text(text);

    /// <summary>
    /// Chains style: background color dark yellow
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    /// <remarks>
    /// The exact color used is up to the terminal and can often be customized by the user.
    /// </remarks>
    [MustUseReturnValue]
    public AnsiTextFormatter BgDarkYellow() => Bg(ConsoleColor.DarkYellow);

    #endregion DarkYellow

    #region White

    /// <summary>
    /// Chains style: text color white
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    /// <remarks>
    /// The exact color used is up to the terminal and can often be customized by the user.
    /// </remarks>
    [MustUseReturnValue]
    public AnsiTextFormatter White() => In(ConsoleColor.White);

    /// <summary>
    /// Returns the specified text with the current style plus: text color white
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    /// <remarks>
    /// The exact color used is up to the terminal and can often be customized by the user.
    /// </remarks>
    [MustUseReturnValue]
    public string White(string text) => In(ConsoleColor.White).Text(text);

    /// <summary>
    /// Chains style: background color white
    /// </summary>
    /// <remarks>
    /// See <see href="TermText"/> for more details.
    /// </remarks>
    /// <remarks>
    /// The exact color used is up to the terminal and can often be customized by the user.
    /// </remarks>
    [MustUseReturnValue]
    public AnsiTextFormatter BgWhite() => Bg(ConsoleColor.White);

    #endregion White

}

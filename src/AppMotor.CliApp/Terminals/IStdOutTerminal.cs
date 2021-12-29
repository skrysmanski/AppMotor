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
using System.ComponentModel;
using System.IO;

using AppMotor.Core.Extensions;

using JetBrains.Annotations;

namespace AppMotor.CliApp.Terminals;

/// <summary>
/// Represents the standard output (i.e. analog to <see cref="Console.Out"/>) of a terminal.
/// </summary>
/// <seealso cref="ITerminal"/>
/// <seealso cref="IOutputTerminal"/>
/// <seealso cref="IErrorOutTerminal"/>
public interface IStdOutTerminal
{
    /// <summary>
    /// The standard out stream.
    /// </summary>
    [PublicAPI]
    TextWriter Out { get; }

    /// <summary>
    /// Whether <see cref="Out"/> is redirected (to a file or the input
    /// of another process).
    /// </summary>
    [PublicAPI]
    bool IsOutputRedirected { get; }

    /// <summary>
    /// The background color of the terminal.
    /// </summary>
    /// <remarks>
    /// To set the foreground (text) color, use <see cref="Write(ColoredString)"/>
    /// or <see cref="WriteLine(ColoredString)"/>.
    /// </remarks>
    [PublicAPI]
    ConsoleColor BackgroundColor { get; set; }

    /// <summary>
    /// Writes the specified object to the terminal's standard output.
    /// </summary>
    [PublicAPI]
    void Write([Localizable(true)] object? value)
    {
        this.Out.Write(value);
    }

    /// <summary>
    /// Writes the specified string to the terminal's standard output.
    /// </summary>
    [PublicAPI]
    void Write([Localizable(true)] string? value)
    {
        this.Out.Write(value);
    }

    /// <summary>
    /// Formats <paramref name="format"/> with <paramref name="args"/> and writes the result
    /// to the terminal's standard output.
    /// </summary>
    [PublicAPI]
    [StringFormatMethod("format")]
    void Write([Localizable(true)] string format, params object[] args)
    {
        Write(format.With(args));
    }

    /// <summary>
    /// Writes the specified colored string to the terminal's standard output.
    /// </summary>
    [PublicAPI]
    void Write(ColoredString? coloredString);

    /// <summary>
    /// Writes the specified object to the terminal's standard output
    /// and appends a line break at the end.
    /// </summary>
    [PublicAPI]
    void WriteLine([Localizable(true)] object? value)
    {
        this.Out.WriteLine(value);
    }

    /// <summary>
    /// Writes the specified string to the terminal's standard output
    /// and appends a line break at the end.
    /// </summary>
    [PublicAPI]
    void WriteLine([Localizable(true)] string? value)
    {
        this.Out.WriteLine(value);
    }

    /// <summary>
    /// Formats <paramref name="format"/> with <paramref name="args"/> and writes the result
    /// to the terminal's standard output and appends a line break at the end.
    /// </summary>
    [PublicAPI]
    [StringFormatMethod("format")]
    void WriteLine([Localizable(true)] string format, params object[] args)
    {
        WriteLine(format.With(args));
    }

    /// <summary>
    /// Writes the specified colored string to the terminal's standard output
    /// and appends a line break at the end.
    /// </summary>
    [PublicAPI]
    void WriteLine(ColoredString? coloredString)
    {
        Write(coloredString);
        WriteLine();
    }

    /// <summary>
    /// Writes a line break to the terminal's standard output.
    /// </summary>
    [PublicAPI]
    void WriteLine()
    {
        this.Out.WriteLine();
    }
}
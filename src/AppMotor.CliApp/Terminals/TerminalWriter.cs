// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Globalization;

using AppMotor.Core.Globalization;

namespace AppMotor.CliApp.Terminals;

public class TerminalWriter : ITerminalWriter
{
    public delegate void WriteFunc(string? value);

    /// <inheritdoc />
    public string NewLine { get; set; } = Environment.NewLine;

    /// <inheritdoc />
    public CultureInfo Culture { get; set; } = UICulture.FormatsAndSorting;

    private readonly WriteFunc _writeFunc;

    private readonly object _writeLock = new();

    public TerminalWriter(WriteFunc writeFunc)
    {
        this._writeFunc = writeFunc;
    }

    /// <inheritdoc />
    public void Write(string? value)
    {
        lock (this._writeLock)
        {
            this._writeFunc(value);
        }
    }
}

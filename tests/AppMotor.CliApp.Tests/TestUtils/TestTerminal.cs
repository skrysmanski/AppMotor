// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Text;

using AppMotor.CliApp.Terminals;

namespace AppMotor.CliApp.TestUtils;

internal sealed class TestTerminal : ITerminal
{
    /// <inheritdoc />
    public TextReader Input => throw new NotSupportedException();

    /// <inheritdoc />
    public bool IsInputRedirected => true; // tests always run "non-interactive"

    /// <inheritdoc />
    public bool IsKeyAvailable => throw new NotSupportedException();

    /// <inheritdoc />
    public ITerminalWriter Error { get; }

    /// <inheritdoc />
    public bool IsErrorRedirected => throw new NotSupportedException();

    /// <inheritdoc />
    public ITerminalWriter Out { get; }

    private readonly StringBuilder _outWriter = new();

    public string CurrentOutput => this._outWriter.ToString();

    /// <inheritdoc />
    public bool IsOutputRedirected => false;

    public TestTerminal()
    {
        var threadSafeWriter = TextWriter.Synchronized(new StringWriter(this._outWriter));
        this.Out = new TerminalWriter(threadSafeWriter.Write);
        this.Error = new TerminalWriter(threadSafeWriter.Write);
    }

    public void ResetOutput()
    {
        this._outWriter.Clear();
    }

    /// <inheritdoc />
    public ConsoleKeyInfo ReadKey(bool displayPressedKey = true) => throw new NotSupportedException();

    /// <inheritdoc />
    public string ReadLine() => throw new NotSupportedException();
}

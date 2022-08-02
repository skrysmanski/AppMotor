// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Text;

using AppMotor.CliApp;
using AppMotor.CliApp.Terminals;

namespace AppMotor.TestCore.Utils;

/// <summary>
/// Provides an implementation of <see cref="ITerminal"/> that can be used as terminal for
/// <see cref="CliApplication"/>s in tests. The current output of the terminal can be
/// obtained via <see cref="CurrentOutput"/>.
/// </summary>
public sealed class TestTerminal : ITerminal
{
    /// <inheritdoc />
    public TextReader In => TextReader.Null;

    /// <inheritdoc />
    public bool IsInputRedirected => true; // tests always run "non-interactive"

    /// <inheritdoc />
    public bool IsKeyAvailable => throw new InvalidOperationException("Input is redirected.");

    /// <inheritdoc />
    public ITerminalWriter Error { get; }

    /// <inheritdoc />
    public bool IsErrorRedirected => true;

    /// <inheritdoc />
    public ITerminalWriter Out { get; }

    private readonly StringBuilder _outWriter = new();

    /// <summary>
    /// The current output of the terminal. Can be cleared with <see cref="ClearOutput"/>.
    /// </summary>
    public string CurrentOutput => this._outWriter.ToString();

    /// <inheritdoc />
    public bool IsOutputRedirected => true;

    /// <summary>
    /// Constructor.
    /// </summary>
    public TestTerminal()
    {
#pragma warning disable CA2000 // Dispose objects before losing scope -> no need to dispose StringWriter instances
        var threadSafeWriter = TextWriter.Synchronized(new StringWriter(this._outWriter));
#pragma warning restore CA2000 // Dispose objects before losing scope
        this.Out = new SimpleTerminalWriter(threadSafeWriter.Write);
        this.Error = new SimpleTerminalWriter(threadSafeWriter.Write);
    }

    /// <summary>
    /// Clears <see cref="CurrentOutput"/>.
    /// </summary>
    public void ClearOutput()
    {
        this._outWriter.Clear();
    }

    /// <inheritdoc />
    public ConsoleKeyInfo ReadKey(bool displayPressedKey = true) => throw new NotSupportedException();

    /// <inheritdoc />
    public string ReadLine() => throw new NotSupportedException();
}

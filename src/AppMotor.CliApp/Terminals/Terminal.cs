// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

using AppMotor.CliApp.Terminals.Formatting;
using AppMotor.Core.Extensions;

using JetBrains.Annotations;

namespace AppMotor.CliApp.Terminals;

/// <summary>
/// Replacement for <see cref="Console"/> that supports <see cref="TermText"/>.
/// </summary>
[ExcludeFromCodeCoverage]
public static class Terminal
{
    /// <summary>
    /// The terminal as <see cref="ITerminalWindow"/> and <see cref="ITerminal"/>.
    /// </summary>
    [PublicAPI]
    public static ITerminalWindow Instance { get; } = new TerminalAsInstance();

    /// <summary>
    /// The standard input stream.
    /// </summary>
    [PublicAPI]
    public static TextReader In => Console.In;

    /// <summary>
    /// Whether <see cref="In"/> is redirected (to a file or the output
    /// of another process). If this property is <c>true</c>, some members
    /// of this class won't work anymore (<see cref="IsKeyAvailable"/>,
    /// <see cref="ReadKey"/>). Also, <see cref="ReadLine"/> can return
    /// <c>null</c> in this case.
    /// </summary>
    /// <remarks>
    /// You MAY interpret a value of <c>true</c> as "the process is non-interactive".
    /// It's not 100% correct (because the input source may still be interactive)
    /// but it could be a good estimation if you have no other way of figuring out
    /// whether the process runs interactively or not.
    /// </remarks>
    [PublicAPI]
    public static bool IsInputRedirected => Console.IsInputRedirected;

    /// <summary>
    /// Whether a key press is available to be read by <see cref="ReadKey"/>. If
    /// this property is <c>true</c>, <see cref="ReadKey"/> will return immediately.
    /// If this is <c>false</c>, <see cref="ReadKey"/> will block until a key is pressed.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if <see cref="IsInputRedirected"/>
    /// is <c>true</c>.</exception>
    [PublicAPI]
    public static bool IsKeyAvailable => Console.KeyAvailable;

    /// <summary>
    /// The standard output stream.
    /// </summary>
    [PublicAPI]
    // IMPORTANT: Don't use "new TerminalWriter(Console.Out.Write)" here as this would not
    //   work properly if "Console.Out" is changed after creating the writer.
    public static ITerminalWriter Out { get; } = new TerminalWriter(value => Console.Out.Write(value));

    /// <summary>
    /// Whether <see cref="Out"/> is redirected (to a file or the input of another process). If <c>false</c>,
    /// this means that <see cref="Out"/> writes to the "physical" console/terminal.
    /// </summary>
    /// <remarks>
    /// Unlike <see cref="IsInputRedirected"/>, the value of this property should not have
    /// any effects on the properties/methods in this interface.
    /// </remarks>
    /// <remarks>
    /// Note to implementers: If <see cref="Out"/> doesn't represent the "physical" console/terminal, you
    /// should return <c>true</c> here.
    /// </remarks>
    [PublicAPI]
    public static bool IsOutputRedirected => Console.IsOutputRedirected;

    /// <summary>
    /// The standard error output stream.
    /// </summary>
    [PublicAPI]
    // IMPORTANT: Don't use "new TerminalWriter(Console.Error.Write)" here as this would not
    //   work properly if "Console.Error" is changed after creating the writer.
    public static ITerminalWriter Error { get; } = new TerminalWriter(value => Console.Error.Write(value));

    /// <summary>
    /// Whether <see cref="Error"/> is redirected (to a file or the input of another process). If <c>false</c>,
    /// this means that <see cref="Error"/> writes to the "physical" console/terminal.
    /// </summary>
    /// <remarks>
    /// Unlike <see cref="IsInputRedirected"/>, the value of this property should not have
    /// any effects on the properties/methods in this interface.
    /// </remarks>
    /// <remarks>
    /// Note to implementers: If <see cref="Error"/> doesn't represent the "physical" console/terminal, you
    /// should return <c>true</c> here.
    /// </remarks>
    [PublicAPI]
    public static bool IsErrorRedirected => Console.IsErrorRedirected;

    /// <summary>
    /// The width of the terminal window; i.e. how many characters can be displayed
    /// on a single line.
    ///
    /// <para>Note: This class does not differentiate between "buffer width" and
    /// "window width" (as it is done for the height). It's very uncommon (if not
    /// often impossible) to have a window width different from the buffer width.</para>
    /// </summary>
    /// <remarks>
    /// Changing the terminal size is only supported on Windows. This is why
    /// this API is not exposed here. Use <see cref="Console.SetWindowSize"/>
    /// to do this.
    /// </remarks>
    /// <seealso cref="TerminalBufferHeight"/>
    /// <seealso cref="TerminalWindowHeight"/>
    [PublicAPI]
    public static int TerminalWidth => Console.BufferWidth;

    /// <summary>
    /// The full height of the terminal buffer (i.e. how many lines are stored
    /// at most in the scroll back buffer). Note that this value is usually
    /// higher than the number of visible lines (<see cref="TerminalWindowHeight"/>).
    /// </summary>
    /// <remarks>
    /// Changing the terminal buffer size is only supported on Windows. This is why
    /// this API is not exposed here. Use <see cref="Console.BufferHeight"/>
    /// to do this.
    /// </remarks>
    /// <seealso cref="TerminalWidth"/>
    /// <seealso cref="TerminalWindowHeight"/>
    [PublicAPI]
    public static int TerminalBufferHeight => Console.BufferHeight;

    /// <summary>
    /// The height of the terminal window; i.e. the number of visible lines.
    /// </summary>
    /// <remarks>
    /// Changing the terminal size is only supported on Windows. This is why
    /// this API is not exposed here. Use <see cref="Console.SetWindowSize"/>
    /// to do this.
    /// </remarks>
    /// <seealso cref="TerminalWidth"/>
    /// <seealso cref="TerminalBufferHeight"/>
    [PublicAPI]
    public static int TerminalWindowHeight => Console.WindowHeight;

    /// <summary>
    /// The column of the cursor within the terminal's buffer.
    /// </summary>
    /// <seealso cref="SetCursorPosition"/>
    /// <seealso cref="CursorTop"/>
    [PublicAPI]
    public static int CursorLeft
    {
        get => Console.CursorLeft;
        set => Console.CursorLeft = value;
    }

    /// <summary>
    /// The line of the cursor within the terminal's buffer.
    /// </summary>
    /// <seealso cref="SetCursorPosition"/>
    /// <seealso cref="CursorLeft"/>
    [PublicAPI]
    public static int CursorTop
    {
        get => Console.CursorTop;
        set => Console.CursorTop = value;
    }

    /// <summary>
    /// Whether the key combination <c>Ctrl + C</c>  is treated as ordinary input (<c>true</c>) or
    /// as an interruption that is handled by the operating system (<c>false</c>; the default).
    ///
    /// <para>Note: Instead of using this property, you should consider using <see cref="TerminateKeyCombinationPressed"/>
    /// instead.</para>
    /// </summary>
    /// <remarks>
    /// If the value of this property is <c>false</c> and <c>Ctrl + C</c> is pressed, the pressed
    /// keys are not stored in the input buffer and the operating system terminates the currently
    /// executing process.
    ///
    /// <para>This is what most user will expect. So only change this property to <c>true</c>,
    /// if you really must. In this case, users can still terminate the process via <c>Ctrl + Break</c>.
    /// </para>
    /// </remarks>
    /// <seealso cref="TerminateKeyCombinationPressed"/>
    [PublicAPI]
    public static bool TreatControlCAsInput
    {
        get => Console.TreatControlCAsInput;
        set => Console.TreatControlCAsInput = value;
    }

    /// <summary>
    /// Occurs when one of the "terminate process" key combinations has been press (i.e. either
    /// <c>Ctrl + C</c> or <c>Ctrl + Break</c>).
    /// </summary>
    /// <remarks>
    /// This event will only be raised for <c>Ctrl + C</c> if <see cref="TreatControlCAsInput"/>
    /// is <c>false</c> (the default). This event will always be raised for <c>Ctrl + Break</c>.
    ///
    /// <para>The event handlers will be given a <see cref="ConsoleCancelEventArgs"/> instance.
    /// This instance contains the property <see cref="ConsoleCancelEventArgs.Cancel"/> which
    /// is <c>false</c> by default. If it's still <c>false</c> when all event handlers have
    /// completed, the currently executing process is terminated. On the other hand, if one
    /// of the handlers sets it to <c>true</c>, the current process will not terminate but
    /// continue running.</para>
    ///
    /// <para>Note that the event handlers for this event will be executed on a thread pool
    /// thread (i.e. especially not on the main thread).</para>
    /// </remarks>
    /// <seealso cref="TreatControlCAsInput"/>
    [PublicAPI]
    public static event EventHandler<ConsoleCancelEventArgs>? TerminateKeyCombinationPressed;

    static Terminal()
    {
        Console.CancelKeyPress += OnTerminateKeyCombinationPressed;
    }

    private static void OnTerminateKeyCombinationPressed(object? sender, ConsoleCancelEventArgs e)
    {
        TerminateKeyCombinationPressed?.Invoke(null, e);
    }

    /// <summary>
    /// Writes the specified object to the terminal's standard output.
    /// </summary>
    public static void Write<T>([Localizable(true)] T? value) where T : IConvertible
    {
        Out.Write(value);
    }

    /// <summary>
    /// Writes the specified object to the terminal's standard output.
    /// </summary>
    [PublicAPI]
    public static void Write([Localizable(true)] object? value)
    {
        Out.Write(value);
    }

    /// <summary>
    /// Writes the specified string to the terminal's standard output.
    /// </summary>
    [PublicAPI]
    public static void Write([Localizable(true)] string? value)
    {
        Out.Write(value);
    }

    /// <summary>
    /// Formats <paramref name="format"/> with <paramref name="args"/> and writes the result
    /// to the terminal's standard output.
    /// </summary>
    [PublicAPI]
    [StringFormatMethod("format")]
    public static void Write([Localizable(true)] string format, params object[] args)
    {
        Write(format.With(args));
    }

    /// <summary>
    /// Writes the specified object to the terminal's standard output
    /// and appends a line break at the end.
    /// </summary>
    [PublicAPI]
    public static void WriteLine([Localizable(true)] object? value)
    {
        Out.WriteLine(value);
    }

    /// <summary>
    /// Writes the specified string to the terminal's standard output
    /// and appends a line break at the end.
    /// </summary>
    [PublicAPI]
    public static void WriteLine([Localizable(true)] string? value)
    {
        Out.WriteLine(value);
    }

    /// <summary>
    /// Formats <paramref name="format"/> with <paramref name="args"/> and writes the result
    /// to the terminal's standard output and appends a line break at the end.
    /// </summary>
    [PublicAPI]
    [StringFormatMethod("format")]
    public static void WriteLine([Localizable(true)] string format, params object[] args)
    {
        WriteLine(format.With(args));
    }

    /// <summary>
    /// Writes a line break to the terminal's standard output.
    /// </summary>
    [PublicAPI]
    public static void WriteLine()
    {
        Out.WriteLine();
    }

    /// <summary>
    /// Obtains the next character or function key pressed by the user. Note that
    /// this call will block until the user presses a key. To avoid this, check
    /// <see cref="IsKeyAvailable"/>.
    /// </summary>
    /// <param name="displayPressedKey">Whether the pressed key should be displayed
    /// on the terminal or not. Default to <c>true</c>.</param>
    /// <exception cref="InvalidOperationException">Thrown if <see cref="IsInputRedirected"/>
    /// is <c>true</c>.</exception>
    [PublicAPI]
    public static ConsoleKeyInfo ReadKey(bool displayPressedKey = true)
    {
        return Console.ReadKey(!displayPressedKey);
    }

    /// <summary>
    /// Reads the next line of characters from the standard input stream.
    /// </summary>
    /// <returns>
    /// The read line (without the end-of-line characters). Returns <c>null</c>
    /// if the input stream has been redirected (<see cref="IsInputRedirected"/>)
    /// and no more lines are available - or if the user hits <c>Ctrl+C</c>.
    /// </returns>
    [PublicAPI]
    public static string? ReadLine()
    {
        return Console.ReadLine();
    }

    /// <summary>
    /// Clears the terminal buffer (i.e. makes the terminal "blank").
    /// </summary>
    [PublicAPI]
    public static void Clear()
    {
        Console.Clear();
    }

    /// <summary>
    /// Plays a beep sound through the terminal "speaker".
    /// </summary>
    [PublicAPI]
    public static void Beep()
    {
        try
        {
            Console.Beep();
        }
        catch (Exception)
        {
            // Ignore.
        }
    }

    /// <summary>
    /// Sets the window's title.
    /// </summary>
    /// <remarks>
    /// Reading the window's title is only supported on Windows. This
    /// why this API is not exposed in the class. To read the title
    /// on Windows, use <see cref="Console.Title"/>.
    /// </remarks>
    [PublicAPI]
    public static void SetWindowTitle([Localizable(true)] string title)
    {
        Console.Title = title;
    }

    /// <summary>
    /// Sets the cursor position within the terminal's buffer (see <see cref="TerminalWidth"/>
    /// and <see cref="TerminalBufferHeight"/>). The cursor's position determines where
    /// the next character will be written.
    /// </summary>
    /// <remarks>
    /// If the cursor is placed outside the visible area (vertically), the window will automatically
    /// scroll so that the cursor becomes visible.
    /// </remarks>
    /// <param name="left">The column of the cursor within the terminal's buffer.</param>
    /// <param name="top">The line of the cursor within the terminal's buffer.</param>
    /// <seealso cref="CursorLeft"/>
    /// <seealso cref="CursorTop"/>
    [PublicAPI]
    public static void SetCursorPosition(int left, int top)
    {
        Console.SetCursorPosition(left: left, top: top);
    }

    [SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
    private sealed class TerminalAsInstance : ITerminalWindow
    {
        /// <inheritdoc />
        public TextReader In => Terminal.In;

        /// <inheritdoc />
        public bool IsInputRedirected => Terminal.IsInputRedirected;

        /// <inheritdoc />
        public bool IsKeyAvailable => Terminal.IsKeyAvailable;

        /// <inheritdoc />
        public ITerminalWriter Out => Terminal.Out;

        /// <inheritdoc />
        public bool IsOutputRedirected => Terminal.IsOutputRedirected;

        /// <inheritdoc />
        public ITerminalWriter Error => Terminal.Error;

        /// <inheritdoc />
        public bool IsErrorRedirected => Terminal.IsErrorRedirected;

        /// <inheritdoc />
        public int TerminalWidth => Terminal.TerminalWidth;

        /// <inheritdoc />
        public int TerminalBufferHeight => Terminal.TerminalBufferHeight;

        /// <inheritdoc />
        public int TerminalWindowHeight => Terminal.TerminalWindowHeight;

        /// <inheritdoc />
        public int CursorLeft
        {
            get => Terminal.CursorLeft;
            set => Terminal.CursorLeft = value;
        }

        /// <inheritdoc />
        public int CursorTop
        {
            get => Terminal.CursorTop;
            set => Terminal.CursorTop = value;
        }

        /// <inheritdoc />
        public ConsoleKeyInfo ReadKey(bool displayPressedKey = true) => Terminal.ReadKey(displayPressedKey);

        /// <inheritdoc />
        public string? ReadLine() => Terminal.ReadLine();

        /// <inheritdoc />
        public void Clear() => Terminal.Clear();

        /// <inheritdoc />
        public void Beep() => Terminal.Beep();

        /// <inheritdoc />
        public void SetWindowTitle(string title) => Terminal.SetWindowTitle(title);

        /// <inheritdoc />
        public void SetCursorPosition(int left, int top) => Terminal.SetCursorPosition(left, top);
    }
}

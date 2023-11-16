// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

using AppMotor.CliApp.Terminals.Formatting;
using AppMotor.Core.Colors;
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
    /// Whether <see cref="In"/> is redirected (to a file or the output of another process). If this property
    /// is <c>true</c>, some members of this class won't work anymore (<see cref="IsKeyAvailable"/>, <see cref="ReadKey"/>).
    /// Also, <see cref="ReadLine"/> can return <c>null</c> in this case.
    /// </summary>
    /// <remarks>
    /// You MAY interpret a value of <c>true</c> as "the process is non-interactive". It's not 100% correct (because the
    /// input source may still be interactive) but it could be a good estimation if you have no other way of figuring out
    /// whether the process runs interactively or not. See also <see cref="ReadKey"/> for more details.
    /// </remarks>
    [PublicAPI]
    public static bool IsInputRedirected => Console.IsInputRedirected;

    /// <summary>
    /// Whether a key press is available to be read by <see cref="ReadKey"/>. If this property is <c>true</c>,
    /// <see cref="ReadKey"/> will return immediately. If this is <c>false</c>, <see cref="ReadKey"/> will block
    /// until a key is pressed.
    /// </summary>
    /// <remarks>
    /// Note that this property differs from using <c>In.Peek()</c>; see <see cref="ReadKey"/> for more details.
    /// </remarks>
    /// <exception cref="InvalidOperationException">Thrown if <see cref="IsInputRedirected"/> is <c>true</c>.</exception>
    [PublicAPI]
    public static bool IsKeyAvailable => Console.KeyAvailable;

    /// <summary>
    /// The standard output stream.
    /// </summary>
    [PublicAPI]
    public static ITerminalWriter Out { get; } = new ConsoleTerminalWriter(stdErr: false);

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
    public static ITerminalWriter Error { get; } = new ConsoleTerminalWriter(stdErr: true);

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
    [PublicAPI]
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
    /// Obtains the next character or function key pressed by the user. Note that this call will block until the user
    /// presses a key. To avoid this, check <see cref="IsKeyAvailable"/>.
    /// </summary>
    /// <param name="displayPressedKey">Whether the pressed key should be displayed on the terminal or not. Default to
    /// <c>true</c>.</param>
    /// <remarks>
    /// Note that this method differs from using <c>In.Read()</c> as <see cref="In"/> can only contain key presses for
    /// keys that can be mapped to a <c>char</c>; for example, the keys F1-F12, insert, delete, home, end, and the arrow
    /// keys can be read through this method but not through <see cref="In"/>. This is why <see cref="IsInputRedirected"/>
    /// does have an effect on the members of this type whereas its output equivalents (e.g.
    /// <see cref="IsOutputRedirected"/>) are only informational.
    /// </remarks>
    /// <remarks>
    /// This method doesn't return modifier key presses (i.e. ctrl, alt, shift, ...).
    /// </remarks>
    /// <exception cref="InvalidOperationException">Thrown if <see cref="IsInputRedirected"/> is <c>true</c>.</exception>
    [PublicAPI]
    public static ConsoleKeyInfo ReadKey(bool displayPressedKey = true)
    {
        return Console.ReadKey(!displayPressedKey);
    }

    /// <summary>
    /// Reads the next line of characters from the standard input stream.
    /// </summary>
    /// <returns>
    /// The read line (without the end-of-line characters). Returns <c>null</c> if the input stream has been redirected
    /// (<see cref="IsInputRedirected"/>) and no more lines are available - or if the user hits <c>Ctrl+C</c>.
    ///
    /// <para>If the input stream has not been redirected, this method will block until to user presses either Enter or
    /// <c>Ctrl+C</c>.</para>
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

    private sealed class ConsoleTerminalWriter : TerminalWriterBase
    {
        private TextWriter TextWriter { get; }

        private readonly SuppressAnsiColorSequencesStreamParser _noColorStreamParser;

        /// <summary>
        /// Fallback parser if ANSI is not supported (required only on Windows).
        /// </summary>
        private readonly AnsiFallbackStreamParser? _ansiFallbackStreamParser;

        public ConsoleTerminalWriter(bool stdErr)
        {
            this.TextWriter = stdErr ? Console.Error : Console.Out;
            this._noColorStreamParser = new SuppressAnsiColorSequencesStreamParser(this);

            if (!AnsiSupportOnWindows.Enable())
            {
                this._ansiFallbackStreamParser = new AnsiFallbackStreamParser(this);
            }
        }

        /// <inheritdoc />
        protected override void WriteCore(string value)
        {
            if (this._ansiFallbackStreamParser is not null)
            {
                this._ansiFallbackStreamParser.ParseNext(value);
            }
            else
            {
                if (this.EnableColors)
                {
                    this.TextWriter.Write(value);
                }
                else
                {
                    this._noColorStreamParser.ParseNext(value);
                }
            }
        }

        private sealed class AnsiFallbackStreamParser : AnsiColorStreamParser
        {
            /// <summary>
            /// This lock is required because both <see cref="Console.ForegroundColor"/> and <see cref="Console.BackgroundColor"/>
            /// affect both <see cref="Console.Out"/> and(!) <see cref="Console.Error"/>. With this lock we make sure that only
            /// one of the streams is written to at the same time.
            /// </summary>
            private static readonly object s_consoleColorLock = new();

            private readonly ConsoleTerminalWriter _consoleTerminalWriter;

            /// <summary>
            /// The foreground color to use for the next text write operation.
            /// </summary>
            /// <remarks>
            /// We can't use <see cref="Console.ForegroundColor"/> directly to store this value because
            /// the colors for <see cref="Console.Out"/> and <see cref="Console.Error"/> must be managed
            /// independently.
            /// </remarks>
            private ConsoleColor? _currentForegroundColor;

            /// <summary>
            /// The background color to use for the next text write operation.
            /// </summary>
            /// <remarks>
            /// We can't use <see cref="Console.BackgroundColor"/> directly to store this value because
            /// the colors for <see cref="Console.Out"/> and <see cref="Console.Error"/> must be managed
            /// independently.
            /// </remarks>
            private ConsoleColor? _currentBackgroundColor;

            public AnsiFallbackStreamParser(ConsoleTerminalWriter consoleTerminalWriter)
            {
                this._consoleTerminalWriter = consoleTerminalWriter;
            }

            /// <inheritdoc />
            protected override void OnTextColor(ConsoleColor color)
            {
                this._currentForegroundColor = color;
            }

            /// <inheritdoc />
            protected override void OnTextColor(int colorIndex)
            {
                // Ignore; not supported
                // While it would technically possible to convert the color index into an RGB color (and then
                // call "OnTextColor(RgbColor)", the latter won't be able to reduce the color back down to
                // "ConsoleColor". See remarks there.
            }

            /// <inheritdoc />
            protected override void OnTextColor(RgbColor color)
            {
                // Ignore; not supported
                // NOTE: While it would be probably possible to "reduce" the RGB color to one values of "ConsoleColor",
                //   a short search on the Internet didn't provide any obvious/easy solutions for the problem. You would
                //   most likely calculate the color difference between the RGB color and each ConsoleColor and use the
                //   closest one. However, this seems to be a hard a problem - see, for example, the note at the top of
                //   of this StackOverflow answer: https://stackoverflow.com/a/35114586/614177
                //
                //   And since - at the time of writing - the oldest Windows version (that's still supported by Microsoft)
                //   that (probably) doesn't support ANSI sequences, Windows Server 2012 R2, only has one year left for
                //   support, it doesn't seem sensible to put effort in making this work.
            }

            /// <inheritdoc />
            protected override void OnBackgroundColor(ConsoleColor color)
            {
                this._currentBackgroundColor = color;
            }

            /// <inheritdoc />
            protected override void OnBackgroundColor(int colorIndex)
            {
                // Ignore; not supported
                // See remarks in "OnForegroundColor(int)".
            }

            /// <inheritdoc />
            protected override void OnBackgroundColor(RgbColor color)
            {
                // Ignore; not supported
                // See remarks in "OnForegroundColor(ConsoleColor)".
            }

            /// <inheritdoc />
            protected override void OnResetColors(bool resetForegroundColor, bool resetBackgroundColor)
            {
                if (resetForegroundColor)
                {
                    this._currentForegroundColor = null;
                }

                if (resetBackgroundColor)
                {
                    this._currentBackgroundColor = null;
                }
            }

            /// <inheritdoc />
            protected override void OnText(ReadOnlySpan<char> text)
            {
                if (!this._consoleTerminalWriter.EnableColors || (this._currentForegroundColor is null && this._currentBackgroundColor is null))
                {
                    this._consoleTerminalWriter.TextWriter.Write(text);
                    return;
                }

                lock (s_consoleColorLock)
                {
                    if (this._currentForegroundColor != null)
                    {
                        Console.ForegroundColor = this._currentForegroundColor.Value;
                    }
                    if (this._currentBackgroundColor != null)
                    {
                        Console.BackgroundColor = this._currentBackgroundColor.Value;
                    }

                    this._consoleTerminalWriter.TextWriter.Write(text);

                    Console.ResetColor();
                }
            }

            /// <inheritdoc />
            protected override void OnNonColorAnsiEscapeSequence(ReadOnlySpan<char> escapeSequenceContents)
            {
                // Ignore; not supported
            }
        }

        private sealed class SuppressAnsiColorSequencesStreamParser : SuppressAnsiColorSequencesStreamParserBase
        {
            private readonly ConsoleTerminalWriter _consoleTerminalWriter;

            public SuppressAnsiColorSequencesStreamParser(ConsoleTerminalWriter consoleTerminalWriter)
            {
                this._consoleTerminalWriter = consoleTerminalWriter;
            }

            /// <inheritdoc />
            protected override void OnText(ReadOnlySpan<char> text)
            {
                this._consoleTerminalWriter.TextWriter.Write(text);
            }

            /// <inheritdoc />
            protected override void OnNonColorAnsiEscapeSequence(ReadOnlySpan<char> escapeSequenceContents)
            {
                this._consoleTerminalWriter.TextWriter.Write(AnsiEscapeSequence.Create(escapeSequenceContents));
            }
        }
    }
}

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
using System.Text;

using JetBrains.Annotations;

namespace AppMotor.Core.System
{
    /// <summary>
    /// Replacement for <see cref="Console"/> that supports <see cref="ColoredString"/>.
    /// </summary>
    public static class Terminal
    {
        /// <summary>
        /// The standard input stream.
        /// </summary>
        [PublicAPI]
        public static TextReader Input => Console.In;

        /// <summary>
        /// The encoding used for <see cref="Input"/>.
        /// </summary>
        /// <remarks>
        /// For details and defaults, see <see cref="Console.InputEncoding"/>.
        /// </remarks>
        [PublicAPI,]
        public static Encoding InputEncoding
        {
            get => Console.InputEncoding;
            set => Console.InputEncoding = value;
        }

        /// <summary>
        /// Whether <see cref="Input"/> is redirected (to a file or the output
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
        /// The standard error output stream.
        /// </summary>
        [PublicAPI]
        public static TextWriter Error => Console.Error;

        /// <summary>
        /// The encoding used for the various <c>Write()</c> and <c>WriteLine()</c> methods
        /// and for <see cref="Error"/>.
        /// </summary>
        /// <remarks>
        /// For details and defaults, see <see cref="Console.OutputEncoding"/>.
        /// </remarks>
        [PublicAPI]
        public static Encoding OutputEncoding
        {
            get => Console.OutputEncoding;
            set => Console.OutputEncoding = value;
        }

        /// <summary>
        /// The background color of the terminal.
        /// </summary>
        /// <remarks>
        /// To set the foreground (text) color, use <see cref="Write(ColoredString)"/>
        /// or <see cref="WriteLine(ColoredString)"/>.
        /// </remarks>
        [PublicAPI]
        public static ConsoleColor BackgroundColor
        {
            get => Console.BackgroundColor;
            set => Console.BackgroundColor = value;
        }

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

        private static void OnTerminateKeyCombinationPressed(object sender, ConsoleCancelEventArgs e)
        {
            TerminateKeyCombinationPressed?.Invoke(null, e);
        }

        /// <summary>
        /// Writes the specified object to the terminal's standard output.
        /// </summary>
        [PublicAPI]
        public static void Write([Localizable(true)] object? value)
        {
            Console.Write(value);
        }

        /// <summary>
        /// Writes the specified string to the terminal's standard output.
        /// </summary>
        [PublicAPI]
        public static void Write([Localizable(true)] string? value)
        {
            Console.Write(value);
        }

        /// <summary>
        /// Writes the specified colored string to the terminal's standard output.
        /// </summary>
        [PublicAPI]
        public static void Write(ColoredString? coloredString)
        {
            if (coloredString == null || coloredString.Count == 0)
            {
                return;
            }

            var originalColor = Console.ForegroundColor;

            try
            {
                foreach (var coloredSubstring in coloredString)
                {
                    Console.ForegroundColor = coloredSubstring.Color ?? originalColor;

                    Console.Write(coloredSubstring.Text);
                }
            }
            finally
            {
                Console.ForegroundColor = originalColor;
            }
        }

        /// <summary>
        /// Writes the specified object to the terminal's standard output
        /// and appends a line break at the end.
        /// </summary>
        [PublicAPI]
        public static void WriteLine([Localizable(true)] object? value)
        {
            Console.WriteLine(value);
        }

        /// <summary>
        /// Writes the specified string to the terminal's standard output
        /// and appends a line break at the end.
        /// </summary>
        [PublicAPI]
        public static void WriteLine([Localizable(true)] string? value)
        {
            Console.WriteLine(value);
        }

        /// <summary>
        /// Writes the specified colored string to the terminal's standard output
        /// and appends a line break at the end.
        /// </summary>
        [PublicAPI]
        public static void WriteLine(ColoredString? coloredString)
        {
            Write(coloredString);
            WriteLine();
        }

        /// <summary>
        /// Writes a line break to the terminal's standard output.
        /// </summary>
        [PublicAPI]
        public static void WriteLine()
        {
            Console.WriteLine();
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
        /// and no more lines are available.
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
    }
}

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
using System.Text;

using JetBrains.Annotations;

namespace AppMotor.CliApp.Terminals
{
    /// <summary>
    /// Represents a terminal that allows the user to interact with the terminal window itself.
    /// </summary>
    /// <seealso cref="ITerminal"/>
    public interface ITerminalWindow : ITerminal
    {
        /// <summary>
        /// The encoding used for <see cref="IInputTerminal.Input"/>.
        /// </summary>
        /// <remarks>
        /// For details and defaults, see <see cref="Console.InputEncoding"/>.
        /// </remarks>
        [PublicAPI]
        Encoding InputEncoding { get; set; }

        /// <summary>
        /// The encoding used for the various <c>Write()</c> and <c>WriteLine()</c> methods
        /// and for <see cref="IErrorOutputTerminal.Error"/>.
        /// </summary>
        /// <remarks>
        /// For details and defaults, see <see cref="Console.OutputEncoding"/>.
        /// </remarks>
        [PublicAPI]
        Encoding OutputEncoding { get; set; }

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
        int TerminalWidth { get; }

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
        int TerminalBufferHeight { get; }

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
        int TerminalWindowHeight { get; }

        /// <summary>
        /// The column of the cursor within the terminal's buffer.
        /// </summary>
        /// <seealso cref="SetCursorPosition"/>
        /// <seealso cref="CursorTop"/>
        [PublicAPI]
        int CursorLeft { get; set; }

        /// <summary>
        /// The line of the cursor within the terminal's buffer.
        /// </summary>
        /// <seealso cref="SetCursorPosition"/>
        /// <seealso cref="CursorLeft"/>
        [PublicAPI]
        int CursorTop { get; set; }

        /// <summary>
        /// Clears the terminal buffer (i.e. makes the terminal "blank").
        /// </summary>
        [PublicAPI]
        void Clear();

        /// <summary>
        /// Plays a beep sound through the terminal "speaker".
        /// </summary>
        [PublicAPI]
        void Beep();

        /// <summary>
        /// Sets the window's title.
        /// </summary>
        /// <remarks>
        /// Reading the window's title is only supported on Windows. This
        /// why this API is not exposed in the class. To read the title
        /// on Windows, use <see cref="Console.Title"/>.
        /// </remarks>
        [PublicAPI]
        void SetWindowTitle([Localizable(true)] string title);

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
        void SetCursorPosition(int left, int top);
    }
}

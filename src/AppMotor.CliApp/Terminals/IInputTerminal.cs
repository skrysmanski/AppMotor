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

using JetBrains.Annotations;

namespace AppMotor.CliApp.Terminals;

/// <summary>
/// Represents the input of a terminal.
/// </summary>
/// <seealso cref="ITerminal"/>
public interface IInputTerminal
{
    /// <summary>
    /// The standard input stream.
    /// </summary>
    [PublicAPI]
    TextReader Input { get; }

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
    bool IsInputRedirected { get; }

    /// <summary>
    /// Whether a key press is available to be read by <see cref="ReadKey"/>. If
    /// this property is <c>true</c>, <see cref="ReadKey"/> will return immediately.
    /// If this is <c>false</c>, <see cref="ReadKey"/> will block until a key is pressed.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if <see cref="IsInputRedirected"/>
    /// is <c>true</c>.</exception>
    [PublicAPI]
    bool IsKeyAvailable { get; }

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
    ConsoleKeyInfo ReadKey(bool displayPressedKey = true);

    /// <summary>
    /// Reads the next line of characters from the standard input stream.
    /// </summary>
    /// <returns>
    /// The read line (without the end-of-line characters). Returns <c>null</c>
    /// if the input stream has been redirected (<see cref="IsInputRedirected"/>)
    /// and no more lines are available - or if the user hits <c>Ctrl+C</c>.
    /// </returns>
    [PublicAPI]
    string? ReadLine();
}
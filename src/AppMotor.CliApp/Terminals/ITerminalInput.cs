// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using JetBrains.Annotations;

namespace AppMotor.CliApp.Terminals;

/// <summary>
/// Represents the input of a terminal. For the output side, see <see cref="ITerminalOutput"/>.
/// </summary>
/// <seealso cref="ITerminal"/>
public interface ITerminalInput
{
    /// <summary>
    /// The standard input stream.
    /// </summary>
    [PublicAPI]
    TextReader In { get; }

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
    bool IsInputRedirected { get; }

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
    bool IsKeyAvailable { get; }

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
    /// <see cref="ITerminalOutput.IsOutputRedirected"/>) are only informational.
    /// </remarks>
    /// <remarks>
    /// This method doesn't return modifier key presses (i.e. ctrl, alt, shift, ...).
    /// </remarks>
    /// <exception cref="InvalidOperationException">Thrown if <see cref="IsInputRedirected"/> is <c>true</c>.</exception>
    [PublicAPI]
    ConsoleKeyInfo ReadKey(bool displayPressedKey = true);

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
    string? ReadLine();
}

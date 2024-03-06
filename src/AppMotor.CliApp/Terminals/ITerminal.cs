// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

namespace AppMotor.CliApp.Terminals;

/// <summary>
/// Represents a terminal/console without any "fancy" functionality; i.e. it supports reading and writing but
/// no interaction with the terminal window - like setting the terminal's title, cursor position or obtaining
/// the terminal's width. For advanced features, see <see cref="ITerminalWindow"/>.
/// </summary>
/// <remarks>
/// This interface primarily exists for unit testing purposes where no actual terminal/console may be available.
/// See "TestTerminal" in the "AppMotor.TestCore" package for an implementation.
/// </remarks>
/// <remarks>
/// Instances of this interface may or may not also implement <see cref="ITerminalWindow"/>. You can use an
/// "is"/"as" conversion to check for this and adopt your code accordingly.
/// </remarks>
public interface ITerminal : ITerminalInput, ITerminalOutput;

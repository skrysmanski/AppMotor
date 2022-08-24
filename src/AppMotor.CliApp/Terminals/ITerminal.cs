// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

namespace AppMotor.CliApp.Terminals;

/// <summary>
/// Represents a terminal/console without any "fancy" functionality; i.e. it supports reading and writing but
/// no interaction with the terminal window - like setting the terminal's title, cursor position or obtaining
/// the terminal's width. For advanced features, see <see cref="ITerminalWindow"/>.
/// </summary>
public interface ITerminal : IOutputTerminal, IInputTerminal
{
}
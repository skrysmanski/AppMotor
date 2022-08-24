// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

namespace AppMotor.CliApp.Terminals;

/// <summary>
/// Combination of <see cref="IStdOutTerminal"/> and <see cref="IErrorOutTerminal"/>.
/// </summary>
public interface IOutputTerminal : IStdOutTerminal, IErrorOutTerminal
{
}
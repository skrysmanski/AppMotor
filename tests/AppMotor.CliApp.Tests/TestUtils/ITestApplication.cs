// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using JetBrains.Annotations;

namespace AppMotor.CliApp.TestUtils;

[PublicAPI]
internal interface ITestApplication
{
    string TerminalOutput { get; }
}

// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.CliApp.CommandLine;

namespace AppMotor.CliApp.TestUtils;

internal class TestVerbGroup : CliVerb
{
    public TestVerbGroup(string name, params CliVerb[] subVerbs) : base(name)
    {
        this.SubVerbs = subVerbs;
    }
}
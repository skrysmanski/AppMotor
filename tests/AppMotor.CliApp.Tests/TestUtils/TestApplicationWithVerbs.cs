// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.CliApp.CommandLine;

namespace AppMotor.CliApp.TestUtils;

internal class TestApplicationWithVerbs : TestApplicationWithVerbsBase
{
    /// <inheritdoc />
    public TestApplicationWithVerbs(params CliVerb[] verbs)
    {
        this.Verbs = verbs;
    }
}

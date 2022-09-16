// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.Core.Utils;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.Utils;

/// <summary>
/// Tests for <see cref="NewLineTypes"/> and <see cref="NewLineTypesExtensions"/>.
/// </summary>
public sealed class NewLineTypesTests
{
    [Fact]
    public void Test_GetNewLineCharacters()
    {
        NewLineTypes.SystemDefault.GetNewLineCharacters().ShouldBe(Environment.NewLine);
        NewLineTypes.CrLf.GetNewLineCharacters().ShouldBe("\r\n");
        NewLineTypes.Lf.GetNewLineCharacters().ShouldBe("\n");
        Should.Throw<ArgumentOutOfRangeException>(() => ((NewLineTypes)5000).GetNewLineCharacters());
    }
}

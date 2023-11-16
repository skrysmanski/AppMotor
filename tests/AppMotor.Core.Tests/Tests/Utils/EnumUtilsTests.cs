// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.Core.Utils;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.Utils;

public sealed class EnumUtilsTests
{
    [Fact]
    public void TestGetValues()
    {
        // test
        MyTestEnum[] enumValues = EnumUtils.GetValues<MyTestEnum>();

        // verify
        enumValues.ShouldContain(MyTestEnum.Value1);
        enumValues.ShouldContain(MyTestEnum.Value2);
        enumValues.ShouldContain(MyTestEnum.Value3);
        enumValues.Length.ShouldBe(3);
    }

    private enum MyTestEnum
    {
        Value1,
        Value2,
        Value3,
    }
}

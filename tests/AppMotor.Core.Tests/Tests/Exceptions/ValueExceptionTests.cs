// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.Core.Exceptions;
using AppMotor.Core.Utils;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.Exceptions;

public sealed class ValueExceptionTests
{
    [Fact]
    public void TestDefaultConstructor()
    {
        var ex = new ValueException();

        ex.ValueName.ShouldBe(null);
        ex.Message.ShouldBe(Validate.ExceptionMessages.DEFAULT_MESSAGE);
    }
}

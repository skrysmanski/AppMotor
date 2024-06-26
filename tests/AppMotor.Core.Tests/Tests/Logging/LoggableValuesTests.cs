﻿// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Numerics;

using AppMotor.Core.ComponentModel;
using AppMotor.Core.Logging;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.Logging;

public sealed class LoggableValuesTests
{
    [Fact]
    public void TestIsSimpleLoggableType()
    {
        // setup
        typeof(MyCustomLoggableType2).MarkWith<SimpleLoggableValueMarker>();
        typeof(MyCustomSensitiveType2).MarkWith<SensitiveValueMarker>();

        // test
        LoggableValues.IsSimpleLoggableType(typeof(object)).ShouldBe(false);
        LoggableValues.IsSimpleLoggableType(typeof(Exception)).ShouldBe(false);

        LoggableValues.IsSimpleLoggableType(typeof(string)).ShouldBe(true);

        LoggableValues.IsSimpleLoggableType(typeof(byte)).ShouldBe(true);
        LoggableValues.IsSimpleLoggableType(typeof(byte?)).ShouldBe(true);
        LoggableValues.IsSimpleLoggableType(typeof(sbyte)).ShouldBe(true);
        LoggableValues.IsSimpleLoggableType(typeof(sbyte?)).ShouldBe(true);
        LoggableValues.IsSimpleLoggableType(typeof(short)).ShouldBe(true);
        LoggableValues.IsSimpleLoggableType(typeof(short?)).ShouldBe(true);
        LoggableValues.IsSimpleLoggableType(typeof(ushort)).ShouldBe(true);
        LoggableValues.IsSimpleLoggableType(typeof(ushort?)).ShouldBe(true);
        LoggableValues.IsSimpleLoggableType(typeof(int)).ShouldBe(true);
        LoggableValues.IsSimpleLoggableType(typeof(int?)).ShouldBe(true);
        LoggableValues.IsSimpleLoggableType(typeof(uint)).ShouldBe(true);
        LoggableValues.IsSimpleLoggableType(typeof(uint?)).ShouldBe(true);
        LoggableValues.IsSimpleLoggableType(typeof(long)).ShouldBe(true);
        LoggableValues.IsSimpleLoggableType(typeof(long?)).ShouldBe(true);
        LoggableValues.IsSimpleLoggableType(typeof(ulong)).ShouldBe(true);
        LoggableValues.IsSimpleLoggableType(typeof(ulong?)).ShouldBe(true);

        LoggableValues.IsSimpleLoggableType(typeof(float)).ShouldBe(true);
        LoggableValues.IsSimpleLoggableType(typeof(float?)).ShouldBe(true);
        LoggableValues.IsSimpleLoggableType(typeof(double)).ShouldBe(true);
        LoggableValues.IsSimpleLoggableType(typeof(double?)).ShouldBe(true);
        LoggableValues.IsSimpleLoggableType(typeof(decimal)).ShouldBe(true);
        LoggableValues.IsSimpleLoggableType(typeof(decimal?)).ShouldBe(true);

        LoggableValues.IsSimpleLoggableType(typeof(BigInteger)).ShouldBe(true);
        LoggableValues.IsSimpleLoggableType(typeof(BigInteger?)).ShouldBe(true);

        LoggableValues.IsSimpleLoggableType(typeof(bool)).ShouldBe(true);
        LoggableValues.IsSimpleLoggableType(typeof(bool?)).ShouldBe(true);
        LoggableValues.IsSimpleLoggableType(typeof(char)).ShouldBe(true);
        LoggableValues.IsSimpleLoggableType(typeof(char?)).ShouldBe(true);
        LoggableValues.IsSimpleLoggableType(typeof(Guid)).ShouldBe(true);
        LoggableValues.IsSimpleLoggableType(typeof(Guid?)).ShouldBe(true);
        LoggableValues.IsSimpleLoggableType(typeof(DateTime)).ShouldBe(true);
        LoggableValues.IsSimpleLoggableType(typeof(DateTime?)).ShouldBe(true);
        LoggableValues.IsSimpleLoggableType(typeof(TimeSpan)).ShouldBe(true);
        LoggableValues.IsSimpleLoggableType(typeof(TimeSpan?)).ShouldBe(true);
        LoggableValues.IsSimpleLoggableType(typeof(DateTimeOffset)).ShouldBe(true);
        LoggableValues.IsSimpleLoggableType(typeof(DateTimeOffset?)).ShouldBe(true);

        // Enums
        LoggableValues.IsSimpleLoggableType(typeof(MyEnum)).ShouldBe(true);
        LoggableValues.IsSimpleLoggableType(typeof(MyEnum?)).ShouldBe(true);

        // Custom types
        LoggableValues.IsSimpleLoggableType(typeof(MyCustomLoggableType1)).ShouldBe(true);
        LoggableValues.IsSimpleLoggableType(typeof(MyCustomLoggableType2)).ShouldBe(true);

        LoggableValues.IsSimpleLoggableType(typeof(MyCustomSensitiveType1)).ShouldBe(false);
        LoggableValues.IsSimpleLoggableType(typeof(MyCustomSensitiveType2)).ShouldBe(false);
    }

    private enum MyEnum;

    private sealed class MyCustomLoggableType1 : ISimpleLoggableValue;

    private sealed class MyCustomLoggableType2;

    private sealed class MyCustomSensitiveType1 : ISimpleLoggableValue, ISensitiveValue;

    private sealed class MyCustomSensitiveType2 : ISimpleLoggableValue;
}

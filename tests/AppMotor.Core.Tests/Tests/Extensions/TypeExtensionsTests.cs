#region License
// Copyright 2020 AppMotor Framework (https://github.com/skrysmanski/AppMotor)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System.Numerics;

using AppMotor.Core.Extensions;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.Extensions;

public sealed class TypeExtensionsTests
{
    [Fact]
    public void TestIsNumericType()
    {
        typeof(string).IsNumericType().ShouldBe(false);
        typeof(char).IsNumericType().ShouldBe(false);

        typeof(byte).IsNumericType().ShouldBe(true);
        typeof(sbyte).IsNumericType().ShouldBe(true);
        typeof(short).IsNumericType().ShouldBe(true);
        typeof(ushort).IsNumericType().ShouldBe(true);
        typeof(int).IsNumericType().ShouldBe(true);
        typeof(uint).IsNumericType().ShouldBe(true);
        typeof(long).IsNumericType().ShouldBe(true);
        typeof(ulong).IsNumericType().ShouldBe(true);

        typeof(float).IsNumericType().ShouldBe(true);
        typeof(double).IsNumericType().ShouldBe(true);
        typeof(decimal).IsNumericType().ShouldBe(true);

        typeof(BigInteger).IsNumericType().ShouldBe(true);
    }

    [Fact]
    public void TestIsNumericIntegerType()
    {
        typeof(string).IsNumericIntegerType().ShouldBe(false);
        typeof(char).IsNumericIntegerType().ShouldBe(false);

        typeof(byte).IsNumericIntegerType().ShouldBe(true);
        typeof(sbyte).IsNumericIntegerType().ShouldBe(true);
        typeof(short).IsNumericIntegerType().ShouldBe(true);
        typeof(ushort).IsNumericIntegerType().ShouldBe(true);
        typeof(int).IsNumericIntegerType().ShouldBe(true);
        typeof(uint).IsNumericIntegerType().ShouldBe(true);
        typeof(long).IsNumericIntegerType().ShouldBe(true);
        typeof(ulong).IsNumericIntegerType().ShouldBe(true);

        typeof(float).IsNumericIntegerType().ShouldBe(false);
        typeof(double).IsNumericIntegerType().ShouldBe(false);
        typeof(decimal).IsNumericIntegerType().ShouldBe(false);

        typeof(BigInteger).IsNumericIntegerType().ShouldBe(true);
    }

    [Fact]
    public void TestIsNumericFloatType()
    {
        typeof(string).IsNumericFloatType().ShouldBe(false);
        typeof(char).IsNumericFloatType().ShouldBe(false);

        typeof(byte).IsNumericFloatType().ShouldBe(false);
        typeof(sbyte).IsNumericFloatType().ShouldBe(false);
        typeof(short).IsNumericFloatType().ShouldBe(false);
        typeof(ushort).IsNumericFloatType().ShouldBe(false);
        typeof(int).IsNumericFloatType().ShouldBe(false);
        typeof(uint).IsNumericFloatType().ShouldBe(false);
        typeof(long).IsNumericFloatType().ShouldBe(false);
        typeof(ulong).IsNumericFloatType().ShouldBe(false);

        typeof(float).IsNumericFloatType().ShouldBe(true);
        typeof(double).IsNumericFloatType().ShouldBe(true);
        typeof(decimal).IsNumericFloatType().ShouldBe(true);

        typeof(BigInteger).IsNumericFloatType().ShouldBe(false);
    }

    [Fact]
    public void TestIsNullableValueType()
    {
        typeof(int).IsNullableValueType().ShouldBe(false);
        typeof(int?).IsNullableValueType().ShouldBe(true);

        typeof(KeyValuePair<int, int>).IsNullableValueType().ShouldBe(false);
        typeof(KeyValuePair<int, int>?).IsNullableValueType().ShouldBe(true);

        typeof(string).IsNullableValueType().ShouldBe(false);
        typeof(object).IsNullableValueType().ShouldBe(false);
    }

    [Fact]
    public void TestIsNullableType()
    {
#pragma warning disable 618 // obsolete
        typeof(int).IsNullableType().ShouldBe(false);
        typeof(int?).IsNullableType().ShouldBe(true);

        typeof(KeyValuePair<int, int>).IsNullableType().ShouldBe(false);
        typeof(KeyValuePair<int, int>?).IsNullableType().ShouldBe(true);

        typeof(string).IsNullableType().ShouldBe(true);
        typeof(object).IsNullableType().ShouldBe(true);
#pragma warning restore 618
    }

    [Fact]
    public void TestIs()
    {
        typeof(ClassA).Is<ITestInterface>().ShouldBe(true);
        typeof(ClassA).Is(typeof(ITestInterface)).ShouldBe(true);

        typeof(ClassB).Is<ITestInterface>().ShouldBe(true);
        typeof(ClassB).Is(typeof(ITestInterface)).ShouldBe(true);

        typeof(ITestInterface).Is<ITestInterface>().ShouldBe(true);
        typeof(ITestInterface).Is(typeof(ITestInterface)).ShouldBe(true);

        typeof(object).Is<ITestInterface>().ShouldBe(false);
        typeof(object).Is(typeof(ITestInterface)).ShouldBe(false);
    }

    private interface ITestInterface
    {

    }

    private class ClassA : ITestInterface
    {

    }

    private class ClassB : ClassA
    {

    }
}
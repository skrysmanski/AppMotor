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

using AppMotor.Core.DataModel;
using AppMotor.Core.Extensions;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.Extensions;

public sealed class TypeExtensionsTests
{
    [Fact]
    public void Test_IsNumericType()
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
    public void Test_IsNumericIntegerType()
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
    public void Test_IsNumericFloatType()
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
    public void Test_IsNullableValueType()
    {
        typeof(int).IsNullableValueType().ShouldBe(false);
        typeof(int?).IsNullableValueType().ShouldBe(true);

        typeof(KeyValuePair<int, int>).IsNullableValueType().ShouldBe(false);
        typeof(KeyValuePair<int, int>?).IsNullableValueType().ShouldBe(true);

        typeof(string).IsNullableValueType().ShouldBe(false);
        typeof(object).IsNullableValueType().ShouldBe(false);
    }

    [Fact]
    public void Test_IsNullableType()
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
    public void Test_Is()
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

    [Theory]
    [InlineData("+",  UnaryOperators.UnaryPlus)]
    [InlineData("-",  UnaryOperators.UnaryNegation)]
    [InlineData("++", UnaryOperators.Increment)]
    [InlineData("--", UnaryOperators.Decrement)]
    [InlineData("!",  UnaryOperators.LogicalNot)]
    [InlineData("~",  UnaryOperators.OnesComplement)]
    public void Test_GetOperator_Unary(string operatorAsString, UnaryOperators operatorAsEnum)
    {
        var operatorFromEnum = typeof(OperatorsTestContainer).GetOperator(operatorAsEnum);
        var operatorFromString = typeof(OperatorsTestContainer).GetOperator(operatorAsString, unary: true);

        operatorFromEnum.ShouldNotBeNull();
        operatorFromString.ShouldNotBeNull();

        operatorFromEnum.ShouldBeSameAs(operatorFromString);

        operatorFromEnum.Name.ShouldBe("op_" + operatorAsEnum);
        operatorFromEnum.GetParameters().Select(p => p.ParameterType).ShouldBe(new[] { typeof(OperatorsTestContainer) });
    }

    [Theory]
    [InlineData("+",  BinaryOperators.Addition)]
    [InlineData("-",  BinaryOperators.Subtraction)]
    [InlineData("*",  BinaryOperators.Multiply)]
    [InlineData("/",  BinaryOperators.Division)]
    [InlineData("%",  BinaryOperators.Modulus)]
    [InlineData("==", BinaryOperators.Equality)]
    [InlineData("!=", BinaryOperators.Inequality)]
    [InlineData("<",  BinaryOperators.LessThan)]
    [InlineData(">",  BinaryOperators.GreaterThan)]
    [InlineData("<=", BinaryOperators.LessThanOrEqual)]
    [InlineData(">=", BinaryOperators.GreaterThanOrEqual)]
    [InlineData("&",  BinaryOperators.BitwiseAnd)]
    [InlineData("|",  BinaryOperators.BitwiseOr)]
    [InlineData("^",  BinaryOperators.ExclusiveOr)]
    [InlineData("<<", BinaryOperators.LeftShift)]
    [InlineData(">>", BinaryOperators.RightShift)]
    public void Test_GetOperator_Binary_SameType(string operatorAsString, BinaryOperators operatorAsEnum)
    {
        var operatorFromEnum = typeof(OperatorsTestContainer).GetOperator(operatorAsEnum);
        var operatorFromString = typeof(OperatorsTestContainer).GetOperator(operatorAsString, unary: false);

        operatorFromEnum.ShouldNotBeNull();
        operatorFromString.ShouldNotBeNull();

        operatorFromEnum.ShouldBeSameAs(operatorFromString);

        operatorFromEnum.Name.ShouldBe("op_" + operatorAsEnum);
        Type secondType = operatorAsEnum switch
        {
            BinaryOperators.LeftShift => typeof(int),
            BinaryOperators.RightShift => typeof(int),
            _ => typeof(OperatorsTestContainer),
        };
        operatorFromEnum.GetParameters().Select(p => p.ParameterType).ShouldBe(new[] { typeof(OperatorsTestContainer), secondType });
    }

    [Theory]
    [InlineData("+",  BinaryOperators.Addition)]
    [InlineData("-",  BinaryOperators.Subtraction)]
    [InlineData("*",  BinaryOperators.Multiply)]
    [InlineData("/",  BinaryOperators.Division)]
    [InlineData("%",  BinaryOperators.Modulus)]
    [InlineData("==", BinaryOperators.Equality)]
    [InlineData("!=", BinaryOperators.Inequality)]
    [InlineData("<",  BinaryOperators.LessThan)]
    [InlineData(">",  BinaryOperators.GreaterThan)]
    [InlineData("<=", BinaryOperators.LessThanOrEqual)]
    [InlineData(">=", BinaryOperators.GreaterThanOrEqual)]
    [InlineData("&",  BinaryOperators.BitwiseAnd)]
    [InlineData("|",  BinaryOperators.BitwiseOr)]
    [InlineData("^",  BinaryOperators.ExclusiveOr)]
    public void Test_GetOperator_Binary_DifferentType(string operatorAsString, BinaryOperators operatorAsEnum)
    {
        void TestGetOperator(bool otherTypeIsSecondOperand)
        {
            var operatorFromEnum = typeof(OperatorsTestContainer).GetOperator(operatorAsEnum, otherType: typeof(string), otherTypeIsSecondOperand);
            var operatorFromString = typeof(OperatorsTestContainer).GetOperator(operatorAsString, otherType: typeof(string), otherTypeIsSecondOperand);

            operatorFromEnum.ShouldNotBeNull();
            operatorFromString.ShouldNotBeNull();

            operatorFromEnum.ShouldBeSameAs(operatorFromString);

            operatorFromEnum.Name.ShouldBe("op_" + operatorAsEnum);
            if (otherTypeIsSecondOperand)
            {
                operatorFromEnum.GetParameters().Select(p => p.ParameterType).ShouldBe(new[] { typeof(OperatorsTestContainer), typeof(string) });
            }
            else
            {
                operatorFromEnum.GetParameters().Select(p => p.ParameterType).ShouldBe(new[] { typeof(string), typeof(OperatorsTestContainer) });
            }
        }

        TestGetOperator(otherTypeIsSecondOperand: true);
        TestGetOperator(otherTypeIsSecondOperand: false);
    }

    /// <summary>
    /// This test primarily exists to increase the code coverage.
    /// </summary>
    [Theory]
    [InlineData("<<", BinaryOperators.LeftShift)]
    [InlineData(">>", BinaryOperators.RightShift)]
    public void Test_GetOperator_ShiftOperator(string operatorAsString, BinaryOperators operatorAsEnum)
    {
        var operatorInfo = typeof(OperatorsTestContainer).GetOperator(operatorAsString, otherType: typeof(int), otherTypeIsSecondOperand: true);

        operatorInfo.ShouldNotBeNull();

        operatorInfo.Name.ShouldBe("op_" + operatorAsEnum);
        operatorInfo.GetParameters().Select(p => p.ParameterType).ShouldBe(new[] { typeof(OperatorsTestContainer), typeof(int) });
    }

    [Fact]
    public void Test_GetOperator_InvalidOperator()
    {
        Should.Throw<ArgumentException>(() => typeof(OperatorsTestContainer).GetOperator("xxx"));
        Should.Throw<ArgumentException>(() => typeof(OperatorsTestContainer).GetOperator("xxx", typeof(string)));
    }

#pragma warning disable CS0660, CS0661
    private sealed class OperatorsTestContainer
#pragma warning restore CS0660, CS0661
    {
        // ReSharper disable UnusedParameter.Local

        #region Unary Operators

        public static OperatorsTestContainer operator +(OperatorsTestContainer left)
        {
            throw new NotSupportedException();
        }

        public static OperatorsTestContainer operator -(OperatorsTestContainer left)
        {
            throw new NotSupportedException();
        }

        public static OperatorsTestContainer operator ++(OperatorsTestContainer left)
        {
            throw new NotSupportedException();
        }

        public static OperatorsTestContainer operator --(OperatorsTestContainer left)
        {
            throw new NotSupportedException();
        }

        public static OperatorsTestContainer operator !(OperatorsTestContainer left)
        {
            throw new NotSupportedException();
        }

        public static OperatorsTestContainer operator ~(OperatorsTestContainer left)
        {
            throw new NotSupportedException();
        }

        #endregion Unary Operators

        #region Binary Operators

        #region Same Type

        public static OperatorsTestContainer operator +(OperatorsTestContainer left, OperatorsTestContainer right)
        {
            throw new NotSupportedException();
        }

        public static OperatorsTestContainer operator -(OperatorsTestContainer left, OperatorsTestContainer right)
        {
            throw new NotSupportedException();
        }

        public static OperatorsTestContainer operator *(OperatorsTestContainer left, OperatorsTestContainer right)
        {
            throw new NotSupportedException();
        }

        public static OperatorsTestContainer operator /(OperatorsTestContainer left, OperatorsTestContainer right)
        {
            throw new NotSupportedException();
        }

        public static OperatorsTestContainer operator %(OperatorsTestContainer left, OperatorsTestContainer right)
        {
            throw new NotSupportedException();
        }

        public static bool operator ==(OperatorsTestContainer left, OperatorsTestContainer right)
        {
            throw new NotSupportedException();
        }

        public static bool operator !=(OperatorsTestContainer left, OperatorsTestContainer right)
        {
            throw new NotSupportedException();
        }

        public static bool operator <(OperatorsTestContainer left, OperatorsTestContainer right)
        {
            throw new NotSupportedException();
        }

        public static bool operator >(OperatorsTestContainer left, OperatorsTestContainer right)
        {
            throw new NotSupportedException();
        }

        public static bool operator <=(OperatorsTestContainer left, OperatorsTestContainer right)
        {
            throw new NotSupportedException();
        }

        public static bool operator >=(OperatorsTestContainer left, OperatorsTestContainer right)
        {
            throw new NotSupportedException();
        }

        public static OperatorsTestContainer operator &(OperatorsTestContainer left, OperatorsTestContainer right)
        {
            throw new NotSupportedException();
        }

        public static OperatorsTestContainer operator |(OperatorsTestContainer left, OperatorsTestContainer right)
        {
            throw new NotSupportedException();
        }

        public static OperatorsTestContainer operator ^(OperatorsTestContainer left, OperatorsTestContainer right)
        {
            throw new NotSupportedException();
        }

        #endregion Same Type

        #region Different Type First

        public static OperatorsTestContainer operator +(string left, OperatorsTestContainer right)
        {
            throw new NotSupportedException();
        }

        public static OperatorsTestContainer operator -(string left, OperatorsTestContainer right)
        {
            throw new NotSupportedException();
        }

        public static OperatorsTestContainer operator *(string left, OperatorsTestContainer right)
        {
            throw new NotSupportedException();
        }

        public static OperatorsTestContainer operator /(string left, OperatorsTestContainer right)
        {
            throw new NotSupportedException();
        }

        public static OperatorsTestContainer operator %(string left, OperatorsTestContainer right)
        {
            throw new NotSupportedException();
        }

        public static bool operator ==(string left, OperatorsTestContainer right)
        {
            throw new NotSupportedException();
        }

        public static bool operator !=(string left, OperatorsTestContainer right)
        {
            throw new NotSupportedException();
        }

        public static bool operator <(string left, OperatorsTestContainer right)
        {
            throw new NotSupportedException();
        }

        public static bool operator >(string left, OperatorsTestContainer right)
        {
            throw new NotSupportedException();
        }

        public static bool operator <=(string left, OperatorsTestContainer right)
        {
            throw new NotSupportedException();
        }

        public static bool operator >=(string left, OperatorsTestContainer right)
        {
            throw new NotSupportedException();
        }

        public static OperatorsTestContainer operator &(string left, OperatorsTestContainer right)
        {
            throw new NotSupportedException();
        }

        public static OperatorsTestContainer operator |(string left, OperatorsTestContainer right)
        {
            throw new NotSupportedException();
        }

        public static OperatorsTestContainer operator ^(string left, OperatorsTestContainer right)
        {
            throw new NotSupportedException();
        }

        #endregion Different Type First

        #region Different Type Second

        public static OperatorsTestContainer operator +(OperatorsTestContainer left, string right)
        {
            throw new NotSupportedException();
        }

        public static OperatorsTestContainer operator -(OperatorsTestContainer left, string right)
        {
            throw new NotSupportedException();
        }

        public static OperatorsTestContainer operator *(OperatorsTestContainer left, string right)
        {
            throw new NotSupportedException();
        }

        public static OperatorsTestContainer operator /(OperatorsTestContainer left, string right)
        {
            throw new NotSupportedException();
        }

        public static OperatorsTestContainer operator %(OperatorsTestContainer left, string right)
        {
            throw new NotSupportedException();
        }

        public static bool operator ==(OperatorsTestContainer left, string right)
        {
            throw new NotSupportedException();
        }

        public static bool operator !=(OperatorsTestContainer left, string right)
        {
            throw new NotSupportedException();
        }

        public static bool operator <(OperatorsTestContainer left, string right)
        {
            throw new NotSupportedException();
        }

        public static bool operator >(OperatorsTestContainer left, string right)
        {
            throw new NotSupportedException();
        }

        public static bool operator <=(OperatorsTestContainer left, string right)
        {
            throw new NotSupportedException();
        }

        public static bool operator >=(OperatorsTestContainer left, string right)
        {
            throw new NotSupportedException();
        }

        public static OperatorsTestContainer operator &(OperatorsTestContainer left, string right)
        {
            throw new NotSupportedException();
        }

        public static OperatorsTestContainer operator |(OperatorsTestContainer left, string right)
        {
            throw new NotSupportedException();
        }

        public static OperatorsTestContainer operator ^(OperatorsTestContainer left, string right)
        {
            throw new NotSupportedException();
        }

        #endregion Different Type Second

        #region Special

        public static OperatorsTestContainer operator <<(OperatorsTestContainer left, int right)
        {
            throw new NotSupportedException();
        }

        public static OperatorsTestContainer operator >>(OperatorsTestContainer left, int right)
        {
            throw new NotSupportedException();
        }

        #endregion Special

        #endregion Binary Operators

        // ReSharper restore UnusedParameter.Local
    }
}

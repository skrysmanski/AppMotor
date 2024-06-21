// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Collections;
using System.Numerics;
using System.Reflection;

using AppMotor.Core.ComponentModel;
using AppMotor.Core.Extensions;

using JetBrains.Annotations;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.Extensions;

public sealed class TypeExtensionsTests
{
    [Theory]
    [InlineData(typeof(string),     false)]
    [InlineData(typeof(char),       false)]
    [InlineData(typeof(bool),       false)]
    [InlineData(typeof(IntPtr),     false)]
    [InlineData(typeof(UIntPtr),    false)]
    [InlineData(typeof(byte),       true)]
    [InlineData(typeof(sbyte),      true)]
    [InlineData(typeof(short),      true)]
    [InlineData(typeof(ushort),     true)]
    [InlineData(typeof(int),        true)]
    [InlineData(typeof(uint),       true)]
    [InlineData(typeof(long),       true)]
    [InlineData(typeof(ulong),      true)]
    [InlineData(typeof(Int128),     true)]
    [InlineData(typeof(UInt128),    true)]
    [InlineData(typeof(BigInteger), true)]
    [InlineData(typeof(float),      true)]
    [InlineData(typeof(double),     true)]
    [InlineData(typeof(Half),       true)]
    [InlineData(typeof(decimal),    true)]
    public void Test_IsNumericType(Type type, bool expectedResult)
    {
        type.IsNumericType(includeNullables: false).ShouldBe(expectedResult);

        if (expectedResult)
        {
            var nullableType = typeof(Nullable<>).MakeGenericType(type);
            nullableType.IsNumericType(includeNullables: false).ShouldBe(false);
            nullableType.IsNumericType(includeNullables: true).ShouldBe(true);
        }
    }

    [Theory]
    [InlineData(typeof(string),     false)]
    [InlineData(typeof(char),       false)]
    [InlineData(typeof(bool),       false)]
    [InlineData(typeof(IntPtr),     false)]
    [InlineData(typeof(UIntPtr),    false)]
    [InlineData(typeof(byte),       true)]
    [InlineData(typeof(sbyte),      true)]
    [InlineData(typeof(short),      true)]
    [InlineData(typeof(ushort),     true)]
    [InlineData(typeof(int),        true)]
    [InlineData(typeof(uint),       true)]
    [InlineData(typeof(long),       true)]
    [InlineData(typeof(ulong),      true)]
    [InlineData(typeof(Int128),     true)]
    [InlineData(typeof(UInt128),    true)]
    [InlineData(typeof(BigInteger), true)]
    [InlineData(typeof(float),      false)]
    [InlineData(typeof(double),     false)]
    [InlineData(typeof(Half),       false)]
    [InlineData(typeof(decimal),    false)]
    public void Test_IsNumericIntegerType(Type type, bool expectedResult)
    {
        type.IsNumericIntegerType(includeNullables: false).ShouldBe(expectedResult);

        if (expectedResult)
        {
            var nullableType = typeof(Nullable<>).MakeGenericType(type);
            nullableType.IsNumericIntegerType(includeNullables: false).ShouldBe(false);
            nullableType.IsNumericIntegerType(includeNullables: true).ShouldBe(true);
        }
    }

    [Theory]
    [InlineData(typeof(string),     false)]
    [InlineData(typeof(char),       false)]
    [InlineData(typeof(bool),       false)]
    [InlineData(typeof(IntPtr),     false)]
    [InlineData(typeof(UIntPtr),    false)]
    [InlineData(typeof(byte),       false)]
    [InlineData(typeof(sbyte),      false)]
    [InlineData(typeof(short),      false)]
    [InlineData(typeof(ushort),     false)]
    [InlineData(typeof(int),        false)]
    [InlineData(typeof(uint),       false)]
    [InlineData(typeof(long),       false)]
    [InlineData(typeof(ulong),      false)]
    [InlineData(typeof(Int128),     false)]
    [InlineData(typeof(UInt128),    false)]
    [InlineData(typeof(BigInteger), false)]
    [InlineData(typeof(float),      true)]
    [InlineData(typeof(double),     true)]
    [InlineData(typeof(Half),       true)]
    [InlineData(typeof(decimal),    true)]
    public void Test_IsNumericFloatType(Type type, bool expectedResult)
    {
        type.IsNumericFloatType(includeNullables: false).ShouldBe(expectedResult);

        if (expectedResult)
        {
            var nullableType = typeof(Nullable<>).MakeGenericType(type);
            nullableType.IsNumericFloatType(includeNullables: false).ShouldBe(false);
            nullableType.IsNumericFloatType(includeNullables: true).ShouldBe(true);
        }
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

        typeof(ClassB).Is<ClassA>().ShouldBe(true);
        typeof(ClassB).Is(typeof(ClassA)).ShouldBe(true);

        typeof(ClassA).Is<ClassB>().ShouldBe(false);
        typeof(ClassA).Is(typeof(ClassB)).ShouldBe(false);

        typeof(ITestInterface).Is<ITestInterface>().ShouldBe(true);
        typeof(ITestInterface).Is(typeof(ITestInterface)).ShouldBe(true);

        typeof(object).Is<ITestInterface>().ShouldBe(false);
        typeof(object).Is(typeof(ITestInterface)).ShouldBe(false);

        typeof(ITestInterface).Is<object>().ShouldBe(true);
        typeof(ITestInterface).Is(typeof(object)).ShouldBe(true);

        typeof(object).Is<object>().ShouldBe(true);
        typeof(object).Is(typeof(object)).ShouldBe(true);

        typeof(ClassB).Is<object>().ShouldBe(true);
        typeof(ClassB).Is(typeof(object)).ShouldBe(true);

        typeof(int).Is<object>().ShouldBe(true);
        typeof(int).Is(typeof(object)).ShouldBe(true);

        typeof(ClassB).Is<int>().ShouldBe(false);
        typeof(ClassB).Is(typeof(int)).ShouldBe(false);
    }

    [Fact]
    public void Test_Is_OpenGeneric()
    {
        // Non-nested types
        typeof(List<string>).Is(typeof(IReadOnlyCollection<>)).ShouldBe(true);
        typeof(List<>).Is(typeof(IReadOnlyCollection<>)).ShouldBe(true);
        typeof(List<>).Is(typeof(IReadOnlyCollection<string>)).ShouldBe(false);
        typeof(List<>).Is(typeof(object)).ShouldBe(true);
        typeof(List<>).Is(typeof(int)).ShouldBe(false);
        typeof(List<int>).Is(typeof(GenericClassA<>)).ShouldBe(false);

        // Nested types
        typeof(GenericClassB<int, string>).Is(typeof(IGenericTestInterface<>)).ShouldBe(true);
        typeof(GenericClassB<int, string>).Is(typeof(GenericClassA<>)).ShouldBe(true);
        typeof(GenericClassC<int, string>).Is(typeof(GenericClassA<>)).ShouldBe(true);

        typeof(IGenericTestInterface<>).Is(typeof(object)).ShouldBe(true);

        typeof(GenericStructA<string>).Is(typeof(IGenericTestInterface<>)).ShouldBe(true);

        typeof(MultiIEnumerableTestClass).Is(typeof(IEnumerable<>)).ShouldBe(true);
    }

    private interface ITestInterface;

    private class ClassA : ITestInterface;

    private class ClassB : ClassA;

    private interface IGenericTestInterface<in T>
    {
        [UsedImplicitly] // only required to exist
        void DoSomething(T value);
    }

    private class GenericClassA<TValue1> : IGenericTestInterface<TValue1>
    {
        public void DoSomething(TValue1 value) => throw new NotSupportedException();
    }

    private class GenericClassB<TValue1, TValue2> : GenericClassA<TValue1>, IGenericTestInterface<TValue2>
    {
        public void DoSomething(TValue2 value) => throw new NotSupportedException();
    }

    // For code coverage purposes
    private class GenericClassC<TValue1, TValue2> : GenericClassB<TValue1, TValue2>;

    private struct GenericStructA<TValue> : IGenericTestInterface<TValue>
    {
        public void DoSomething(TValue value) => throw new NotSupportedException();
    }

    [Theory]
    [InlineData(typeof(List<string>), typeof(string))]
    [InlineData(typeof(IReadOnlyCollection<int>), typeof(int))]
    [InlineData(typeof(IEnumerable<int>), typeof(int))]
    [InlineData(typeof(bool?[]), typeof(bool?))]
    [InlineData(typeof(string), typeof(char))]
    [InlineData(typeof(IEnumerable), null)]
    [InlineData(typeof(ICollection), null)]
    [InlineData(typeof(IList), null)]
    [InlineData(typeof(StringComparer), null)]
    public void Test_GetCollectionItemType(Type typeToTest, Type? expectedCollectionItemType)
    {
        typeToTest.GetCollectionItemType().ShouldBe(expectedCollectionItemType);
    }

    [Theory]
    [InlineData(typeof(List<>))]
    [InlineData(typeof(IReadOnlyCollection<>))]
    [InlineData(typeof(IEnumerable<>))]
    public void Test_GetCollectionItemType_OpenGeneric(Type typeToTest)
    {
        // Test
        var collectionItemType = typeToTest.GetCollectionItemType();

        // Verify
        collectionItemType.ShouldNotBeNull();
        collectionItemType.IsGenericTypeParameter.ShouldBe(true);
    }

    [Fact]
    public void Test_GetCollectionItemType_AmbiguousMatch()
    {
        Should.Throw<AmbiguousMatchException>(() => typeof(MultiIEnumerableTestClass).GetCollectionItemType());
    }

    private class MultiIEnumerableTestClass : List<int>, IEnumerable<string>
    {
        public new IEnumerator<string> GetEnumerator() => throw new NotSupportedException();
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

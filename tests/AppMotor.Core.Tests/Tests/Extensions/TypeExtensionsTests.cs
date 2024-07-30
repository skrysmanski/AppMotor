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

/// <summary>
/// Tests for <see cref="Core.Extensions.TypeExtensions"/>.
/// </summary>
public sealed class TypeExtensionsTests
{
    [Fact]
    public void Test_GetCSharpName()
    {
        typeof(string).GetCSharpName().ShouldBe("string");
        typeof(string).GetCSharpName(includeNamespaces: true).ShouldBe("string");
        typeof(string).GetCSharpName(includeNamespaces: false).ShouldBe("string");

        typeof(List<string>).GetCSharpName().ShouldBe("List<string>");
        typeof(List<string>).GetCSharpName(includeNamespaces: true).ShouldBe("System.Collections.Generic.List<string>");
        typeof(List<string>).GetCSharpName(includeNamespaces: false).ShouldBe("List<string>");
    }

    [Fact]
    public void Test_GetDefaultValue()
    {
        typeof(string).GetDefaultValue().ShouldBe(null);
        typeof(int).GetDefaultValue().ShouldBe(0);
        typeof(bool).GetDefaultValue().ShouldBe(false);
    }

    [Fact]
    public void Test_GetDefaultValue_NullableValueTypes()
    {
        // Verify assumptions
        default(bool?).ShouldBe(null);
        default(int?).ShouldBe(null);

        // Tests
        typeof(bool?).GetDefaultValue().ShouldBe(null);
        typeof(int?).GetDefaultValue().ShouldBe(null);
    }

    /// <summary>
    /// Tests that mutating the default value of a mutable struct doesn't mutate the default value itself.
    /// This version works with regular C# (where such a thing isn't possible - see <see cref="Test_GetDefaultValue_MutableStruct_Reflection"/>).
    /// </summary>
    [Fact]
    public void Test_GetDefaultValue_MutableStruct_Regular()
    {
        var value1 = (MutableTestStruct)typeof(MutableTestStruct).GetDefaultValue()!;
        value1.Value.ShouldBe(0);

        value1.Value = 42;

        var value2 = (MutableTestStruct)typeof(MutableTestStruct).GetDefaultValue()!;
        // Value must not carry over from previous default value.
        value2.Value.ShouldBe(0);
    }

    /// <summary>
    /// Tests that mutating the default value of a mutable struct doesn't mutate the default value itself.
    /// This version works with reflection (which seems to be the only way to mutate boxed structs).
    /// </summary>
    [Fact]
    public void Test_GetDefaultValue_MutableStruct_Reflection()
    {
        var value1 = typeof(MutableTestStruct).GetDefaultValue()!;
        ((MutableTestStruct)value1).Value.ShouldBe(0);

        typeof(MutableTestStruct).GetProperty(nameof(MutableTestStruct.Value))!.SetValue(value1, 42);

        var value2 = (MutableTestStruct)typeof(MutableTestStruct).GetDefaultValue()!;
        // Value must not carry over from previous default value.
        value2.Value.ShouldBe(0);
    }

    private struct MutableTestStruct
    {
        public int Value { get; set; }
    }

    /// <summary>
    /// Tests that <see cref="Core.Extensions.TypeExtensions.GetDefaultValue"/> doesn't call the parameterless constructor on structs
    /// (which are allowed since some previous C# version).
    /// </summary>
    [Fact]
    public void Test_GetDefaultValue_StructWithParameterlessConstructor()
    {
        // Verify assumption
        default(TestStructWithParameterlessConstructor).Value.ShouldBe(0);

        // Test
        ((TestStructWithParameterlessConstructor)typeof(TestStructWithParameterlessConstructor).GetDefaultValue()!).Value.ShouldBe(0);
    }

    private readonly struct TestStructWithParameterlessConstructor
    {
        public int Value { get; }

        // ReSharper disable once UnusedMember.Local
        public TestStructWithParameterlessConstructor()
        {
            this.Value = 43;
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

        typeof(Nullable<>).IsNullableValueType().ShouldBe(false);
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

        typeof(ClassA).Is<ClassB>().ShouldBe(false);
        typeof(ClassA).Is(typeof(ClassB)).ShouldBe(false);

        typeof(ClassA).Is<ITestInterface>().ShouldBe(true);
        typeof(ClassA).Is(typeof(ITestInterface)).ShouldBe(true);

        typeof(ITestInterface).Is<ITestInterface>().ShouldBe(true);
        typeof(ITestInterface).Is(typeof(ITestInterface)).ShouldBe(true);

        typeof(ClassA).Is<ClassA>().ShouldBe(true);
        typeof(ClassA).Is(typeof(ClassA)).ShouldBe(true);
        typeof(ClassB).Is<ClassB>().ShouldBe(true);
        typeof(ClassB).Is(typeof(ClassB)).ShouldBe(true);

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

        typeof(GenericClassA<string>).Is(typeof(GenericClassA<string>)).ShouldBe(true);
        typeof(GenericClassA<string>).Is<GenericClassA<string>>().ShouldBe(true);
        typeof(GenericClassA<string>).Is(typeof(GenericClassA<int>)).ShouldBe(false);
        typeof(GenericClassA<string>).Is<GenericClassA<int>>().ShouldBe(false);

        typeof(SealedClass).Is(typeof(SealedClass)).ShouldBe(true);
        typeof(SealedClass).Is<SealedClass>().ShouldBe(true);

        typeof(MultiIEnumerableTestClass).Is<IEnumerable<int>>().ShouldBe(true);
        typeof(MultiIEnumerableTestClass).Is<IEnumerable<string>>().ShouldBe(true);
        typeof(MultiIEnumerableTestClass).Is<IEnumerable<bool>>().ShouldBe(false);
    }

    [Theory]
    // Non-nested types
    [InlineData(typeof(List<string>), typeof(IReadOnlyCollection<>), true)]
    [InlineData(typeof(List<>), typeof(IReadOnlyCollection<>), true)]
    [InlineData(typeof(List<>), typeof(IReadOnlyCollection<string>), false)]
    [InlineData(typeof(List<>), typeof(object), true)]
    [InlineData(typeof(List<>), typeof(int), false)]
    [InlineData(typeof(List<int>), typeof(List<>), true)]
    [InlineData(typeof(List<int>), typeof(GenericClassA<>), false)]

    // Generic structs
    [InlineData(typeof(KeyValuePair<string, int>), typeof(KeyValuePair<,>), true)]
    [InlineData(typeof(KeyValuePair<,>), typeof(KeyValuePair<,>), true)]
    [InlineData(typeof(KeyValuePair<,>), typeof(KeyValuePair<string, int>), false)]

    // Nested tye
    [InlineData(typeof(GenericClassB<int, string>), typeof(IGenericTestInterface<>), true)]
    [InlineData(typeof(GenericClassB<int, string>), typeof(GenericClassA<>), true)]
    [InlineData(typeof(GenericClassC<int, string>), typeof(GenericClassA<>), true)]

    [InlineData(typeof(IGenericTestInterface<>), typeof(object), true)]
    [InlineData(typeof(GenericStructA<string>), typeof(IGenericTestInterface<>), true)]
    [InlineData(typeof(MultiIEnumerableTestClass), typeof(IEnumerable<>), true)]
    public void Test_Is_OpenGeneric(Type typeToCheck, Type typeToCompare, bool expectedResult)
    {
        typeToCheck.Is(typeToCompare).ShouldBe(expectedResult);
    }

    private interface ITestInterface;

    private class ClassA : ITestInterface;

    private class ClassB : ClassA;

    private sealed class SealedClass : ClassA;

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
    [InlineData(typeof(StringComparer), null)]
    public void Test_GetCollectionItemType(Type typeToTest, Type? expectedCollectionItemType)
    {
        typeToTest.GetCollectionItemType().ShouldBe(expectedCollectionItemType);
    }

    [Theory]
    [InlineData(typeof(IEnumerable))]
    [InlineData(typeof(ICollection))]
    [InlineData(typeof(IList))]
    public void Test_GetCollectionItemType_NonGenericCollection(Type typeToTest)
    {
        typeToTest.GetCollectionItemType().ShouldBe(typeof(object));
        typeToTest.GetCollectionItemType(enableNonGenericCollectionSupport: true).ShouldBe(typeof(object));
        typeToTest.GetCollectionItemType(enableNonGenericCollectionSupport: false).ShouldBeNull();
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

    [Fact]
    public void Test_GetParameterlessConstructor()
    {
        typeof(object).GetParameterlessConstructor().ShouldNotBeNull();
        typeof(string).GetParameterlessConstructor().ShouldBeNull();

        typeof(TestClassWithPrivateConstructor).GetParameterlessConstructor().ShouldBeNull();
        typeof(TestClassWithPrivateConstructor).GetParameterlessConstructor(publicOnly: false).ShouldNotBeNull();
    }

    // ReSharper disable once ConvertToStaticClass
    private sealed class TestClassWithPrivateConstructor
    {
        private TestClassWithPrivateConstructor() { }
    }

    [Fact]
    public void Test_TryMakeGenericType()
    {
        // Success
        typeof(INumberBase<>).TryMakeGenericType(typeof(int)).ShouldBe(typeof(INumberBase<int>));

        // Success: Strange but apparently ok
        typeof(INumberBase<>).TryMakeGenericType(typeof(INumberBase<>).GetGenericArguments()[0]).ShouldBe(typeof(INumberBase<>));

        // Failure: constraint violation
        typeof(INumberBase<>).TryMakeGenericType(typeof(string)).ShouldBe(null);

        // Failure: not an open generic
        Should.Throw<InvalidOperationException>(() => typeof(string).TryMakeGenericType(typeof(int)));
        Should.Throw<InvalidOperationException>(() => typeof(INumberBase<int>).TryMakeGenericType(typeof(int)));

        // Failure: wrong type param count
        Should.Throw<ArgumentException>(() => typeof(INumberBase<>).TryMakeGenericType(typeof(int), typeof(long)));
        Should.Throw<ArgumentException>(() => typeof(INumberBase<>).TryMakeGenericType());

        // Failure: invalid type parameter
        Should.Throw<ArgumentException>(() => typeof(INumberBase<>).TryMakeGenericType(typeof(void)));
        Should.Throw<ArgumentException>(() => typeof(INumberBase<>).TryMakeGenericType(typeof(int*)));
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
        operatorFromEnum.GetParameters().Select(p => p.ParameterType).ShouldBe([typeof(OperatorsTestContainer)]);
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
        operatorFromEnum.GetParameters().Select(p => p.ParameterType).ShouldBe([typeof(OperatorsTestContainer), secondType]);
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
                operatorFromEnum.GetParameters().Select(p => p.ParameterType).ShouldBe([typeof(OperatorsTestContainer), typeof(string)]);
            }
            else
            {
                operatorFromEnum.GetParameters().Select(p => p.ParameterType).ShouldBe([typeof(string), typeof(OperatorsTestContainer)]);
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
        operatorInfo.GetParameters().Select(p => p.ParameterType).ShouldBe([typeof(OperatorsTestContainer), typeof(int)]);
    }

    [Fact]
    public void Test_GetOperator_InvalidOperator()
    {
        Should.Throw<ArgumentException>(() => typeof(OperatorsTestContainer).GetOperator("xxx"));
        Should.Throw<ArgumentException>(() => typeof(OperatorsTestContainer).GetOperator("xxx", typeof(string)));
    }

    [Fact]
    public void Test_GetImplicitOperatorFrom()
    {
        // Test
        var operatorInfo = typeof(OperatorsTestContainer).GetImplicitOperatorFrom(typeof(int));

        // Verify
        operatorInfo.ShouldNotBeNull();
        operatorInfo.ReturnType.ShouldBe(typeof(OperatorsTestContainer));
        operatorInfo.GetParameters().Select(p => p.ParameterType).ShouldBe([typeof(int)]);
    }

    [Theory]
    [InlineData(typeof(string))]
    [InlineData(typeof(bool))]
    public void Test_GetImplicitOperatorTo(Type otherType)
    {
        // Test
        var operatorInfo = typeof(OperatorsTestContainer).GetImplicitOperatorTo(otherType);

        // Verify
        operatorInfo.ShouldNotBeNull();
        operatorInfo.ReturnType.ShouldBe(otherType);
        operatorInfo.GetParameters().Select(p => p.ParameterType).ShouldBe([typeof(OperatorsTestContainer)]);
    }

    [Fact]
    public void Test_GetExplicitOperatorFrom()
    {
        // Test
        var operatorInfo = typeof(OperatorsTestContainer).GetExplicitOperatorFrom(typeof(long));

        // Verify
        operatorInfo.ShouldNotBeNull();
        operatorInfo.ReturnType.ShouldBe(typeof(OperatorsTestContainer));
        operatorInfo.GetParameters().Select(p => p.ParameterType).ShouldBe([typeof(long)]);
    }

    [Theory]
    [InlineData(typeof(ulong))]
    [InlineData(typeof(ushort))]
    public void Test_GetExplicitOperatorTo(Type otherType)
    {
        // Test
        var operatorInfo = typeof(OperatorsTestContainer).GetExplicitOperatorTo(otherType);

        // Verify
        operatorInfo.ShouldNotBeNull();
        operatorInfo.ReturnType.ShouldBe(otherType);
        operatorInfo.GetParameters().Select(p => p.ParameterType).ShouldBe([typeof(OperatorsTestContainer)]);
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

        #region Implicit & Explicit Operators

        public static implicit operator OperatorsTestContainer(int value) => throw new NotSupportedException();

        // NOTE: The following two operators just differ by their return type - which is normally not supported for C# methods.
        public static implicit operator string(OperatorsTestContainer value) => throw new NotSupportedException();
        public static implicit operator bool(OperatorsTestContainer value) => throw new NotSupportedException();

        public static explicit operator OperatorsTestContainer(long value) => throw new NotSupportedException();

        // NOTE: The following two operators just differ by their return type - which is normally not supported for C# methods.
        public static explicit operator ulong(OperatorsTestContainer value) => throw new NotSupportedException();
        public static explicit operator ushort(OperatorsTestContainer value) => throw new NotSupportedException();

        #endregion Implicit & Explicit Operators

        // ReSharper restore UnusedParameter.Local
    }
}

// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.Core.ComponentModel;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.ComponentModel;

/// <summary>
/// Tests for <see cref="ParameterlessConstructor"/>.
/// </summary>
public sealed class ParameterlessConstructorTests
{
    [Theory]
    [InlineData(typeof(TestClassWithPublicConstructor), true)]
    [InlineData(typeof(TestClassWithProtectedConstructor), false)]
    [InlineData(typeof(TestClassWithInternalConstructor), false)]
    [InlineData(typeof(TestClassWithPrivateConstructor), false)]
    [InlineData(typeof(TestStructWithoutConstructors), true)]
    [InlineData(typeof(TestStructWithoutExplicitParameterlessConstructor), true)]
    [InlineData(typeof(TestStructWithExplicitParameterlessConstructor), true)]
    [InlineData(typeof(TestStructWithoutConstructors?), true)]
    [InlineData(typeof(TestStructWithoutExplicitParameterlessConstructor?), true)]
    [InlineData(typeof(TestStructWithExplicitParameterlessConstructor?), true)]
    [InlineData(typeof(object), true)]
    public void Test_GetForType_Success(Type type, bool isPublicConstructor)
    {
        // Test: public constructor
        var publicConstructor = ParameterlessConstructor.GetForType(type);
        if (isPublicConstructor)
        {
            publicConstructor.ShouldNotBeNull();

            var instanceFromPublicConstructor = publicConstructor.Invoke();

            var underlyingType = Nullable.GetUnderlyingType(type);
            instanceFromPublicConstructor.GetType().ShouldBe(underlyingType ?? type);
            return;
        }

        publicConstructor.ShouldBeNull();

        // Test: non-public constructor
        var nonPublicConstructor = ParameterlessConstructor.GetForType(type, publicOnly: false);

        nonPublicConstructor.ShouldNotBeNull();

        var instanceFromNonPublicConstructor = nonPublicConstructor.Invoke();
        instanceFromNonPublicConstructor.GetType().ShouldBe(type);
    }

    [Theory]
    [InlineData(typeof(TestClassWithoutParameterlessConstructor))]
    [InlineData(typeof(AbstractTestClass))]
    [InlineData(typeof(IReadOnlyCollection<int>))]
    [InlineData(typeof(StaticTestClass))]
    [InlineData(typeof(int[]))]
    [InlineData(typeof(StringComparison))] // enum
    [InlineData(typeof(StringComparison?))] // nullable enum
    [InlineData(typeof(int))] // primitive type
    [InlineData(typeof(int?))] // nullable primitive type
    [InlineData(typeof(TestClassWithStaticConstructor))]
    [InlineData(typeof(string))]
    public void Test_GetForType_ReturnsNull(Type type)
    {
        ParameterlessConstructor.GetForType(type).ShouldBeNull();
        ParameterlessConstructor.GetForType(type, publicOnly: false).ShouldBeNull();
    }

    private sealed class TestClassWithPublicConstructor;

    // ReSharper disable once ConvertToStaticClass
    private class TestClassWithProtectedConstructor
    {
        protected TestClassWithProtectedConstructor() { }
    }

    // ReSharper disable once ConvertToStaticClass
    private sealed class TestClassWithInternalConstructor
    {
        // ReSharper disable once EmptyConstructor
        internal TestClassWithInternalConstructor() { }
    }

    // ReSharper disable once ConvertToStaticClass
    private sealed class TestClassWithPrivateConstructor
    {
        private TestClassWithPrivateConstructor() { }
    }

    private abstract class AbstractTestClass;

    /// <summary>
    /// Has a static (parameterless) constructor but no parameterless instance constructor.
    /// </summary>
    private sealed class TestClassWithStaticConstructor
    {
        private static readonly int s_value;

        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        public int Value { get; }

        static TestClassWithStaticConstructor()
        {
            s_value = new Random().Next();
        }

        public TestClassWithStaticConstructor(int? value)
        {
            this.Value = value ?? s_value;
        }
    }

    private static class StaticTestClass;

    private sealed class TestClassWithoutParameterlessConstructor
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        public int Value { get; }

        public TestClassWithoutParameterlessConstructor(int value)
        {
            this.Value = value;
        }
    }

    private readonly struct TestStructWithoutConstructors;

    private readonly struct TestStructWithoutExplicitParameterlessConstructor
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        public int Value { get; }

        // ReSharper disable once UnusedMember.Local
        public TestStructWithoutExplicitParameterlessConstructor(int value)
        {
            this.Value = value;
        }
    }

    private readonly struct TestStructWithExplicitParameterlessConstructor
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        public int Value { get; }

        // ReSharper disable once UnusedMember.Local
        public TestStructWithExplicitParameterlessConstructor()
        {
            this.Value = new Random().Next();
        }
    }
}

// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;

using AppMotor.Core.Extensions;
using AppMotor.Core.Utils;
using AppMotor.TestCore.TestData;

using JetBrains.Annotations;

using Shouldly;

using Xunit;
using Xunit.Abstractions;

namespace AppMotor.Core.Tests.Utils;

/// <summary>
/// Tests for <see cref="NumberTypeConverter"/>.
/// </summary>
public sealed class NumberTypeConverterTests
{
    // Defines the order of types so that we can have a stable test case order.
    private static readonly Type[] s_numberTypes = [
        typeof(byte),
        typeof(sbyte),
        typeof(char),
        typeof(short),
        typeof(ushort),
        typeof(int),
        typeof(uint),
        typeof(long),
        typeof(ulong),
        typeof(Int128),
        typeof(UInt128),
        typeof(BigInteger),
        typeof(Half),
        typeof(float),
        typeof(double),
        typeof(decimal),
        typeof(CustomNumberType),
    ];

    private static readonly Dictionary<Type, Func<int, object>> s_numberFactories = new()
    {
        [typeof(byte)] = val => (byte)val,
        [typeof(sbyte)] = val => (sbyte)val,
        [typeof(char)] = val => (char)val,
        [typeof(short)] = val => (short)val,
        [typeof(ushort)] = val => (ushort)val,
        [typeof(int)] = val => val,
        [typeof(uint)] = val => (uint)val,
        [typeof(long)] = val => (long)val,
        [typeof(ulong)] = val => (ulong)val,
        [typeof(Int128)] = val => (Int128)val,
        [typeof(UInt128)] = val => (UInt128)val,
        [typeof(BigInteger)] = val => (BigInteger)val,
        [typeof(Half)] = val => (Half)val,
        [typeof(float)] = val => (float)val,
        [typeof(double)] = val => (double)val,
        [typeof(decimal)] = val => (decimal)val,
        [typeof(CustomNumberType)] = val => new CustomNumberType(val),
    };

    [MustUseReturnValue]
    private static object GetNumberForType(Type targetType, int number)
    {
        var nonNullableTargetType = Nullable.GetUnderlyingType(targetType);
        if (nonNullableTargetType is not null)
        {
            return GetNumberForType(nonNullableTargetType, number);
        }

        return s_numberFactories[targetType](number);
    }

    [Theory]
    [ClassData(typeof(NumberTypeConverterTestDataProvider))]
    public void Test_ConvertChecked(NumberTypeConverterTestData testData)
    {
        // Setup
        object sourceNumber = GetNumberForType(testData.SourceType, 42);
        object expectedTargetNumber = GetNumberForType(testData.TargetType, 42);

        // Test
        var convertedNumber = NumberTypeConverter.ConvertChecked(sourceNumber, testData.TargetType);

        // Verify
        convertedNumber.ShouldBe(expectedTargetNumber);
    }

    [Theory]
    [InlineData(-1, typeof(byte))]
    [InlineData(256, typeof(byte))]
    [InlineData(short.MinValue - 1, typeof(short))]
    [InlineData(short.MaxValue + 1, typeof(short))]
    public void Test_ConvertChecked_Overflow(int sourceNumber, Type targetNumberType)
    {
        Should.Throw<OverflowException>(() => NumberTypeConverter.ConvertChecked(sourceNumber, targetNumberType));
    }

    [Theory]
    [InlineData(typeof(string), typeof(int), "Type")]
    [InlineData(42, typeof(string), "string")]
    public void Test_ConvertChecked_InvalidNumberType(object source, Type targetType, string typeNameForException)
    {
        // Test
        var ex = Should.Throw<InvalidOperationException>(() => NumberTypeConverter.ConvertChecked(source, targetType));

        // Verify
        ex.Message.ShouldContain($"Type '{typeNameForException}' is not a recognized number type.");
    }

    [Theory]
    [ClassData(typeof(NumberTypeConverterTestDataProvider))]
    public void Test_ConvertSaturating(NumberTypeConverterTestData testData)
    {
        // Setup
        object sourceNumber = GetNumberForType(testData.SourceType, 42);
        object expectedTargetNumber = GetNumberForType(testData.TargetType, 42);

        // Test
        var convertedNumber = NumberTypeConverter.ConvertSaturating(sourceNumber, testData.TargetType);

        // Verify
        convertedNumber.ShouldBe(expectedTargetNumber);
    }

    [Theory]
    [InlineData(-1, typeof(byte), 0)]
    [InlineData(256, typeof(byte), 255)]
    [InlineData(short.MinValue - 1, typeof(short), short.MinValue)]
    [InlineData(short.MaxValue + 1, typeof(short), short.MaxValue)]
    public void Test_ConvertSaturating_Overflow(int sourceNumber, Type targetNumberType, int expectedTargetNumber)
    {
        // Test
        var targetNumber = NumberTypeConverter.ConvertSaturating(sourceNumber, targetNumberType);

        // Verify
        targetNumber.ShouldBe(expectedTargetNumber);
    }

    [Theory]
    [InlineData(typeof(string), typeof(int), "Type")]
    [InlineData(42, typeof(string), "string")]
    public void Test_ConvertSaturating_InvalidNumberType(object source, Type targetType, string typeNameForException)
    {
        // Test
        var ex = Should.Throw<InvalidOperationException>(() => NumberTypeConverter.ConvertSaturating(source, targetType));

        // Verify
        ex.Message.ShouldContain($"Type '{typeNameForException}' is not a recognized number type.");
    }

    [Theory]
    [ClassData(typeof(NumberTypeConverterTestDataProvider))]
    public void Test_ConvertTruncating(NumberTypeConverterTestData testData)
    {
        // Setup
        object sourceNumber = GetNumberForType(testData.SourceType, 42);
        object expectedTargetNumber = GetNumberForType(testData.TargetType, 42);

        // Test
        var convertedNumber = NumberTypeConverter.ConvertTruncating(sourceNumber, testData.TargetType);

        // Verify
        convertedNumber.ShouldBe(expectedTargetNumber);
    }

    [Theory]
    [InlineData(-1, typeof(byte), 255)]
    [InlineData(256, typeof(byte), 0)]
    [InlineData(short.MinValue - 1, typeof(short), short.MaxValue)]
    [InlineData(short.MaxValue + 1, typeof(short), short.MinValue)]
    public void Test_ConvertTruncating_Overflow(int sourceNumber, Type targetNumberType, int expectedTargetNumber)
    {
        // Test
        var targetNumber = NumberTypeConverter.ConvertTruncating(sourceNumber, targetNumberType);

        // Verify
        targetNumber.ShouldBe(expectedTargetNumber);
    }

    [Theory]
    [InlineData(typeof(string), typeof(int), "Type")]
    [InlineData(42, typeof(string), "string")]
    public void Test_ConvertTruncating_InvalidNumberType(object source, Type targetType, string typeNameForException)
    {
        // Test
        var ex = Should.Throw<InvalidOperationException>(() => NumberTypeConverter.ConvertTruncating(source, targetType));

        // Verify
        ex.Message.ShouldContain($"Type '{typeNameForException}' is not a recognized number type.");
    }

    private sealed class NumberTypeConverterTestDataProvider : TestDataBase
    {
        /// <inheritdoc />
        public override IEnumerator<object?[]> GetEnumerator()
        {
            foreach (var sourceNumberType in s_numberTypes)
            {
                foreach (var targetNumberType in s_numberTypes)
                {
                    yield return [new NumberTypeConverterTestData(sourceNumberType, targetNumberType)];

                    if (targetNumberType.IsValueType)
                    {
                        yield return [new NumberTypeConverterTestData(sourceNumberType, typeof(Nullable<>).MakeGenericType(targetNumberType))];
                    }
                }
            }
        }
    }

    /// <summary>
    /// This custom test data class exists so that we can customize the test case description (via <see cref="ToString"/>).
    /// </summary>
    public sealed class NumberTypeConverterTestData : IXunitSerializable
    {
        public Type SourceType { get; private set; }

        public Type TargetType { get; private set; }

        [Obsolete("Only for serializer")]
        [UsedImplicitly]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public NumberTypeConverterTestData()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
        }

        public NumberTypeConverterTestData(Type sourceType, Type targetType)
        {
            this.SourceType = sourceType;
            this.TargetType = targetType;
        }

        /// <inheritdoc />
        public void Deserialize(IXunitSerializationInfo info)
        {
            this.SourceType = info.GetValue<Type>(nameof(this.SourceType));
            this.TargetType = info.GetValue<Type>(nameof(this.TargetType));
        }

        /// <inheritdoc />
        public void Serialize(IXunitSerializationInfo info)
        {
            info.AddValue(nameof(this.SourceType), this.SourceType);
            info.AddValue(nameof(this.TargetType), this.TargetType);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{this.SourceType.GetCSharpName()} -> {this.TargetType.GetCSharpName()}";
        }
    }

    /// <summary>
    /// A custom number type.
    /// </summary>
    private sealed class CustomNumberType : INumberBase<CustomNumberType>
    {
        private readonly int _value;

        public CustomNumberType(int value)
        {
            this._value = value;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj) => ReferenceEquals(this, obj) || (obj is CustomNumberType other && Equals(other));

        /// <inheritdoc />
        public bool Equals(CustomNumberType? other) => this._value == other?._value;

        /// <inheritdoc />
        public override int GetHashCode() => this._value;

        /// <inheritdoc />
        public override string ToString() => this._value.ToString();

        /// <inheritdoc />
        public static bool TryConvertFromChecked<TOther>(TOther value, [MaybeNullWhen(false)] out CustomNumberType result)
            where TOther : INumberBase<TOther>
        {
            if (!TryConvertChecked(value, out int intResult))
            {
                result = null;
                return false;
            }

            result = new CustomNumberType(intResult);
            return true;
        }

        /// <inheritdoc />
        public static bool TryConvertToChecked<TOther>(CustomNumberType value, [MaybeNullWhen(false)] out TOther result)
            where TOther : INumberBase<TOther>
        {
            return TryConvertChecked(value._value, out result);
        }

        [MustUseReturnValue]
        private static bool TryConvertChecked<TSelf, TOther>(TSelf value, [MaybeNullWhen(false)] out TOther result)
            where TSelf : INumberBase<TSelf>
            where TOther : INumberBase<TOther>

        {
            if (typeof(TSelf) == typeof(TOther))
            {
                result = (TOther)(object)value;
                return true;
            }

            return TSelf.TryConvertToChecked(value, out result) || TOther.TryConvertFromChecked(value, out result);
        }

        /// <inheritdoc />
        public static bool TryConvertFromSaturating<TOther>(TOther value, [MaybeNullWhen(false)] out CustomNumberType result)
            where TOther : INumberBase<TOther>
        {
            if (!TryConvertSaturating(value, out int intResult))
            {
                result = null;
                return false;
            }

            result = new CustomNumberType(intResult);
            return true;
        }

        /// <inheritdoc />
        public static bool TryConvertToSaturating<TOther>(CustomNumberType value, [MaybeNullWhen(false)] out TOther result)
            where TOther : INumberBase<TOther>
        {
            return TryConvertSaturating(value._value, out result);
        }

        [MustUseReturnValue]
        private static bool TryConvertSaturating<TSelf, TOther>(TSelf value, [MaybeNullWhen(false)] out TOther result)
            where TSelf : INumberBase<TSelf>
            where TOther : INumberBase<TOther>

        {
            if (typeof(TSelf) == typeof(TOther))
            {
                result = (TOther)(object)value;
                return true;
            }

            return TSelf.TryConvertToSaturating(value, out result) || TOther.TryConvertFromSaturating(value, out result);
        }

        /// <inheritdoc />
        public static bool TryConvertFromTruncating<TOther>(TOther value, [MaybeNullWhen(false)] out CustomNumberType result)
            where TOther : INumberBase<TOther>
        {
            if (!TryConvertTruncating(value, out int intResult))
            {
                result = null;
                return false;
            }

            result = new CustomNumberType(intResult);
            return true;
        }

        /// <inheritdoc />
        public static bool TryConvertToTruncating<TOther>(CustomNumberType value, [MaybeNullWhen(false)] out TOther result)
            where TOther : INumberBase<TOther>
        {
            return TryConvertTruncating(value._value, out result);
        }

        [MustUseReturnValue]
        private static bool TryConvertTruncating<TSelf, TOther>(TSelf value, [MaybeNullWhen(false)] out TOther result)
            where TSelf : INumberBase<TSelf>
            where TOther : INumberBase<TOther>

        {
            if (typeof(TSelf) == typeof(TOther))
            {
                result = (TOther)(object)value;
                return true;
            }

            return TSelf.TryConvertToTruncating(value, out result) || TOther.TryConvertFromTruncating(value, out result);
        }

        #region Unimplemented

        /// <inheritdoc />
        public string ToString(string? format, IFormatProvider? formatProvider) => throw new NotSupportedException();

        /// <inheritdoc />
        public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => throw new NotSupportedException();

        /// <inheritdoc />
        public static CustomNumberType Parse(string s, IFormatProvider? provider) => throw new NotSupportedException();

        /// <inheritdoc />
        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out CustomNumberType result) => throw new NotSupportedException();

        /// <inheritdoc />
        public static CustomNumberType Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => throw new NotSupportedException();

        /// <inheritdoc />
        public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out CustomNumberType result) => throw new NotSupportedException();

        /// <inheritdoc />
        public static CustomNumberType operator +(CustomNumberType left, CustomNumberType right) => throw new NotSupportedException();

        /// <inheritdoc />
        public static CustomNumberType AdditiveIdentity => throw new NotSupportedException();

        /// <inheritdoc />
        public static CustomNumberType operator --(CustomNumberType value) => throw new NotSupportedException();

        /// <inheritdoc />
        public static CustomNumberType operator /(CustomNumberType left, CustomNumberType right) => throw new NotSupportedException();

        /// <inheritdoc />
        public static bool operator ==(CustomNumberType? left, CustomNumberType? right) => throw new NotSupportedException();

        /// <inheritdoc />
        public static bool operator !=(CustomNumberType? left, CustomNumberType? right) => throw new NotSupportedException();

        /// <inheritdoc />
        public static CustomNumberType operator ++(CustomNumberType value) => throw new NotSupportedException();

        /// <inheritdoc />
        public static CustomNumberType MultiplicativeIdentity => throw new NotSupportedException();

        /// <inheritdoc />
        public static CustomNumberType operator *(CustomNumberType left, CustomNumberType right) => throw new NotSupportedException();

        /// <inheritdoc />
        public static CustomNumberType operator -(CustomNumberType left, CustomNumberType right) => throw new NotSupportedException();

        /// <inheritdoc />
        public static CustomNumberType operator -(CustomNumberType value) => throw new NotSupportedException();

        /// <inheritdoc />
        public static CustomNumberType operator +(CustomNumberType value) => throw new NotSupportedException();

        /// <inheritdoc />
        public static CustomNumberType Abs(CustomNumberType value) => throw new NotSupportedException();

        /// <inheritdoc />
        public static bool IsCanonical(CustomNumberType value) => throw new NotSupportedException();

        /// <inheritdoc />
        public static bool IsComplexNumber(CustomNumberType value) => throw new NotSupportedException();

        /// <inheritdoc />
        public static bool IsEvenInteger(CustomNumberType value) => throw new NotSupportedException();

        /// <inheritdoc />
        public static bool IsFinite(CustomNumberType value) => throw new NotSupportedException();

        /// <inheritdoc />
        public static bool IsImaginaryNumber(CustomNumberType value) => throw new NotSupportedException();

        /// <inheritdoc />
        public static bool IsInfinity(CustomNumberType value) => throw new NotSupportedException();

        /// <inheritdoc />
        public static bool IsInteger(CustomNumberType value) => throw new NotSupportedException();

        /// <inheritdoc />
        public static bool IsNaN(CustomNumberType value) => throw new NotSupportedException();

        /// <inheritdoc />
        public static bool IsNegative(CustomNumberType value) => throw new NotSupportedException();

        /// <inheritdoc />
        public static bool IsNegativeInfinity(CustomNumberType value) => throw new NotSupportedException();

        /// <inheritdoc />
        public static bool IsNormal(CustomNumberType value) => throw new NotSupportedException();

        /// <inheritdoc />
        public static bool IsOddInteger(CustomNumberType value) => throw new NotSupportedException();

        /// <inheritdoc />
        public static bool IsPositive(CustomNumberType value) => throw new NotSupportedException();

        /// <inheritdoc />
        public static bool IsPositiveInfinity(CustomNumberType value) => throw new NotSupportedException();

        /// <inheritdoc />
        public static bool IsRealNumber(CustomNumberType value) => throw new NotSupportedException();

        /// <inheritdoc />
        public static bool IsSubnormal(CustomNumberType value) => throw new NotSupportedException();

        /// <inheritdoc />
        public static bool IsZero(CustomNumberType value) => throw new NotSupportedException();

        /// <inheritdoc />
        public static CustomNumberType MaxMagnitude(CustomNumberType x, CustomNumberType y) => throw new NotSupportedException();

        /// <inheritdoc />
        public static CustomNumberType MaxMagnitudeNumber(CustomNumberType x, CustomNumberType y) => throw new NotSupportedException();

        /// <inheritdoc />
        public static CustomNumberType MinMagnitude(CustomNumberType x, CustomNumberType y) => throw new NotSupportedException();

        /// <inheritdoc />
        public static CustomNumberType MinMagnitudeNumber(CustomNumberType x, CustomNumberType y) => throw new NotSupportedException();

        /// <inheritdoc />
        public static CustomNumberType Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider) => throw new NotSupportedException();

        /// <inheritdoc />
        public static CustomNumberType Parse(string s, NumberStyles style, IFormatProvider? provider) => throw new NotSupportedException();

        /// <inheritdoc />
        /// <inheritdoc />
        public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, [MaybeNullWhen(false)] out CustomNumberType result) => throw new NotSupportedException();

        /// <inheritdoc />
        public static bool TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, [MaybeNullWhen(false)] out CustomNumberType result) => throw new NotSupportedException();

        /// <inheritdoc />
        public static CustomNumberType One => throw new NotSupportedException();

        /// <inheritdoc />
        public static int Radix => throw new NotSupportedException();

        /// <inheritdoc />
        public static CustomNumberType Zero => throw new NotSupportedException();

        #endregion Unimplemented
    }
}

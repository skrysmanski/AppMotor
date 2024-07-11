// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Collections.Concurrent;
using System.Numerics;

using AppMotor.Core.Exceptions;
using AppMotor.Core.Extensions;

using JetBrains.Annotations;

namespace AppMotor.Core.Utils;

/// <summary>
/// Allows you to convert numbers from one type into another number type - without generics.
/// </summary>
public static class NumberTypeConverter
{
    private static readonly ConcurrentDictionary<Type, INumberTypeAdapter> s_customNumberTypeAdapters = new();

    /// <summary>
    /// <para>Converts <paramref name="sourceNumber"/> into the target number type. Throws an overflow exception for any values
    /// that fall outside the representable range of <paramref name="targetType"/>.</para>
    ///
    /// <para>This method behaves like a <c>checked</c> cast: <c>var x = checked((byte)y);</c></para>
    /// </summary>
    ///
    /// <param name="sourceNumber">The number to convert.</param>
    /// <param name="targetType">The target number type.</param>
    ///
    /// <remarks>
    /// Uses <see cref="INumberBase{TSelf}.CreateChecked{TOther}"/> under the hood.
    /// </remarks>
    ///
    /// <exception cref="OverflowException">Thrown if <paramref name="sourceNumber"/> doesn't fit into <paramref name="targetType"/>.</exception>
    /// <exception cref="InvalidOperationException">Thrown if either the source type or the target type isn't a number type (i.e. doesn't
    /// implement <see cref="INumberBase{TSelf}"/>).</exception>
    [MustUseReturnValue]
    public static object ConvertChecked(object sourceNumber, Type targetType)
    {
        return Convert(sourceNumber, targetType, CreateMethods.Checked);
    }

    /// <summary>
    /// Converts <paramref name="sourceNumber"/> into the target number type. Returns <c>MinValue</c> or <c>MaxValue</c> for any values
    /// that fall outside the representable range of <paramref name="targetType"/> (e.g. converting 384 to <c>byte</c> will result in
    /// 255; converting -384 to <c>byte</c> will result in 0).
    /// </summary>
    ///
    /// <param name="sourceNumber">The number to convert.</param>
    /// <param name="targetType">The target number type.</param>
    ///
    /// <remarks>
    /// Uses <see cref="INumberBase{TSelf}.CreateSaturating{TOther}"/> under the hood.
    /// </remarks>
    ///
    /// <exception cref="InvalidOperationException">Thrown if either the source type or the target type isn't a number type (i.e. doesn't
    /// implement <see cref="INumberBase{TSelf}"/>).</exception>
    [MustUseReturnValue]
    public static object ConvertSaturating(object sourceNumber, Type targetType)
    {
        return Convert(sourceNumber, targetType, CreateMethods.Saturating);
    }

    /// <summary>
    /// <para>Converts <paramref name="sourceNumber"/> into the target number type. Truncates any bits that fall outside the
    /// representable range of <paramref name="targetType"/> (e.g. converting -384 to <c>byte</c> will result in 128 because it takes the
    /// lowest 8 bits; -384 has a hex representation of 0xFE80, and the lowest 8 bits is 0x80, which is 128).</para>
    ///
    /// <para>This method behaves like a regular (unchecked) number cast: <c>var x = (byte)y;</c></para>
    /// </summary>
    ///
    /// <param name="sourceNumber">The number to convert.</param>
    /// <param name="targetType">The target number type.</param>
    ///
    /// <remarks>
    /// Uses <see cref="INumberBase{TSelf}.CreateTruncating{TOther}"/> under the hood.
    /// </remarks>
    ///
    /// <exception cref="InvalidOperationException">Thrown if either the source type or the target type isn't a number type (i.e. doesn't
    /// implement <see cref="INumberBase{TSelf}"/>).</exception>
    [MustUseReturnValue]
    public static object ConvertTruncating(object sourceNumber, Type targetType)
    {
        return Convert(sourceNumber, targetType, CreateMethods.Truncating);
    }

    [MustUseReturnValue]
    private static object Convert(object sourceNumber, Type targetType, CreateMethods createMethod)
    {
        var sourceType = sourceNumber.GetType();

        if (sourceType == targetType)
        {
            return sourceNumber;
        }

        var sourceTypeAdapter = GetNumberTypeAdapter(sourceType);
        if (sourceTypeAdapter is null)
        {
            throw CreateConversionNotPossibleException(notANumberType: sourceType);
        }

        var nonNullableTargetType = Nullable.GetUnderlyingType(targetType);
        var targetTypeAdapter = nonNullableTargetType is not null ? GetNumberTypeAdapter(nonNullableTargetType) : GetNumberTypeAdapter(targetType);
        if (targetTypeAdapter is null)
        {
            throw CreateConversionNotPossibleException(notANumberType: targetType);
        }

        return sourceTypeAdapter.ConvertNumber(sourceNumber, targetTypeAdapter, createMethod);

        [MustUseReturnValue]
        InvalidOperationException CreateConversionNotPossibleException(Type notANumberType)
        {
            return new InvalidOperationException(
                $"Can't convert value of type '{sourceType.GetCSharpName()}' into '{targetType.GetCSharpName()}': " +
                $"Type '{notANumberType.GetCSharpName()}' is not a recognized number type."
            );
        }
    }

    [MustUseReturnValue]
    private static INumberTypeAdapter? GetNumberTypeAdapter(Type numberType)
    {
        switch (Type.GetTypeCode(numberType))
        {
            case TypeCode.Byte:
                return NumberTypeAdapter<byte>.Instance;
            case TypeCode.SByte:
                return NumberTypeAdapter<sbyte>.Instance;
            case TypeCode.Char:
                return NumberTypeAdapter<char>.Instance;
            case TypeCode.Int16:
                return NumberTypeAdapter<short>.Instance;
            case TypeCode.UInt16:
                return NumberTypeAdapter<ushort>.Instance;
            case TypeCode.Int32:
                return NumberTypeAdapter<int>.Instance;
            case TypeCode.UInt32:
                return NumberTypeAdapter<uint>.Instance;
            case TypeCode.Int64:
                return NumberTypeAdapter<long>.Instance;
            case TypeCode.UInt64:
                return NumberTypeAdapter<ulong>.Instance;

            case TypeCode.Single:
                return NumberTypeAdapter<float>.Instance;
            case TypeCode.Double:
                return NumberTypeAdapter<double>.Instance;
            case TypeCode.Decimal:
                return NumberTypeAdapter<decimal>.Instance;

            case TypeCode.Object:
                if (numberType == typeof(Half))
                {
                    return NumberTypeAdapter<Half>.Instance;
                }
                else if (numberType == typeof(Int128))
                {
                    return NumberTypeAdapter<Int128>.Instance;
                }
                else if (numberType == typeof(UInt128))
                {
                    return NumberTypeAdapter<UInt128>.Instance;
                }
                else if (numberType == typeof(BigInteger))
                {
                    return NumberTypeAdapter<BigInteger>.Instance;
                }

                if (s_customNumberTypeAdapters.TryGetValue(numberType, out var cachedCustomNumberTypeAdapter))
                {
                    return cachedCustomNumberTypeAdapter;
                }

                if (typeof(INumberBase<>).TryMakeGenericType(numberType) != null)
                {
                    var customNumberTypeAdapterType = typeof(NumberTypeAdapter<>).MakeGenericType(numberType);
                    var customNumberTypeAdapter = (INumberTypeAdapter)Activator.CreateInstance(customNumberTypeAdapterType)!;

                    s_customNumberTypeAdapters[numberType] = customNumberTypeAdapter;

                    return customNumberTypeAdapter;
                }
                break;
        }

        return null;
    }

    private enum CreateMethods
    {
        Checked,
        Saturating,
        Truncating,
    }

    private interface INumberTypeAdapter
    {
        [MustUseReturnValue]
        object ConvertNumber(object sourceNumber, INumberTypeAdapter targetTypeAdapter, CreateMethods createMethod);

        [MustUseReturnValue]
        object CreateNumber<TSource>(TSource sourceNumber, CreateMethods createMethod) where TSource : INumberBase<TSource>;
    }

    private sealed class NumberTypeAdapter<T> : INumberTypeAdapter where T : INumberBase<T>
    {
        private static NumberTypeAdapter<T>? s_instance;

        public static NumberTypeAdapter<T> Instance
        {
            get
            {
                // NOTE: This property is "lazy" so that we don't unnecessarily create this instance for
                //   custom number types (for which we never access this property).
                // NOTE 2: We don't care about thread-safety here because - in the worst case - we simply
                //   create multiple instances of this adapter and then discard all but one.
                s_instance ??= new();

                return s_instance;
            }
        }

        /// <inheritdoc />
        public object ConvertNumber(object sourceNumber, INumberTypeAdapter targetTypeAdapter, CreateMethods createMethod)
        {
            return targetTypeAdapter.CreateNumber((T)sourceNumber, createMethod);
        }

        /// <inheritdoc />
        public object CreateNumber<TSource>(TSource sourceNumber, CreateMethods createMethod) where TSource : INumberBase<TSource>
        {
            switch (createMethod)
            {
                case CreateMethods.Checked:
                    return T.CreateChecked(sourceNumber);
                case CreateMethods.Saturating:
                    return T.CreateSaturating(sourceNumber);
                case CreateMethods.Truncating:
                    return T.CreateTruncating(sourceNumber);
                default:
                    // CODE COVERAGE: can't be reached
                    throw new UnexpectedSwitchValueException(nameof(createMethod), createMethod);
            }
        }
    }
}

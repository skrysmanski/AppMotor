// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Numerics;
using System.Reflection;

using AppMotor.Core.DataModel;
using AppMotor.Core.Utils;

using JetBrains.Annotations;

namespace AppMotor.Core.Extensions;

/// <summary>
/// Extension methods for <see cref="Type"/>.
/// </summary>
public static class TypeExtensions
{
    /// <summary>
    /// Returns whether this type is a (scalar) numeric type. This list includes <c>byte</c>
    /// and <c>sbyte</c> as well as <see cref="BigInteger"/>. However, <see cref="Complex"/>
    /// is excluded.
    /// </summary>
    /// <remarks>
    /// This method checks the type's full name but ignores the type's assembly. It's therefor
    /// not 100% foolproof. However, the chance that someone recreates one of the system types
    /// which then should not be a numeric value is rather slim (or even impossible?).
    /// </remarks>
    /// <seealso cref="IsNumericIntegerType"/>
    /// <seealso cref="IsNumericFloatType"/>
    [MustUseReturnValue]
    public static bool IsNumericType(this Type type)
    {
        Validate.ArgumentWithName(nameof(type)).IsNotNull(type);

        switch (type.FullName)
        {
            case "System.Byte":
            case "System.SByte":
            case "System.Int16":
            case "System.UInt16":
            case "System.Int32":
            case "System.UInt32":
            case "System.Int64":
            case "System.UInt64":
            case "System.Numerics.BigInteger":
                return true;

            case "System.Single":
            case "System.Double":
            case "System.Decimal":
                return true;

            default:
                return false;
        }
    }

    /// <summary>
    /// Returns whether this type is an integer type. This list includes <c>byte</c>
    /// and <c>sbyte</c> as well as <see cref="BigInteger"/>.
    /// </summary>
    /// <remarks>
    /// This method checks the type's full name but ignores the type's assembly. It's therefor
    /// not 100% foolproof. However, the chance that someone recreates one of the system types
    /// which then should not be a numeric value is rather slim (or even impossible?).
    /// </remarks>
    /// <seealso cref="IsNumericType"/>
    /// <seealso cref="IsNumericFloatType"/>
    [MustUseReturnValue]
    public static bool IsNumericIntegerType(this Type type)
    {
        Validate.ArgumentWithName(nameof(type)).IsNotNull(type);

        switch (type.FullName)
        {
            case "System.Byte":
            case "System.SByte":
            case "System.Int16":
            case "System.UInt16":
            case "System.Int32":
            case "System.UInt32":
            case "System.Int64":
            case "System.UInt64":
            case "System.Numerics.BigInteger":
                return true;

            default:
                return false;
        }
    }

    /// <summary>
    /// Returns whether this type is a floating point type. This list includes <c>float</c>,
    /// <c>double</c> and <c>decimal</c>.
    /// </summary>
    /// <remarks>
    /// This method checks the type's full name but ignores the type's assembly. It's therefor
    /// not 100% foolproof. However, the chance that someone recreates one of the system types
    /// which then should not be a numeric value is rather slim (or even impossible?).
    /// </remarks>
    /// <seealso cref="IsNumericIntegerType"/>
    /// <seealso cref="IsNumericType"/>
    [MustUseReturnValue]
    public static bool IsNumericFloatType(this Type type)
    {
        Validate.ArgumentWithName(nameof(type)).IsNotNull(type);

        switch (type.FullName)
        {
            case "System.Single":
            case "System.Double":
            case "System.Decimal":
                return true;

            default:
                return false;
        }
    }

    /// <summary>
    /// Returns whether this type can be <c>null</c>. Returns <c>true</c> for
    /// all reference types and for all nullable value types.
    /// </summary>
    /// <seealso cref="IsNullableValueType"/>
    [PublicAPI, MustUseReturnValue]
    [Obsolete("Nullable and non-nullable reference types (available since C# 8) can't be differentiated this way. " +
              "As such this method may return 'true' even for non-nullable reference type. " +
              "Because of this, it's no longer advisable to use this method.")]
    public static bool IsNullableType(this Type type)
    {
        Validate.ArgumentWithName(nameof(type)).IsNotNull(type);

        return !type.IsValueType || type.IsNullableValueType();
    }

    /// <summary>
    /// Returns whether this is a nullable value type.
    /// </summary>
    /// <seealso cref="IsNullableType"/>
    [MustUseReturnValue]
    public static bool IsNullableValueType(this Type type)
    {
        Validate.ArgumentWithName(nameof(type)).IsNotNull(type);

        return Nullable.GetUnderlyingType(type) != null;
    }

    /// <summary>
    /// This method does the same as <see cref="Type.IsAssignableFrom"/>. However,
    /// <c>IsAssignableFrom</c> is often very confusing because it swaps the order
    /// of base type and child type when compared to <c>is</c> checks. This method
    /// aims to be better understandable.
    /// </summary>
    /// <remarks>
    /// Unlike <see cref="Type.IsAssignableFrom"/>, the parameter <paramref name="typeToCheck"/>
    /// can't be <c>null</c> here.
    /// </remarks>
    [MustUseReturnValue]
    public static bool Is<TBaseType>(this Type typeToCheck)
    {
        return typeToCheck.Is(typeof(TBaseType));
    }

    /// <summary>
    /// This method does the same as <see cref="Type.IsAssignableFrom"/>. However,
    /// <c>IsAssignableFrom</c> is often very confusing because it swaps the order
    /// of base type and child type when compared to <c>is</c> checks. This method
    /// aims to be better understandable.
    /// </summary>
    /// <remarks>
    /// Unlike <see cref="Type.IsAssignableFrom"/>, the parameter <paramref name="typeToCheck"/>
    /// can't be <c>null</c> here.
    /// </remarks>
    [MustUseReturnValue]
    public static bool Is(this Type typeToCheck, Type baseType)
    {
        Validate.ArgumentWithName(nameof(typeToCheck)).IsNotNull(typeToCheck);
        Validate.ArgumentWithName(nameof(baseType)).IsNotNull(baseType);

        return baseType.IsAssignableFrom(typeToCheck);
    }

    /// <summary>
    /// Returns the <see cref="MethodInfo"/> for the specified custom operator defined in this type.
    /// For binary operators (i.e. with two parameters/operands), this method assumes that both operands
    /// are of this type. Returns <c>null</c>, if the operator method doesn't exist.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="operator">The operator as string (e.g. "+", "--", "==", ...)</param>
    /// <param name="unary">The operators "+" and "-" exist both as unary and binary operator.
    /// If <c>true</c>, the unary operator is returned; if <c>false</c>, the binary operator
    /// is returned.</param>
    [MustUseReturnValue]
    public static MethodInfo? GetOperator(this Type type, string @operator, bool unary = false)
    {
        UnaryOperators? unaryOperator = @operator switch
        {
            "+"  => unary ? UnaryOperators.UnaryPlus : null,
            "-"  => unary ? UnaryOperators.UnaryNegation : null,
            "++" => UnaryOperators.Increment,
            "--" => UnaryOperators.Decrement,
            "!"  => UnaryOperators.LogicalNot,
            "~"  => UnaryOperators.OnesComplement,
            _    => null,
        };

        if (unaryOperator != null)
        {
            return type.GetOperator(unaryOperator.Value);
        }
        else
        {
            switch (@operator)
            {
                case "<<":
                    return type.GetOperator(BinaryOperators.LeftShift, otherType: typeof(int));
                case ">>":
                    return type.GetOperator(BinaryOperators.RightShift, otherType: typeof(int));
                default:
                    return type.GetOperator(@operator, otherType: type);
            }
        }
    }

    /// <summary>
    /// Returns the <see cref="MethodInfo"/> for the specified custom binary (i.e. two parameters/operands) operator
    /// defined in this type. Returns <c>null</c>, if the operator method doesn't exist.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="operator">The operator as string (e.g. "+", "/", "==", ...)</param>
    /// <param name="otherType">The type of the other operand (one is always this type);
    /// can be the same as this type</param>
    /// <param name="otherTypeIsSecondOperand">Whether <paramref name="otherType"/> is used as first (<c>false</c>)
    /// or as second (<c>true</c>) operand; the latter is the default</param>
    [MustUseReturnValue]
    public static MethodInfo? GetOperator(this Type type, string @operator, Type otherType, bool otherTypeIsSecondOperand = true)
    {
        BinaryOperators binaryOperator = @operator switch
        {
            "+"  => BinaryOperators.Addition,
            "-"  => BinaryOperators.Subtraction,
            "*"  => BinaryOperators.Multiply,
            "/"  => BinaryOperators.Division,
            "%"  => BinaryOperators.Modulus,
            "==" => BinaryOperators.Equality,
            "!=" => BinaryOperators.Inequality,
            "<"  => BinaryOperators.LessThan,
            ">"  => BinaryOperators.GreaterThan,
            "<=" => BinaryOperators.LessThanOrEqual,
            ">=" => BinaryOperators.GreaterThanOrEqual,
            "&"  => BinaryOperators.BitwiseAnd,
            "|"  => BinaryOperators.BitwiseOr,
            "^"  => BinaryOperators.ExclusiveOr,
            "<<" => BinaryOperators.LeftShift,
            ">>" => BinaryOperators.RightShift,
            _    => throw new ArgumentException($"The operator '{@operator}' is not a valid binary operator."),
        };

        return type.GetOperator(binaryOperator, otherType, otherTypeIsSecondOperand);
    }

    /// <summary>
    /// Returns the <see cref="MethodInfo"/> for the specified custom unary (i.e. one parameter/operand) operator
    /// defined in this type. This method assumes that both operands are of this type. Returns <c>null</c>, if
    /// the operator method doesn't exist.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="operator">The operator</param>
    [MustUseReturnValue]
    public static MethodInfo? GetOperator(this Type type, UnaryOperators @operator)
    {
        return type.GetMethod("op_" + @operator, BindingFlags.Public | BindingFlags.Static, [type]);
    }

    /// <summary>
    /// Returns the <see cref="MethodInfo"/> for the specified custom binary (i.e. two parameters/operands) operator
    /// defined in this type. This method assumes that both operands are of this type. Returns <c>null</c>, if the
    /// operator method doesn't exist.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="operator">The operator</param>
    [MustUseReturnValue]
    public static MethodInfo? GetOperator(this Type type, BinaryOperators @operator)
    {
        switch (@operator)
        {
            case BinaryOperators.LeftShift:
            case BinaryOperators.RightShift:
                return type.GetOperator(@operator, otherType: typeof(int));
            default:
                return type.GetOperator(@operator, otherType: type);
        }
    }

    /// <summary>
    /// Returns the <see cref="MethodInfo"/> for the specified custom binary (i.e. two parameters/operands) operator
    /// defined in this type. Returns <c>null</c>, if the operator method doesn't exist.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="operator">The operator</param>
    /// <param name="otherType">The type of the other operand (one is always this type);
    /// can be the same as this type</param>
    /// <param name="otherTypeIsSecondOperand">Whether <paramref name="otherType"/> is used as first (<c>false</c>)
    /// or as second (<c>true</c>) operand; the latter is the default</param>
    [MustUseReturnValue]
    public static MethodInfo? GetOperator(this Type type, BinaryOperators @operator, Type otherType, bool otherTypeIsSecondOperand = true)
    {
        if (otherTypeIsSecondOperand)
        {
            return type.GetMethod("op_" + @operator, BindingFlags.Public | BindingFlags.Static, [type, otherType]);
        }
        else
        {
            return type.GetMethod("op_" + @operator, BindingFlags.Public | BindingFlags.Static, [otherType, type]);
        }
    }
}

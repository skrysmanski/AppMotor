// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Collections;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;

using AppMotor.Core.ComponentModel;
using AppMotor.Core.Utils;

using JetBrains.Annotations;

namespace AppMotor.Core.Extensions;

/// <summary>
/// Extension methods for <see cref="Type"/>.
/// </summary>
public static class TypeExtensions
{
    /// <summary>
    /// Returns the name of this type in C# syntax: e.g. "List&lt;string&gt;" instead of "List`1"
    /// or "ushort" instead of "UInt16".
    /// </summary>
    /// <param name="type">This type</param>
    /// <param name="includeNamespaces">If <c>true</c>, all type names (except for types for which a C# keyword
    /// exists) will be returned fully-qualified with their namespace. If <c>false</c> (the default), only the
    /// type's name will be returned (i.e. without namespace).</param>
    /// <remarks>
    /// For more control over the returned string, use the class <see cref="CSharpTypeName"/> instead.
    /// </remarks>
    [MustUseReturnValue]
    public static string GetCSharpName(this Type type, bool includeNamespaces = false)
    {
        return CSharpTypeName.GetName(type, includeNamespaces ? CSharpTypeName.IncludeNamespaceFlags.All : CSharpTypeName.IncludeNamespaceFlags.None);
    }

    /// <summary>
    /// Returns the value of <c>default(T)</c> for the specified type.
    /// </summary>
    [MustUseReturnValue]
    public static object? GetDefaultValue(this Type type)
    {
        if (type.IsValueType)
        {
            return RuntimeHelpers.GetUninitializedObject(type);
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Returns whether this type is a (scalar) numeric type. This list includes <c>byte</c>
    /// and <c>sbyte</c> as well as <see cref="BigInteger"/>. However, <see cref="Complex"/>
    /// is excluded.
    /// </summary>
    /// <param name="type">The type to check</param>
    /// <param name="includeNullables">Whether this method should return <c>true</c> for nullable
    /// number types as well. If <c>true</c>, this method will return <c>true</c> for <c>int?</c>.
    /// If <c>false</c>, it will return <c>false</c> in this case.</param>
    /// <seealso cref="IsNumericIntegerType"/>
    /// <seealso cref="IsNumericFloatType"/>
    [MustUseReturnValue]
    public static bool IsNumericType(this Type type, bool includeNullables)
    {
        // Check if the type is a nullable value type and use the underlying type for the check.
        if (includeNullables && type.IsValueType)
        {
            var underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType is not null)
            {
                return underlyingType.IsNumericType(includeNullables: false);
            }
        }

        if (type.IsPrimitive)
        {
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
                    return true;

                case "System.Single":
                case "System.Double":
                    return true;

                default:
                    return false;
            }
        }
        else if (type.IsValueType)
        {
            switch (type.FullName)
            {
                case "System.Int128":
                case "System.UInt128":
                case "System.Numerics.BigInteger":
                    return true;

                case "System.Half":
                case "System.Decimal":
                    return true;

                default:
                    return false;
            }
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Returns whether this type is an integer type. This list includes <c>byte</c>, <c>sbyte</c>, <see cref="BigInteger"/>,
    /// <c>Int128</c>, and <c>UInt128</c>.
    /// </summary>
    /// <param name="type">The type to check</param>
    /// <param name="includeNullables">Whether this method should return <c>true</c> for nullable
    /// number types as well. If <c>true</c>, this method will return <c>true</c> for <c>int?</c>.
    /// If <c>false</c>, it will return <c>false</c> in this case.</param>
    /// <seealso cref="IsNumericType"/>
    /// <seealso cref="IsNumericFloatType"/>
    [MustUseReturnValue]
    public static bool IsNumericIntegerType(this Type type, bool includeNullables)
    {
        // Check if the type is a nullable value type and use the underlying type for the check.
        if (includeNullables && type.IsValueType)
        {
            var underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType is not null)
            {
                return underlyingType.IsNumericType(includeNullables: false);
            }
        }

        if (type.IsPrimitive)
        {
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
                    return true;

                default:
                    return false;
            }
        }
        else if (type.IsValueType)
        {
            switch (type.FullName)
            {
                case "System.Int128":
                case "System.UInt128":
                case "System.Numerics.BigInteger":
                    return true;

                default:
                    return false;
            }
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Returns whether this type is a floating point type. This list includes <c>float</c>,
    /// <c>double</c>, <c>decimal</c>, and <c>Half</c>.
    /// </summary>
    /// <param name="type">The type to check</param>
    /// <param name="includeNullables">Whether this method should return <c>true</c> for nullable
    /// number types as well. If <c>true</c>, this method will return <c>true</c> for <c>int?</c>.
    /// If <c>false</c>, it will return <c>false</c> in this case.</param>
    /// <seealso cref="IsNumericIntegerType"/>
    /// <seealso cref="IsNumericType"/>
    [MustUseReturnValue]
    public static bool IsNumericFloatType(this Type type, bool includeNullables)
    {
        // Check if the type is a nullable value type and use the underlying type for the check.
        if (includeNullables && type.IsValueType)
        {
            var underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType is not null)
            {
                return underlyingType.IsNumericType(includeNullables: false);
            }
        }

        if (type.IsPrimitive)
        {
            switch (type.FullName)
            {
                case "System.Single":
                case "System.Double":
                    return true;

                default:
                    return false;
            }
        }
        else if (type.IsValueType)
        {
            switch (type.FullName)
            {
                case "System.Half":
                case "System.Decimal":
                    return true;

                default:
                    return false;
            }
        }
        else
        {
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
    /// Returns the type of the items of the specified collection type (i.e. a type implementing <see cref="IEnumerable{T}"/>).
    /// Returns <c>null</c> if this type is not a collection type. Also returns <c>null</c> if <paramref name="type"/> only
    /// implements a non-generic collection interface (i.e. <see cref="IEnumerable"/>, <see cref="ICollection"/>, <see cref="IList"/>, ...)
    /// and <paramref name="enableNonGenericCollectionSupport"/> is <c>false</c> (it's <c>true</c> by default).
    /// </summary>
    /// <param name="type">This type</param>
    /// <param name="enableNonGenericCollectionSupport">Affects the return type if <paramref name="type"/> only implements
    /// a non-generic collection interface but not <see cref="IEnumerable{T}"/>. If this is <c>true</c> (the default),
    /// returns <c>typeof(object)</c> in this case. If <c>false</c>, returns <c>null</c> in this case.</param>
    /// <remarks>
    /// This method also works if this type is an array type.
    /// </remarks>
    /// <exception cref="AmbiguousMatchException">Thrown if this type implements <see cref="IEnumerable{T}"/> multiple
    /// times with different type arguments.</exception>
    [MustUseReturnValue, Pure]
    public static Type? GetCollectionItemType(this Type type, bool enableNonGenericCollectionSupport = true)
    {
        if (type.IsArray)
        {
            return type.GetElementType();
        }

        // NOTE: We need this extra check here because "FindInterface()" below doesn't return anything if
        //   "type" is actually "IEnumerable<T>".
        if (type.IsInterface && IsIEnumerableOfT(type))
        {
            return type.GetGenericArguments()[0];
        }

        var implementedIEnumerableInterfaceTypes = type.FindInterfaces(FindInterfacesFilter, null);

        if (implementedIEnumerableInterfaceTypes.Length == 0)
        {
            // IEnumerable<T> is not implemented.
            if (enableNonGenericCollectionSupport && type.Is<IEnumerable>())
            {
                return typeof(object);
            }

            return null;
        }
        else if (implementedIEnumerableInterfaceTypes.Length == 1)
        {
            return implementedIEnumerableInterfaceTypes[0].GetGenericArguments()[0];
        }
        else
        {
            // NOTE: Getting here is possible but doesn't really make sense because both "implementations"
            //   would share the non-generic IEnumerable implementation - and which type would be returned
            //   in this case?
            throw new AmbiguousMatchException($"The type '{type}' implements 'IEnumerable<T>' multiple times with different type arguments.");
        }

        static bool FindInterfacesFilter(Type interfaceTypeToCheck, object? criteria)
        {
            return IsIEnumerableOfT(interfaceTypeToCheck);
        }

        static bool IsIEnumerableOfT(Type interfaceTypeToCheck)
        {
            return interfaceTypeToCheck.IsGenericType && interfaceTypeToCheck.GetGenericTypeDefinition() == typeof(IEnumerable<>);
        }
    }

    /// <summary>
    /// This method does the same as <see cref="Type.IsAssignableFrom"/>. However,
    /// <c>IsAssignableFrom</c> is often very confusing because it swaps the order
    /// of base type and child type when compared to <c>is</c> checks. This method
    /// aims to be better understandable.
    /// </summary>
    /// <remarks>
    /// The non-generic version of this method (<see cref="Is"/>) also supports open
    /// generic types (e.g. <c>IList&lt;&gt;</c> ) for <typeparamref name="TBaseOrInterfaceType"/>.
    /// </remarks>
    /// <remarks>
    /// Unlike <see cref="Type.IsAssignableFrom"/>, the parameter <paramref name="typeToCheck"/>
    /// can't be <c>null</c> here.
    /// </remarks>
    [MustUseReturnValue]
    public static bool Is<TBaseOrInterfaceType>(this Type typeToCheck)
    {
        return typeToCheck.Is(typeof(TBaseOrInterfaceType));
    }

    /// <summary>
    /// This method does the same as <see cref="Type.IsAssignableFrom"/>. However,
    /// <c>IsAssignableFrom</c> is often very confusing because it swaps the order
    /// of base type and child type when compared to <c>is</c> checks. This method
    /// aims to be better understandable.
    /// </summary>
    /// <remarks>
    /// This method also supports open generic types (e.g. <c>IList&lt;&gt;</c> ) - both
    /// for <paramref name="baseOrInterfaceType"/> as well as this type.
    /// </remarks>
    /// <remarks>
    /// Unlike <see cref="Type.IsAssignableFrom"/>, the parameter <paramref name="typeToCheck"/>
    /// can't be <c>null</c> here.
    /// </remarks>
    [MustUseReturnValue]
    public static bool Is(this Type typeToCheck, Type baseOrInterfaceType)
    {
        Validate.ArgumentWithName(nameof(typeToCheck)).IsNotNull(typeToCheck);
        Validate.ArgumentWithName(nameof(baseOrInterfaceType)).IsNotNull(baseOrInterfaceType);

        // This includes enums, arrays, and value types.
        if (baseOrInterfaceType.IsSealed)
        {
            return false;
        }

        if (baseOrInterfaceType.IsGenericTypeDefinition)
        {
            //
            // Open generic type (e.g. "IList<>").
            //
            // NOTE: At the time of writing (.NET 8/C# 12) it's NOT possible to have a partially open generics
            //   (e.g. "IDictionary<string,>").
            //
            if (baseOrInterfaceType.IsInterface)
            {
                var interfaceTypeFullName = baseOrInterfaceType.FullName!;

                //
                // Try "Type.GetInterface()" first, if possible.
                //
                // "Type.GetInterface()" doesn't work for nested interfaces.
                if (!baseOrInterfaceType.IsNested)
                {
                    try
                    {
                        var implementedInterface = typeToCheck.GetInterface(interfaceTypeFullName);
                        return implementedInterface?.Assembly == baseOrInterfaceType.Assembly;
                    }
                    catch (AmbiguousMatchException)
                    {
                        // This type implements multiple interfaces (with either different type arguments or
                        // from different assemblies) with the same name.
                        //
                        // NOTE: Because the search in "Type.GetInterface()" only looks at the type's full name
                        //   but not at the assembly, we can NOT assume "return true;" in case of this exception
                        //   (as the exception may be caused by an interface with the same name but from a different
                        //   assembly).
                    }
                }

                //
                // Use "Type.FindInterfaces()" if "Type.GetInterface()" couldn't be used.
                //
                return typeToCheck.FindInterfaces(FindOpenGenericInterfaceFilter, baseOrInterfaceType).Length != 0;
            }
            else
            {
                var baseType = typeToCheck.BaseType;

                while (baseType is not null && baseType != typeof(object))
                {
                    if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == baseOrInterfaceType)
                    {
                        return true;
                    }

                    baseType = baseType.BaseType;
                }

                return false;
            }
        }

        return baseOrInterfaceType.IsAssignableFrom(typeToCheck);

        static bool FindOpenGenericInterfaceFilter(Type interfaceTypeToCheck, object? criteria)
        {
            if (!interfaceTypeToCheck.IsGenericType)
            {
                // We only get here for open generic types. So if "interfaceTypeToCheck" is not a generic type,
                // it can't be the type we're looking for.
                return false;
            }

            return interfaceTypeToCheck.GetGenericTypeDefinition().Equals(criteria);
        }
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

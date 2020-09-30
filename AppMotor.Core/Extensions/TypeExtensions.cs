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

using System;
using System.Numerics;

using AppMotor.Core.Utils;

using JetBrains.Annotations;

namespace AppMotor.Core.Extensions
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Returns whether this type is a (scalar) numeric type. This list includes <c>byte</c>
        /// and <c>sbyte</c> as well as <see cref="BigInteger"/>. However <see cref="Complex"/>
        /// is excluded.
        /// </summary>
        /// <remarks>
        /// This method checks the type's full name but ignores the type's assembly. It's therefor
        /// not 100% fool proof. However, the chance that someone recreates one of the system types
        /// which then should not be a numeric value is rather slim (or even impossible?).
        /// </remarks>
        /// <seealso cref="IsNumericIntegerType"/>
        /// <seealso cref="IsNumericFloatType"/>
        [PublicAPI, Pure]
        public static bool IsNumericType(this Type type)
        {
            Validate.Argument.IsNotNull(type, nameof(type));

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
        /// not 100% fool proof. However, the chance that someone recreates one of the system types
        /// which then should not be a numeric value is rather slim (or even impossible?).
        /// </remarks>
        /// <seealso cref="IsNumericType"/>
        /// <seealso cref="IsNumericFloatType"/>
        [PublicAPI, Pure]
        public static bool IsNumericIntegerType(this Type type)
        {
            Validate.Argument.IsNotNull(type, nameof(type));

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
        /// not 100% fool proof. However, the chance that someone recreates one of the system types
        /// which then should not be a numeric value is rather slim (or even impossible?).
        /// </remarks>
        /// <seealso cref="IsNumericIntegerType"/>
        /// <seealso cref="IsNumericType"/>
        [PublicAPI, Pure]
        public static bool IsNumericFloatType(this Type type)
        {
            Validate.Argument.IsNotNull(type, nameof(type));

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
        [PublicAPI, Pure]
        public static bool IsNullableType(this Type type)
        {
            Validate.Argument.IsNotNull(type, nameof(type));

            return !type.IsValueType || type.IsNullableValueType();
        }

        /// <summary>
        /// Returns whether this is a nullable value type.
        /// </summary>
        /// <seealso cref="IsNullableType"/>
        [PublicAPI, Pure]
        public static bool IsNullableValueType(this Type type)
        {
            Validate.Argument.IsNotNull(type, nameof(type));

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
        [PublicAPI, Pure]
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
        [PublicAPI, Pure]
        public static bool Is(this Type typeToCheck, Type baseType)
        {
            Validate.Argument.IsNotNull(typeToCheck, nameof(typeToCheck));
            Validate.Argument.IsNotNull(baseType, nameof(baseType));

            return baseType.IsAssignableFrom(typeToCheck);
        }
    }
}

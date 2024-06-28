// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Reflection;

using JetBrains.Annotations;

namespace AppMotor.Core.ComponentModel;

/// <summary>
/// Represents the parameterless constructor for a certain type. To get it for a type,
/// use <see cref="GetForType"/>.
/// </summary>
public abstract class ParameterlessConstructor
{
    // Prevent other child classes except our own.
    internal ParameterlessConstructor()
    {
    }

    /// <summary>
    /// Invokes this parameterless constructor to create a new instance of the type this
    /// constructor belongs to.
    /// </summary>
    [MustUseReturnValue]
    public abstract object Invoke();

    /// <summary>
    /// Returns the parameterless constructor for the specified type. Returns <c>null</c> if
    /// the type doesn't have a parameterless constructor. Also returns <c>null</c> for types
    /// without constructors (interfaces, enums, simple types, arrays) and for <c>abstract</c> types.
    /// </summary>
    /// <param name="type">The type for which to return the parameterless constructor.</param>
    /// <param name="publicOnly">Whether to only consider a <c>public</c> parameterless constructor
    /// (the default). If <c>false</c>, this method will also look for a private parameterless constructor.</param>
    /// <remarks>
    /// For nullable value types, this method will return the constructor for the underlying value type.
    /// </remarks>
    [MustUseReturnValue]
    public static ParameterlessConstructor? GetForType(Type type, bool publicOnly = true)
    {
        if (type.IsAbstract || type.IsInterface || type.IsEnum || type.IsArray || type.IsPrimitive)
        {
            return null;
        }

        if (type.IsValueType)
        {
            // Check for nullable types - in this case we get the constructor for the underlying type.
            var underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType is not null)
            {
                return GetForType(underlyingType);
            }

            // Value types always have a public parameterless constructor.
            return (ParameterlessConstructor)Activator.CreateInstance(typeof(PublicParameterlessConstructor<>).MakeGenericType(type), [])!;
        }

        ConstructorInfo? parameterlessConstructor;

        if (publicOnly)
        {
            parameterlessConstructor = type.GetConstructor([]);
        }
        else
        {
            parameterlessConstructor = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, []);
        }

        if (parameterlessConstructor is null)
        {
            return null;
        }

        if (parameterlessConstructor.IsPublic)
        {
            return (ParameterlessConstructor)Activator.CreateInstance(typeof(PublicParameterlessConstructor<>).MakeGenericType(type), [])!;
        }
        else
        {
            return new NonPublicParameterlessConstructor(parameterlessConstructor);
        }
    }

    private sealed class NonPublicParameterlessConstructor : ParameterlessConstructor
    {
        private readonly ConstructorInfo _constructor;

        public NonPublicParameterlessConstructor(ConstructorInfo constructor)
        {
            this._constructor = constructor;
        }

        /// <inheritdoc />
        public override object Invoke()
        {
            return this._constructor.Invoke(null);
        }
    }

    private sealed class PublicParameterlessConstructor<T> : ParameterlessConstructor where T : new()
    {
        /// <inheritdoc />
        public override object Invoke()
        {
            return new T();
        }
    }
}

#region License
// Copyright 2021 AppMotor Framework (https://github.com/skrysmanski/AppMotor)
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using AppMotor.Core.Exceptions;

using JetBrains.Annotations;

using NotNullOnExitAttribute = System.Diagnostics.CodeAnalysis.NotNullAttribute;

namespace AppMotor.Core.Utils;

/// <summary>
/// Provides various validation methods for parameters and values in general. Raises <see cref="ArgumentException"/>s
/// and <see cref="ValueException"/>s.
/// </summary>
/// <remarks>
/// For details on the design of this class, see: https://github.com/skrysmanski/AppMotor/issues/11
/// </remarks>
public static class Validate
{
    private static readonly ArgumentExceptionFactory s_argumentExceptionFactory = new();

    private static readonly ValueExceptionFactory s_valueExceptionFactory = new();

    /// <summary>
    /// Creates a validator for arguments/parameters. Throws <see cref="ArgumentException"/>s (or one of its
    /// child classes) in case the validation fails.
    ///
    /// <para>Usage: <c>Validate.ArgumentWithName(nameof(myParam)).IsNotNull(myParam);</c></para>
    /// </summary>
    /// <remarks>
    /// This method is called <c>ArgumentWithName</c> - rather than <c>ParameterWithName</c> - because its
    /// associated exception is named <see cref="ArgumentException"/> (and not <c>ParameterException</c>).
    /// </remarks>
    /// <seealso cref="ValueWithName"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static NamedValidator ArgumentWithName([InvokerParameterName] string paramName)
    {
        return new NamedValidator(s_argumentExceptionFactory, paramName);
    }

    /// <summary>
    /// Creates a validator for regular values (i.e. everything that's not an argument/parameter). Throws
    /// <see cref="ValueException"/>s (or one of its child classes) in case the validation fails.
    ///
    /// <para>Usage: <c>Validate.ValueWithName(nameof(myValue)).IsNotNull(myValue);</c></para>
    /// </summary>
    /// <seealso cref="ArgumentWithName"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static NamedValidator ValueWithName(string valueName)
    {
        return new NamedValidator(s_valueExceptionFactory, valueName);
    }

    /// <summary>
    /// Non-generic version of <see cref="ExceptionFactoryBase{TBaseException}"/>. Used by <see cref="NamedValidator"/>
    /// to create exceptions (as <see cref="NamedValidator"/> is also non-generic).
    /// </summary>
    internal interface IExceptionFactory
    {
        /// <summary>
        /// The type of factory - for use in <see cref="NamedValidator.ToString"/>.
        /// </summary>
        string TypeNameForToString { get; }

        /// <inheritdoc cref="NamedValidator.CreateNullException"/>
        [MustUseReturnValue]
        Exception CreateNullException(string? valueName);

        /// <inheritdoc cref="NamedValidator.CreateCollectionIsReadOnlyException"/>
        [MustUseReturnValue]
        Exception CreateCollectionIsReadOnlyException(string? valueName);

        /// <inheritdoc cref="NamedValidator.CreateRootException"/>
        [MustUseReturnValue]
        Exception CreateRootException(string message, string? valueName);
    }

    /// <summary>
    /// Creates <see cref="ArgumentException"/>s.
    /// </summary>
    /// <seealso cref="ValueExceptionFactory"/>
    private sealed class ArgumentExceptionFactory : ExceptionFactoryBase<ArgumentException>
    {
        /// <inheritdoc />
        public override string TypeNameForToString => "argument/parameter";

        /// <inheritdoc />
        protected override ArgumentException CreateNullException(string? valueName)
        {
            return new ArgumentNullException(
                message: ExceptionMessages.VALUE_IS_NULL, // <-- we specify this here so that the message doesn't get translated
                paramName: valueName ?? ExceptionMessages.DEFAULT_VALUE_NAME
            );
        }

        /// <inheritdoc />
        protected override ArgumentException CreateCollectionIsReadOnlyException(string? valueName)
        {
            return new CollectionIsReadOnlyArgumentException(paramName: valueName ?? ExceptionMessages.DEFAULT_VALUE_NAME);
        }

        /// <inheritdoc />
        protected override ArgumentException CreateRootException(string message, string? valueName)
        {
            return new ArgumentException(message: message, paramName: valueName ?? ExceptionMessages.DEFAULT_VALUE_NAME);
        }
    }

    /// <summary>
    /// Creates <see cref="ValueException"/>s.
    /// </summary>
    /// <seealso cref="ArgumentExceptionFactory"/>
    private sealed class ValueExceptionFactory : ExceptionFactoryBase<ValueException>
    {
        /// <inheritdoc />
        public override string TypeNameForToString => "value";

        /// <inheritdoc />
        protected override ValueException CreateNullException(string? valueName)
        {
            return new ValueNullException(message: null, valueName: valueName ?? ExceptionMessages.DEFAULT_VALUE_NAME);
        }

        /// <inheritdoc />
        protected override ValueException CreateCollectionIsReadOnlyException(string? valueName)
        {
            return new CollectionIsReadOnlyValueException(valueName: valueName ?? ExceptionMessages.DEFAULT_VALUE_NAME);
        }

        /// <inheritdoc />
        protected override ValueException CreateRootException(string message, string? valueName)
        {
            return new ValueException(message: message, valueName: valueName ?? ExceptionMessages.DEFAULT_VALUE_NAME);
        }
    }

    /// <summary>
    /// Base class for an exception factory for a certain value type (either value or argument).
    /// </summary>
    /// <typeparam name="TBaseException">The common base exception type for the value type</typeparam>
    private abstract class ExceptionFactoryBase<TBaseException> : IExceptionFactory where TBaseException : Exception
    {
        /// <inheritdoc />
        public abstract string TypeNameForToString { get; }

        /// <inheritdoc />
        Exception IExceptionFactory.CreateNullException(string? valueName)
        {
            return CreateNullException(valueName);
        }

        /// <inheritdoc />
        Exception IExceptionFactory.CreateCollectionIsReadOnlyException(string? valueName)
        {
            return CreateCollectionIsReadOnlyException(valueName);
        }

        /// <inheritdoc />
        Exception IExceptionFactory.CreateRootException(string message, string? valueName)
        {
            return CreateRootException(message, valueName);
        }

        /// <summary>
        /// See <see cref="NamedValidator.CreateNullException"/> for details.
        /// </summary>
        protected abstract TBaseException CreateNullException(string? valueName);

        /// <summary>
        /// See <see cref="NamedValidator.CreateCollectionIsReadOnlyException"/> for details.
        /// </summary>
        protected abstract TBaseException CreateCollectionIsReadOnlyException(string? valueName);

        /// <summary>
        /// See <see cref="NamedValidator.CreateRootException"/> for details.
        /// </summary>
        protected abstract TBaseException CreateRootException(string message, string? valueName);
    }

    internal static class ExceptionMessages
    {
        public const string DEFAULT_VALUE_NAME = "unknown";

        public const string DEFAULT_MESSAGE = "The value is invalid.";

        public const string VALUE_IS_NULL = "The value must not be null.";

        public const string STRING_IS_EMPTY = "The string must not be empty.";

        public const string STRING_IS_WHITE_SPACES = "The string must not contain just white space characters.";

        public const string COLLECTION_IS_EMPTY = "The collection must not be empty.";
    }
}

/// <summary>
/// This struct is the base for all validation methods (e.g. <see cref="IsNotNull{T}(T?)"/>). It can be used to write new validation methods
/// as extension methods. For extension methods, use one of the <c>Create...Exception()</c> methods to create the validation exception.
/// It will automatically have the correct type (i.e. <see cref="ArgumentException"/> or <see cref="ValueException"/>) and value name.
/// </summary>
/// <remarks>
/// This struct is not a real data type but rather a "trick" to achieve extensibility for validation methods. Instances of this struct
/// can't be created from the outside. They must always be obtained via <see cref="Validate.ArgumentWithName"/> or <see cref="Validate.ValueWithName"/>.
///
/// <para>Note that this is a <c>struct</c> on purpose to make creating it cheap and so that it can't be <c>null</c> when used in
/// extension methods (primarily prevents CA1062).</para>
/// </remarks>
[SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "Not a real data type")]
public readonly struct NamedValidator
{
    private readonly string? _valueName;

    private readonly Validate.IExceptionFactory? _exceptionFactory;

    private Validate.IExceptionFactory ExceptionFactory => this._exceptionFactory ?? throw new InvalidOperationException("No exception factory has been set.");

    internal NamedValidator(Validate.IExceptionFactory exceptionFactory, string? valueName)
    {
        this._exceptionFactory = exceptionFactory;
        this._valueName = valueName;
    }

    /// <summary>
    /// Creates a <see cref="ArgumentNullException"/> or <see cref="ValueNullException"/> based on the value's type.
    /// </summary>
    [PublicAPI, MustUseReturnValue]
    public Exception CreateNullException()
    {
        return this.ExceptionFactory.CreateNullException(this._valueName);
    }

    /// <summary>
    /// Creates a <see cref="CollectionIsReadOnlyArgumentException"/> or <see cref="CollectionIsReadOnlyArgumentException"/>
    /// based on the value's type.
    /// </summary>
    [PublicAPI, MustUseReturnValue]
    public Exception CreateCollectionIsReadOnlyException()
    {
        return this.ExceptionFactory.CreateCollectionIsReadOnlyException(this._valueName);
    }

    /// <summary>
    /// Creates a <see cref="ArgumentException"/> or <see cref="ValueException"/> based on the value's type.
    /// </summary>
    /// <param name="message">The exception message to be used by the created exception.</param>
    [PublicAPI, MustUseReturnValue]
    public Exception CreateRootException(string message)
    {
        return this.ExceptionFactory.CreateRootException(message, this._valueName);
    }

    /// <summary>
    /// Validates that the given (reference type) value is not null.
    /// </summary>
    /// <remarks>
    /// This check is split into this method and the other overload so that it can't be
    /// accidentally called for non-nullable value types. Especially structs may be misinterpreted
    /// as classes and thus unnecessarily checked for null. (This is why this method needs the
    /// "where T : class" constraint.)
    ///
    /// <para>Also, the overload for value types becomes faster this way as it avoids any boxing. See:
    /// https://sharplab.io/#v2:EYLgtghgzgLgpgJwDQxASwDYB8ACAmARgFgAoHAZgAJ9KBhSgb1MpcuAHt2NKBZAgHgAqAPgAUgygA8AlJQDuAC0RxKEkNTyUAvMKmU0USgDsArhgwBuUgEgOXXniFjBAfimzFy1ZXWwEJgGMYbV1JfUNTcysSAF8gA=
    /// </para>
    ///
    /// <para>Only downside is that we can no longer use this to check values with unconstrained generic types.
    /// Instead, <see cref="IsNotNullUnconstrained{T}"/> must be used.</para>
    /// </remarks>
    /// <seealso cref="IsNotNullUnconstrained{T}"/>
    [PublicAPI]
    public void IsNotNull<T>([InstantHandle, NoEnumeration, NotNullOnExit] T? value) where T : class
    {
        if (value is null)
        {
            throw CreateNullException();
        }
    }

    /// <summary>
    /// Validates that the given (value type) value is not null.
    /// </summary>
    /// <remarks>
    /// This check is split into this method and the other <c>IsNotNull</c> overload. See other overload for more details.
    /// </remarks>
    /// <seealso cref="IsNotNullUnconstrained{T}"/>
    [PublicAPI]
    public void IsNotNull<T>([InstantHandle, NoEnumeration, NotNullOnExit] T? value) where T : struct
    {
        if (!value.HasValue)
        {
            throw CreateNullException();
        }
    }

    /// <summary>
    /// Validates that the given (unconstrained generic type) value is not null.
    ///
    /// <para>Note: You should prefer <c>IsNotNull()</c> instead. Only use this method
    /// if you have a value with a generic type that is unconstrained.</para>
    /// </summary>
    [PublicAPI]
    public void IsNotNullUnconstrained<T>([InstantHandle, NoEnumeration, NotNullOnExit] T value)
    {
        if (value is null)
        {
            throw CreateNullException();
        }
    }

    /// <summary>
    /// Validates that the the given string is neither <c>null</c> nor empty.
    /// </summary>
    [PublicAPI]
    public void IsNotNullOrEmpty([NotNullOnExit] string? value)
    {
        if (value is null)
        {
            throw CreateNullException();
        }

        if (value.Length == 0)
        {
            throw CreateRootException(Validate.ExceptionMessages.STRING_IS_EMPTY);
        }
    }

    /// <summary>
    /// Validates that the the given string is neither <c>null</c> nor empty nor only white space characters.
    /// </summary>
    [PublicAPI]
    public void IsNotNullOrWhiteSpace([NotNullOnExit] string? value)
    {
        // NOTE: For performance reasons (in case this check passes), we use this check first. If it fails,
        //   we're already in an exception situation and then performance is no longer this important.
        if (string.IsNullOrWhiteSpace(value))
        {
            if (value is null)
            {
                throw CreateNullException();
            }

            if (value.Length == 0)
            {
                throw CreateRootException(Validate.ExceptionMessages.STRING_IS_EMPTY);
            }

            throw CreateRootException(Validate.ExceptionMessages.STRING_IS_WHITE_SPACES);
        }
    }

    /// <summary>
    /// Validates that the the given collection is neither <c>null</c> nor empty.
    /// </summary>
    [PublicAPI]
    public void IsNotNullOrEmpty<T>([NotNullOnExit] IReadOnlyCollection<T>? value)
    {
        if (value is null)
        {
            throw CreateNullException();
        }

        if (value.Count == 0)
        {
            throw CreateRootException(Validate.ExceptionMessages.COLLECTION_IS_EMPTY);
        }
    }

    /// <summary>
    /// Verifies that the <see cref="ICollection{T}.IsReadOnly"/> property of the specified value
    /// is <c>false</c>; otherwise a <see cref="CollectionIsReadOnlyArgumentException"/> or
    /// <see cref="CollectionIsReadOnlyValueException"/> is thrown (depending on the value type).
    /// </summary>
    [PublicAPI]
    public void IsNotReadOnly<T>(ICollection<T> value)
    {
        if (value.IsReadOnly)
        {
            throw CreateCollectionIsReadOnlyException();
        }
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Validator for {this.ExceptionFactory.TypeNameForToString} '{this._valueName}'";
    }
}

/// <summary>
/// Contains extension methods that use <see cref="Validate"/>.
/// </summary>
/// <remarks>
/// These methods are not defined directly in <see cref="Validate"/> so that the don't appear in
/// the code completion list of <see cref="Validate"/>.
/// </remarks>
public static class ValidationExtensionMethods
{
    /// <summary>
    /// This method does the same thing as <see cref="NamedValidator.IsNotNull{T}(T)"/> and primarily
    /// exists for constructor chaining where argument members are passed to another constructor
    /// and thus can't be validated with a statement.
    /// </summary>
    /// <returns>Simply returns <paramref name="value"/>.</returns>
    [PublicAPI]
    [MustUseReturnValue]
    public static T AsNotNullArgument<T>(
            [InstantHandle, NoEnumeration, NotNullOnExit] this T? value,
            [InvokerParameterName] string paramName
        )
        where T : class
    {
        Validate.ArgumentWithName(paramName).IsNotNull(value);

        return value;
    }

    /// <summary>
    /// This method does the same thing as <see cref="NamedValidator.IsNotNull{T}(T?)"/> and primarily
    /// exists for constructor chaining where argument members are passed to another constructor
    /// and thus can't be validated with a statement.
    /// </summary>
    /// <returns>Simply returns <paramref name="value"/>.</returns>
    [PublicAPI]
    [MustUseReturnValue]
    public static T AsNotNullArgument<T>(
            [InstantHandle, NoEnumeration, NotNullOnExit] this T? value,
            [InvokerParameterName] string paramName
        )
        where T : struct
    {
        Validate.ArgumentWithName(paramName).IsNotNull(value);

        return value.Value;
    }

    /// <summary>
    /// This method does the same thing as <see cref="NamedValidator.IsNotNullUnconstrained{T}"/> and primarily
    /// exists for constructor chaining where argument members are passed to another constructor
    /// and thus can't be validated with a statement.
    /// </summary>
    /// <returns>Simply returns <paramref name="value"/>.</returns>
    [PublicAPI]
    [MustUseReturnValue]
    [return: NotNullOnExit]
    public static T AsNotNullArgumentUnconstrained<T>(
            [InstantHandle, NoEnumeration, NotNullOnExit] this T value,
            [InvokerParameterName] string paramName
        )
    {
        Validate.ArgumentWithName(paramName).IsNotNullUnconstrained(value);

        return value;
    }
}
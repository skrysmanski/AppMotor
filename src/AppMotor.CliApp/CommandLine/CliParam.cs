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
using System.Collections;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Linq;

using AppMotor.Core.DataModel;
using AppMotor.Core.Exceptions;
using AppMotor.Core.Extensions;
using AppMotor.Core.Utils;

using JetBrains.Annotations;

namespace AppMotor.CliApp.CommandLine
{
    /// <summary>
    /// Represents a command line parameter - either named or positional. See <see cref="CliParamBase.ParameterType"/>,
    /// and <see cref="CliParamBase.PositionIndex"/> for more details on the difference between these two parameter types.
    ///
    /// <para>Parameter must (usually) be defined in a container type - either in a subclass of <see cref="CliCommand"/>
    /// or <see cref="CliApplicationWithParams"/>.</para>
    /// </summary>
    /// <typeparam name="T">The type of this parameter. Required parameters should be non-nullable. Optional
    /// parameters can either be nullable (when <see cref="DefaultValue"/> is <c>null</c>) or non-nullable
    /// (when <see cref="DefaultValue"/> is not <c>null</c>).</typeparam>
    public class CliParam<T> : CliParamBase
    {
        /// <summary>
        /// <para>The default value of this parameter. If set, the parameter is considered "optional"; if
        /// not set, the parameter is considered "required".</para>
        ///
        /// <para>Parameters are usually "required" by default. Exceptions are:</para>
        ///
        /// <list type="bullet">
        /// <item>
        /// <description>If <typeparamref name="T"/> is a nullable value type (e.g. <c>int?</c>), the default value will
        /// be set to <c>null</c>.</description>
        /// </item>
        /// <item>
        /// <description>If <typeparamref name="T"/> is <c>bool</c> and the parameter is a named parameter (see
        /// <see cref="CliParamBase.ParameterType"/>), the default value will be set to <c>false</c>.</description>
        /// </item>
        /// </list>
        ///
        /// <para>Note: To make a nullable reference type parameter (e.g. <c>string?</c>) optional, set this property
        /// to <c>null</c>.</para>
        /// </summary>
        [PublicAPI]
        public Optional<T> DefaultValue { get; init; }

        private readonly Lazy<Symbol> _underlyingImplementation;

        /// <inheritdoc />
        internal override Symbol UnderlyingImplementation => this._underlyingImplementation.Value;

        /// <summary>
        /// The value of this parameter. Important: Only available from within <see cref="CliCommand.Execute"/> of
        /// the class that defines this parameter.
        /// </summary>
        [PublicAPI]
        public T Value
        {
            get
            {
                if (!this._value.IsSet)
                {
                    throw new InvalidOperationException("This value can't be accessed at this state.");
                }

                return this._value.Value;
            }
        }

        private Optional<T> _value;

        /// <summary>
        /// Creates a named parameter (in contrast to a positional one). See <see cref="CliParamTypes.Named"/> for more details.
        /// </summary>
        /// <param name="primaryName">The primary name for this parameter; for better documentation purposes this
        /// should start with either "--", "-" or "/".</param>
        /// <param name="aliases">Other names that represent the same parameter. Usually the <paramref name="primaryName"/>
        /// would be the long form (like <c>--length</c>) and the alias(es) would be the short form (like <c>-l</c>).</param>
        public CliParam(string primaryName, params string[] aliases)
            : this(ParamsUtils.Combine(primaryName, aliases))
        {
        }

        /// <summary>
        /// Creates a named parameter (in contrast to a positional one). See <see cref="CliParamTypes.Named"/> for more details.
        ///
        /// <para>Note: You should prefer the other constructor (<see cref="CliParam{T}(string,string[])"/>) over this
        /// one.</para>
        /// </summary>
        /// <param name="names">The names/aliases for this parameter; for better documentation purposes these
        /// should start with either "--", "-" or "/".</param>
        public CliParam(IEnumerable<string> names)
            : base(names)
        {
            this._underlyingImplementation = new Lazy<Symbol>(CreateUnderlyingNamedParameter);

            if (typeof(T) == typeof(bool) || typeof(T).IsNullableValueType())
            {
                this.DefaultValue = default(T)!;
            }
        }

        /// <summary>
        /// Creates a positional parameter (in contrast to a named parameter). See <see cref="CliParamTypes.Positional"/> for more details.
        /// </summary>
        /// <param name="name">The name of this parameter; only used for generating the help text.</param>
        /// <param name="positionIndex">The position of this parameter among all other positional parameters; positional parameters
        /// are ordered by this value</param>
        public CliParam(string name, int positionIndex)
            : base(name, positionIndex)
        {
            this._underlyingImplementation = new Lazy<Symbol>(CreateUnderlyingPositionalParameter);

            if (typeof(T).IsNullableValueType())
            {
                this.DefaultValue = default(T)!;
            }
        }

        private Symbol CreateUnderlyingNamedParameter()
        {
            var option = new Option<T>(this.Names.ToArray(), this.HelpText);

            if (this.DefaultValue.IsSet)
            {
                if (ShouldSetUnderlyingDefaultValueForOptionalParameter())
                {
                    option.Argument.SetDefaultValue(this.DefaultValue.Value);
                }
            }
            else
            {
                option.IsRequired = true;
            }

            return option;
        }

        private Symbol CreateUnderlyingPositionalParameter()
        {
            var argument = new Argument<T>(this.PrimaryName, this.HelpText);

            if (this.DefaultValue.IsSet)
            {
                // Since we don't always set the default value on the argument (to hide it from the help output),
                // we have to set the arity of the argument definition instead to define that it's optional.
                // See also: https://github.com/dotnet/command-line-api/issues/1156
                argument.Arity = IsCollectionParam() ? ArgumentArity.ZeroOrMore : ArgumentArity.ZeroOrOne;

                if (ShouldSetUnderlyingDefaultValueForOptionalParameter())
                {
                    argument.SetDefaultValue(this.DefaultValue.Value);
                }
            }
            else
            {
                //
                // No default value
                //

                // Set arity explicitly to mark the argument definition as "required".
                // NOTE: This is also a bug fix for: https://github.com/dotnet/command-line-api/issues/1158
                argument.Arity = IsCollectionParam() ? ArgumentArity.OneOrMore : ArgumentArity.ExactlyOne;
            }

            return argument;
        }

        /// <summary>
        /// This method determines whether the default value is set in the underlying implementation
        /// (i.e. <see cref="Argument.SetDefaultValue"/>). For certain cases (especially when the
        /// default value is <c>null</c>), we don't set the default value as it will show up
        /// as <c>[default: ]</c> in the parameters help text - and that's not useful.
        ///
        /// <para>For more details, see <c>HelpBuilder.ShouldShowDefaultValueHint</c>.</para>
        /// </summary>
        [MustUseReturnValue]
        private bool ShouldSetUnderlyingDefaultValueForOptionalParameter()
        {
            if (this.ParameterType == CliParamTypes.Named && typeof(T) == typeof(bool))
            {
                return this.DefaultValue.Value as bool? == true;
            }
            else if (typeof(T).IsNullableValueType())
            {
                return this.DefaultValue.Value is not null;
            }
            else if (!typeof(T).IsValueType)
            {
                if (this.DefaultValue.Value is null)
                {
                    return false;
                }
                else if (IsCollectionParam())
                {
                    // Check if the collection is empty.
                    // NOTE: We don't use "ICollection" here as this would excluded "IReadOnlyCollection".
                    foreach (var _ in (IEnumerable)this.DefaultValue.Value)
                    {
                        return true;
                    }

                    return false;
                }

                return true;
            }
            else
            {
                return true;
            }
        }

        [MustUseReturnValue]
        private static bool IsCollectionParam()
        {
            return typeof(T).Is<IEnumerable>() && typeof(T) != typeof(string);
        }

        internal sealed override void SetValueFromParseResult(ParseResult parseResult)
        {
            switch (this.UnderlyingImplementation)
            {
                case Option<T> option:
                {
                    OptionResult? result = parseResult.FindResultFor(option);

                    // NOTE: If the option is not present on the command line, the underlying implementation will generate
                    //   an implicit result (if the option is optional). But only if we have set the default value at the
                    //   underlying implementation. There are cases where we don't do this
                    //   (see "ShouldSetUnderlyingDefaultValueForOptionalParameter()") in which case we have to use our
                    //   default value.
                    if (result is null)
                    {
                        this._value = this.DefaultValue.Value;
                    }
                    else
                    {
                        this._value = result.GetValueOrDefault<T>()!;
                    }

                    break;
                }

                case Argument<T> argument:
                {
                    ArgumentResult? result = parseResult.FindResultFor(argument);

                    // NOTE: If the option is not present on the command line, the underlying implementation will generate
                    //   an implicit result (if the option is optional). But only if we have set the default value at the
                    //   underlying implementation. There are cases where we don't do this
                    //   (see "ShouldSetUnderlyingDefaultValueForOptionalParameter()") in which case we have to use our
                    //   default value.
                    if (result is null)
                    {
                        this._value = this.DefaultValue.Value;
                    }
                    else
                    {
                        this._value = result.GetValueOrDefault<T>()!;
                    }

                    break;
                }

                default:
                    throw new UnexpectedSwitchValueException(nameof(this.UnderlyingImplementation), this.UnderlyingImplementation.GetType());
            }
        }
    }
}

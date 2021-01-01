﻿#region License
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
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Linq;

using AppMotor.Core.DataModel;
using AppMotor.Core.Exceptions;

using JetBrains.Annotations;

namespace AppMotor.CliApp.CommandLine
{
    /// <summary>
    /// Represents a command line parameter - either named or positional. See <see cref="CliParamBase.IsNamedParameter"/>,
    /// <see cref="CliParamBase.IsPositionalParameter"/>, and <see cref="CliParamBase.PositionIndex"/> for more details on
    /// the difference between these two parameter types.
    /// </summary>
    /// <typeparam name="T">The type of this parameter. Required parameters should be non-nullable. Optional
    /// parameters can either be nullable (when <see cref="DefaultValue"/> is <c>null</c>) or non-nullable
    /// (when <see cref="DefaultValue"/> is not <c>null</c>).</typeparam>
    public class CliParam<T> : CliParamBase
    {
        /// <summary>
        /// The default value of this parameter. If set, the parameter is considered "optional"; if
        /// not set, the parameter is considered "required".
        ///
        /// <para>Note: If <typeparamref name="T"/> is <c>bool</c>, the default value is set to "false"
        /// by default. No other type has a default default value.</para>
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
        /// Creates a named parameter (in contrast to a positional one). See <see cref="CliParamBase.IsNamedParameter"/> for more details.
        /// </summary>
        /// <param name="primaryName">The primary name for this parameter; for better documentation purposes this
        /// should start with either "--", "-" or "/".</param>
        /// <param name="aliases">Other names that represent the same parameter. Usually the <paramref name="primaryName"/>
        /// would be the long form (like <c>--length</c>) and the alias(es) would be the short form (like <c>-l</c>).</param>
        public CliParam(string primaryName, params string[] aliases)
            : base(primaryName, aliases)
        {
            this._underlyingImplementation = new Lazy<Symbol>(CreateUnderlyingNamedParameter);

            if (typeof(T) == typeof(bool))
            {
                this.DefaultValue = (T)FALSE_AS_OBJECT;
            }
        }

        /// <summary>
        /// Creates a positional parameter (in contrast to a named parameter). See <see cref="CliParamBase.IsPositionalParameter"/> for more details.
        /// </summary>
        /// <param name="name">The name of this parameter; only used for generating the help text.</param>
        /// <param name="positionIndex">The position of this parameter among all other positional parameters; positional parameters
        /// are ordered by this value</param>
        public CliParam(string name, int positionIndex)
            : base(name, positionIndex)
        {
            this._underlyingImplementation = new Lazy<Symbol>(CreateUnderlyingPositionalParameter);
        }

        private Symbol CreateUnderlyingNamedParameter()
        {
            var option = new Option<T>(this.Names.ToArray(), this.HelpText);

            if (this.DefaultValue.IsSet)
            {
                option.Argument.SetDefaultValue(this.DefaultValue.Value);
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
                argument.SetDefaultValue(this.DefaultValue.Value);
            }

            return argument;
        }

        internal sealed override void SetValueFromParseResult(ParseResult parseResult)
        {
            switch (this.UnderlyingImplementation)
            {
                case Option<T> option:
                {
                    OptionResult? result = parseResult.FindResultFor(option);

                    // NOTE: If the option is not present on the command line, an implicit result will be generated
                    //   (if the option is optional). Thus, in our case, the result should never be null. (It would
                    //   only be "null" if we passed an option that's unknown to the parser at parse time.)
                    if (result is null)
                    {
                        throw new UnexpectedBehaviorException($"No parse result for option '{option.Name}'.");
                    }

                    this._value = result.GetValueOrDefault<T>()!;

                    break;
                }

                case Argument<T> argument:
                {
                    ArgumentResult? result = parseResult.FindResultFor(argument);

                    // NOTE: If the argument is not present on the command line, an implicit result will be generated.
                    //   (if the argument is optional) Thus, in our case, the result should never be null. (It would
                    //   only be "null" if we passed an option that's unknown to the parser at parse time.)
                    if (result is null)
                    {
                        throw new UnexpectedBehaviorException($"No parse result for argument '{argument.Name}'.");
                    }

                    this._value = result.GetValueOrDefault<T>()!;

                    break;
                }

                default:
                    throw new UnexpectedSwitchValueException(nameof(this.UnderlyingImplementation), this.UnderlyingImplementation.GetType());
            }
        }
    }
}
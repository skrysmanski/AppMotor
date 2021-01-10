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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.CommandLine;

using AppMotor.CliApp.CommandLine.Utils;
using AppMotor.Core.Utils;

using JetBrains.Annotations;

namespace AppMotor.CliApp.CommandLine
{
    /// <summary>
    /// Represents a verb in a command line (like <c>add</c> in <c>git add .</c>). Verbs can be executable (if <see cref="Command"/>
    /// is set) or simply used for grouping.
    /// </summary>
    public class CliVerb
    {
        /// <summary>
        /// The name of this verb - as it has to be entered by the user on the command line.
        /// </summary>
        [PublicAPI]
        public string Name { get; }

        /// <summary>
        /// Aliases of <see cref="Name"/> (i.e. alternative names the user can enter to "target" this verb).
        ///
        /// <para>You may use this for abbreviations of the primary name (e.g. <c>mv</c> as alias for <c>move</c>).</para>
        /// </summary>
        [PublicAPI]
        public ImmutableList<string> Aliases { get; }

        /// <summary>
        /// The help text for this verb.
        /// </summary>
        [PublicAPI]
        public string? HelpText { get; init; }

        /// <summary>
        /// The command for this verb.
        /// </summary>
        [PublicAPI]
        public CliCommand? Command { get; init; }

        /// <summary>
        /// The sub verbs of this verb. May be <c>null</c> (or an empty collection) if no sub
        /// verbs exist.
        /// </summary>
        // NOTE: The type of this property is not ImmutableArray on purpose - because you can't initialize
        //   ImmutableArray with an array initializer (i.e. "new[] { ... }") - which is what we want for ease of use.
        [PublicAPI]
        public IReadOnlyList<CliVerb>? SubVerbs
        {
            get => this._subVerbs;
            init => this._subVerbs = value?.ToImmutableArray();
        }

        private readonly ImmutableArray<CliVerb>? _subVerbs;

        private Command? _underlyingImplementation;

        /// <summary>
        /// Constructor.
        /// </summary>
        public CliVerb(string name, params string[] aliases)
            : this(name, command: null, aliases)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public CliVerb(string name, CliCommand? command, params string[] aliases)
        {
            ValidateCommandName(name);
            Validate.Argument.IsNotNull(aliases, nameof(aliases));

            foreach (var alias in aliases)
            {
                ValidateCommandName(alias);
            }

            this.Name = name;
            this.Aliases = aliases.ToImmutableList();
            this.Command = command;
        }

        private static void ValidateCommandName(string name)
        {
            Validate.Argument.IsNotNullOrWhiteSpace(name, nameof(name));

            if (HelpParamUtils.IsHelpParamName(name) || name.Equals(HelpParamUtils.HelpCommandName, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException($"The name '{name}' is reserved and can't be used.", nameof(name));
            }
        }

        /// <summary>
        /// Creates the underlying implementation.
        /// </summary>
        internal Command ToUnderlyingImplementation(bool enableDebugParam)
        {
            if (this._underlyingImplementation is null)
            {
                var command = new Command(this.Name, this.HelpText ?? this.Command?.HelpText);

                foreach (var alias in this.Aliases)
                {
                    command.AddAlias(alias);
                }

                if (this.SubVerbs != null)
                {
                    foreach (var subVerb in this.SubVerbs)
                    {
                        command.AddCommand(subVerb.ToUnderlyingImplementation(enableDebugParam: enableDebugParam));
                    }
                }

                if (this.Command is not null)
                {
                    var commandHandler = new CliCommand.CliCommandHandler(this.Command, enableDebugParam: enableDebugParam);

                    foreach (var cliParam in commandHandler.AllParams)
                    {
                        command.Add(cliParam.UnderlyingImplementation);
                    }

                    command.Handler = commandHandler;
                }

                this._underlyingImplementation = command;
            }

            return this._underlyingImplementation;
        }
    }
}

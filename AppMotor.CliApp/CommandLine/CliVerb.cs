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
using System.Collections.Immutable;
using System.CommandLine;

using AppMotor.CliApp.CommandLine.Utils;
using AppMotor.Core.Utils;

using JetBrains.Annotations;

namespace AppMotor.CliApp.CommandLine
{
    /// <summary>
    /// Represents a verb in a command line (like <c>add</c> in <c>git add .</c>). Verbs can be executable (via <see cref="CliCommand"/>)
    /// or simply used for grouping (via <see cref="CliVerbGroup"/>).
    /// </summary>
    public abstract class CliVerb
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
        public virtual string? HelpText => null;

        private readonly Lazy<Command> _underlyingImplementation;

        internal Command UnderlyingImplementation => this._underlyingImplementation.Value;

        /// <summary>
        /// Constructor.
        /// </summary>
        protected CliVerb(string name, string[] aliases)
        {
            ValidateCommandName(name);
            Validate.Argument.IsNotNull(aliases, nameof(aliases));

            foreach (var alias in aliases)
            {
                ValidateCommandName(alias);
            }

            this.Name = name;
            this.Aliases = aliases.ToImmutableList();

            this._underlyingImplementation = new Lazy<Command>(ToUnderlyingImplementation);
        }

        private static void ValidateCommandName(string name)
        {
            Validate.Argument.IsNotNull(name, nameof(name));

            if (HelpParamUtils.IsHelpParamName(name) || name.Equals(HelpParamUtils.HelpCommandName, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException($"The name '{name}' is reserved and can't be used.", nameof(name));
            }
        }

        /// <summary>
        /// Creates the underlying implementation. Callers should not call this method but use
        /// <see cref="UnderlyingImplementation"/> instead.
        /// </summary>
        // ReSharper disable once MemberCanBeProtected.Global
        internal abstract Command ToUnderlyingImplementation();
    }
}

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

using System.Collections.Generic;
using System.CommandLine;

using JetBrains.Annotations;

namespace AppMotor.CliApp.CommandLine
{
    /// <summary>
    /// Groups multiple sub verbs (i.e. <see cref="CliCommand"/> or <see cref="CliVerbGroup"/>) under a common name. This class is mostly
    /// like <see cref="CliCommand"/> except that it can't be executed (i.e. no <see cref="CliCommand.Executor"/>)
    /// and it can't have parameters (i.e. <see cref="CliParam{T}"/>).
    /// </summary>
    public abstract class CliVerbGroup : CliVerb
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The primary name of this verb group.</param>
        /// <param name="aliases">Optional alias names that also represent this verb group.</param>
        protected CliVerbGroup(string name, params string[] aliases) : base(name, aliases)
        {
        }

        /// <summary>
        /// Returns all sub verbs (i.e. <see cref="CliCommand"/> or <see cref="CliVerbGroup"/>) for this
        /// verb group.
        /// </summary>
        [PublicAPI]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1024:Use properties where appropriate", Justification = "Potentially expensive execution")]
        protected abstract IEnumerable<CliVerb> GetSubVerbs();

        internal sealed override Command ToUnderlyingImplementation()
        {
            var command = new Command(this.Name, this.HelpText);

            foreach (var alias in this.Aliases)
            {
                command.AddAlias(alias);
            }

            foreach (var subCommand in GetSubVerbs())
            {
                command.AddCommand(subCommand.UnderlyingImplementation);
            }

            return command;
        }
    }
}

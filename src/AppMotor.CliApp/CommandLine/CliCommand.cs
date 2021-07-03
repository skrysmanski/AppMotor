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
using System.CommandLine.Invocation;
using System.Threading;
using System.Threading.Tasks;

using AppMotor.CliApp.CommandLine.Utils;
using AppMotor.CliApp.Properties;
using AppMotor.CliApp.Terminals;

using JetBrains.Annotations;

namespace AppMotor.CliApp.CommandLine
{
    /// <summary>
    /// Represents a command (or executable verb; see <see cref="CliVerb"/>) in a command line call; e.g.
    /// in "git add ." the word "add" is the command.
    ///
    /// <para>Commands can be nested like "myapp command1 subcommmand --some-option".</para>
    ///
    /// <para>Can also be used as root command with <see cref="CliApplicationWithCommand"/>.</para>
    /// </summary>
    public abstract class CliCommand
    {
        /// <summary>
        /// The help text for this command.
        /// </summary>
        [PublicAPI]
        public virtual string? HelpText => null;

        /// <summary>
        /// The main/execute method for this command. Implementations can access all command line parameters though
        /// the <see cref="CliParam{T}.Value"/> properties of this <see cref="CliParam{T}"/> instance.
        ///
        /// <para>Recommendation: For ease of use, use the array syntax (<c>=&gt;</c>) when implementing
        /// this property.</para>
        /// </summary>
        protected abstract CliCommandExecutor Executor { get; }

        /// <summary>
        /// The terminal to use within this application. Inherited from the <see cref="CliApplication"/> this
        /// command runs in.
        /// </summary>
        /// <remarks>
        /// This property mainly exists for unit testing purposes where you need to obtain
        /// everything written to the terminal.
        /// </remarks>
        [PublicAPI]
        public ITerminal Terminal => this._terminal ?? throw new InvalidOperationException("The terminal is not available in this command state.");

        private ITerminal? _terminal;

        /// <summary>
        /// Runs this command.
        /// </summary>
        /// <returns>The exit code for the running program.</returns>
        private async Task<int> Execute(ITerminal terminal, CancellationToken cancellationToken)
        {
            this._terminal = terminal;
            return await this.Executor.Execute(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
        }

        /// <summary>
        /// Returns all parameters defined for this command. The default implementation uses reflection to find all properties
        /// and fields of type <see cref="CliParamBase"/>. Inheritors may override this method either to filter its result or provide
        /// their own list.
        /// </summary>
        [PublicAPI]
        protected virtual IEnumerable<CliParamBase> GetAllParams()
        {
            return CliParamUtils.GetAllParamsFor(this);
        }

        internal sealed class CliCommandHandler : ICommandHandler
        {
            private readonly CliCommand _command;

            private readonly ITerminal _terminal;

            private readonly CancellationToken _cancellationToken;

            public ImmutableArray<CliParamBase> AllParams { get; }

            private CliParam<bool>? DebugParam { get; }

            public CliCommandHandler(CliCommand command, bool enableDebugParam, ITerminal terminal, CancellationToken cancellationToken)
            {
                this._command = command;
                this._terminal = terminal;
                this._cancellationToken = cancellationToken;

                var paramsCollectionBuilder = new ParamsCollectionBuilder();

                foreach (var cliParam in command.GetAllParams())
                {
                    paramsCollectionBuilder.AddParam(cliParam);
                }

                if (enableDebugParam)
                {
                    var availableAliases = new List<string>();

                    if (!paramsCollectionBuilder.RegisteredAliases.Contains("--debug"))
                    {
                        availableAliases.Add("--debug");
                    }

                    if (!paramsCollectionBuilder.RegisteredAliases.Contains("-d"))
                    {
                        availableAliases.Add("-d");
                    }

                    if (availableAliases.Count > 0)
                    {
                        this.DebugParam = new(availableAliases)
                        {
                            HelpText = LocalizableResources.DebugParamHelpText,
                        };

                        paramsCollectionBuilder.AddParam(this.DebugParam);
                    }
                }

                this.AllParams = paramsCollectionBuilder.Build();
            }

            /// <inheritdoc />
            public async Task<int> InvokeAsync(InvocationContext context)
            {
                foreach (var cliParam in this.AllParams)
                {
                    cliParam.SetValueFromParseResult(context.ParseResult);
                }

                if (this.DebugParam?.Value == true && !DebuggerUtils.IsDebuggerAttached)
                {
                    DebuggerUtils.LaunchDebugger(this._terminal);
                }

                return await this._command.Execute(this._terminal, this._cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
            }
        }
    }
}

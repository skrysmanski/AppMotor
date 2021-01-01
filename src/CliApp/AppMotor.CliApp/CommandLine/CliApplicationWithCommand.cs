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
using System.CommandLine;
using System.Threading.Tasks;

using AppMotor.CliApp.CommandLine.Utils;
using AppMotor.Core.Utils;

using JetBrains.Annotations;

namespace AppMotor.CliApp.CommandLine
{
    /// <summary>
    /// Represents a command line application with automatic command line argument parsing that only does
    /// one function - like the <c>mv</c>, <c>rm</c>, or <c>dir</c> commands.
    ///
    /// <para>The main command is specified either via constructor or via the <see cref="Command"/> property.</para>
    ///
    /// <para>If you need an application that bundles various functions, use <see cref="CliApplicationWithVerbs"/> instead.
    /// If you want to specify the <see cref="CliParam{T}"/>s directly in a sub class of this class, use
    /// <see cref="CliApplicationWithParams"/> instead.</para>
    /// </summary>
    public class CliApplicationWithCommand : CliApplication
    {
        /// <summary>
        /// The description of this application. Used for generating the help text.
        /// </summary>
        [PublicAPI]
        protected string? AppDescription { get; init; }

        /// <inheritdoc />
        protected sealed override CliApplicationExecutor MainExecutor => new(Execute);

        public CliCommand Command
        {
            get => this._command ?? throw new InvalidOperationException($"The property '{nameof(this.Command)}' has never been set.");
            init => this._command = value;
        }

        private readonly CliCommand? _command;

        public CliApplicationWithCommand()
        {
        }

        public CliApplicationWithCommand(CliCommand command)
        {
            Validate.Argument.IsNotNull(command, nameof(command));

            this.Command = command;
        }

        private async Task<int> Execute(string[] args)
        {
            var rootCommand = RootCommandFactory.CreateRootCommand(
                appDescription: this.AppDescription,
                exceptionHandlerFunc: ProcessUnhandledException
            );

            var commandHandler = new CliCommand.CliCommandHandler(this.Command);

            foreach (var cliParam in commandHandler.AllParams)
            {
                rootCommand.Add(cliParam.UnderlyingImplementation);
            }

            rootCommand.Handler = commandHandler;

            return await rootCommand.InvokeAsync(args, new CommandLineConsole(this.Terminal)).ConfigureAwait(continueOnCapturedContext: false);
        }
    }
}

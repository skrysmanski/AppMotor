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
using System.Collections.Immutable;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Threading.Tasks;

using AppMotor.CliApp.CommandLine.Utils;
using AppMotor.Core.Utils;

using JetBrains.Annotations;

namespace AppMotor.CliApp.CommandLine
{
    /// <summary>
    /// Represents a command line application with automatic command line argument parsing that only does
    /// one function - like the <c>mv</c>, <c>rm</c>, or <c>dir</c> commands. If you need an application
    /// that bundles various functions, use <see cref="CliApplicationWithCommands"/> instead.
    /// </summary>
    /// <remarks>
    /// Parameters (<see cref="CliParam{T}"/>) defined in a sub class are detected automatically (via reflection).
    /// Otherwise you can override <see cref="GetAllParams"/>.
    /// </remarks>
    public abstract class CliApplicationWithoutCommands : CliApplication
    {
        private ImmutableList<CliParam>? m_allParams;

        /// <summary>
        /// The description of this application. Used for generating the help text.
        /// </summary>
        [PublicAPI]
        protected virtual string? AppDescription => null;

        /// <inheritdoc />
        protected sealed override CliApplicationExecutor MainExecutor => new(Execute);

        /// <summary>
        /// Executes this application. Implementations can access all command line parameters though the <see cref="CliParam{T}.Value"/>
        /// properties of the <see cref="CliParam{T}"/> instances declared in this class (or its base classes).
        /// </summary>
        protected abstract CliCommandExecutor Executor { get; }

        private async Task<int> Execute(string[] args)
        {
            this.m_allParams = GetAllParams().ToImmutableList();

            var rootCommand = RootCommandFactory.CreateRootCommand(
                appDescription: this.AppDescription,
                exceptionHandlerFunc: ProcessUnhandledException
            );

            foreach (var cliParam in this.m_allParams)
            {
                rootCommand.Add(cliParam.UnderlyingImplementation);
            }

            rootCommand.Handler = new CliCommandHandler(this);

            return await rootCommand.InvokeAsync(args, new CommandLineConsole(this.Terminal));
        }

        /// <summary>
        /// Returns all parameters defined for this application. The default implementation uses reflection to find all properties
        /// and fields of type <see cref="CliParam"/>. Inheritors may override this method either to filter its result or provide
        /// their own list.
        /// </summary>
        [PublicAPI]
        protected virtual IEnumerable<CliParam> GetAllParams()
        {
            return CliParamUtils.GetAllParamsFor(this);
        }

        private void SetAllParamValues(ParseResult parseResult)
        {
            // Should never happen.
            Validate.Value.IsNotNull(this.m_allParams, nameof(this.m_allParams));

            foreach (var cliParam in this.m_allParams)
            {
                cliParam.SetValueFromParseResult(parseResult);
            }
        }

        private sealed class CliCommandHandler : ICommandHandler
        {
            private readonly CliApplicationWithoutCommands m_app;

            public CliCommandHandler(CliApplicationWithoutCommands app)
            {
                this.m_app = app;
            }

            /// <inheritdoc />
            public async Task<int> InvokeAsync(InvocationContext context)
            {
                this.m_app.SetAllParamValues(context.ParseResult);

                return await this.m_app.Executor.Execute();
            }
        }
    }
}

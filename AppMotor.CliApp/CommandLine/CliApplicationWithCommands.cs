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
using System.CommandLine;
using System.Linq;
using System.Threading.Tasks;

using AppMotor.CliApp.CommandLine.Utils;

using JetBrains.Annotations;

namespace AppMotor.CliApp.CommandLine
{
    /// <summary>
    /// Represents a command line application with automatic command line argument parsing that bundles various
    /// functions (via <see cref="CliCommand"/>s) - like the <c>git</c> or <c>dotnet</c> commands. If you need an
    /// application that only does one thing, use <see cref="CliApplicationWithoutCommands"/> instead.
    /// </summary>
    /// <remarks>
    /// Sub classes cannot have parameters (<see cref="CliParam{T}"/>) of their own. Parameter can only exist
    /// on the commands.
    /// </remarks>
    public abstract class CliApplicationWithCommands : CliApplication
    {
        /// <summary>
        /// The description of this application. Used for generating the help text.
        /// </summary>
        [PublicAPI]
        protected virtual string? AppDescription => null;

        /// <inheritdoc />
        protected sealed override CliApplicationExecutor MainExecutor => new(Execute);

        private async Task<int> Execute(string[] args)
        {
            var rootCommand = RootCommandFactory.CreateRootCommand(
                appDescription: this.AppDescription,
                exceptionHandlerFunc: ProcessUnhandledException
            );

            foreach (var cliVerb in GetVerbs())
            {
                rootCommand.AddCommand(cliVerb.UnderlyingImplementation);
            }

            return await rootCommand.InvokeAsync(
                SortHelpFirst(args),
                new CommandLineConsole(this.Terminal)
            );
        }

        /// <summary>
        /// The "System.Commandline" library only supports the help parameter as first parameter;
        /// e.g. "myapp --help mycommand" is supported but "myapp mycommand --help" is not. Thus, we
        /// simply move the help parameter to the front, if there is one in the args.
        /// </summary>
        [MustUseReturnValue]
        private static string[] SortHelpFirst(string[] args)
        {
            if (args.Length > 0 && HelpParamUtils.IsHelpParamName(args[0]))
            {
                // Help param is already first param. Nothing to do.
                return args;
            }

            if (args.Length > 0 && string.Equals(args[0], HelpParamUtils.HelpCommandName, StringComparison.OrdinalIgnoreCase))
            {
                // Usage of virtual "help" command
                var newArgs = new List<string>(args.Length);
                newArgs.Add(HelpParamUtils.DefaultHelpParamName);
                newArgs.AddRange(args[1..]);
                return newArgs.ToArray();
            }

            bool hasHelpParam = args.Any(HelpParamUtils.IsHelpParamName);
            if (!hasHelpParam)
            {
                // No help param used. Nothing to do.
                return args;
            }
            else
            {
                // Set first arg to the help param and add the others afterwards.
                var newArgs = new List<string>(args.Length);
                newArgs.Add(HelpParamUtils.DefaultHelpParamName);
                newArgs.AddRange(args.Where(arg => !HelpParamUtils.IsHelpParamName(arg)));
                return newArgs.ToArray();
            }
        }

        /// <summary>
        /// Returns all verbs (<see cref="CliCommand"/> and <see cref="CliVerbGroup"/>) that are supported by
        /// this application.
        /// </summary>
        protected abstract IEnumerable<CliVerb> GetVerbs();
    }
}

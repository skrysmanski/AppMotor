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

using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;

using AppMotor.CliApp.Terminals;

namespace AppMotor.CliApp.CommandLine.Utils;

internal static class RootCommandInvoker
{
    public static async Task<int> InvokeRootCommand(
            string? appDescription,
            IEnumerable<Symbol> rootSymbols,
            ICommandHandler? commandHandler,
            ITerminal terminal,
            string[] args,
            Func<Exception, int> exceptionHandlerFunc
        )
    {
        var rootCommand = new RootCommand();

        if (!string.IsNullOrWhiteSpace(appDescription))
        {
            rootCommand.Description = appDescription;
        }

        foreach (var symbol in rootSymbols)
        {
            rootCommand.Add(symbol);
        }

        if (commandHandler is not null)
        {
            rootCommand.Handler = commandHandler;
        }

        // IMPORTANT: This must be called after all root symbols have been added - otherwise
        //   the "--version" and the "--help" option will be listed before other named parameters.
        CreatePipelineFor(rootCommand, exceptionHandlerFunc);

        return await rootCommand.InvokeAsync(args, CommandLineConsole.FromTerminal(terminal))
                                .ConfigureAwait(continueOnCapturedContext: false);
    }

    private static void CreatePipelineFor(RootCommand rootCommand, Func<Exception, int> exceptionHandlerFunc)
    {
        var builder = new CommandLineBuilder(rootCommand);

        builder.UseDefaults();

        builder.UseExceptionHandler((exception, context) => {
            int exitCode = exceptionHandlerFunc(exception);
            context.ExitCode = exitCode;
        });

        builder.Build();
    }
}

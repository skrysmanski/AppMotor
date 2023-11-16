// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;

using AppMotor.CliApp.Terminals;

using JetBrains.Annotations;

namespace AppMotor.CliApp.CommandLine.Utils;

internal static class RootCommandInvoker
{
    [MustUseReturnValue]
    public static async Task<int> InvokeRootCommandAsync(
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
            switch (symbol)
            {
                case Argument argument:
                    rootCommand.Add(argument);
                    break;

                case Option option:
                    rootCommand.Add(option);
                    break;

                case Command command:
                    rootCommand.Add(command);
                    break;

                default:
                    // NOTE: Can't be reached by code coverage because we can't create our own "Symbol" implementation.
                    throw new NotSupportedException($"The 'Symbol' child type '{symbol.GetType()}' is not supported.");
            }
        }

        if (commandHandler is not null)
        {
            rootCommand.Handler = commandHandler;
        }

        // IMPORTANT: This must be called after all root symbols have been added - otherwise
        //   the "--version" and the "--help" option will be listed before other named parameters.
        var parser = CreatePipelineFor(rootCommand, exceptionHandlerFunc);

        return await parser.InvokeAsync(args, CommandLineConsole.FromTerminal(terminal))
                           .ConfigureAwait(continueOnCapturedContext: false);
    }

    private static Parser CreatePipelineFor(RootCommand rootCommand, Func<Exception, int> exceptionHandlerFunc)
    {
        var builder = new CommandLineBuilder(rootCommand);

        builder.UseDefaults();

        builder.UseExceptionHandler((exception, context) => {
            int exitCode = exceptionHandlerFunc(exception);
            context.ExitCode = exitCode;
        });

        return builder.Build();
    }
}

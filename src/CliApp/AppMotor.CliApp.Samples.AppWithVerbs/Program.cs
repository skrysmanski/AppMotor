using System;

using AppMotor.CliApp.CommandLine;

namespace AppMotor.CliApp.Samples.AppWithVerbs
{
    /// <summary>
    /// This sample provides a command line interface similar to Git's CLI.
    /// </summary>
    internal static class Program
    {
        private static int Main(string[] args)
        {
            var app = new CliApplicationWithVerbs()
            {
                AppDescription = "Command line interface for the Geet version control system",
                Verbs = new[]
                {
                    new CliVerb("clone", new CloneCommand()),
                    new CliVerb("init", new InitCommand()),
                    new CliVerb("add", new AddCommand()),
                    new CliVerb("mv", new MoveCommand()),
                },
            };

            return app.Run(args);
        }
    }
}

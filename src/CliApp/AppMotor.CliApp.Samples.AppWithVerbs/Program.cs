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
                AppDescription = ".NET wrapper around Git (for demonstration purposes). The commands are non-functional.",
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

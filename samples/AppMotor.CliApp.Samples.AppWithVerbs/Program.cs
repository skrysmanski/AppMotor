using AppMotor.CliApp.CommandLine;
using AppMotor.CliApp.Samples.AppWithVerbs;

var app = new CliApplicationWithVerbs()
{
    AppDescription = ".NET wrapper around Git (for demonstration purposes). The commands are non-functional.",
    Verbs = new[]
    {
        new CliVerb("clone", new CloneCommand()),
        new CliVerb("add", new AddCommand()),
    },
};

return app.Run(args);

using AppMotor.CliApp.CommandLine;

namespace AppMotor.CliApp.Samples.AppWithVerbs
{
    internal abstract class GitCommandBase : CliCommand
    {
        protected CliParam<bool> VerboseParam { get; } = new("--verbose", "-v")
        {
            HelpText = "Run verbosely. Does not affect the reporting of progress status to the standard error stream.",
        };
    }
}

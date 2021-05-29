using System;
using System.IO;

using AppMotor.CliApp.CommandLine;
using AppMotor.CliApp.Terminals;

namespace AppMotor.CliApp.Samples.AppWithVerbs
{
    internal sealed class CloneCommand : GitCommandBase
    {
        /// <inheritdoc />
        public override string HelpText => "Clone a repository into a new directory";

        /// <inheritdoc />
        protected override CliCommandExecutor Executor => new(Execute);

        private CliParam<string> RepositoryParam { get; } = new(name: "repository", positionIndex: 0)
        {
            HelpText = "The (possibly remote) repository to clone from.",
        };

        private CliParam<DirectoryInfo?> DirectoryParam { get; } = new(name: "directory", positionIndex: 1)
        {
            HelpText = "The name of a new directory to clone into. The \"humanish\" part of the source repository is used if no directory is explicitly given (repo for /path/to/repo.git and foo for host.xz:foo/.git). Cloning into an existing directory is only allowed if the directory is empty.",
            DefaultValue = null,
        };

        private CliParam<string?> BranchParam { get; } = new("--branch", "-b")
        {
            DefaultValue = null,
            HelpText = "Instead of pointing the newly created HEAD to the branch pointed to by the cloned repository’s HEAD, point to <name> branch instead.",
        };

        private CliParam<string[]> ConfigParam { get; } = new("--config", "-c")
        {
            DefaultValue = Array.Empty<string>(),
            HelpText = "Set a configuration variable in the newly-created repository; this takes effect immediately after the repository is initialized, but before the remote history is fetched or any files checked out.",
        };

        private void Execute()
        {
            if (this.QuietParam.Value)
            {
                Terminal.WriteLine((TextInDarkGray)"This command would run quite.");
                Terminal.WriteLine();
            }

            if (this.VerboseParam.Value)
            {
                Terminal.WriteLine((TextInDarkGray)"This command would run verbose.");
                Terminal.WriteLine();
            }

            Terminal.WriteLine("This command would clone the repository:");
            Terminal.WriteLine();
            Terminal.WriteLine((TextInCyan)$"  {this.RepositoryParam.Value}");
            Terminal.WriteLine();
            Terminal.WriteLine("To: ");
            Terminal.WriteLine();
            Terminal.WriteLine((TextInCyan)$"  {this.DirectoryParam.Value}");
            Terminal.WriteLine();
            Terminal.WriteLine("With the selected branch:");
            Terminal.WriteLine();
            Terminal.WriteLine((TextInCyan)$"  {this.BranchParam.Value ?? "<the default branch>"}");
            Terminal.WriteLine();

            foreach (var configPair in this.ConfigParam.Value)
            {
                Terminal.WriteLine($"Setting config: {configPair}");
            }
        }
    }
}

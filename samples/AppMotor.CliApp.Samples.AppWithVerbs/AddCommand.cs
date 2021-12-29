using System.IO;

using AppMotor.CliApp.CommandLine;
using AppMotor.CliApp.Terminals;

namespace AppMotor.CliApp.Samples.AppWithVerbs;

internal class AddCommand : GitCommandBase
{
    /// <inheritdoc />
    public override string HelpText => "Add file contents to the index";

    /// <inheritdoc />
    protected override CliCommandExecutor Executor => new(Execute);

    private CliParam<bool> DryRunParam { get; } = new("--dry-run", "-n")
    {
        HelpText = "Don’t actually add the file(s), just show if they exist and/or will be ignored.",
    };

    private CliParam<string?> ChmodParam { get; } = new("--chmod")
    {
        DefaultValue = null,
        HelpText = "Override the executable bit of the added files. The executable bit is only changed in the index, the files on disk are left unchanged.",
    };

    private CliParam<FileInfo[]> FilesParam { get; } = new("files", positionIndex: 0)
    {
        HelpText = "The files to add.",
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

        if (this.DryRunParam.Value)
        {
            Terminal.WriteLine("Just a dry-run.");
            Terminal.WriteLine();
        }

        if (this.ChmodParam.Value is null)
        {
            Terminal.WriteLine("This command would add the following files:");
        }
        else
        {
            Terminal.WriteLine($"This command would add the following files (with chmod={this.ChmodParam.Value}):");
        }
        Terminal.WriteLine();

        foreach (var fileToAdd in this.FilesParam.Value)
        {
            Terminal.WriteLine((TextInCyan)$" - {fileToAdd.FullName}");
        }
    }
}
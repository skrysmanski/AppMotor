using AppMotor.CliApp.CommandLine;

using static AppMotor.CliApp.Terminals.Formatting.TermText;

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
            this.Terminal.WriteLine(DarkGray("This command would run quite."));
            this.Terminal.WriteLine();
        }

        if (this.VerboseParam.Value)
        {
            this.Terminal.WriteLine(DarkGray("This command would run verbose."));
            this.Terminal.WriteLine();
        }

        if (this.DryRunParam.Value)
        {
            this.Terminal.WriteLine("Just a dry-run.");
            this.Terminal.WriteLine();
        }

        if (this.ChmodParam.Value is null)
        {
            this.Terminal.WriteLine("This command would add the following files:");
        }
        else
        {
            this.Terminal.WriteLine($"This command would add the following files (with chmod={this.ChmodParam.Value}):");
        }

        this.Terminal.WriteLine();

        foreach (var fileToAdd in this.FilesParam.Value)
        {
            this.Terminal.WriteLine(Cyan($" - {fileToAdd.FullName}"));
        }
    }
}

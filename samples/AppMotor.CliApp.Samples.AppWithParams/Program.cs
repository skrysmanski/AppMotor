using AppMotor.CliApp.CommandLine;
using AppMotor.Core.Exceptions;

namespace AppMotor.CliApp.Samples.AppWithParams;

internal sealed class Program : CliApplicationWithParams
{
    /// <inheritdoc />
    protected override CliCommandExecutor Executor => new(Execute);

    private CliParam<FileInfo> SourceParam { get; } = new("source", positionIndex: 0)
    {
        HelpText = "The source to copy.",
    };

    private CliParam<string> DestParam { get; } = new("destination", positionIndex: 1)
    {
        HelpText = "The target to copy to.",
    };

    private CliParam<bool> NoPromptParam { get; } = new("-y")
    {
        HelpText = "Suppresses prompting to confirm you want to overwrite an existing destination file. (With some very long text that's hopefully so long that it has to wrap.)",
    };

    private CliParam<bool> AssumeTargetIsDirectoryParam { get; } = new("--assume-directory", "-d")
    {
        HelpText = "Assume the target is a directory if the target does not exist.",
    };

    private static int Main(string[] args)
    {
        return Run<Program>(args);
    }

    private int Execute()
    {
        if (!this.SourceParam.Value.Exists)
        {
            throw new ErrorMessageException($"The source file '{this.SourceParam.Value.FullName}' does not exist.");
        }

        FileInfo destFile;

        if (Directory.Exists(this.DestParam.Value))
        {
            destFile = new FileInfo(Path.Combine(this.DestParam.Value, this.SourceParam.Value.Name));
        }
        else if (File.Exists(this.DestParam.Value))
        {
            destFile = new FileInfo(this.DestParam.Value);
        }
        else
        {
            // Target does not exist either as file or directory.
            if (this.AssumeTargetIsDirectoryParam.Value)
            {
                destFile = new FileInfo(Path.Combine(this.DestParam.Value, this.SourceParam.Value.Name));
            }
            else
            {
                destFile = new FileInfo(this.DestParam.Value);
            }
        }

        if (!destFile.Directory!.Exists)
        {
            destFile.Directory.Create();
        }

        if (destFile.Exists && !this.NoPromptParam.Value)
        {
            this.Terminal.WriteLine($"The target file '{destFile.FullName}' already exists. Overwrite? (y/n)");
            var input = this.Terminal.ReadLine();
            switch (input?.Trim().ToLowerInvariant())
            {
                case "y":
                    break;

                case "n":
                    return 1;

                default:
                    throw new ErrorMessageException("Invalid response.");
            }
        }

        this.SourceParam.Value.CopyTo(destFile.FullName, overwrite: true);

        this.Terminal.WriteLine($"Copied '{this.SourceParam.Value.Name}' to '{destFile.FullName}'");
        return 0;
    }
}
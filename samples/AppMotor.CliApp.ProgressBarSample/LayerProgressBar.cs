using AppMotor.CliApp.ProgressBarSample.DataModels;
using AppMotor.CliApp.Terminals;
using AppMotor.Core.Extensions;

namespace AppMotor.CliApp.ProgressBarSample;

internal sealed class LayerProgressBar
{
    private const int BLOCK_COUNT = 30;

    /// <summary>
    /// The characters for a rotating line animation.
    /// </summary>
    private const string PROGRESS_ANIMATION_CHARS = @"|/-\";

    private readonly LayerInfo _layerInfo;

    public string LayerId => this._layerInfo.Id;

    private int _animationIndex;

    public LayerProgressBar(LayerInfo layerInfo)
    {
        this._layerInfo = layerInfo;
    }

    public void Update(LayerPullInfo pullInfo)
    {
        Terminal.Write($"{this._layerInfo.Id}: ");

        if (!pullInfo.IsDownloadComplete)
        {
            if (pullInfo.DownloadedSizeInKb == 0)
            {
                Terminal.Write("Waiting");
            }
            else
            {
                ReportProgress("Downloading", pullInfo.DownloadedSizeInKb);
            }
        }
        else if (!pullInfo.IsExtractionComplete)
        {
            if (pullInfo.ExtractedSizeInKb == 0)
            {
                Terminal.Write("Download complete");
            }
            else
            {
                ReportProgress("Extracting", pullInfo.ExtractedSizeInKb);
            }
        }
        else
        {
            Terminal.Write("Pull complete");
        }

        // Clear the remaining of the line
        if (Terminal.CursorLeft < Terminal.TerminalWidth - 1)
        {
            Terminal.Write(new string(' ', Terminal.TerminalWidth - 1 - Terminal.CursorLeft));
        }

        Terminal.WriteLine();
    }

    private void ReportProgress(string what, int sizeInKb)
    {
        Terminal.Write($"{what} ");

        // Make sure value is in [0..1] range
        double value = Math.Min(1, (double)sizeInKb / this._layerInfo.SizeInKb);

        int progressBlockCount = (int)(value * BLOCK_COUNT);
        int percent = (int)(value * 100);

        this._animationIndex = (this._animationIndex + 1) % PROGRESS_ANIMATION_CHARS.Length;

        string text = "[{0}{1}] {2,3}% {3}".WithIC(
            new string('#', progressBlockCount),
            new string('-', BLOCK_COUNT - progressBlockCount),
            percent,
            PROGRESS_ANIMATION_CHARS[this._animationIndex]
        );

        Terminal.Write(text);

        if (this._layerInfo.SizeInKb < 1024)
        {
            Terminal.Write("  {0}KB/{1}KB".WithIC(sizeInKb, this._layerInfo.SizeInKb));
        }
        else
        {
            Terminal.Write("  {0:0.00}MB/{1:0.00}MB".WithIC(sizeInKb / 1024.0, this._layerInfo.SizeInKb / 1024.0));
        }
    }
}

namespace AppMotor.CliApp.ProgressBarSample.DataModels;

internal sealed class LayerPullInfo
{
    private readonly LayerInfo _layer;

    public string LayerId => this._layer.Id;

    public int DownloadedSizeInKb { get; private set; }

    public int ExtractedSizeInKb { get; private set; }

    public bool IsDownloadComplete => this.DownloadedSizeInKb >= this._layer.SizeInKb;

    public bool IsExtractionComplete => this.ExtractedSizeInKb >= this._layer.SizeInKb;

    public LayerPullInfo(LayerInfo layer)
    {
        this._layer = layer;
    }

    public void IncreaseDownloadedSize(int additionalSizeInKb)
    {
        this.DownloadedSizeInKb += additionalSizeInKb;

        if (this.DownloadedSizeInKb > this._layer.SizeInKb)
        {
            this.DownloadedSizeInKb = this._layer.SizeInKb;
        }
    }

    public void IncreaseExtractedSize(int additionalSizeInKb)
    {
        this.ExtractedSizeInKb += additionalSizeInKb;

        if (this.ExtractedSizeInKb > this._layer.SizeInKb)
        {
            this.ExtractedSizeInKb = this._layer.SizeInKb;
        }
    }
}

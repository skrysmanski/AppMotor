using AppMotor.CliApp.ProgressBarSample.DataModels;
using AppMotor.CliApp.Terminals;

using JetBrains.Annotations;

namespace AppMotor.CliApp.ProgressBarSample;

internal static class Program
{
    private const int MAX_PARALLEL_DOWNLOADS = 4;

    private const int DOWNLOAD_SPEED_IN_KB_PER_SEC = 25_000;

    private const int EXTRACT_SPEED_IN_KB_PER_SEC = 35_000;

    private const int SIMULATED_PROGRESS_STEPS_PER_SECOND = 20;

    private static readonly IReadOnlyList<LayerInfo> LAYERS = new[]
    {
        new LayerInfo("93619dbc5b36", SizeInKb: 39_270),
        new LayerInfo("626033c43d70", SizeInKb: 34),
        new LayerInfo("37d5d7efb64e", SizeInKb: 14_123),
        new LayerInfo("ac563158d721", SizeInKb: 12),
        new LayerInfo("688ba7d5c01a", SizeInKb: 80_220),
        new LayerInfo("00e060b6d11d", SizeInKb: 200),
        new LayerInfo("1c04857f594f", SizeInKb: 52_240),
        new LayerInfo("4d7cfa90e6ea", SizeInKb: 4_170),
        new LayerInfo("e0431212d27d", SizeInKb: 9_370),
    };

    public static void Main()
    {
        var layerPullInfos = LAYERS.Select(layer => new LayerPullInfo(layer)).ToDictionary(layerPullInfo => layerPullInfo.LayerId);
        var layerProgressBars = LAYERS.Select(layer => new LayerProgressBar(layer)).ToList();

        int startTopPosition = Terminal.CursorTop;

        while (true)
        {
            //
            // Determine layers for downloading and extraction
            //
            List<LayerPullInfo> layersForDownload = DetermineLayersForDownload(LAYERS, layerPullInfos);
            LayerPullInfo? layerForExtraction = DetermineLayerForExtraction(LAYERS, layerPullInfos);

            //
            // Simulate layer downloading
            //
            if (layersForDownload.Count > 0)
            {
                int downloadSpeedPerLayer = DOWNLOAD_SPEED_IN_KB_PER_SEC / SIMULATED_PROGRESS_STEPS_PER_SECOND / layersForDownload.Count;
                foreach (var layerPullInfo in layersForDownload)
                {
                    layerPullInfo.IncreaseDownloadedSize(downloadSpeedPerLayer);
                }
            }

            //
            // Simulate layer extraction
            //
            layerForExtraction?.IncreaseExtractedSize(EXTRACT_SPEED_IN_KB_PER_SEC / SIMULATED_PROGRESS_STEPS_PER_SECOND);

            //
            // Display progress UI
            //
            Terminal.CursorLeft = 0;
            Terminal.CursorTop = startTopPosition;

            foreach (var layerProgressBar in layerProgressBars)
            {
                layerProgressBar.Update(layerPullInfos[layerProgressBar.LayerId]);
            }

            //
            // Finish condition and sleep before next update
            //
            if (layersForDownload.Count == 0 && layerForExtraction is null)
            {
                // Nothing more to download and nothing more to extract. We're done.
                break;
            }

            Thread.Sleep(1000 / SIMULATED_PROGRESS_STEPS_PER_SECOND);
        }
    }

    [MustUseReturnValue]
    private static List<LayerPullInfo> DetermineLayersForDownload(IEnumerable<LayerInfo> layers, IReadOnlyDictionary<string, LayerPullInfo> layerPullInfos)
    {
        var downloadingLayers = new List<LayerPullInfo>(capacity: MAX_PARALLEL_DOWNLOADS);

        foreach (var layer in layers)
        {
            var layerPullInfo = layerPullInfos[layer.Id];

            if (!layerPullInfo.IsDownloadComplete)
            {
                if (downloadingLayers.Count < MAX_PARALLEL_DOWNLOADS)
                {
                    downloadingLayers.Add(layerPullInfo);
                }
            }
        }

        return downloadingLayers;
    }

    [MustUseReturnValue]
    private static LayerPullInfo? DetermineLayerForExtraction(IEnumerable<LayerInfo> layers, IReadOnlyDictionary<string, LayerPullInfo> layerPullInfos)
    {
        foreach (var layer in layers)
        {
            var layerPullInfo = layerPullInfos[layer.Id];
            if (!layerPullInfo.IsDownloadComplete)
            {
                break;
            }

            if (!layerPullInfo.IsExtractionComplete)
            {
                return layerPullInfo;
            }
        }

        return null;
    }
}

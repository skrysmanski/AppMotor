using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AppMotor.CliApp.Hosting.Sample;

internal sealed class MyTestServer : IHostedService
{
    private readonly ILogger<MyTestServer> _logger;

    public MyTestServer(ILogger<MyTestServer> logger)
    {
        this._logger = logger;
    }

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken)
    {
        this._logger.LogInformation("Test server started");
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken)
    {
        this._logger.LogInformation("Test server stopped");
        return Task.CompletedTask;
    }
}
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Havagan.Example.CoreWindowsServiceOnFullFramework;

internal class WorkerService : IHostedService
{
    internal const string ServiceName = "WorkerService";

    public WorkerService(ILogger<WorkerService> logger, IHostApplicationLifetime appLifetime)
    {
        _logger = logger;

        appLifetime.ApplicationStarted.Register(OnStarted);
        appLifetime.ApplicationStopping.Register(OnStopping);
        appLifetime.ApplicationStopped.Register(OnStopped);

        _logger.LogDebug("0. Constructor has been called.");
    }

    private readonly ILogger<WorkerService> _logger;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("1. StartAsync has been called.");

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("4. StopAsync has been called.");

        return Task.CompletedTask;
    }

    private void OnStarted()
    {
        _logger.LogDebug("2. OnStarted has been called.");
    }

    private void OnStopping()
    {
        _logger.LogDebug("3. OnStopping has been called.");
    }

    private void OnStopped()
    {
        _logger.LogDebug("5. OnStopped has been called.");
    }
}

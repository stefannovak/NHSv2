namespace NHSv2.Communications.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting Communications worker...");
        _logger.LogInformation("Awaiting messages");
        
        await Task.Delay(-1, stoppingToken);
        
        _logger.LogInformation("Ending Communications worker...");
    }
}
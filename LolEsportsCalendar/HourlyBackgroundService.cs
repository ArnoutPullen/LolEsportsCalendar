using LolEsportsCalendar.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LolEsportsCalendar;

class HourlyBackgroundService(ILogger<HourlyBackgroundService> logger,
    LolEsportsService lolEsportsService) : IHostedService, IDisposable
{
    private readonly PeriodicTimer periodicTimer = new(TimeSpan.FromHours(1));

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Background Hosted Service running.");

        // Import events once
        await lolEsportsService.ImportEvents(cancellationToken);

        // Import events every hour
        while (await periodicTimer.WaitForNextTickAsync(cancellationToken))
        {
            try
            {
                await lolEsportsService.ImportEvents(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation("Background Hosted Service is stopping.");
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error while importing events");
            }
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Background Hosted Service is stopping.");
        await Log.CloseAndFlushAsync();
    }

    public void Dispose()
    {
        periodicTimer?.Dispose();
    }
}

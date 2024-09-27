using LolEsportsCalendar.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace LolEsportsCalendar
{
	class ConsoleApp(LolEsportsService lolEsportsService, ILogger<ConsoleApp> logger)
    {
        public async Task RunAsync()
		{
			try
			{
				await lolEsportsService.ImportEvents();
			}
			catch (Exception exception)
			{
				logger.LogError(exception, "Error while importing events");
			}
		}
	}
}

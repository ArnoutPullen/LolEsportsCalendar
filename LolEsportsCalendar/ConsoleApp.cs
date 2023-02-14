using LolEsportsCalendar.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace LolEsportsCalendar
{
	class ConsoleApp
	{
		private readonly LolEsportsService _lolEsportsService;
		private readonly ILogger<ConsoleApp> _logger;

		public ConsoleApp(LolEsportsService lolEsportsService, ILogger<ConsoleApp> logger)
		{
			_lolEsportsService = lolEsportsService;
			_logger = logger;
		}

		public async Task RunAsync()
		{
			try
			{
				await _lolEsportsService.ImportEvents();
			}
			catch (Exception exception)
			{
				_logger.LogError(exception, "Error while importing events");
			}
		}
	}
}

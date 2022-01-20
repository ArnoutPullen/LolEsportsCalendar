using System.Threading.Tasks;

namespace LolEsportsCalendar
{
	class ConsoleApp
	{
		private readonly LolEsportsService _lolEsportsService;

		public ConsoleApp(LolEsportsService lolEsportsService)
		{
			_lolEsportsService = lolEsportsService;
		}

		public async Task RunAsync()
		{
			await _lolEsportsService.ImportMissingCalendarsAsync();
			await _lolEsportsService.ImportEventsForSelectedCalendarsAsync();
		}
	}
}

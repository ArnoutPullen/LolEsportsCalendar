using Google.Apis.Calendar.v3.Data;
using LolEsportsApiClient;
using LolEsportsApiClient.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LolEsportsCalendar
{
	public class LolEsportsService
	{
		private readonly List<string> selectedLeagues = new List<string>
		{
			"LEC",
			"European Masters",
			"Worlds"
		};
		private readonly Dictionary<string, string> leagueLookup = new Dictionary<string, string>();

		public LolEsportsClient _lolEsportsClient = new LolEsportsClient();
		public GoogleCalendarService _googleCalendarService = new GoogleCalendarService();

		public LolEsportsService()
		{
			BuildLeagueLookup().GetAwaiter().GetResult();
		}

		public async Task<List<League>> GetLeagues()
		{
			List<League> leagues = null;

			try
			{
				leagues = await _lolEsportsClient.GetLeagues();
			}
			catch (Exception exception)
			{
				Console.WriteLine(exception.Message);
			}

			return leagues;
		}

		public async Task BuildLeagueLookup()
		{
			var leagues = await GetLeagues();

			if (leagues != null)
			{
				foreach (var league in leagues)
				{
					leagueLookup.Add(league.Name, league.Id);
				}
			}
		}

		public async Task ImportMissingCalendars()
		{
			try
			{
				var leagues = await GetLeagues();

				foreach (var league in leagues)
				{
					// Skip unselected leagues
					if (!selectedLeagues.Contains(league.Name))
					{
						continue;
					}

					// Check if calendar exists for league
					bool exists = _googleCalendarService.CalendarExists(league.Name);

					if (!exists)
					{
						_googleCalendarService.InsertLeagueAsCalendar(league);
					}
				}
			}
			catch (Exception exception)
			{
				Console.WriteLine(exception.Message);
				throw;
			}
		}

		public async Task ImportEventsForAllCalendars()
		{
			try
			{
				var leauges = await GetLeagues();

				foreach (League leauge in leauges)
				{
					// Skip unselected leagues
					if (!selectedLeagues.Contains(leauge.Name))
					{
						continue;
					}

					ImportEventsForLeague(leauge.Name).GetAwaiter().GetResult();
				}
			}
			catch (Exception exception)
			{
				Console.WriteLine(exception.Message);
			}
		}

		public async Task ImportEventsForLeague(string leagueName)
		{
			try
			{
				// Get calendarId
				string calendarId = _googleCalendarService.FindCalendarId(leagueName);

				// Get leagueId
				string leagueId = leagueLookup[leagueName];

				// Get events of league
				var events = await _lolEsportsClient.GetScheduleByLeague(leagueId);

				foreach (EsportEvent esportEvent in events)
				{
					// Skip unselected leagues
					if (!selectedLeagues.Contains(esportEvent.League.Name))
					{
						continue;
					}

					// Convert LeagueEvent to GoogleEvent
					Event @event = _googleCalendarService.ConvertEsportEventToGoogleEvent(esportEvent);

					// Insert or Update GoogleEvent
					_googleCalendarService.InsertOrUpdateEvent(@event, calendarId, @event.Id);
				}
			}
			catch (Exception exception)
			{
				Console.WriteLine(exception.Message);
			}
		}
	}
}

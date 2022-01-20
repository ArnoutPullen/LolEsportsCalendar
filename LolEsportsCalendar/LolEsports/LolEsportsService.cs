using Google.Apis.Calendar.v3.Data;
using LolEsportsApiClient;
using LolEsportsApiClient.Models;
using LolEsportsCalendar.GoogleCalendar;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LolEsportsCalendar.LolEsports
{
	public class LolEsportsService
	{
		private readonly Dictionary<string, string> leagueLookup = new Dictionary<string, string>();

		public LolEsportsClient _lolEsportsClient;
		public GoogleCalendarService _googleCalendarService;
		private ILogger<LolEsportsService> _logger;
		private LolEsportsOptions _options;

		public LolEsportsService(GoogleCalendarService googleCalendarService, LolEsportsClient lolEsportsClient, ILogger<LolEsportsService> logger, IOptions<LolEsportsOptions> options)
		{
			_googleCalendarService = googleCalendarService;
			_lolEsportsClient = lolEsportsClient;
			_logger = logger;
			_options = options.Value;
		}

		public async Task<List<League>> GetLeaguesAsync()
		{
			List<League> leagues = null;

			try
			{
				leagues = await _lolEsportsClient.GetLeaguesAsync();
			}
			catch (Exception exception)
			{
				_logger.LogError("Error while getting leauges", exception);
			}

			return leagues;
		}

		public async Task BuildLeagueLookupAsync()
		{
			var leagues = await GetLeaguesAsync();

			if (leagues != null)
			{
				foreach (var league in leagues)
				{
					leagueLookup.Add(league.Name, league.Id);
				}
			}
		}

		public async Task ImportMissingCalendarsAsync()
		{
			try
			{
				var leagues = await GetLeaguesAsync();

				foreach (var league in leagues)
				{
					if (SkipLeague(league.Name))
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
				_logger.LogError("Error while importing missing calendars", exception);
			}
		}

		public async Task ImportEventsForAllCalendarsAsync()
		{
			try
			{
				var leagues = await GetLeaguesAsync();

				foreach (League league in leagues)
				{
					if (SkipLeague(league.Name))
					{
						continue;
					}

					ImportEventsForLeagueAsync(league.Name).GetAwaiter().GetResult();
				}
			}
			catch (Exception exception)
			{
				_logger.LogError("Error while importing events for all calendars", exception);
			}
		}

		public async Task ImportEventsForSelectedCalendarsAsync()
		{
			await BuildLeagueLookupAsync();

			try
			{
				foreach (var calendar in _options.Calendars)
				{
					await ImportEventsForLeagueAsync(calendar);
				}
			}
			catch (Exception exception)
			{

				_logger.LogError("Error while importing events for selected calendars", exception);
			}
		}

		public async Task ImportEventsForLeagueAsync(string leagueName)
		{
			try
			{
				// Get calendarId
				string calendarId = _googleCalendarService.FindCalendarId(leagueName);

				// Get leagueId
				string leagueId = leagueLookup[leagueName];

				// Get events of league
				var events = await _lolEsportsClient.GetScheduleByLeagueAsync(leagueId);

				foreach (EsportEvent esportEvent in events)
				{
					if (SkipLeague(esportEvent.League.Name))
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
				_logger.LogError("Error while importing events for leauge {0}", exception, leagueName);
			}

			_logger.LogInformation("Events imported for league {0}", leagueName);
		}

		public bool SkipLeague(string leagueName)
		{
			// By default skip league
			bool contains = true;

			// Don't skip leagues if calendars not set in options
			if (_options.Calendars == null) {
				return false;
			}

			foreach (string calendar in _options.Calendars)
			{
				if (leagueName == calendar) {
					contains = false;
				}
			}

			return contains;
		}
	}
}

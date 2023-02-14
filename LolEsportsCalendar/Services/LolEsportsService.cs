using Google;
using Google.Apis.Calendar.v3.Data;
using GoogleCalendarApiClient.Services;
using LolEsportsApiClient;
using LolEsportsApiClient.Models;
using LolEsportsCalendar.GoogleCalendar;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LolEsportsCalendar.Services
{
	public class LolEsportsService
	{
		private readonly LolEsportsClient _lolEsportsClient;
		private readonly LolEsportsOptions _options;
		private readonly GoogleCalendarService _googleCalendarService;
		private readonly CalendarsService _calendarsService;
		private readonly EventsService _eventsService;
		private readonly ILogger<LolEsportsService> _logger;
		private int _rateLimitExceededCount = 0;

		public LolEsportsService(
			GoogleCalendarService googleCalendarService,
			LolEsportsClient lolEsportsClient,
			EventsService eventsService,
			CalendarsService calendarsService,
			ILogger<LolEsportsService> logger,
			IOptions<LolEsportsOptions> options
		)
		{
			_googleCalendarService = googleCalendarService;
			_calendarsService = calendarsService;
			_eventsService = eventsService;
			_lolEsportsClient = lolEsportsClient;
			_logger = logger;
			_options = options.Value;
		}

		public async Task ImportEvents()
		{
			string[] leagueNames = _options.Leagues;

			if (leagueNames != null) 
			{
				foreach(var leagueName in leagueNames)
				{
					Calendar calendar = FindOrCreateCalendarByLeagueName(leagueName);

					if (calendar != null)
					{
						League league = _lolEsportsClient.GetLeagueByName(leagueName);

						if (league == null)
                        {
							_logger.LogWarning("Couldn't find league with name {0}", leagueName);
						} else
						{
							// Import events for calendar
							await ImportEventsForLeagueAsync(league, calendar);
						}
					}
				}
			} else {
				await ImportEventsForAllCalendarsAsync();
			}

			_logger.LogInformation("All events imported");
			Console.ReadLine();
		}

		public async Task ImportEventsForAllCalendarsAsync()
		{
			try
			{
				List<League> leagues = await _lolEsportsClient.GetLeaguesAsync();

				foreach (League league in leagues)
				{
					Calendar calendar = FindOrCreateCalendarByLeagueName(league.Name);

					if (calendar != null)
					{
						// Import events for calendar
						await ImportEventsForLeagueAsync(league, calendar);

						// Wait 1 sec
						Thread.Sleep(60);
					}

				}
			}
			catch (Exception exception)
			{
				_logger.LogError("Error while importing events for all calendars", exception);
			}
		}

		public async Task ImportEventsForLeagueAsync(League league, Calendar calendar)
		{
			try
			{
				// Get scheduled events of league
				List<EsportEvent> esportEvents = await _lolEsportsClient.GetScheduleByLeagueAsync(league);

				foreach (EsportEvent esportEvent in esportEvents)
				{
					// Convert LeagueEvent to GoogleEvent
					Event googleEvent = _googleCalendarService.ConvertEsportEventToGoogleEvent(esportEvent);

					// Insert or Update GoogleEvent
					_eventsService.InsertOrUpdate(googleEvent, calendar, googleEvent.Id); // TODO
				}
			}
			catch (GoogleApiException exception)
			{
				// :( Google can't handle all requests
				// https://developers.google.com/calendar/api/guides/quota
				// https://stackoverflow.com/a/40990761/5828267
				if (exception.Error.Code == 403 && exception.Error.Message == "Rate Limit Exceeded")
				{
					_rateLimitExceededCount++;

					if (_rateLimitExceededCount < 8)
					{
						// Wait
						Thread.Sleep(60 * _rateLimitExceededCount);

						// Retry
						await ImportEventsForLeagueAsync(league, calendar);
					}
				}
				_logger.LogError("Error while importing events for leauge {0}", exception, league.Name);
			}
			catch (Exception exception)
			{
				_logger.LogError("Error while importing events for leauge {0}", exception, league.Name);
			}

			_logger.LogInformation("Events imported for league {0}", league.Name);
		}

		public Calendar FindOrCreateCalendarByLeagueName(string leagueName)
		{
			Calendar calendar = null;
			string calendarId = null;

			// Get calendars
			var calendars = _googleCalendarService.GetCalendarLookup();

			// Get league
			League league = _lolEsportsClient.GetLeagueByName(leagueName);

			if (league == null)
			{
				_logger.LogWarning("Couldn't find league with name {0}", leagueName);
				return calendar;
			}

			foreach (var c in calendars)
			{
				if (c.Key == league.Name)
				{
					calendarId = c.Value;
				}
				else
				if (c.Key == league.Slug)
				{
					calendarId = c.Value;
				}
			}

			// Get existing calendar
			if (calendarId != null)
			{
				calendar = _calendarsService.Get(calendarId);
			}

			// Creat new calendar
			if (calendar == null)
			{
				_logger.LogInformation("Creating new calendar {0}", league.Name);
				Calendar newCalendar = ConvertLeagueToCalendar(league);

				return _calendarsService.Insert(newCalendar);
			}

			return calendar;
		}

		public Calendar ConvertLeagueToCalendar(League league)
		{
			Calendar calendar = new Calendar
			{

				Summary = league.Name,
				Description = league.Name + " / " + league.Region,
				// ETag = "Test",
				// Kind = "Test",
				// Location = "Test",
				// TimeZone = "Europe/Amsterdam"
			};

			return calendar;
		}
	}
}

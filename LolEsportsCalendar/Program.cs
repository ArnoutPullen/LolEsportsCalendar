using Google.Apis.Calendar.v3.Data;
using GoogleCalendarApiClient;
using GoogleCalendarApiClient.Services;
using LolEsportsApiClient.Models;
using LolEsportsApiClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LolEsportsCalendar
{
	class Program
	{
		static readonly Dictionary<string, string> leagueLookup = new Dictionary<string, string>();
		static LolEsportsClient _lolEsportsClient = new LolEsportsClient();
		static GoogleCalendarService _googleCalendarService = new GoogleCalendarService();

		static readonly List<string> selectedLeagues = new List<string>
		{
			"LEC",
			"European Masters",
			"Worlds"
		};

		static void Main(string[] args)
		{
			BuildLeagueLookup().GetAwaiter().GetResult();

			// Google Calendar API

			// Clear calendar
			string calendarId = _googleCalendarService.FindCalendarId("LEC");
			// ClearCalendar(calendarId);

			// test duplicates
			Event @event = new Event()
			{
				// Id = "107597929688619119",
				ICalUID = "107597929688619119",
				Summary = "Test",
				Start = new EventDateTime() {
					DateTime = DateTime.Now
				},
				End = new EventDateTime()
				{
					DateTime = DateTime.Now
				}
			};

			EsportEvent esportEvent = new EsportEvent()
			{
				// Id = "107597929688619119",
				Match = new Match() {
					Id = "107597929688619119"
				},
				StartTime = DateTimeOffset.Now
			};
			// var newEvent = _eventsService.Insert(@event, calendarId);
			// Convert EsportEvent to Event
			 Event _event = _googleCalendarService.ConvertEsportEventToGoogleEvent(esportEvent);

			var newEvent = _googleCalendarService.InsertOrUpdateEvent(_event, calendarId, esportEvent.Match.Id);
			Console.WriteLine(newEvent.Summary);

			// SyncCalendars().GetAwaiter().GetResult();

			// SyncScheduledEvents
			// SyncEventsForLeague().GetAwaiter().GetResult();

			// Lolesports API
			// RunAsync().GetAwaiter().GetResult();

			Console.ReadLine();
		}

		static async Task SyncCalendars()
		{
			try
			{
				var leagues = await _lolEsportsClient.GetLeagues();

				foreach (var league in leagues)
				{
					// Skip unselected leagues
					if (!selectedLeagues.Contains(league.Name)) {
						continue;
					}

					// Check if calendar exists for league
					bool exists = _googleCalendarService.CalendarExists(league.Name);

					if (!exists) {
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

		static async Task BuildLeagueLookup()
		{
			try
			{
				var leagues = await _lolEsportsClient.GetLeagues();

				foreach (var league in leagues)
				{
					leagueLookup.Add(league.Name, league.Id);
				}
			}
			catch (Exception exception)
			{
				Console.WriteLine(exception.Message);
				throw;
			}
		}

		static async Task SyncEventsForLeague()
		{
			try
			{
				string calendarId = _googleCalendarService.FindCalendarId("LEC");
				string leagueId = leagueLookup["LEC"];
				var events = await _lolEsportsClient.GetScheduleByLeague(leagueId);

				foreach (EsportEvent esportEvent in events)
				{
					// Skip unselected leagues
					if (!selectedLeagues.Contains(esportEvent.League.Name))
					{
						continue;
					}
					Event @event = _googleCalendarService.ConvertEsportEventToGoogleEvent(esportEvent);
					_googleCalendarService.InsertEvent(@event, calendarId);
				}
			}
			catch (Exception exception)
			{
				Console.WriteLine(exception.Message);
				throw;
			}
		}

		static async Task RunAsync()
		{
			try
			{
				// 1 get leagues
				/*
				Console.WriteLine("1 Get Leagues:");
				var leagues = await lolEsportsClient.GetLeagues();

				foreach (var league in leagues)
				{
					Console.WriteLine(league.Name);
				}
				*/

				// 2 get schedules
				/*
				Console.WriteLine("2 Get Schedule:");
				var events = await lolEsportsClient.GetSchedule();

				foreach (var _event in events)
				{
					Console.WriteLine(_event.League.Name);
				}
				*/

				// 3 get schedule of league
				var leaugeId = "98767991302996019";
				Console.WriteLine("2 Get Schedule of league:");
				var events = await _lolEsportsClient.GetScheduleByLeague(leaugeId);

				foreach (var _event in events)
				{
					Console.WriteLine(_event.League.Name);
				}
			}
			catch (Exception exception)
			{
				Console.WriteLine(exception.Message);
				throw;
			}
			Console.ReadLine();
		}
	}
}

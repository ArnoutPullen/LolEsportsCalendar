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
		static readonly Dictionary<string, string> calendarLookup = new Dictionary<string, string>();
		static readonly Dictionary<string, string> leagueLookup = new Dictionary<string, string>();
		static CalendarList calendars;

		static GoogleCalendarClient _googleCalendarClient = new GoogleCalendarClient();
		static CalendarsService _calendarsService;
		static CalendarListService _calendarListService;
		static EventsService _eventsService;
		static LolEsportsClient _lolEsportsClient = new LolEsportsClient();

		static readonly List<string> selectedLeagues = new List<string>
		{
			"LEC",
			"European Masters",
			"Worlds"
		};

		static void Main(string[] args)
		{
			_calendarsService = new CalendarsService(_googleCalendarClient.calendarService);
			_calendarListService = new CalendarListService(_googleCalendarClient.calendarService);
			_eventsService = new EventsService(_googleCalendarClient.calendarService);

			// Google Calendar API
			BuildCalendarLookup();

			BuildLeagueLookup().GetAwaiter().GetResult();

			// Clear calendar
			string calendarId = calendarLookup["LEC"];
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
			 Event _event = ConvertEsportEventToGoogleEvent(esportEvent);

			var newEvent = InsertOrUpdateEvent(_event, calendarId, esportEvent.Match.Id);
			Console.WriteLine(newEvent.Summary);

			// SyncCalendars().GetAwaiter().GetResult();

			// SyncScheduledEvents
			// SyncEventsForLeague().GetAwaiter().GetResult();

			// Lolesports API
			// RunAsync().GetAwaiter().GetResult();

			// Test();
			Console.ReadLine();
		}

		static void Test()
		{
			string calendarId = calendarLookup["LEC"];
			Event @event = _eventsService.Get(calendarId, "106269680921808997");
			Console.WriteLine(@event);
		}

		static void BuildCalendarLookup()
		{
			// View
			CalendarList calendarList = _calendarListService.ViewCalendarList();
			calendars = calendarList;

			// Add calendars to lookup
			foreach (var c in calendarList.Items)
			{
				// calendarLookup.Add(c.Id, c.Summary);
				calendarLookup.Add(c.Summary, c.Id);
			}
		}

		static Calendar InsertLeagueAsCalendar(League league)
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

			Calendar newCalendar = _calendarsService.InsertCalendar(calendar);

			if (calendar != null)
			{
				calendarLookup.Add(calendar.Summary, calendar.Id);
				Console.WriteLine("Created calendar {0}", calendar.Summary);
			} else {
				Console.WriteLine("Error while creating calendar");
			}

			return newCalendar;
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
					bool exists = calendarLookup.ContainsKey(league.Name);

					if (!exists) {
						InsertLeagueAsCalendar(league);
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

		static Event GetEvent(string calendarId, string eventId)
		{
			Event newEvent = null;
			try
			{
				newEvent = _eventsService.Get(calendarId, eventId);
			}
			catch (Exception exception)
			{
				Console.WriteLine(exception.Message);
			}

			return newEvent;
		}

		static Event InsertOrUpdateEvent(Event @event, string calendarId, string eventId)
		{
			Event changedEvent;

			// Check if event already exists
			var existingEvent = GetEvent(calendarId, eventId);

			if (existingEvent == null)
			{
				changedEvent = InsertEvent(@event, calendarId);
			}
			else
			{
				changedEvent = UpdateEvent(@event, calendarId, eventId);
			}

			return changedEvent;
		}

		static Event InsertEvent(Event @event, string calendarId)
		{
			Event newEvent = _eventsService.Insert(@event, calendarId);

			if (newEvent != null)
			{
				Console.WriteLine("Created event {0}", newEvent.Summary);
			}
			else
			{
				Console.WriteLine("Error while creating event in calendar, {0}", calendarId);
			}

			return newEvent;
		}

		static Event UpdateEvent(Event @event, string calendarId, string eventId)
		{
			Event updatedEvent = _eventsService.Update(@event, calendarId, eventId);

			if (updatedEvent != null)
			{
				Console.WriteLine("Updated event {0}", updatedEvent.Summary);
			}
			else
			{
				Console.WriteLine("Error while updating event in calendar, {0}", calendarId);
			}

			return updatedEvent;
		}

		static Event ConvertEsportEventToGoogleEvent(EsportEvent esportEvent)
		{
			// Convert EsportEvent to Event
			Event @event = new Event()
			{
				Id = esportEvent.Match.Id,
				// ICalUID = esportEvent.Match.Id,
				Start = new EventDateTime { DateTime = esportEvent.StartTime.UtcDateTime },
				End = new EventDateTime { DateTime = esportEvent.StartTime.UtcDateTime },
				// Summary = esportEvent.Match.Teams[0].Code + " vs " + esportEvent.Match.Teams[1].Code
				Summary = "Test"
			};

			return @event;
		}

		static void ClearCalendar(string calendarId)
		{
			// get events
			Events events = _eventsService.List(calendarId);
			Console.WriteLine("Deleting {0} events", events.Items.Count);

			// delete events
			foreach (Event e in events.Items)
			{
				string deleted = _eventsService.Delete(calendarId, e.Id);

				if (deleted == "") {
					Console.WriteLine("Deleted {0} from calendar {1}", e.Summary, calendarId);
				} else {
					Console.WriteLine("Error while deleting {0} from calendar {1}", e.Summary, calendarId);
				}
			}
		}

		static async Task SyncEventsForLeague()
		{
			try
			{
				string calendarId = calendarLookup["LEC"];
				string leagueId = leagueLookup["LEC"];
				var events = await _lolEsportsClient.GetScheduleByLeague(leagueId);

				foreach (EsportEvent esportEvent in events)
				{
					// Skip unselected leagues
					if (!selectedLeagues.Contains(esportEvent.League.Name))
					{
						continue;
					}
					Event @event = ConvertEsportEventToGoogleEvent(esportEvent);
					InsertEvent(@event, calendarId);
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

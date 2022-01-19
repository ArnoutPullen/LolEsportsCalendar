using Google.Apis.Calendar.v3.Data;
using GoogleCalendarApiClient;
using GoogleCalendarApiClient.Services;
using LolEsportsApiClient.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LolEsportsCalendar
{
	public class GoogleCalendarService
	{
		private readonly Dictionary<string, string> calendarLookup = new Dictionary<string, string>();
		private CalendarList calendars;
		readonly List<string> selectedLeagues = new List<string>
		{
			"LEC",
			"European Masters",
			"Worlds"
		};

		private GoogleCalendarClient _googleCalendarClient = new GoogleCalendarClient();
		private CalendarsService _calendarsService;
		private CalendarListService _calendarListService;
		private EventsService _eventsService;

		public GoogleCalendarService()
		{
			_calendarsService = new CalendarsService(_googleCalendarClient.calendarService);
			_calendarListService = new CalendarListService(_googleCalendarClient.calendarService);
			_eventsService = new EventsService(_googleCalendarClient.calendarService);

			BuildCalendarLookup();
		}

		public void BuildCalendarLookup()
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

		public string FindCalendarId(string leagueName)
		{
			return calendarLookup[leagueName];
		}

		public bool CalendarExists(string key)
		{
			return calendarLookup.ContainsKey(key);
		}

		public void ClearCalendar(string calendarId)
		{
			// get events
			Events events = _eventsService.List(calendarId);
			Console.WriteLine("Deleting {0} events", events.Items.Count);

			// delete events
			foreach (Event e in events.Items)
			{
				string deleted = _eventsService.Delete(calendarId, e.Id);

				if (deleted == "")
				{
					Console.WriteLine("Deleted {0} from calendar {1}", e.Summary, calendarId);
				}
				else
				{
					Console.WriteLine("Error while deleting {0} from calendar {1}", e.Summary, calendarId);
				}
			}
		}

		public Calendar InsertLeagueAsCalendar(League league)
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
			}
			else
			{
				Console.WriteLine("Error while creating calendar");
			}

			return newCalendar;
		}

		public Event GetEvent(string calendarId, string eventId)
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

		public Event InsertOrUpdateEvent(Event @event, string calendarId, string eventId)
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

		public Event InsertEvent(Event @event, string calendarId)
		{
			Event newEvent = _eventsService.Insert(@event, calendarId);

			if (newEvent != null)
			{
				Console.WriteLine("Created event {0} in calendar {1}", newEvent.Summary, calendarId);
			}
			else
			{
				Console.WriteLine("Error while creating event in calendar, {0}", calendarId);
			}

			return newEvent;
		}

		public Event UpdateEvent(Event @event, string calendarId, string eventId)
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

		public Event ConvertEsportEventToGoogleEvent(EsportEvent esportEvent)
		{
			// Convert EsportEvent to Event
			Event @event = new Event()
			{
				Id = esportEvent.Match.Id,
				// ICalUID = esportEvent.Match.Id,
				Start = new EventDateTime { DateTime = esportEvent.StartTime.UtcDateTime },
				End = new EventDateTime { DateTime = esportEvent.StartTime.UtcDateTime },
				Summary = esportEvent.Match.Teams[0].Code + " vs " + esportEvent.Match.Teams[1].Code
			};

			return @event;
		}
	}
}

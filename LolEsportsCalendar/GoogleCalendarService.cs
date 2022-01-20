using Google.Apis.Calendar.v3.Data;
using GoogleCalendarApiClient;
using GoogleCalendarApiClient.Services;
using LolEsportsApiClient.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

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

		private CalendarsService _calendarsService;
		private CalendarListService _calendarListService;
		private EventsService _eventsService;
		private ILogger<GoogleCalendarService> _logger;

		public GoogleCalendarService(CalendarsService calendarsService, CalendarListService calendarListService, EventsService eventsService, ILogger<GoogleCalendarService> logger)
		{
			_logger = logger;
			_calendarsService = calendarsService;
			_calendarListService = calendarListService;
			_eventsService = eventsService;

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
			_logger.LogInformation("Deleting {0} events", events.Items.Count);

			// delete events
			foreach (Event e in events.Items)
			{
				string deleted = _eventsService.Delete(calendarId, e.Id);

				if (deleted == "")
				{
					_logger.LogInformation("Deleted {0} from calendar {1}", e.Summary, calendarId);
				}
				else
				{
					_logger.LogInformation("Something went wrong while deleting event {0}, from calendar {1}", e.Summary, calendarId);
				}
			}
		}

		public Calendar InsertLeagueAsCalendar(League league)
		{
			Calendar newCalendar = null;
			Calendar calendar = new Calendar
			{
				Summary = league.Name,
				Description = league.Name + " / " + league.Region,
				// ETag = "Test",
				// Kind = "Test",
				// Location = "Test",
				// TimeZone = "Europe/Amsterdam"
			};

			try
			{
				newCalendar = _calendarsService.InsertCalendar(calendar);
			}
			catch (Exception exception)
			{
				_logger.LogError(exception, "Error while creating calendar", calendar.Summary);
			}

			if (newCalendar != null)
			{
				_logger.LogInformation("Created calendar {0}", calendar.Summary);
				calendarLookup.Add(calendar.Summary, calendar.Id);
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
				_logger.LogError(exception, "Error while getting event {0} from calendar {1}", calendarId, eventId);
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
			Event newEvent = null;

			try
			{
				newEvent = _eventsService.Insert(@event, calendarId);
			}
			catch (Exception exception)
			{
				_logger.LogError(exception, "Error while inserting event {0} in calendar {1}", @event.Summary, calendarId);
			}

			if (newEvent != null)
			{
				_logger.LogInformation("Created event {0} in calendar {1}", newEvent.Summary, calendarId);
			}

			return newEvent;
		}

		public Event UpdateEvent(Event @event, string calendarId, string eventId)
		{
			Event updatedEvent = null;

			try
			{
				updatedEvent = _eventsService.Update(@event, calendarId, eventId);
			}
			catch (Exception exception)
			{
				_logger.LogError(exception, "Error while updating event {0} in calendar {1}", @event.Summary, calendarId);
			}

			if (updatedEvent != null)
			{
				_logger.LogInformation("Updated event {0} in calendar {1}", updatedEvent.Summary, calendarId);
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

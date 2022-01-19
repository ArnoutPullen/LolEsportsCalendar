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
		static GoogleCalendarService _googleCalendarService = new GoogleCalendarService();
		static LolEsportsService _lolEsportsService = new LolEsportsService();

		static void Main(string[] args)
		{
			// Google Calendar API
			// TestDuplicatedEvents();

			// Lolesports API
			_lolEsportsService.ImportMissingCalendars().GetAwaiter().GetResult();
			_lolEsportsService.ImportEventsForSelectedCalendars();
			// _lolEsportsService.ImportEventsForAllCalendars().GetAwaiter().GetResult();
			// _lolEsportsService.ImportEventsForLeague("LEC").GetAwaiter().GetResult();

			Console.ReadLine();
		}

		static void TestDuplicatedEvents()
		{
			// Clear calendar
			string calendarId = _googleCalendarService.FindCalendarId("LEC");
			// ClearCalendar(calendarId);

			// test duplicates
			Event @event = new Event()
			{
				// Id = "107597929688619119",
				ICalUID = "107597929688619119",
				Summary = "Test",
				Start = new EventDateTime()
				{
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
				Match = new Match()
				{
					Id = "107597929688619119"
				},
				StartTime = DateTimeOffset.Now
			};
			// var newEvent = _eventsService.Insert(@event, calendarId);
			// Convert EsportEvent to Event
			Event _event = _googleCalendarService.ConvertEsportEventToGoogleEvent(esportEvent);

			var newEvent = _googleCalendarService.InsertOrUpdateEvent(_event, calendarId, esportEvent.Match.Id);
			Console.WriteLine(newEvent.Summary);
		}
	}
}

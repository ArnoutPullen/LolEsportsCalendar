using Google.Apis.Calendar.v3.Data;
using LolEsportsApiClient.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace GoogleCalendarApiClient.Services
{
    public class GoogleCalendarService
    {
        private readonly Dictionary<string, string> calendarLookup = [];
        private readonly CalendarsService _calendarsService;
        private readonly CalendarListService _calendarListService;
        private readonly ILogger<GoogleCalendarService> _logger;

        public GoogleCalendarService(CalendarsService calendarsService, CalendarListService calendarListService, ILogger<GoogleCalendarService> logger)
        {
            _logger = logger;
            _calendarsService = calendarsService;
            _calendarListService = calendarListService;

            BuildCalendarLookup();
        }

        public void BuildCalendarLookup()
        {
            // View
            CalendarList calendarList = _calendarListService.List();

            // Add calendars to lookup
            foreach (var calendar in calendarList.Items)
            {
                if (!calendarLookup.ContainsKey(calendar.Summary))
                {
                    calendarLookup.Add(calendar.Summary, calendar.Id);
                }
            }
        }

        public Dictionary<string, string> GetCalendarLookup()
        {
            return calendarLookup;
        }

        public Calendar InsertLeagueAsCalendar(League league)
        {
            Calendar newCalendar = null;
            Calendar calendar = ConvertLeagueToCalendar(league);

            try
            {
                newCalendar = _calendarsService.Insert(calendar);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error while creating calendar: {CalendarSummary}", calendar.Summary);
            }

            if (newCalendar != null)
            {
                _logger.LogInformation("Created calendar {CalendarSummary}", calendar.Summary);
                calendarLookup.Add(calendar.Summary, calendar.Id);
            }

            return newCalendar;
        }

        public static Calendar ConvertLeagueToCalendar(League league)
        {
            Calendar calendar = new()
            {
                Summary = league.Name,
                Description = league.Name + " / " + league.Region,
            };

            return calendar;
        }

        public static Event ConvertEsportEventToGoogleEvent(EsportEvent esportEvent)
        {
            if (esportEvent.Match == null) throw new MissingMemberException("EsportEvent is missing match property");

            Event @event = new()
            {
                Id = esportEvent.Match.Id,
                Start = new EventDateTime { DateTimeDateTimeOffset = esportEvent.StartTime.UtcDateTime },
                End = new EventDateTime { DateTimeDateTimeOffset = esportEvent.StartTime.UtcDateTime.AddHours(1) },
                Summary = esportEvent.Match.Teams[0].Code + " vs " + esportEvent.Match.Teams[1].Code
            };

            return @event;
        }
    }
}

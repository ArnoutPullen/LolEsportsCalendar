using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace GoogleCalendarApi.Services
{
	public class CalendarsService
    {
        private readonly CalendarService _calendarService;

        public CalendarsService(CalendarService calendarService)
		{
            _calendarService = calendarService;
		}

        /// <summary>Returns metadata for a calendar.
		/// <see href="https://developers.google.com/calendar/api/v3/reference/calendars/get"/>
		/// </summary>
        public Calendar ViewCalendar(string calendarId)
        {
            CalendarsResource.GetRequest getRequest = _calendarService.Calendars.Get(calendarId);
            Calendar calendar = getRequest.Execute();

            return calendar;
        }

        /// <summary>Creates a secondary calendar.
		/// <see href="https://developers.google.com/calendar/api/v3/reference/calendars/insert"/>
		/// </summary>
        public Calendar InsertCalendar(Calendar calendar)
        {
            CalendarsResource.InsertRequest insertRequest = _calendarService.Calendars.Insert(calendar);
            Calendar newCalendar = insertRequest.Execute();

            return newCalendar;
        }
    }
}

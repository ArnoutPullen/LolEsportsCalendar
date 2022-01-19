using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;

namespace GoogleCalendarApiClient.Services
{
	public class CalendarsService
    {
        private readonly CalendarService _service;

        public CalendarsService(CalendarService calendarService)
        {
            _service = calendarService;
        }

        /// <summary>Returns metadata for a calendar.
		/// <see href="https://developers.google.com/calendar/api/v3/reference/calendars/get"/>
		/// </summary>
        public Calendar ViewCalendar(string calendarId)
        {
            CalendarsResource.GetRequest getRequest = _service.Calendars.Get(calendarId);
            Calendar calendar = getRequest.Execute();

            return calendar;
        }

        /// <summary>Creates a secondary calendar.
		/// <see href="https://developers.google.com/calendar/api/v3/reference/calendars/insert"/>
		/// </summary>
        public Calendar InsertCalendar(Calendar calendar)
        {
            CalendarsResource.InsertRequest insertRequest = _service.Calendars.Insert(calendar);
            Calendar newCalendar = insertRequest.Execute();

            return newCalendar;
        }
    }
}

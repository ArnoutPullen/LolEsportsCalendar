using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Microsoft.Extensions.Logging;
using System;
using static Google.Apis.Calendar.v3.CalendarsResource;

namespace GoogleCalendarApiClient.Services
{
	public class CalendarsService
    {
        private readonly CalendarService _service;
        private ILogger<CalendarsService> _logger;

        public CalendarsService(CalendarService calendarService, ILogger<CalendarsService> logger)
        {
            _service = calendarService;
            _logger = logger;
        }

        /// <summary>Returns metadata for a calendar.
		/// <see href="https://developers.google.com/calendar/api/v3/reference/calendars/get"/>
		/// </summary>
        public Calendar Get(string calendarId)
        {
            Calendar calendar = null;

            try
            {
                GetRequest getRequest = _service.Calendars.Get(calendarId);
                calendar = getRequest.Execute();
            }
			catch (Exception exception)
            {
                _logger.LogError(exception, "Error while getting Calendar with id {0}", calendarId);
            }

            return calendar;
        }

        /// <summary>Creates a secondary calendar.
		/// <see href="https://developers.google.com/calendar/api/v3/reference/calendars/insert"/>
		/// </summary>
        public Calendar Insert(Calendar calendar)
        {
            InsertRequest insertRequest = _service.Calendars.Insert(calendar);
            Calendar newCalendar = insertRequest.Execute();

            return newCalendar;
        }

        public Calendar InsertOrUpdate(Calendar calendar, string calendarId)
        {
            Calendar _calendar;
            Calendar exists = Get(calendar.Id);

            if (exists == null)
            {
                _calendar = Insert(calendar);
            }
            else
            {
                _calendar = Update(calendar, calendarId);
            }

            return _calendar;
        }

        /// <summary>Updates metadata for a calendar.
		/// <see href="https://developers.google.com/calendar/api/v3/reference/calendars/update"/>
		/// </summary>
        public Calendar Update(Calendar calendar, string calendarId)
        {
            UpdateRequest updateRequest = _service.Calendars.Update(calendar, calendarId);
            Calendar updatedCalendar = updateRequest.Execute();

            return updatedCalendar;
        }

        /// <summary>Updates metadata for a calendar. This method supports patch semantics.
		/// <see href="https://developers.google.com/calendar/api/v3/reference/calendars/patch"/>
		/// </summary>
        public Calendar Patch(Calendar calendar, string calendarId)
        {
            PatchRequest patchRequest = _service.Calendars.Patch(calendar, calendarId);
            Calendar patchedCalendar = patchRequest.Execute();

            return patchedCalendar;
        }

        /// <summary>Clears a primary calendar. This operation deletes all events associated with the primary calendar of an account. 
        /// <see href="https://developers.google.com/calendar/api/v3/reference/calendars/clear"/>
        /// </summary>
        public string Clear(string calendarId)
        {
            ClearRequest clearRequest = _service.Calendars.Clear(calendarId);
            string cleared = clearRequest.Execute();

            return cleared;
        }

        /// <summary>Deletes a secondary calendar. Use CalendarsService.Clear for clearing all events on primary calendars.
        /// <see href="https://developers.google.com/calendar/api/v3/reference/calendars/delete"/>
        /// </summary>
        public string Delete(string calendarId)
        {
            DeleteRequest deleteRequest = _service.Calendars.Delete(calendarId);
            string deleted = deleteRequest.Execute();

            return deleted;
        }
    }
}

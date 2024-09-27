using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Microsoft.Extensions.Logging;
using System;
using static Google.Apis.Calendar.v3.CalendarsResource;

namespace GoogleCalendarApiClient.Services
{
	public class CalendarsService(CalendarService calendarService, ILogger<CalendarsService> logger)
    {

        /// <summary>Returns metadata for a calendar.
        /// <see href="https://developers.google.com/calendar/api/v3/reference/calendars/get"/>
        /// </summary>
        public Calendar Get(string calendarId)
        {
            Calendar calendar = null;

            try
            {
                GetRequest getRequest = calendarService.Calendars.Get(calendarId);
                calendar = getRequest.Execute();
            }
			catch (Exception exception)
            {
                logger.LogError(exception, "Error while getting Calendar with id {CalendarId}", calendarId);
            }

            return calendar;
        }

        /// <summary>Creates a secondary calendar.
		/// <see href="https://developers.google.com/calendar/api/v3/reference/calendars/insert"/>
		/// </summary>
        public Calendar Insert(Calendar calendar)
        {
            InsertRequest insertRequest = calendarService.Calendars.Insert(calendar);
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
            UpdateRequest updateRequest = calendarService.Calendars.Update(calendar, calendarId);
            Calendar updatedCalendar = updateRequest.Execute();

            return updatedCalendar;
        }

        /// <summary>Updates metadata for a calendar. This method supports patch semantics.
		/// <see href="https://developers.google.com/calendar/api/v3/reference/calendars/patch"/>
		/// </summary>
        public Calendar Patch(Calendar calendar, string calendarId)
        {
            PatchRequest patchRequest = calendarService.Calendars.Patch(calendar, calendarId);
            Calendar patchedCalendar = patchRequest.Execute();

            return patchedCalendar;
        }

        /// <summary>Clears a primary calendar. This operation deletes all events associated with the primary calendar of an account. 
        /// <see href="https://developers.google.com/calendar/api/v3/reference/calendars/clear"/>
        /// </summary>
        public string Clear(string calendarId)
        {
            ClearRequest clearRequest = calendarService.Calendars.Clear(calendarId);
            string cleared = clearRequest.Execute();

            return cleared;
        }

        /// <summary>Deletes a secondary calendar. Use CalendarsService.Clear for clearing all events on primary calendars.
        /// <see href="https://developers.google.com/calendar/api/v3/reference/calendars/delete"/>
        /// </summary>
        public string Delete(string calendarId)
        {
            DeleteRequest deleteRequest = calendarService.Calendars.Delete(calendarId);
            string deleted = deleteRequest.Execute();

            return deleted;
        }
    }
}

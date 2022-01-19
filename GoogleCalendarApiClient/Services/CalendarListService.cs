using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using System;
using System.Collections.Generic;
using System.Text;
using static Google.Apis.Calendar.v3.CalendarListResource;

namespace GoogleCalendarApiClient.Services
{
	public class CalendarListService
	{
		private readonly CalendarService _service;

		public CalendarListService(CalendarService calendarService)
		{
			_service = calendarService;
		}

		/// <summary>Returns the calendars on the user's calendar list.
		/// <see href="https://developers.google.com/calendar/api/v3/reference/calendarList/get"/>
		/// </summary>
		public CalendarList ViewCalendarList()
		{
			ListRequest listRequest = _service.CalendarList.List();
			CalendarList calendarList = listRequest.Execute();

			return calendarList;
		}

		/// <summary>Inserts an existing calendar into the user's calendar list.
		/// <see href="https://developers.google.com/calendar/api/v3/reference/calendarList/insert"/>
		/// </summary>
		public CalendarListEntry InsertCalendarList(CalendarListEntry calendar)
		{
			CalendarListResource.InsertRequest insertRequest = _service.CalendarList.Insert(calendar);
			var calendarListEntry = insertRequest.Execute();

			return calendarListEntry;
		}
	}
}

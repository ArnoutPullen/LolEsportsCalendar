using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Microsoft.Extensions.Logging;
using System;
using static Google.Apis.Calendar.v3.CalendarListResource;

namespace GoogleCalendarApiClient.Services
{
	public class CalendarListService
	{
		private readonly CalendarService _service;
		private readonly ILogger<CalendarListService> _logger;

		public CalendarListService(CalendarService calendarService, ILogger<CalendarListService> logger)
		{
			_service = calendarService;
			_logger = logger;
		}

		/// <summary>Returns the calendars on the user's calendar list.
		/// <see href="https://developers.google.com/calendar/api/v3/reference/calendarList/list"/>
		/// </summary>
		public CalendarList List()
		{
			ListRequest listRequest = _service.CalendarList.List();
			CalendarList calendarList = listRequest.Execute();

			return calendarList;
		}

		/// <summary>Returns a calendar from the user's calendar list.
		/// <see href="https://developers.google.com/calendar/api/v3/reference/calendarList/get"/>
		/// </summary>
		public CalendarListEntry Get(string calendarId)
		{
			CalendarListEntry calendarListEntry = null;

			try
			{
				GetRequest getRequest = _service.CalendarList.Get(calendarId);
				calendarListEntry = getRequest.Execute();
			}
			catch (Exception exception)
			{
				_logger.LogError(exception, "Error while getting CalendarListEntry with id {0}", calendarId);
			}

			return calendarListEntry;
		}

		/// <summary>Watch for changes to CalendarList resources. 
		/// <see href="https://developers.google.com/calendar/api/v3/reference/calendarList/watch"/>
		/// </summary>
		public Channel Watch(Channel body)
		{
			WatchRequest watchRequest = _service.CalendarList.Watch(body);
			Channel channel = watchRequest.Execute();

			return channel;
		}

		/// <summary>Inserts an existing calendar into the user's calendar list.
		/// <see href="https://developers.google.com/calendar/api/v3/reference/calendarList/insert"/>
		/// </summary>
		public CalendarListEntry Insert(CalendarListEntry calendar)
		{
			InsertRequest insertRequest = _service.CalendarList.Insert(calendar);
			var calendarListEntry = insertRequest.Execute();

			return calendarListEntry;
		}

		public CalendarListEntry InsertOrUpdate(CalendarListEntry calendar, string calendarId)
		{
			CalendarListEntry calendarListEntry;
			CalendarListEntry exists = Get(calendar.Id);

			if (exists == null)
			{
				calendarListEntry = Insert(calendar);
			} else 
			{
				calendarListEntry = Update(calendar, calendarId);
			}

			return calendarListEntry;
		}

		/// <summary>Updates an existing calendar on the user's calendar list.
		/// <see href="https://developers.google.com/calendar/api/v3/reference/calendarList/update"/>
		/// </summary>
		public CalendarListEntry Update(CalendarListEntry calendar, string calendarId)
		{
			UpdateRequest updateRequest = _service.CalendarList.Update(calendar, calendarId);
			CalendarListEntry calendarListEntry = updateRequest.Execute();

			return calendarListEntry;
		}

		/// <summary>Updates an existing calendar on the user's calendar list. This method supports patch semantics.
		/// <see href="https://developers.google.com/calendar/api/v3/reference/calendarList/patch"/>
		/// </summary>
		public CalendarListEntry Patch(CalendarListEntry calendar, string calendarId)
		{
			PatchRequest patchRequest = _service.CalendarList.Patch(calendar, calendarId);
			CalendarListEntry calendarListEntry = patchRequest.Execute();

			return calendarListEntry;
		}

		/// <summary>Removes a calendar from the user's calendar list.
		/// <see href="https://developers.google.com/calendar/api/v3/reference/calendarList/delete"/>
		/// </summary>
		public string Delete(string calendarId)
		{
			DeleteRequest deleteRequest = _service.CalendarList.Delete(calendarId);
			string deleted = deleteRequest.Execute();

			return deleted;
		}
	}
}

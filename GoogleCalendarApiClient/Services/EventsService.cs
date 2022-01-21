using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Microsoft.Extensions.Logging;
using System;

namespace GoogleCalendarApiClient.Services
{
	public class EventsService
	{
		private readonly CalendarService _service;
		private readonly ILogger<EventsService> _logger;

		public EventsService(CalendarService calendarService, ILogger<EventsService> logger)
		{
			_logger = logger;
			_service = calendarService;
		}

		/// <summary>Returns events on the specified calendar.
		/// <see href="https://developers.google.com/calendar/api/v3/reference/events/insert"/>
		/// </summary>
		public Events List(string calendarId)
		{
			EventsResource.ListRequest listRequest = _service.Events.List(calendarId);
			Events events = listRequest.Execute();

			return events;
		}

		/// <summary>Returns an event.
		/// <see href="https://developers.google.com/calendar/api/v3/reference/events/get"/>
		/// </summary>
		public Event Get(string calendarId, string eventId)
		{ 
			Event _event = null;

			try
			{
				EventsResource.GetRequest getRequest = _service.Events.Get(calendarId, eventId);
				_event = getRequest.Execute();
			}
			catch (Exception exception)
			{
				_logger.LogError(exception, "Error while getting Event with id {0}", eventId);
			}
			
			return _event;
		}

		/// <summary>Creates an event.
		/// <see href="https://developers.google.com/calendar/api/v3/reference/events/insert"/>
		/// </summary>
		public Event Insert(Event _event, string calendarId)
		{
			EventsResource.InsertRequest insertRequest = _service.Events.Insert(_event, calendarId);
			Event newEvent = insertRequest.Execute();

			return newEvent;
		}

		public Event InsertOrUpdate(Event _event, string calendarId, string eventId)
		{
			Event insertedOrUpdatedEvent;
			Event existing = Get(calendarId, eventId);

			if (existing == null)
			{
				insertedOrUpdatedEvent = Insert(_event, calendarId);
			}
			else
			{
				insertedOrUpdatedEvent = Update(_event, calendarId, eventId);
			}

			return insertedOrUpdatedEvent;
		}

		/// <summary>Updates an event.
		/// <see href="https://developers.google.com/calendar/api/v3/reference/events/update"/>
		/// </summary>
		public Event Update(Event _event, string calendarId, string eventId)
		{
			EventsResource.UpdateRequest updateRequest = _service.Events.Update(_event, calendarId, eventId);
			Event updatedEvent = updateRequest.Execute();

			return updatedEvent;
		}

		/// <summary>Imports an event. This operation is used to add a private copy of an existing event to a calendar.
		/// <see href="https://developers.google.com/calendar/api/v3/reference/events/import"/>
		/// </summary>
		public Event Import(Event _event, string calendarId)
		{
			EventsResource.ImportRequest importRequest = _service.Events.Import(_event, calendarId);
			Event newEvent = importRequest.Execute();

			return newEvent;
		}

		/// <summary>Deletes an event.
		/// <see href="https://developers.google.com/calendar/api/v3/reference/events/delete"/>
		/// </summary>
		public string Delete(string calendarId, string eventId)
		{
			EventsResource.DeleteRequest deleteRequest = _service.Events.Delete(calendarId, eventId);
			string result = deleteRequest.Execute();

			return result;
		}
	}
}

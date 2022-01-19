using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace GoogleCalendarApiClient.Services
{
	public class EventsService
	{
		private readonly CalendarService _service;

		public EventsService(CalendarService calendarService)
		{
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
			Event @event = null;

			try
			{
				EventsResource.GetRequest getRequest = _service.Events.Get(calendarId, eventId);
				@event = getRequest.Execute();
			}
			catch (Exception exception)
			{
				// Console.WriteLine(exception.Message);
			}
			
			return @event;
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

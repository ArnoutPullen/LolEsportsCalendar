using Google;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;

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
			catch (GoogleApiException exception)
			{
				if (exception.Error.Code == 404)
				{
					_logger.LogDebug(exception, "Couldn't find Event with id {0}", eventId);
				} else
				{
					_logger.LogError(exception, "Error while getting Event with id {0}", eventId);
				}
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
		public Event Insert(Event _event, Calendar calendar)
		{
			EventsResource.InsertRequest insertRequest = _service.Events.Insert(_event, calendar.Id);
			Event newEvent = insertRequest.Execute();

			return newEvent;
		}

		public Event InsertOrUpdate(Event _event, Calendar calendar, string eventId = null)
        {
			Event existing = Get(calendar.Id, eventId);

			if (existing == null)
			{
				_logger.LogInformation("Inserting Event {0} ({1}) in calendar {2}", _event.Summary, eventId, calendar.Summary);
				return Insert(_event, calendar);
			}

			// Compare events, only update when data changed
			bool equals = Compare(_event, existing);

			if (!equals)
			{
				_logger.LogInformation("Updating Event {0} ({1}) in calendar {2}", _event.Summary, eventId, calendar.Summary);
				return Update(_event, calendar, eventId);
			}

			// TODO: Show league name
			_logger.LogDebug("Event unchanged while trying to InsertOrUpdate Event {0} ({1}) in calendar {2}", _event.Summary, eventId, calendar.Summary);

			return existing;
		}

		public bool Compare(object expectedObject, object actualObject)
		{
			Type expectedObjectType = expectedObject.GetType();
			Type actualObjectType = actualObject.GetType();
			bool equals = true;

			foreach (PropertyInfo expectedProperty in expectedObjectType.GetProperties())
			{
				object expectedPropertyValue = expectedProperty.GetValue(expectedObject);
				object actualPropertyValue = actualObjectType.GetProperty(expectedProperty.Name).GetValue(actualObject);

				// Skip if both null
				if (expectedPropertyValue == null && actualPropertyValue == null) 
				{
					continue;
				}

				// Skip if string not changed
				if (expectedPropertyValue is string && actualPropertyValue is string)
				{
					bool stringEquals = string.Equals(expectedPropertyValue, actualPropertyValue);

					if (stringEquals == false)
					{
						equals = false;
						break;
					}

					continue;
				}

				// Skip if value not changed
				if (expectedPropertyValue == actualPropertyValue)
				{
					continue;
				}

				// Skip if expected value is null
				if (expectedPropertyValue == null)
				{
					continue;
				}

				if (expectedPropertyValue is DateTime expectedDateTime)
				{
					int result = expectedDateTime.CompareTo(actualPropertyValue);

					if (result != 0)
					{
						equals = false;
						break;
					}

					continue;
				}

				if (expectedPropertyValue is EventDateTime expectedEventDateTime)
				{
					EventDateTime actualEventDateTime = (EventDateTime)actualPropertyValue;
					DateTime actualDateTime = actualEventDateTime.DateTime.Value;
					int result = expectedEventDateTime.DateTime.Value.CompareTo(actualDateTime);

					if (result != 0)
					{
						equals = false;
						break;
					}

					continue;
				}

				_logger.LogDebug("{0} Not Equals {1} != {2}", expectedProperty.Name, expectedPropertyValue, actualPropertyValue);
			}

			return equals;
		}

		/// <summary>Updates an event.
		/// <see href="https://developers.google.com/calendar/api/v3/reference/events/update"/>
		/// </summary>
		public Event Update(Event _event, Calendar calendar, string eventId)
		{
			EventsResource.UpdateRequest updateRequest = _service.Events.Update(_event, calendar.Id, eventId);
			Event updatedEvent = updateRequest.Execute();

			return updatedEvent;
		}

		/// <summary>Imports an event. This operation is used to add a private copy of an existing event to a calendar.
		/// <see href="https://developers.google.com/calendar/api/v3/reference/events/import"/>
		/// </summary>
		public Event Import(Event _event, Calendar calendar)
		{
			EventsResource.ImportRequest importRequest = _service.Events.Import(_event, calendar.Id);
			Event newEvent = importRequest.Execute();

			return newEvent;
		}

		/// <summary>Deletes an event.
		/// <see href="https://developers.google.com/calendar/api/v3/reference/events/delete"/>
		/// </summary>
		public string Delete(Calendar calendar, Event _event)
		{
			EventsResource.DeleteRequest deleteRequest = _service.Events.Delete(calendar.Id, _event.Id);
			string result = deleteRequest.Execute();

			return result;
		}
	}
}

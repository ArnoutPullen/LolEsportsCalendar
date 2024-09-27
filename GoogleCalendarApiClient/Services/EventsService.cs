using Google;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;

namespace GoogleCalendarApiClient.Services
{
    public class EventsService(CalendarService calendarService, ILogger<EventsService> logger)
    {
        /// <summary>Returns events on the specified calendar.
        /// <see href="https://developers.google.com/calendar/api/v3/reference/events/insert"/>
        /// </summary>
        public Events List(string calendarId)
        {
            EventsResource.ListRequest listRequest = calendarService.Events.List(calendarId);
            return listRequest.Execute();
        }

        /// <summary>Returns an event.
        /// <see href="https://developers.google.com/calendar/api/v3/reference/events/get"/>
        /// </summary>
        public Event Get(string calendarId, string eventId)
        {
            Event _event = null;

            try
            {
                EventsResource.GetRequest getRequest = calendarService.Events.Get(calendarId, eventId);
                _event = getRequest.Execute();
            }
            catch (GoogleApiException exception)
            {
                if (exception.Error.Code == 404)
                {
                    logger.LogDebug(exception, "Couldn't find Event with id {EventId}", eventId);
                }
                else
                {
                    logger.LogError(exception, "Error while getting Event with id {EventId}", eventId);
                }
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error while getting Event with id {EventId}", eventId);
            }

            return _event;
        }

        /// <summary>Creates an event.
        /// <see href="https://developers.google.com/calendar/api/v3/reference/events/insert"/>
        /// </summary>
        public Event Insert(Event _event, Calendar calendar)
        {
            EventsResource.InsertRequest insertRequest = calendarService.Events.Insert(_event, calendar.Id);
            return insertRequest.Execute();
        }

        public Event InsertOrUpdate(Event _event, Calendar calendar, string eventId = null)
        {
            Event existing = Get(calendar.Id, eventId);

            if (existing == null)
            {
                logger.LogInformation("Inserting Event {EventSummary} ({EventId}) in calendar {CalendarSummary}", _event.Summary, eventId, calendar.Summary);
                return Insert(_event, calendar);
            }

            // Compare events, only update when data changed
            bool equals = Compare(_event, existing);

            if (!equals)
            {
                logger.LogInformation("Updating Event {EventSummary} ({EventId}) in calendar {CalendarSummary}", _event.Summary, eventId, calendar.Summary);
                return Update(_event, calendar, eventId);
            }

            // TODO: Show league name
            logger.LogDebug("Event unchanged while trying to InsertOrUpdate Event {EventSummary} ({EventId}) in calendar {CalendarSummary}", _event.Summary, eventId, calendar.Summary);

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
                    DateTimeOffset actualDateTime = actualEventDateTime.DateTimeDateTimeOffset.Value;
                    int result = expectedEventDateTime.DateTimeDateTimeOffset.Value.CompareTo(actualDateTime);

                    if (result != 0)
                    {
                        equals = false;
                        break;
                    }

                    continue;
                }

                logger.LogDebug("{ExpectedPropertyName} Not Equals {ExpectedPropertyValue} != {ActualPropertyValue}", expectedProperty.Name, expectedPropertyValue, actualPropertyValue);
            }

            return equals;
        }

        /// <summary>Updates an event.
        /// <see href="https://developers.google.com/calendar/api/v3/reference/events/update"/>
        /// </summary>
        public Event Update(Event _event, Calendar calendar, string eventId)
        {
            EventsResource.UpdateRequest updateRequest = calendarService.Events.Update(_event, calendar.Id, eventId);
            return updateRequest.Execute();
        }

        /// <summary>Imports an event. This operation is used to add a private copy of an existing event to a calendar.
        /// <see href="https://developers.google.com/calendar/api/v3/reference/events/import"/>
        /// </summary>
        public Event Import(Event _event, Calendar calendar)
        {
            EventsResource.ImportRequest importRequest = calendarService.Events.Import(_event, calendar.Id);
            return importRequest.Execute();
        }

        /// <summary>Deletes an event.
        /// <see href="https://developers.google.com/calendar/api/v3/reference/events/delete"/>
        /// </summary>
        public string Delete(Calendar calendar, Event _event)
        {
            EventsResource.DeleteRequest deleteRequest = calendarService.Events.Delete(calendar.Id, _event.Id);
            return deleteRequest.Execute();
        }
    }
}

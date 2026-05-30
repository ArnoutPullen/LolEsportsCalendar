using Google;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using static Google.Apis.Calendar.v3.EventsResource;

namespace GoogleCalendarApiClient.Services;

public class EventsService(CalendarService calendarService, ILogger<EventsService> logger)
{
    /// <summary>Returns events on the specified calendar.
    /// <see href="https://developers.google.com/calendar/api/v3/reference/events/insert"/>
    /// </summary>
    public async Task<Events> ListAsync(string calendarId, CancellationToken cancellationToken = default)
    {
        ListRequest request = calendarService.Events.List(calendarId);
        return await request.ExecuteAsync(cancellationToken);
    }

    /// <summary>Returns an event.
    /// <see href="https://developers.google.com/calendar/api/v3/reference/events/get"/>
    /// </summary>
    public async Task<Event?> GetAsync(string calendarId, string eventId, CancellationToken cancellationToken = default)
    {
        Event? _event = null;

        try
        {
            GetRequest request = calendarService.Events.Get(calendarId, eventId);
            _event = await request.ExecuteAsync(cancellationToken);
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

            throw;
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
    public async Task<Event> InsertAsync(Event _event, Calendar calendar, CancellationToken cancellationToken = default)
    {
        InsertRequest request = calendarService.Events.Insert(_event, calendar.Id);
        return await request.ExecuteAsync(cancellationToken);
    }

    public async Task<Event> InsertOrUpdateAsync(Event _event, Calendar calendar, string eventId, CancellationToken cancellationToken = default)
    {
        Event? existing = await GetAsync(calendar.Id, eventId, cancellationToken);

        if (existing == null)
        {
            logger.LogInformation("Inserting Event {EventSummary} ({EventId}) in calendar {CalendarSummary}", _event.Summary, eventId, calendar.Summary);
            return await InsertAsync(_event, calendar, cancellationToken);
        }

        // Compare events, only update when data changed
        bool equals = Compare(_event, existing, [
            nameof(Event.Id),
            nameof(Event.Start),
            nameof(Event.End),
            nameof(Event.Summary),
            nameof(Event.Description)
        ]);

        if (!equals)
        {
            logger.LogInformation("Updating Event {EventSummary} ({EventId}) in calendar {CalendarSummary}", _event.Summary, eventId, calendar.Summary);
            return await UpdateAsync(_event, calendar, eventId, cancellationToken);
        }

        // TODO: Show league name
        logger.LogDebug("Event unchanged while trying to InsertOrUpdate Event {EventSummary} ({EventId}) in calendar {CalendarSummary}", _event.Summary, eventId, calendar.Summary);

        return existing;
    }

    /// <summary>Updates an event.
    /// <see href="https://developers.google.com/calendar/api/v3/reference/events/update"/>
    /// </summary>
    public async Task<Event> UpdateAsync(Event _event, Calendar calendar, string eventId, CancellationToken cancellationToken = default)
    {
        UpdateRequest request = calendarService.Events.Update(_event, calendar.Id, eventId);
        return await request.ExecuteAsync(cancellationToken);
    }

    /// <summary>Imports an event. This operation is used to add a private copy of an existing event to a calendar.
    /// <see href="https://developers.google.com/calendar/api/v3/reference/events/import"/>
    /// </summary>
    public async Task<Event> ImportAsync(Event _event, Calendar calendar, CancellationToken cancellationToken = default)
    {
        ImportRequest request = calendarService.Events.Import(_event, calendar.Id);
        return await request.ExecuteAsync(cancellationToken);
    }

    /// <summary>Deletes an event.
    /// <see href="https://developers.google.com/calendar/api/v3/reference/events/delete"/>
    /// </summary>
    public async Task<string> DeleteAsync(Calendar calendar, Event _event, CancellationToken cancellationToken = default)
    {
        DeleteRequest request = calendarService.Events.Delete(calendar.Id, _event.Id);
        return await request.ExecuteAsync(cancellationToken);
    }

    private bool Compare(object expectedObject, object actualObject, string[] propertyNames)
    {
        Type expectedObjectType = expectedObject.GetType();
        Type actualObjectType = actualObject.GetType();
        bool equals = true;

        foreach (PropertyInfo expectedProperty in expectedObjectType.GetProperties())
        {
            if (!propertyNames.Contains(expectedProperty.Name))
            {
                continue;
            }

            object? expectedPropertyValue = expectedProperty.GetValue(expectedObject);
            object? actualPropertyValue = actualObjectType.GetProperty(expectedProperty.Name)?.GetValue(actualObject);

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

            // One null, one not null => not equal
            if (expectedPropertyValue == null || actualPropertyValue == null)
            {
                equals = false;
                break;
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
                EventDateTime? actualEventDateTime = (EventDateTime?)actualPropertyValue;
                if (actualEventDateTime == null)
                {
                    equals = false;
                    break;
                }
                DateTimeOffset? actualDateTime = actualEventDateTime?.DateTimeDateTimeOffset;
                if (actualDateTime == null)
                {
                    equals = false;
                    break;
                }
                int? result = expectedEventDateTime?.DateTimeDateTimeOffset?.CompareTo((DateTimeOffset)actualDateTime);

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
}

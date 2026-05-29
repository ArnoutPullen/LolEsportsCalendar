using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using static Google.Apis.Calendar.v3.CalendarsResource;

namespace GoogleCalendarApiClient.Services;

public class CalendarsService(CalendarService calendarService, ILogger<CalendarsService> logger)
{
    /// <summary>Returns metadata for a calendar.
    /// <see href="https://developers.google.com/calendar/api/v3/reference/calendars/get"/>
    /// </summary>
    public async Task<Calendar?> GetAsync(string calendarId, CancellationToken cancellationToken = default)
    {
        Calendar? calendar = null;

        try
        {
            GetRequest getRequest = calendarService.Calendars.Get(calendarId);
            calendar = await getRequest.ExecuteAsync(cancellationToken);
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
    public async Task<Calendar> InsertAsync(Calendar calendar, CancellationToken cancellationToken = default)
    {
        InsertRequest insertRequest = calendarService.Calendars.Insert(calendar);
        Calendar newCalendar = await insertRequest.ExecuteAsync(cancellationToken);

        return newCalendar;
    }

    public async Task<Calendar> InsertOrUpdateAsync(Calendar calendar, string calendarId, CancellationToken cancellationToken = default)
    {
        Calendar _calendar;
        Calendar? exists = await GetAsync(calendar.Id, cancellationToken);

        if (exists == null)
        {
            _calendar = await InsertAsync(calendar, cancellationToken);
        }
        else
        {
            _calendar = await UpdateAsync(calendar, calendarId, cancellationToken);
        }

        return _calendar;
    }

    /// <summary>Updates metadata for a calendar.
    /// <see href="https://developers.google.com/calendar/api/v3/reference/calendars/update"/>
    /// </summary>
    public async Task<Calendar> UpdateAsync(Calendar calendar, string calendarId, CancellationToken cancellationToken = default)
    {
        UpdateRequest updateRequest = calendarService.Calendars.Update(calendar, calendarId);
        Calendar updatedCalendar = await updateRequest.ExecuteAsync(cancellationToken);

        return updatedCalendar;
    }

    /// <summary>Updates metadata for a calendar. This method supports patch semantics.
    /// <see href="https://developers.google.com/calendar/api/v3/reference/calendars/patch"/>
    /// </summary>
    public async Task<Calendar> PatchAsync(Calendar calendar, string calendarId, CancellationToken cancellationToken = default)
    {
        PatchRequest patchRequest = calendarService.Calendars.Patch(calendar, calendarId);
        Calendar patchedCalendar = await patchRequest.ExecuteAsync(cancellationToken);

        return patchedCalendar;
    }

    /// <summary>Clears a primary calendar. This operation deletes all events associated with the primary calendar of an account. 
    /// <see href="https://developers.google.com/calendar/api/v3/reference/calendars/clear"/>
    /// </summary>
    public async Task<string> ClearAsync(string calendarId, CancellationToken cancellationToken = default)
    {
        ClearRequest clearRequest = calendarService.Calendars.Clear(calendarId);
        string cleared = await clearRequest.ExecuteAsync(cancellationToken);

        return cleared;
    }

    /// <summary>Deletes a secondary calendar. Use CalendarsService.Clear for clearing all events on primary calendars.
    /// <see href="https://developers.google.com/calendar/api/v3/reference/calendars/delete"/>
    /// </summary>
    public async Task<string> DeleteAsync(string calendarId, CancellationToken cancellationToken = default)
    {
        DeleteRequest deleteRequest = calendarService.Calendars.Delete(calendarId);
        string deleted = await deleteRequest.ExecuteAsync(cancellationToken);

        return deleted;
    }
}

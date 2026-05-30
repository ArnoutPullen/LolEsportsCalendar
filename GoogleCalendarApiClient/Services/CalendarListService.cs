using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using static Google.Apis.Calendar.v3.CalendarListResource;

namespace GoogleCalendarApiClient.Services;

public class CalendarListService(CalendarService calendarService, ILogger<CalendarListService> logger)
{
    /// <summary>Returns the calendars on the user's calendar list.
    /// <see href="https://developers.google.com/calendar/api/v3/reference/calendarList/list"/>
    /// </summary>
    public async Task<CalendarList> ListAsync(CancellationToken cancellationToken = default)
    {
        ListRequest request = calendarService.CalendarList.List();
        return await request.ExecuteAsync(cancellationToken);
    }

    public async Task<CalendarList> ListAsync(string nextPageToken, CancellationToken cancellationToken = default)
    {
        ListRequest request = calendarService.CalendarList.List();
        request.PageToken = nextPageToken;
        return await request.ExecuteAsync(cancellationToken);
    }

    /// <summary>Returns a calendar from the user's calendar list.
    /// <see href="https://developers.google.com/calendar/api/v3/reference/calendarList/get"/>
    /// </summary>
    public async Task<CalendarListEntry?> GetAsync(string calendarId, CancellationToken cancellationToken = default)
    {
        CalendarListEntry? calendarListEntry = null;

        try
        {
            GetRequest request = calendarService.CalendarList.Get(calendarId);
            calendarListEntry = await request.ExecuteAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error while getting CalendarListEntry with id {CalendarId}", calendarId);
        }

        return calendarListEntry;
    }

    /// <summary>Watch for changes to CalendarList resources. 
    /// <see href="https://developers.google.com/calendar/api/v3/reference/calendarList/watch"/>
    /// </summary>
    public async Task<Channel> Watch(Channel body, CancellationToken cancellationToken = default)
    {
        WatchRequest request = calendarService.CalendarList.Watch(body);
        return await request.ExecuteAsync(cancellationToken);
    }

    /// <summary>Inserts an existing calendar into the user's calendar list.
    /// <see href="https://developers.google.com/calendar/api/v3/reference/calendarList/insert"/>
    /// </summary>
    public async Task<CalendarListEntry> InsertAsync(CalendarListEntry calendar, CancellationToken cancellationToken = default)
    {
        InsertRequest request = calendarService.CalendarList.Insert(calendar);
        return await request.ExecuteAsync(cancellationToken);
    }

    public async Task<CalendarListEntry> InsertOrUpdate(CalendarListEntry calendar, string calendarId, CancellationToken cancellationToken = default)
    {
        CalendarListEntry calendarListEntry;
        CalendarListEntry? exists = await GetAsync(calendar.Id, cancellationToken);

        if (exists == null)
        {
            calendarListEntry = await InsertAsync(calendar, cancellationToken);
        }
        else
        {
            calendarListEntry = await UpdateAsync(calendar, calendarId, cancellationToken);
        }

        return calendarListEntry;
    }

    /// <summary>Updates an existing calendar on the user's calendar list.
    /// <see href="https://developers.google.com/calendar/api/v3/reference/calendarList/update"/>
    /// </summary>
    public async Task<CalendarListEntry> UpdateAsync(CalendarListEntry calendar, string calendarId, CancellationToken cancellationToken = default)
    {
        UpdateRequest request = calendarService.CalendarList.Update(calendar, calendarId);
        return await request.ExecuteAsync(cancellationToken);
    }

    /// <summary>Updates an existing calendar on the user's calendar list. This method supports patch semantics.
    /// <see href="https://developers.google.com/calendar/api/v3/reference/calendarList/patch"/>
    /// </summary>
    public async Task<CalendarListEntry> PatchAsync(CalendarListEntry calendar, string calendarId, CancellationToken cancellationToken = default)
    {
        PatchRequest request = calendarService.CalendarList.Patch(calendar, calendarId);
        return await request.ExecuteAsync(cancellationToken);
    }

    /// <summary>Removes a calendar from the user's calendar list.
    /// <see href="https://developers.google.com/calendar/api/v3/reference/calendarList/delete"/>
    /// </summary>
    public async Task<string> DeleteAsync(string calendarId, CancellationToken cancellationToken = default)
    {
        DeleteRequest request = calendarService.CalendarList.Delete(calendarId);
        return await request.ExecuteAsync(cancellationToken);
    }
}

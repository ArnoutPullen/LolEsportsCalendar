using Google.Apis.Calendar.v3.Data;
using LolEsportsApiClient.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GoogleCalendarApiClient.Services;

public class GoogleCalendarService(
    CalendarsService calendarsService,
    CalendarListService calendarListService,
    ILogger<GoogleCalendarService> logger)
{
    private Dictionary<string, string>? _calendarLookup = null;

    public async Task<Dictionary<string, string>> GetCalendarLookupAsync(CancellationToken cancellationToken = default)
    {
        return _calendarLookup ??= await BuildCalendarLookupAsync(cancellationToken);
    }

    public async Task<Calendar?> InsertLeagueAsCalendarAsync(League league, CancellationToken cancellationToken = default)
    {
        Calendar? newCalendar = null;
        Calendar calendar = ConvertLeagueToCalendar(league);

        try
        {
            newCalendar = await calendarsService.InsertAsync(calendar, cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error while creating calendar: {CalendarSummary}", calendar.Summary);
        }

        if (newCalendar != null)
        {
            logger.LogInformation("Created calendar {CalendarSummary}", calendar.Summary);
            _calendarLookup ??= await BuildCalendarLookupAsync(cancellationToken);
            _calendarLookup.Add(calendar.Summary, calendar.Id);
        }

        return newCalendar;
    }

    public static Calendar ConvertLeagueToCalendar(League league)
    {
        Calendar calendar = new()
        {
            Summary = league.Name,
            Description = league.Name + " / " + league.Region,
        };

        return calendar;
    }

    public static Google.Apis.Calendar.v3.Data.Event ConvertEsportEventToGoogleEvent(EsportEvent esportEvent)
    {
        if (esportEvent.Match == null) throw new MissingMemberException("EsportEvent is missing match property");

        Google.Apis.Calendar.v3.Data.Event @event = new()
        {
            Id = esportEvent.Match.Id,
            Start = new EventDateTime { DateTimeDateTimeOffset = esportEvent.StartTime.UtcDateTime },
            End = new EventDateTime { DateTimeDateTimeOffset = esportEvent.StartTime.UtcDateTime.AddHours(1) },
            Summary = esportEvent.Match.Teams[0].Code + " vs " + esportEvent.Match.Teams[1].Code
        };

        return @event;
    }

    private async Task<Dictionary<string, string>> BuildCalendarLookupAsync(CancellationToken cancellationToken = default)
    {
        // View
        Dictionary<string, string> calendarLookup = [];
        List<CalendarListEntry> calendarList = await GetAllCalendarsAsync(cancellationToken);

        // Add calendars to lookup
        foreach (var calendar in calendarList)
        {
            if (!calendarLookup.ContainsKey(calendar.Summary))
            {
                calendarLookup.Add(calendar.Summary, calendar.Id);
            }
        }

        return calendarLookup;
    }

    private async Task<List<CalendarListEntry>> GetAllCalendarsAsync(CancellationToken cancellationToken = default)
    {
        List<CalendarListEntry> calendars = [];
        CalendarList calendarList = await calendarListService.ListAsync(cancellationToken);
        calendars.AddRange(calendarList.Items);

        while (calendarList.NextPageToken != null)
        {
            calendarList = await calendarListService.ListAsync(calendarList.NextPageToken, cancellationToken);
            calendars.AddRange(calendarList.Items);
        }

        return calendars;
    }
}

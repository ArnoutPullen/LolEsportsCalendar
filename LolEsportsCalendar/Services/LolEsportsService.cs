using Google;
using Google.Apis.Calendar.v3.Data;
using GoogleCalendarApiClient.Services;
using LolEsportsApiClient;
using LolEsportsApiClient.Models;
using LolEsportsApiClient.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LolEsportsCalendar.Services;

public class LolEsportsService(
        GoogleCalendarService googleCalendarService,
        LolEsportsClient lolEsportsClient,
        EventsService eventsService,
        CalendarsService calendarsService,
        ILogger<LolEsportsService> logger,
        IOptions<LolEsportsOptions> options
    )
{
    private readonly LolEsportsOptions _options = options.Value;
    private int _rateLimitExceededCount = 0;

    public async Task ImportEvents(CancellationToken cancellationToken = default)
    {
        string[] leagueNames = _options.Leagues;

        if (leagueNames != null)
        {
            foreach (var leagueName in leagueNames)
            {
                Calendar? calendar = await FindOrCreateCalendarByLeagueName(leagueName, cancellationToken);

                if (calendar != null)
                {
                    League league = await lolEsportsClient.GetLeagueByName(leagueName, cancellationToken);

                    if (league == null)
                    {
                        logger.LogWarning("Couldn't find league with name {LeagueName}", leagueName);
                    }
                    else
                    {
                        // Import events for calendar
                        await ImportEventsForLeagueAsync(league, calendar, cancellationToken: cancellationToken);
                    }
                }
            }
        }
        else
        {
            await ImportEventsForAllCalendarsAsync(cancellationToken);
        }

        logger.LogInformation("All events imported");
        Console.ReadLine();
    }

    public async Task ImportEventsForAllCalendarsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            List<League> leagues = await lolEsportsClient.GetLeaguesAsync(cancellationToken);

            foreach (League league in leagues)
            {
                Calendar? calendar = await FindOrCreateCalendarByLeagueName(league.Name, cancellationToken);

                if (calendar != null)
                {
                    // Import events for calendar
                    await ImportEventsForLeagueAsync(league, calendar, cancellationToken: cancellationToken);

                    // Wait 1 sec
                    Thread.Sleep(60);
                }

            }
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error while importing events for all calendars");
        }
    }

    public async Task ImportEventsForLeagueAsync(League league, Calendar calendar, string? page = null, CancellationToken cancellationToken = default)
    {
        try
        {
            LolEsportsScheduleResponseData? data = null;

            do
            {
                // Get scheduled events of league
                data = await lolEsportsClient.GetScheduleByLeagueAsync(league, page, cancellationToken);

                foreach (EsportEvent esportEvent in data.Schedule.Events)
                {
                    if (null == esportEvent.Match.Id)
                    {
                        logger.LogError("Error esport event.match.id null");
                        continue;
                    }
                    // Convert LeagueEvent to GoogleEvent
                    Event googleEvent = GoogleCalendarService.ConvertEsportEventToGoogleEvent(esportEvent);

                    // Insert or Update GoogleEvent
                    eventsService.InsertOrUpdate(googleEvent, calendar, googleEvent.Id); // TODO
                }

                page = data.Schedule.Pages.Newer;
            } while (data.Schedule.Pages.Newer != null);
        }
        catch (GoogleApiException exception)
        {
            // :( Google can't handle all requests
            // https://developers.google.com/calendar/api/guides/quota
            // https://stackoverflow.com/a/40990761/5828267
            if (exception.Error.Code == 403 && exception.Error.Message == "Rate Limit Exceeded")
            {
                _rateLimitExceededCount++;

                if (_rateLimitExceededCount < 8)
                {
                    // Wait
                    Thread.Sleep(60 * _rateLimitExceededCount);

                    // Retry
                    await ImportEventsForLeagueAsync(league, calendar, cancellationToken: cancellationToken);
                }
            }
            logger.LogError(exception, "Error while importing events for leauge {LeagueName}", league.Name);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error while importing events for leauge {LeagueName}", league.Name);
        }

        logger.LogInformation("Events imported for league {LeagueName}", league.Name);
    }

    public async Task<Calendar?> FindOrCreateCalendarByLeagueName(string leagueName, CancellationToken cancellationToken = default)
    {
        Calendar? calendar;
        string? calendarId = null;

        // Get calendars
        var calendars = googleCalendarService.GetCalendarLookup();

        // Get league
        League league = await lolEsportsClient.GetLeagueByName(leagueName, cancellationToken);

        if (league == null)
        {
            logger.LogWarning("Couldn't find league with name {LeagueName}", leagueName);
            return null;
        }

        foreach (var c in calendars)
        {
            if (c.Key == league.Name)
            {
                calendarId = c.Value;
            }
            else
            if (c.Key == league.Slug)
            {
                calendarId = c.Value;
            }
        }

        // Get existing calendar
        if (calendarId == null) throw new NullReferenceException(nameof(calendarId));

        calendar = calendarsService.Get(calendarId);

        if (calendar != null) return calendar;

        // Creat new calendar
        logger.LogInformation("Creating new calendar {LeagueName}", league.Name);
        Calendar newCalendar = ConvertLeagueToCalendar(league);

        return calendarsService.Insert(newCalendar);
    }

    public static Calendar ConvertLeagueToCalendar(League league)
    {
        Calendar calendar = new()
        {
            Summary = league.Name,
            Description = league.Name + " / " + league.Region,
            // ETag = "Test",
            // Kind = "Test",
            // Location = "Test",
            // TimeZone = "Europe/Amsterdam"
        };

        return calendar;
    }
}

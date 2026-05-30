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
using System.Linq;
using System.Text;
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
    private int _rateLimitExceededCount = 0;

    public async Task ImportEvents(CancellationToken cancellationToken = default)
    {
        string[]? leagueNames = options.Value.Leagues;

        if (leagueNames != null && leagueNames.Length != 0)
        {
            foreach (var leagueName in leagueNames)
            {
                Calendar? calendar = await FindOrCreateCalendarByLeagueName(leagueName, cancellationToken);

                if (calendar != null)
                {
                    League? league = await lolEsportsClient.GetLeagueByName(leagueName, cancellationToken);

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
                    await Task.Delay(60, cancellationToken);
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

                foreach (EsportEvent esportEvent in data?.Schedule.Events ?? [])
                {
                    if (null == esportEvent.Match?.Id)
                    {
                        logger.LogError("Error esport event.match.id null");
                        continue;
                    }

                    // Convert LeagueEvent to GoogleEvent
                    Google.Apis.Calendar.v3.Data.Event googleEvent = GoogleCalendarService.ConvertEsportEventToGoogleEvent(esportEvent);

                    // Get events details
                    if (esportEvent.Id != null)
                    {
                        var details = await lolEsportsClient.GetEventDetailsAsync(esportEvent.Id, cancellationToken);

                        if (details != null)
                        {
                            foreach (var game in details.Event.Match.Games ?? [])
                            {
                                foreach (var vod in game.Vods)
                                {
                                    logger.LogInformation("Found vod for event {EventId}: {VodProvider} - {VodParameter}", esportEvent.Id, vod.Provider, vod.Parameter);
                                }
                            }
                        }
                    }

                    if (esportEvent.Match.Id != null)
                    {
                        var details = await lolEsportsClient.GetEventDetailsAsync(esportEvent.Match.Id, cancellationToken);

                        if (details != null)
                        {
                            List<(Game, Vod)> youtubeVods = [];
                            List<(Game, Vod)> twitchVods = [];

                            var descriptionBuilder = new StringBuilder();

                            if (esportEvent.StartTime <= DateTime.UtcNow)
                            {
                                descriptionBuilder.Append("LolEsports:<ul>");

                                foreach (var game in details.Event.Match.Games)
                                {
                                    descriptionBuilder.Append($"<li><a href=\"https://lolesports.com/vod/{details.Event.Id}/{game.Number}/\">Game {game.Number}</a></li>");
                                }
                                descriptionBuilder.Append("</ul>");
                            }
                            else
                            {
                                // TODO: Add live urls
                            }

                            var name = $"{details.Event.League.Name} - {details.Event.Match.Teams.FirstOrDefault()?.Code} vs {details.Event.Match.Teams.LastOrDefault()?.Code}";

                            foreach (var game in details.Event.Match.Games ?? [])
                            {
                                var youtubeVod = game.Vods.FirstOrDefault(v => v.Provider == "youtube" && v.Locale == "en-US")
                                    ?? game.Vods.FirstOrDefault(v => v.Provider == "youtube" && v.Locale == "en-GB")
                                    ?? game.Vods.FirstOrDefault(v => v.Provider == "youtube");

                                if (youtubeVod != null)
                                {
                                    youtubeVods.Add((game, youtubeVod));
                                }

                                var twitchVod = game.Vods.FirstOrDefault(v => v.Provider == "twitch" && v.Locale == "en-US")
                                    ?? game.Vods.FirstOrDefault(v => v.Provider == "twitch" && v.Locale == "en-GB")
                                    ?? game.Vods.FirstOrDefault(v => v.Provider == "twitch");

                                if (twitchVod != null)
                                {
                                    twitchVods.Add((game, twitchVod));
                                }
                            }

                            if (youtubeVods.Any())
                            {
                                descriptionBuilder.Append("Youtube:<ul>");
                                foreach (var (game, video) in youtubeVods)
                                {
                                    if (video.StartMillis.HasValue)
                                    {
                                        var offsetSeconds = (int)TimeSpan.FromMilliseconds(video.StartMillis.Value).TotalSeconds;
                                        descriptionBuilder.Append($"<li><a href=\"https://www.youtube.com/watch?v={video.Parameter}&t={offsetSeconds}s\">Game {game.Number}</a></li>");
                                    }
                                    else
                                    {
                                        descriptionBuilder.Append($"<li><a href=\"https://www.youtube.com/watch?v={video.Parameter}\">Game {game.Number}</a></li>");
                                    }
                                }
                                descriptionBuilder.Append("</ul>");
                            }

                            if (twitchVods.Any())
                            {
                                descriptionBuilder.Append("Twitch:<ul>");
                                foreach (var (game, video) in twitchVods)
                                {
                                    if (video.StartMillis.HasValue)
                                    {
                                        var timeSpan = TimeSpan.FromMilliseconds(video.StartMillis.Value);
                                        var twitchTimestamp = "t=";
                                        if (timeSpan.Hours > 0)
                                        {
                                            twitchTimestamp += $"{timeSpan.Hours}h";
                                        }
                                        if (timeSpan.Minutes > 0)
                                        {
                                            twitchTimestamp += $"{timeSpan.Minutes}m";
                                        }
                                        if (timeSpan.Seconds > 0)
                                        {
                                            twitchTimestamp += $"{timeSpan.Seconds}s";
                                        }
                                        descriptionBuilder.Append($"<li><a href=\"https://www.twitch.tv/videos/{video.Parameter}?{twitchTimestamp}\">Game {game.Number}</a></li>");
                                    }
                                    else
                                    {
                                        descriptionBuilder.Append($"<li><a href=\"https://www.twitch.tv/videos/{video.Parameter}\">Game {game.Number}</a></li>");
                                    }
                                }
                                descriptionBuilder.Append("</ul>");
                            }

                            var description = descriptionBuilder.ToString();
                            if (!string.IsNullOrEmpty(description))
                            {
                                googleEvent.Description += description;
                            }
                        }
                    }

                    // Insert or Update GoogleEvent
                    await eventsService.InsertOrUpdateAsync(googleEvent, calendar, googleEvent.Id, cancellationToken);
                }

                page = data?.Schedule.Pages?.Newer;
            } while (data?.Schedule.Pages?.Newer != null);
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
                    await Task.Delay(60 * _rateLimitExceededCount, cancellationToken);

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
        string? calendarId = null;

        // Get calendars
        var calendars = await googleCalendarService.GetCalendarLookupAsync(cancellationToken);

        // Get league
        League? league = await lolEsportsClient.GetLeagueByName(leagueName, cancellationToken);

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

        if (calendarId == null)
        {
            // Creat new calendar
            logger.LogInformation("Creating new calendar {LeagueName}", league.Name);
            Calendar newCalendar = ConvertLeagueToCalendar(league);
            Calendar calendar = await calendarsService.InsertAsync(newCalendar, cancellationToken);
            lolEsportsClient.ClearLeaguesCache();

            return calendar;
        }

        return await calendarsService.GetAsync(calendarId, cancellationToken);
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

using LolEsportsApiClient.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace LolEsportsApiClient;

public class LolEsportsClient(HttpClient httpClient)
{
    private List<League>? _leagues = null;

    public async Task<List<League>> GetLeaguesAsync(CancellationToken cancellationToken = default)
    {
        var response = await GetDataAsync<LolEsportsLeaguesResponseData>("/persisted/gw/getLeagues" + DictionaryToQueryString(), cancellationToken);

        return response?.Leagues ?? [];
    }

    public async Task<League?> GetLeagueByName(string leagueName, CancellationToken cancellationToken = default)
    {
        _leagues ??= await GetLeaguesAsync(cancellationToken);

        foreach (League league in _leagues)
        {
            if (league.Name.Equals(leagueName, System.StringComparison.CurrentCultureIgnoreCase)
                || league.Slug.Equals(leagueName, System.StringComparison.CurrentCultureIgnoreCase))
            {
                return league;
            }
        }

        return null;
    }

    public async Task<List<EsportEvent>> GetScheduleAsync(CancellationToken cancellationToken = default)
    {
        var response = await GetDataAsync<LolEsportsScheduleResponseData>("/persisted/gw/getSchedule" + DictionaryToQueryString(), cancellationToken);

        return response?.Schedule.Events ?? [];
    }

    public async Task<LolEsportsScheduleResponseData?> GetScheduleByLeagueAsync(League league, string? page, CancellationToken cancellationToken = default)
    {
        Dictionary<string, string> query = new()
        {
            { "leagueId", league.Id }
        };

        if (!string.IsNullOrEmpty(page))
        {
            query.Add("pageToken", page);
        }

        return await GetDataAsync<LolEsportsScheduleResponseData>("/persisted/gw/getSchedule" + DictionaryToQueryString(query), cancellationToken);
    }

    public async Task<EventDetailsResponse?> GetEventDetailsAsync(string eventId, CancellationToken cancellationToken = default)
    {
        Dictionary<string, string> query = new()
        {
            { "id", eventId }
        };

        return await GetDataAsync<EventDetailsResponse>("/persisted/gw/getEventDetails" + DictionaryToQueryString(query), cancellationToken);
    }

    public void ClearLeaguesCache()
    {
        _leagues = null;
    }

    private async Task<T?> GetDataAsync<T>(string path, CancellationToken cancellationToken = default) where T : class
    {
        var response = await httpClient.GetAsync(path, cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsAsync<LolEsportsResponse<T>>(cancellationToken);

        return result?.Data ?? throw new InvalidOperationException("Response data was null");
    }

    private static string DictionaryToQueryString(Dictionary<string, string>? query = null)
    {
        NameValueCollection queryString = HttpUtility.ParseQueryString(string.Empty);

        query ??= [];

        queryString.Add("hl", "en-GB");

        foreach (var item in query)
        {
            queryString.Add(item.Key, item.Value);
        }

        return '?' + queryString.ToString();
    }
}

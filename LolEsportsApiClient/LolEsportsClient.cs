using LolEsportsApiClient.Models;
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
        var leaguesResponseData = await GetDataAsync<LolEsportsLeaguesResponseData>("/persisted/gw/getLeagues" + DictionaryToQueryString(), cancellationToken);

        return leaguesResponseData?.Leagues ?? [];
    }

    public async Task<League?> GetLeagueByName(string leagueName, CancellationToken cancellationToken = default)
    {
        _leagues ??= await GetLeaguesAsync(cancellationToken);

        foreach (League league in _leagues)
        {
            if (league.Name == leagueName || league.Slug.Equals(leagueName, System.StringComparison.CurrentCultureIgnoreCase))
            {
                return league;
            }
        }

        return null;
    }

    public async Task<List<EsportEvent>> GetScheduleAsync(CancellationToken cancellationToken = default)
    {
        var leaguesResponseData = await GetDataAsync<LolEsportsScheduleResponseData>("/persisted/gw/getSchedule" + DictionaryToQueryString(), cancellationToken);

        return leaguesResponseData?.Schedule.Events ?? [];
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

    private async Task<T?> GetDataAsync<T>(string path, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync(path, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException("Response not ok");
        }

        LolEsportsResponse<T> data = await response.Content.ReadAsAsync<LolEsportsResponse<T>>(cancellationToken);
        return data.Data;
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

﻿using LolEsportsApiClient.Models;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace LolEsportsApiClient
{
	public class LolEsportsClient
    {
		private readonly HttpClient _httpClient;
        private List<League> _leagues = null;

        public LolEsportsClient(HttpClient httpClient)
		{
		    _httpClient = httpClient;
            _leagues = GetLeaguesAsync().GetAwaiter().GetResult();
        }

        public async Task<List<League>> GetLeaguesAsync()
        {
            var leaguesResponseData = await GetDataAsync<LolEsportsLeaguesResponseData>("/persisted/gw/getLeagues" + DictionaryToQueryString());

			_leagues = leaguesResponseData.Leagues;

            return _leagues;
        }

        public async Task<League> GetLeagueByName(string leagueName)
        {
            _leagues ??= await GetLeaguesAsync();

			foreach (League league in _leagues)
			{
                if (league.Name == leagueName || league.Slug.Equals(leagueName, System.StringComparison.CurrentCultureIgnoreCase))
                {
                    return league;
				}
			}

            return null;
		}

        public async Task<List<EsportEvent>> GetScheduleAsync()
        {
            var leaguesResponseData = await GetDataAsync<LolEsportsScheduleResponseData>("/persisted/gw/getSchedule" + DictionaryToQueryString());

            return leaguesResponseData.Schedule.Events;
        }

        public async Task<LolEsportsScheduleResponseData> GetScheduleByLeagueAsync(League league, string page)
        {
			Dictionary<string, string> query = new()
            {
				{ "leagueId", league.Id }
			};

            if (!string.IsNullOrEmpty(page))
            {
                query.Add("pageToken", page);
            }

            return await GetDataAsync<LolEsportsScheduleResponseData>("/persisted/gw/getSchedule" + DictionaryToQueryString(query));
        }

        private async Task<T> GetDataAsync<T>(string path)
        {
            var response = await _httpClient.GetAsync(path);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException("Response not ok");
            }

            LolEsportsResponse<T> data = await response.Content.ReadAsAsync<LolEsportsResponse<T>>();
            return data.Data;
        }

        private static string DictionaryToQueryString(Dictionary<string, string> query = null)
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
}

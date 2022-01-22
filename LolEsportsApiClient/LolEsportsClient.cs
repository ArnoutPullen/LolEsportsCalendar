using LolEsportsApiClient.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace LolEsportsApiClient
{
	public class LolEsportsClient
    {
		private readonly HttpClient _httpClient;
        private readonly ILogger<LolEsportsClient> _logger;
        private List<League> _leagues = null;

        public LolEsportsClient(HttpClient httpClient, ILogger<LolEsportsClient> logger)
		{
		    _httpClient = httpClient;
            _logger = logger;
		}

        public async Task<List<League>> GetLeaguesAsync()
        {
            var leaguesResponseData = await GetDataAsync<LolEsportsLeaguesResponseData>("/persisted/gw/getLeagues" + DictionaryToQueryString());

			_leagues = leaguesResponseData.Leagues;

            return _leagues;
        }

        public League GetLeagueByName(string leagueName)
        {
            if (_leagues == null)
            {
                _leagues = GetLeaguesAsync().GetAwaiter().GetResult();
            }
			foreach (League league in _leagues)
			{
                if (league.Name == leagueName)
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

        public async Task<List<EsportEvent>> GetScheduleByLeagueAsync(string leagueId)
        {
            Dictionary<string, string> query = new Dictionary<string, string>();
            query.Add("leagueId", leagueId);

            var leaguesResponseData = await GetDataAsync<LolEsportsScheduleResponseData>("/persisted/gw/getSchedule" + DictionaryToQueryString(query));

            return leaguesResponseData.Schedule.Events;
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

            /*
            T data = default;

            try
			{
                var response = await _httpClient.GetAsync(path);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException("Response not ok");
                }

                return await response.Content.ReadAsAsync<LolEsportsResponse<T>>().Data;
            }
			catch (Exception exception)
			{
                _logger.LogError(exception, "Error while getting data from {0}", path);
			}

            return data;
            */
        }

        private string DictionaryToQueryString(Dictionary<string, string> query = null)
        {
            NameValueCollection queryString = HttpUtility.ParseQueryString(string.Empty);

            if (query == null)
            {
                query = new Dictionary<string, string>();
            }

            queryString.Add("hl", "en-GB");

            foreach (var item in query)
            {
                queryString.Add(item.Key, item.Value);
            }

            return '?' + queryString.ToString();
        }
    }
}

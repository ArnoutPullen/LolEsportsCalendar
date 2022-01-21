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
        private ILogger<LolEsportsClient> _logger;

        public LolEsportsClient(HttpClient httpClient, ILogger<LolEsportsClient> logger)
		{
		    _httpClient = httpClient;
            _logger = logger;
		}

        public async Task<List<League>> GetLeaguesAsync()
        {
            var leaguesResponseData = await GetDataAsync<LolEsportsLeaguesResponseData>("/persisted/gw/getLeagues" + DictionaryToQueryString());

            return leaguesResponseData.Leagues;
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
            T data = default;

            try
			{
                var response = await _httpClient.GetAsync(path);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException("Response not ok");
                }

                data = (await response.Content.ReadAsAsync<LolEsportsResponse<T>>()).Data;
            }
			catch (Exception exception)
			{
                _logger.LogError(exception, "Error while getting data from {0}", path);
			}

            return data;
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

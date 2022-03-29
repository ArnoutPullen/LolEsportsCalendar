using LolEsportsApiClient.Models;
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

        public League GetLeagueByName(string leagueName)
        {
            if (_leagues == null)
            {
                _leagues = GetLeaguesAsync().GetAwaiter().GetResult();
            }

			foreach (League league in _leagues)
			{
                if (league.Name == leagueName || league.Slug == leagueName.ToLower())
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
			Dictionary<string, string> query = new Dictionary<string, string>
			{
				{ "leagueId", leagueId }
			};

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

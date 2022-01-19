using LolEsportsApiClient.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace LolEsportsApiClient
{
	public class LolEsportsClient
    {
		private readonly HttpClient _httpClient;

		public LolEsportsClient()
		{
            HttpClient httpClient  = new HttpClient();
			httpClient.BaseAddress = new Uri("https://esports-api.lolesports.com");

            using (StreamReader streamReader = new StreamReader("lolesports-credentials.json"))
            {
                string json = streamReader.ReadToEnd();
                LolEsportsClientSecrets secrets = JsonConvert.DeserializeObject<LolEsportsClientSecrets>(json);
                httpClient.DefaultRequestHeaders.Add("x-api-key", secrets.ApiKey);
                Console.WriteLine(secrets);
            }

            // httpClient.DefaultRequestHeaders.Add("x-api-key", "0TvQnueqKa5mxJntVWt0w4LpLfEkrV1Ta8rQBb9Z");
			// httpClient.DefaultRequestHeaders.Accept.Clear();
			// httpClient.DefaultRequestHeaders.Accept.Add(
			// 	new MediaTypeWithQualityHeaderValue("application/json"));
		    _httpClient = httpClient;
		}

        public async Task<List<League>> GetLeagues()
        {
            var leaguesResponseData = await GetData<LolEsportsLeaguesResponseData>("/persisted/gw/getLeagues" + DictionaryToQueryString());

            return leaguesResponseData.Leagues;
        }

        public async Task<List<EsportEvent>> GetSchedule()
        {
            var leaguesResponseData = await GetData<LolEsportsScheduleResponseData>("/persisted/gw/getSchedule" + DictionaryToQueryString());

            return leaguesResponseData.Schedule.Events;
        }

        public async Task<List<EsportEvent>> GetScheduleByLeague(string leagueId)
        {
            Dictionary<string, string> query = new Dictionary<string, string>();
            query.Add("leagueId", leagueId);

            var leaguesResponseData = await GetData<LolEsportsScheduleResponseData>("/persisted/gw/getSchedule" + DictionaryToQueryString(query));

            return leaguesResponseData.Schedule.Events;
        }

        private async Task<T> GetData<T>(string path)
        {
            var response = await _httpClient.GetAsync(path);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException("Response not ok");
            }

            var httpContent = await response.Content.ReadAsAsync<LolEsportsResponse<T>>();

            return httpContent.Data;
        }

        public string DictionaryToQueryString(Dictionary<string, string> query = null)
        {
            NameValueCollection queryString = HttpUtility.ParseQueryString(string.Empty);

            if (query == null)
            {
                query = new Dictionary<string, string>();
            }

            queryString.Add("hl", "en-GB");

            foreach (var x in query)
            {
                queryString.Add(x.Key, x.Value);
            }

            return '?' + queryString.ToString();
        }
    }

    public class LolEsportsClientSecrets
    {
        [JsonProperty("api_key")]
        public string ApiKey { get; set; }
	}
}

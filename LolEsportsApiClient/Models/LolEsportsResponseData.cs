using Newtonsoft.Json;
using System.Collections.Generic;

namespace LolEsportsApiClient.Models
{
    public class LolEsportsLeaguesResponseData
    {
        [JsonProperty("leagues")]
        public List<League> Leagues { get; set; }
    }

    public class LolEsportsScheduleResponseData
    {
        [JsonProperty("schedule")]
        public Schedule Schedule { get; set; }
    }
}

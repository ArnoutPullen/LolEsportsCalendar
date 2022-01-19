using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LolEsportsApiClient.Models
{
    public partial class LolEsportsResponseData
    {
        [JsonProperty("leagues")]
        public List<League> Leagues { get; set; }

        [JsonProperty("schedule")]
        public Schedule Schedule { get; set; }
    }

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

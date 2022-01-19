using Newtonsoft.Json;
using System.Collections.Generic;

namespace LolEsportsApiClient.Models
{
	public partial class Match
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("flags")]
        public List<Flag> Flags { get; set; }

        [JsonProperty("teams")]
        public List<Team> Teams { get; set; }

        [JsonProperty("strategy")]
        public Strategy Strategy { get; set; }
    }
}

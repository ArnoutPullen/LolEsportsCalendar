using Newtonsoft.Json;
using System;

namespace LolEsportsApiClient.Models
{
	public partial class Team
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("image")]
        public Uri Image { get; set; }

        [JsonProperty("result")]
        public Result Result { get; set; }

        [JsonProperty("record")]
        public Record Record { get; set; }
    }
}

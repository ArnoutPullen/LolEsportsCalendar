using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LolEsportsApiClient.Models
{
    public partial class Strategy
    {
        [JsonProperty("type")]
        public StrategyType Type { get; set; }

        [JsonProperty("count")]
        public long Count { get; set; }
    }
}

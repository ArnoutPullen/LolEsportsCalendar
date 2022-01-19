using Newtonsoft.Json;

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

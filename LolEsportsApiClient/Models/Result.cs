using Newtonsoft.Json;

namespace LolEsportsApiClient.Models
{
	public partial class Result
    {
        [JsonProperty("outcome")]
        public Outcome? Outcome { get; set; }

        [JsonProperty("gameWins")]
        public long GameWins { get; set; }
    }
}

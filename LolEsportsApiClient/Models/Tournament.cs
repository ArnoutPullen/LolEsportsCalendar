using Newtonsoft.Json;

namespace LolEsportsApiClient.Models
{
	public partial class Tournament
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}

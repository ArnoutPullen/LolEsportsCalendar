using Newtonsoft.Json;

namespace LolEsportsApiClient.Models
{
	public partial class Pages
    {
        [JsonProperty("older")]
        public string Older { get; set; }

        [JsonProperty("newer")]
        public string Newer { get; set; }
    }
}

using Newtonsoft.Json;

namespace LolEsportsApiClient.Models;

public partial class Tournament
{
    [JsonProperty("id")]
    public required string Id { get; set; }
}

using Newtonsoft.Json;

namespace LolEsportsApiClient.Models;

public partial class DisplayPriority
{
    [JsonProperty("position")]
    public long Position { get; set; }

    [JsonProperty("status")]
    public string? Status { get; set; }
}

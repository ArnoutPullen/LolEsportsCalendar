using Newtonsoft.Json;

namespace LolEsportsApiClient.Models;

public class EventDetailsResponse
{
    [JsonProperty("event")]
    public required Event Event { get; set; }
}

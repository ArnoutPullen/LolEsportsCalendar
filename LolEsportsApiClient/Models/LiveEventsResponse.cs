using Newtonsoft.Json;

namespace LolEsportsApiClient.Models;

public class LiveEventsResponse
{
    [JsonProperty("schedule")]
    public required LiveSchedule Schedule { get; set; }
}

using Newtonsoft.Json;

namespace LolEsportsApiClient.Models;

public class ScheduleResponse
{
    [JsonProperty("schedule")]
    public required Schedule Schedule { get; set; }
}

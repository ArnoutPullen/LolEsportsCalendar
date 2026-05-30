using Newtonsoft.Json;
using System.Collections.Generic;

namespace LolEsportsApiClient.Models;

public class LolEsportsLeaguesResponseData
{
    [JsonProperty("leagues")]
    public List<League> Leagues { get; set; } = [];
}

public class LolEsportsScheduleResponseData
{
    [JsonProperty("schedule")]
    public required Schedule Schedule { get; set; }
}

public class EventDetailsResponse
{
    [JsonProperty("event")]
    public required Event Event { get; set; }
}

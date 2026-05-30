using Newtonsoft.Json;
using System.Collections.Generic;

namespace LolEsportsApiClient.Models;

public class LiveSchedule
{
    [JsonProperty("events")]
    public List<LiveEvent> Events { get; set; } = [];
}

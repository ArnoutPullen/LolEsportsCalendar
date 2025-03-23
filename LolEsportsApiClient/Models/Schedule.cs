using Newtonsoft.Json;
using System.Collections.Generic;

namespace LolEsportsApiClient.Models;

public partial class Schedule
{
    [JsonProperty("pages")]
    public Pages? Pages { get; set; }

    [JsonProperty("events")]
    public List<EsportEvent> Events { get; set; } = [];
}

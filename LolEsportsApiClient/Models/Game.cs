using Newtonsoft.Json;
using System.Collections.Generic;

namespace LolEsportsApiClient.Models;

public class Game
{
    [JsonProperty("number")]
    public int? Number { get; set; }

    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("state")]
    public string State { get; set; }

    [JsonProperty("teams")]
    public List<Team> Teams { get; set; }

    [JsonProperty("vods")]
    public List<Vod> Vods { get; set; }
}

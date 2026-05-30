using Newtonsoft.Json;
using System.Collections.Generic;

namespace LolEsportsApiClient.Models;

public class Event
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("tournament")]
    public Tournament Tournament { get; set; }

    [JsonProperty("league")]
    public League League { get; set; }

    [JsonProperty("match")]
    public Match Match { get; set; }

    [JsonProperty("streams")]
    public List<object> Streams { get; set; }
}

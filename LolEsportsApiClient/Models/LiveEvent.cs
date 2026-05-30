using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace LolEsportsApiClient.Models;

public class LiveEvent
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("startTime")]
    public DateTime? StartTime { get; set; }

    [JsonProperty("state")]
    public string State { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("blockName")]
    public string BlockName { get; set; }

    [JsonProperty("league")]
    public League League { get; set; }

    [JsonProperty("tournament")]
    public Tournament Tournament { get; set; }

    [JsonProperty("match")]
    public Match Match { get; set; }

    [JsonProperty("streams")]
    public List<LiveStream> Streams { get; set; } = [];
}
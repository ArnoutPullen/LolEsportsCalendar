using Newtonsoft.Json;
using System;

namespace LolEsportsApiClient.Models;

public class Vod
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("parameter")]
    public string Parameter { get; set; }

    [JsonProperty("locale")]
    public string Locale { get; set; }

    [JsonProperty("mediaLocale")]
    public MediaLocale MediaLocale { get; set; }

    [JsonProperty("provider")]
    public string Provider { get; set; }

    [JsonProperty("offset")]
    public int? Offset { get; set; }

    [JsonProperty("firstFrameTime")]
    public DateTime? FirstFrameTime { get; set; }

    [JsonProperty("startMillis")]
    public int? StartMillis { get; set; }

    [JsonProperty("endMillis")]
    public int? EndMillis { get; set; }
}

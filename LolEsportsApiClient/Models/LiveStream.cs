using Newtonsoft.Json;
using System.Collections.Generic;

namespace LolEsportsApiClient.Models;

public class LiveStream
{
    [JsonProperty("parameter")]
    public string Parameter { get; set; }

    [JsonProperty("locale")]
    public string Locale { get; set; }

    [JsonProperty("mediaLocale")]
    public MediaLocale MediaLocale { get; set; }

    [JsonProperty("provider")]
    public string Provider { get; set; }

    [JsonProperty("countries")]
    public List<object> Countries { get; set; }

    [JsonProperty("offset")]
    public int? Offset { get; set; }

    [JsonProperty("statsStatus")]
    public string StatsStatus { get; set; }
}
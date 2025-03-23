using Newtonsoft.Json;

namespace LolEsportsApiClient.Models;

public partial class Stream
{
    [JsonProperty("parameter")]
    public string? Parameter { get; set; }

    [JsonProperty("locale")]
    public string? Locale { get; set; }

    [JsonProperty("provider")]
    public Provider Provider { get; set; }

    [JsonProperty("countries")]
    public string[] Countries { get; set; } = [];

    [JsonProperty("offset")]
    public long Offset { get; set; }
}

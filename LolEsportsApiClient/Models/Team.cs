using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace LolEsportsApiClient.Models;

[DebuggerDisplay("{Code,nq} - {Name,nq}")]
public partial class Team
{
    [JsonProperty("name")]
    public required string Name { get; set; }

    [JsonProperty("code")]
    public required string Code { get; set; }

    [JsonProperty("image")]
    public Uri? Image { get; set; }

    [JsonProperty("result")]
    public Result? Result { get; set; }

    [JsonProperty("record")]
    public Record? Record { get; set; }
}

using Newtonsoft.Json;
using System.Collections.Generic;

namespace LolEsportsApiClient.Models;

public class LeaguesResponse
{
    [JsonProperty("leagues")]
    public List<League> Leagues { get; set; } = [];
}

using System.ComponentModel.DataAnnotations;

namespace LolEsportsApiClient.Options;

public class LolEsportsOptions
{
    public required string ApiKey { get; set; }
    public required string BaseUrl { get; set; }
    public string[]? Leagues { get; set; }
}

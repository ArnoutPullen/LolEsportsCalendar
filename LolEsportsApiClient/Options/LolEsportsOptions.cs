namespace LolEsportsApiClient.Options;

public class LolEsportsOptions
{
    public const string SectionName = "LolEsports";
    public required string ApiKey { get; set; }
    public required string BaseUrl { get; set; }
    public string[]? Leagues { get; set; }
}

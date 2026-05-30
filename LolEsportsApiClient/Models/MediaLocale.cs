using Newtonsoft.Json;

namespace LolEsportsApiClient.Models;

public class MediaLocale
{
    [JsonProperty("locale")]
    public string Locale { get; set; }

    [JsonProperty("englishName")]
    public string EnglishName { get; set; }

    [JsonProperty("translatedName")]
    public string TranslatedName { get; set; }
}

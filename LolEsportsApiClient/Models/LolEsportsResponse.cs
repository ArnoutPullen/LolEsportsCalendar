using Newtonsoft.Json;

namespace LolEsportsApiClient.Models;

public partial class LolEsportsResponse<TResponseDataType>
{
    [JsonProperty("data")]
    public TResponseDataType? Data { get; set; }
}

using Newtonsoft.Json;
using System;

namespace LolEsportsApiClient.Models
{
    public partial class LolEsportsResponse<TResponseDataType>
    {
        [JsonProperty("data")]
        public TResponseDataType Data { get; set; }
    }
}

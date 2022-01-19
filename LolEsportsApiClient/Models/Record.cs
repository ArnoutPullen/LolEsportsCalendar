using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LolEsportsApiClient.Models
{
    public partial class Record
    {
        [JsonProperty("wins")]
        public long Wins { get; set; }

        [JsonProperty("losses")]
        public long Losses { get; set; }
    }
}

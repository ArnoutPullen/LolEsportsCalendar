using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LolEsportsApiClient.Models
{
    public partial class Result
    {
        [JsonProperty("outcome")]
        public Outcome? Outcome { get; set; }

        [JsonProperty("gameWins")]
        public long GameWins { get; set; }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LolEsportsApiClient.Models
{
    public partial class DisplayPriority
    {
        [JsonProperty("position")]
        public long Position { get; set; }

        [JsonProperty("status")]
        // public Status Status { get; set; }
        public string Status { get; set; }
    }
}

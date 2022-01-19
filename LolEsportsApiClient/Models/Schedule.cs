using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LolEsportsApiClient.Models
{
    public partial class Schedule
    {
        [JsonProperty("events")]
        public List<EsportEvent> Events { get; set; }
    }
}

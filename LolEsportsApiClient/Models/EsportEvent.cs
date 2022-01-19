using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LolEsportsApiClient.Models
{
    public partial class EsportEvent
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("startTime")]
        public DateTimeOffset StartTime { get; set; }

        [JsonProperty("state")]
        public State State { get; set; }

        [JsonProperty("type")]
        public EventType Type { get; set; }

        [JsonProperty("blockName")]
        public string BlockName { get; set; }

        [JsonProperty("league")]
        public League League { get; set; }

        [JsonProperty("match", NullValueHandling = NullValueHandling.Ignore)]
        public Match Match { get; set; }
    }
}

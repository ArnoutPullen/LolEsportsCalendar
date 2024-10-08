﻿using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace LolEsportsApiClient.Models
{
    [DebuggerDisplay("{Region,nq} - {Name,nq}")]
    public partial class League
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("image")]
        public Uri Image { get; set; }

        [JsonProperty("priority")]
        public long Priority { get; set; }

        [JsonProperty("displayPriority")]
        public DisplayPriority DisplayPriority { get; set; }
    }
}

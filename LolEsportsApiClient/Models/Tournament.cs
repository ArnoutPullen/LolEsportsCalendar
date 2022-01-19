using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LolEsportsApiClient.Models
{
    public partial class Tournament
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}

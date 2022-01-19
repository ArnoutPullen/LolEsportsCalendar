using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LolEsportsApiClient.Models
{
    public partial class Pages
    {
        [JsonProperty("older")]
        public string Older { get; set; }

        [JsonProperty("newer")]
        public string Newer { get; set; }
    }
}

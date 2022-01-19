using Newtonsoft.Json;
using System.Collections.Generic;

namespace LolEsportsApiClient.Models
{
	public partial class Schedule
    {
        [JsonProperty("events")]
        public List<EsportEvent> Events { get; set; }
    }
}

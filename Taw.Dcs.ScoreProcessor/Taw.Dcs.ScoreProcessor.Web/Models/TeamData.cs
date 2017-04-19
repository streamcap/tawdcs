using Newtonsoft.Json;

namespace Taw.Dcs.ScoreProcessor.Web.Models
{
    public class TeamData
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("tabs")]
        public Tab[] Tabs { get; set; }
    }
}
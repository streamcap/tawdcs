using Newtonsoft.Json;

namespace Taw.Dcs.ScoreProcessor.Web.Models
{
    public class Tab
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("items")]
        public string[] Items { get; set; }
    }
}
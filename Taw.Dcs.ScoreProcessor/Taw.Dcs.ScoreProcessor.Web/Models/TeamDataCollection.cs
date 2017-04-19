using Newtonsoft.Json;

namespace Taw.Dcs.ScoreProcessor.Web.Models
{
    public class TeamDataCollection
    {
        [JsonProperty("teams")]
        public TeamData[] Teams { get; set; }
    }
}
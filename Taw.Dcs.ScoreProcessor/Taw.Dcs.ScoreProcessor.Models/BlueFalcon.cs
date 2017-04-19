namespace Taw.Dcs.ScoreProcessor.Models
{
    public class BlueFalcon : TopListItem
    {
        public string Target { get; set; }

        public override string ToString()
        {
            return $"Blue Falcon: {Name} team killed {Target}.";
        }
    }
}
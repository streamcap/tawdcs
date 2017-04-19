namespace Taw.Dcs.ScoreProcessor.Models
{
    public class TriggerHappy : TopListItem
    {
        public int Count { get; set; }
        public override string ToString()
        {
            return $"Trigger happy: {Name} hit {Count} targets on their own team.";
        }
    }
}
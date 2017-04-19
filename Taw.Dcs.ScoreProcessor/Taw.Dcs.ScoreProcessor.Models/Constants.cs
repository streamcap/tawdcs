using System.Linq;

namespace Taw.Dcs.ScoreProcessor.Models
{
    public static class Constants
    {
        public const string TeamKillEvent = "DESTROY_PENALTY";
        public const string TeamHitEvent = "HIT_PENALTY";
        public const string HitEvent = "HIT_SCORE";
        public const string KillEvent = "DESTROY_SCORE";
        public const string RedCoalitionName = "Red";
        public const string BlueCoalitionName = "Blue";
        public static readonly string[] PositiveEvents = { HitEvent, KillEvent };
        public static readonly string[] NegativeEvents = { TeamHitEvent, TeamKillEvent };
        public static readonly string[] Events = PositiveEvents.Concat(NegativeEvents).ToArray();
        public const string AllCoalitions = "All";
    }
}
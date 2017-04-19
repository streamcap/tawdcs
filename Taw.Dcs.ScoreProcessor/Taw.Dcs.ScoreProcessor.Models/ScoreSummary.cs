using System.Collections.Generic;
using System.Linq;
using Taw.Dcs.ScoreProcessor.Models.TableEntities;

namespace Taw.Dcs.ScoreProcessor.Models
{
    public static class ScoreSummary
    {
        public static IDictionary<string, double> GetScoreSums(IEnumerable<ScoreEvent> redSide, IEnumerable<ScoreEvent> blueSide)
        {
            var scores = new Dictionary<string, double>
            {
                ["blue"] = blueSide.Sum(s => s.Score * s.Times),
                ["red"] = redSide.Sum(s => s.Score * s.Times)
            };
            return scores;
        }

        public static ScoreEvent GetFirstPvpKill(IEnumerable<ScoreEvent> events)
        {
            return events.OrderBy(t => t.Time)
                .FirstOrDefault(l => l.ScoreType == Constants.KillEvent && !string.IsNullOrEmpty(l.TargetPlayerName));
        }

        public static ScoreEvent GetFirstKill(IEnumerable<ScoreEvent> events)
        {
            return events.OrderBy(t => t.Time).FirstOrDefault(l => l.ScoreType == Constants.KillEvent);
        }

        public static IEnumerable<TopListItem> GetTopList(IEnumerable<ScoreEvent> events)
        {
            var playersAndScores = events.Where(e => Constants.Events.Contains(e.ScoreType))
                .GroupBy(r => r.PlayerName)
                .Select(p => new TopListItem
                {
                    Name = p.Key,
                    Score = p.Sum(r => r.Score * r.Times),
                }).ToList();

            var topList = playersAndScores.OrderByDescending(r => r.Score).ToList();
            return topList;
        }
    }
}
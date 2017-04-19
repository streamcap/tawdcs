using System;
using System.Collections.Generic;
using System.Linq;
using Taw.Dcs.ScoreProcessor.Models;
using Taw.Dcs.ScoreProcessor.Models.TableEntities;

namespace Taw.Dcs.ScoreProcessor.Engine
{
    public static class ScoreAdjuster
    {
        public static IEnumerable<ScoreEvent> RunAlgorithm(KillDuplicateAlgorithm i, IEnumerable<ScoreEvent> scoreEvents)
        {
            return Algorithms[i](scoreEvents);
        }

        private static readonly Dictionary<KillDuplicateAlgorithm, Func<IEnumerable<ScoreEvent>, IEnumerable<ScoreEvent>>> Algorithms;

        static ScoreAdjuster()
        {
            Algorithms = new Dictionary<KillDuplicateAlgorithm, Func<IEnumerable<ScoreEvent>, IEnumerable<ScoreEvent>>>
            {
                { KillDuplicateAlgorithm.DoNothing, DoNothing},
                { KillDuplicateAlgorithm.ScrubDuplicates, ScrubDuplicates },
                { KillDuplicateAlgorithm.ShareScoreAmongKillers, ShareScoreAmongKillers }
            };
        }

        public static IEnumerable<KillDuplicateAlgorithm> GetDuplicateAlgorithmsList()
        {
            return Algorithms.Keys;
        }

        private static IEnumerable<ScoreEvent> DoNothing(IEnumerable<ScoreEvent> scoreEvents)
        {
            return scoreEvents;
        }

        private static IEnumerable<ScoreEvent> ScrubDuplicates(IEnumerable<ScoreEvent> scoreEvents)
        {
            var scoreList = scoreEvents.ToList();
            var allInvalidKills = new List<ScoreEvent>();
            var kills = scoreList.Where(e => e.ScoreType == Constants.KillEvent).ToList();
            var hits = scoreList.Where(e => e.ScoreType == Constants.HitEvent).ToList();
            var duplicates = kills.GroupBy(e => e.Time.ToLongTimeString() + e.TargetUnitName).Where(g => g.Count() > 1);

            foreach (var duplicate in duplicates)
            {
                var duplicateTarget = duplicate.First().TargetUnitName;
                var killTime = duplicate.First().Time;

                var lastKillBefore = kills.Where(k => k.TargetUnitName == duplicateTarget && k.Time < killTime)
                    .OrderByDescending(k => k.Time).FirstOrDefault();

                var lastKillTime = lastKillBefore?.Time ?? DateTime.MinValue;
                var lastHitBeforeThisKill = hits.Where(h => h.TargetUnitName == duplicateTarget && lastKillTime < h.Time && h.Time < killTime)
                    .OrderByDescending(h => h.Time).FirstOrDefault();
                var invalidKills = duplicate.Where(k => lastHitBeforeThisKill != null && k.PlayerName != lastHitBeforeThisKill.PlayerName);
                allInvalidKills.AddRange(invalidKills.ToList());
            }
            return scoreList.Except(allInvalidKills);
        }

        private static IEnumerable<ScoreEvent> ShareScoreAmongKillers(IEnumerable<ScoreEvent> scoreEvents)
        {
            var list = scoreEvents.ToList();
            var duplicates = list.Where(e => e.ScoreType == Constants.KillEvent)
                .GroupBy(e => e.Time.ToLongTimeString() + e.TargetUnitName)
                .Where(g => g.Count() > 1);

            foreach (var duplicate in duplicates)
            {
                var highestKillScore = duplicate.Max(e => e.Score * e.Times);
                foreach (var scoreEvent in duplicate)
                {
                    scoreEvent.Times = 1;
                    scoreEvent.Score = highestKillScore / duplicate.Count();
                }
            }
            return list;
        }
    }
}
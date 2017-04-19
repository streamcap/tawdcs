using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;
using Taw.Dcs.ScoreProcessor.Models;
using Taw.Dcs.ScoreProcessor.Models.TableEntities;

namespace Taw.Dcs.ScoreProcessor.Engine
{
    public class LogLinesCleaner : ILogLinesCleaner
    {
        private readonly ILogger _logger;

        public LogLinesCleaner(ILogger logger)
        {
            _logger = logger;
        }

        public IEnumerable<ScoreEvent> CleanScoreEvents(IEnumerable<ScoreEvent> scoreEvents, bool removeErrorLines, KillDuplicateAlgorithm killScoreAlgorithm)
        {
            scoreEvents = ScoreAdjuster.RunAlgorithm(killScoreAlgorithm, scoreEvents);
            scoreEvents = EnsureNegativePointsForPenalty(scoreEvents.ToList());
            if (removeErrorLines)
            {
                var scoreList = scoreEvents.ToList();
                var errorLines = scoreList.Where(ScoreEventIsMissingCoalitionInfo).ToList();
                scoreEvents = scoreList.Except(errorLines);
                return scoreEvents;
            }
            var eventList = scoreEvents.OrderBy(e => e.RunTime).ToList();
            var playersAndCoalitions = eventList.Select(e => new { e.PlayerName, e.PlayerUnitCoalition })
                .Where(e => e.PlayerUnitCoalition != null)
                .Distinct()
                .ToLookup(e => e.PlayerName, e => e.PlayerUnitCoalition);
            foreach (var scoreEvent in eventList)
            {
                EnsureCoalitions(scoreEvent, playersAndCoalitions);
            }
            return eventList;
        }

        private IEnumerable<ScoreEvent> EnsureNegativePointsForPenalty(IList<ScoreEvent> scoreEvents)
        {
            foreach (var scoreEvent in scoreEvents)
            {
                if (!Constants.NegativeEvents.Contains(scoreEvent.ScoreType) || scoreEvent.Score <= 0)
                {
                    continue;
                }
                _logger.Information("Negative correction:" + scoreEvent);
                scoreEvent.Score = scoreEvent.Score * -1;
            }
            return scoreEvents;
        }

        public IEnumerable<ScoreEvent> ScrubDuplicateKills(IEnumerable<ScoreEvent> scoreEvents)
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
            allInvalidKills.ForEach(kill => _logger.Information("Invalid kill score removed: " + kill.ToString()));
            return scoreList.Except(allInvalidKills);
        }

        private static bool ScoreEventIsMissingCoalitionInfo(ScoreEvent scoreEvent)
        {
            return string.IsNullOrEmpty(scoreEvent.PlayerUnitCoalition) || string.IsNullOrEmpty(scoreEvent.TargetUnitCoalition);
        }

        private void EnsureCoalitions(ScoreEvent scoreEvent, ILookup<string, string> playersAndCoalitions)
        {
            if (scoreEvent.PlayerUnitCoalition == null)
            {

                if (!playersAndCoalitions.Contains(scoreEvent.PlayerName))
                {
                    _logger.Error($"No coalition found for player {scoreEvent.PlayerName}!");
                    return;
                }
                scoreEvent.PlayerUnitCoalition = playersAndCoalitions[scoreEvent.PlayerName].First();
            }
            if (scoreEvent.TargetUnitCoalition == null && scoreEvent.TargetPlayerName != null)
            {
                if (!playersAndCoalitions.Contains(scoreEvent.TargetPlayerName))
                {
                    _logger.Error($"No coalition found for targeted player {scoreEvent.PlayerName}!");
                    return;
                }
                scoreEvent.TargetUnitCoalition = playersAndCoalitions[scoreEvent.TargetPlayerName].First();
            }
        }
    }
}

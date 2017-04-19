using System;
using System.Collections.Generic;
using System.Linq;
using Taw.Dcs.ScoreProcessor.Models;
using Taw.Dcs.ScoreProcessor.Models.TableEntities;

namespace Taw.Dcs.ScoreProcessor.Engine
{
    public static class ScoreInformationBuilder
    {
        public static IEnumerable<string> GetToplistLines(IEnumerable<ScoreEvent> events, string coalitionName)
        {
            events = events.Where(e => coalitionName == Constants.AllCoalitions || e.PlayerUnitCoalition == coalitionName).ToList();
            var topList = ScoreSummary.GetTopList(events);
            var lines = topList.Select(line => $"{line.Name}: {line.Score}");
            return lines;
        }

        public static IEnumerable<string> GetFirstKills(IList<ScoreEvent> events, string coalitionName)
        {
            events = events.Where(e => coalitionName == Constants.AllCoalitions || e.PlayerUnitCoalition == coalitionName).ToList();
            if (!events.Any())
            {
                return Enumerable.Empty<string>();
            }
            var lines = new List<string>();
            var firstKill = ScoreSummary.GetFirstKill(events);
            var firstPvpKill = ScoreSummary.GetFirstPvpKill(events);
            if (firstKill != null)
            {
                lines.Add($"First kill: {firstKill.PlayerName} destroyed {firstKill.Target} at {firstKill.Time:HH:mm:ss}.");
            }
            if (firstPvpKill != null)
            {
                lines.Add($"First PvP kill: {firstPvpKill.PlayerName} destroyed {firstPvpKill.TargetPlayerName} at {firstPvpKill.Time:HH:mm:ss}.");
            }
            return lines;
        }

        public static IEnumerable<string> GetMvps(IEnumerable<ScoreEvent> events, string coalitionName)
        {
            events = events.Where(e => coalitionName == Constants.AllCoalitions || e.PlayerUnitCoalition == coalitionName).ToList();
            if (!events.Any())
            {
                return Enumerable.Empty<string>();
            }
            var lines = new List<string>();
            var destroyers = events.Where(e => Constants.PositiveEvents.Contains(e.ScoreType))
                .GroupBy(r => r.PlayerName)
                .Select(p => new
                {
                    Name = p.Key,
                    Score = p.Sum(r => r.Score * r.Times),
                    Hits = p.Count(r => r.ScoreType == Constants.HitEvent),
                    Kills = p.Count(r => r.ScoreType == Constants.KillEvent)
                }).ToList();
            var mvps = destroyers.OrderByDescending(r => r.Score).Take(3);
            lines.AddRange(mvps.Select(mvp => $"MVP: {mvp.Name} scored {mvp.Score} points hitting {mvp.Hits} targets and killing {mvp.Kills} targets."));
            return lines;
        }

        public static string GetSumsText(IEnumerable<ScoreEvent> redSide, IEnumerable<ScoreEvent> blueSide)
        {
            var scores = ScoreSummary.GetScoreSums(redSide, blueSide);
            var sumsText = $"Red side: {scores["red"]}, Blue side: {scores["blue"]}.";
            return sumsText;
        }

        public static string GetWinnerDeclaration(IEnumerable<ScoreEvent> redSide, IEnumerable<ScoreEvent> blueSide)
        {
            var scores = ScoreSummary.GetScoreSums(redSide, blueSide);
            string winnerDeclaration;
            if (Math.Abs(scores["red"] - scores["blue"]) < 0.1)
            {
                winnerDeclaration = $"The game was a tie with both teams scoring {scores["red"]} each!";
            }
            else
            {
                var winner = scores["blue"] > scores["red"] ? "BLUE" : "RED";
                winnerDeclaration =
                    $"{winner} IS THE WINNER WITH {Math.Max(scores["blue"], scores["red"])} OVER {Math.Min(scores["blue"], scores["red"])}!";
            }
            return winnerDeclaration;
        }

        public static string GetNonPenaltyScoreDeclaration(IEnumerable<ScoreEvent> redSide, IEnumerable<ScoreEvent> blueSide)
        {
            var blueNonPenaltyScore =
                blueSide.Where(e => e.ScoreType != Constants.TeamHitEvent && e.ScoreType != Constants.TeamKillEvent)
                    .Sum(e => e.Score * e.Times);
            var redNonPenaltyScore =
                redSide.Where(e => e.ScoreType != Constants.TeamHitEvent && e.ScoreType != Constants.TeamKillEvent)
                    .Sum(e => e.Score * e.Times);
            var nonPenaltyScoreDeclaration =
                $"If not for the penalties, blue would have had {blueNonPenaltyScore} and red would have had {redNonPenaltyScore}...";
            return nonPenaltyScoreDeclaration;
        }

        public static IEnumerable<TriggerHappy> GetTriggerHappies(IEnumerable<ScoreEvent> events, string coalitionName)
        {
            events = events.Where(e => coalitionName == Constants.AllCoalitions || e.PlayerUnitCoalition == coalitionName).ToList();
            return events.Where(e => e.ScoreType == Constants.TeamHitEvent)
                .GroupBy(r => r.PlayerName).Select(p => new TriggerHappy
                {
                    Name = p.Key,
                    Score = p.Sum(r => r.Score * r.Times),
                    Count = p.Count()
                }).ToList();
        }

        public static IEnumerable<BlueFalcon> GetBlueFalcons(IEnumerable<ScoreEvent> events, string coalitionName)
        {
            events = events.Where(e => coalitionName == Constants.AllCoalitions || e.PlayerUnitCoalition == coalitionName).ToList();
            var tkEvents = events.Where(e => e.ScoreType == Constants.TeamKillEvent).ToList();
            var pvpTkEvents = tkEvents.Where(e => !string.IsNullOrEmpty(e.TargetPlayerName)).ToList();

            var bfs = pvpTkEvents.Select(p => new BlueFalcon
            {
                Name = p.PlayerName,
                Target = p.TargetPlayerName
            }).ToList();

            return bfs;
        }

        public static IEnumerable<string> GetHitsAndKills(IList<ScoreEvent> events, string coalitionName, string hitMask, string killMask)
        {
            events = events.Where(e => coalitionName == Constants.AllCoalitions || e.PlayerUnitCoalition == coalitionName).ToList();
            var numberOfHits = events.Where(e => e.ScoreType == hitMask).ToList();
            var numberOfKills = events.Where(e => e.ScoreType == killMask).ToList();
            var lines = new[]
            {
                $"Total number of good hits: {numberOfHits.Count} for {numberOfHits.Sum(s => s.Score * s.Times)} points.",
                $"Total number of good kills: {numberOfKills.Count} for {numberOfKills.Sum(s => s.Score * s.Times)} points."
            };
            return lines;
        }
    }
}
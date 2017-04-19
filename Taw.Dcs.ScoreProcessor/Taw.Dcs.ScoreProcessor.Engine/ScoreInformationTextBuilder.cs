using System.Collections.Generic;
using System.Linq;
using Taw.Dcs.ScoreProcessor.Models;
using Taw.Dcs.ScoreProcessor.Models.TableEntities;

namespace Taw.Dcs.ScoreProcessor.Engine
{
    public static class ScoreInformationTextBuilder
    {
        public static IEnumerable<string> GetTeamsSummaryLines(IList<ScoreEvent> events)
        {
            var redSide = events.Where(e => e.PlayerUnitCoalition == Constants.RedCoalitionName).ToList();
            var blueSide = events.Where(e => e.PlayerUnitCoalition == Constants.BlueCoalitionName).ToList();
            return new[]
            {
                ScoreInformationBuilder.GetSumsText(redSide, blueSide),
                ScoreInformationBuilder.GetWinnerDeclaration(redSide,blueSide),
                ScoreInformationBuilder.GetNonPenaltyScoreDeclaration(redSide, blueSide)
            };
        }

        public static IEnumerable<string> GetPenaltiesLines(IList<ScoreEvent> events, string coalitionName)
        {
            var lines = new List<string>();
            var triggerhappies = ScoreInformationBuilder.GetTriggerHappies(events, coalitionName);
            var blueFalcons = ScoreInformationBuilder.GetBlueFalcons(events, coalitionName);
            lines.AddRange(triggerhappies.OrderByDescending(r => r.Score).Select(triggerhappy => triggerhappy.ToString()));
            lines.AddRange(blueFalcons.Select(blueFalcon => blueFalcon.ToString()));
            return lines;
        }

        public static IEnumerable<string> GetTeamTopListLines(IList<ScoreEvent> events, string coalitionName, bool printFirsts, bool printMvps)
        {
            var lines = ScoreInformationBuilder.GetToplistLines(events, coalitionName).ToList();
            lines.AddRange(ScoreInformationBuilder.GetHitsAndKills(events, coalitionName, Constants.HitEvent, Constants.KillEvent));

            if (printFirsts)
            {
                lines.Add(string.Empty);
                lines.AddRange(ScoreInformationBuilder.GetFirstKills(events, coalitionName));
            }
            if (printMvps)
            {
                lines.Add(string.Empty);
                lines.AddRange(ScoreInformationBuilder.GetMvps(events, coalitionName));
            }
            return lines;
        }

        public static IEnumerable<string> GetAllReportLines(IList<ScoreEvent> events)
        {
            var lines = new List<string>();

            lines.AddRange(GetTeamsSummaryLines(events));
            lines.Add(string.Empty);

            lines.Add("Red player scores:");
            lines.AddRange(GetTeamTopListLines(events, Constants.RedCoalitionName, true, true));
            lines.Add(string.Empty);

            lines.Add("Blue player scores:");
            lines.AddRange(GetTeamTopListLines(events, Constants.BlueCoalitionName, true, true));
            lines.Add(string.Empty);

            lines.Add("All player scores:");
            lines.AddRange(GetTeamTopListLines(events, Constants.AllCoalitions, false, false));
            lines.Add(string.Empty);

            lines.Add("Penalties:");
            lines.AddRange(GetPenaltiesLines(events, Constants.AllCoalitions));
            lines.Add(string.Empty);
            return lines;
        }
    }
}

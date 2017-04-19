using System.Collections.Generic;
using Taw.Dcs.ScoreProcessor.Models.TableEntities;

namespace Taw.Dcs.ScoreProcessor.Engine
{
    public interface ILogLinesCleaner
    {
        IEnumerable<ScoreEvent> CleanScoreEvents(IEnumerable<ScoreEvent> scoreEvents, bool removeErrorLines, KillDuplicateAlgorithm killScoreAlgorithm);
        IEnumerable<ScoreEvent> ScrubDuplicateKills(IEnumerable<ScoreEvent> scoreEvents);
    }
}
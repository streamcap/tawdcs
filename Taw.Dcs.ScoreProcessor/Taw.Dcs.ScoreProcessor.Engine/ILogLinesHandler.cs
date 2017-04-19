using System.Collections.Generic;
using Taw.Dcs.ScoreProcessor.Models.TableEntities;

namespace Taw.Dcs.ScoreProcessor.Engine
{
    public interface IScoreEventsHandler
    {
        IList<ScoreEvent> GetCleanedScoreEvents(GameNameEntity entity, string dateFormat, bool removeErrorLines, KillDuplicateAlgorithm killScoreAlgorithm);
    }
}
using System.Collections.Generic;
using System.Linq;
using Taw.Dcs.ScoreProcessor.Models.TableEntities;
using Taw.Dcs.ScoreProcessor.Storage;

namespace Taw.Dcs.ScoreProcessor.Engine
{
    public class ScoreEventsHandler : IScoreEventsHandler
    {
        private readonly ITableStorageReadRepository _repository;
        private readonly ILogLinesCleaner _cleaner;

        public ScoreEventsHandler(ITableStorageReadRepository repository, ILogLinesCleaner cleaner)
        {
            _repository = repository;
            _cleaner = cleaner;
        }

        public IList<ScoreEvent> GetCleanedScoreEvents(GameNameEntity entity, string dateFormat, bool removeErrorLines, KillDuplicateAlgorithm killScoreAlgorithm)
        {
            var events = _repository.QueryScoreEvents(entity.GameName, entity.RunTime, dateFormat).ToList();
            events = _cleaner.CleanScoreEvents(events, removeErrorLines, killScoreAlgorithm).ToList();
            return events;
        }
    }
}
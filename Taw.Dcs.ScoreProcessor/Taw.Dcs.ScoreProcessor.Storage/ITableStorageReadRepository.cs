using System;
using System.Collections.Generic;
using Taw.Dcs.ScoreProcessor.Models.TableEntities;

namespace Taw.Dcs.ScoreProcessor.Storage
{
    public interface ITableStorageReadRepository
    {
        IEnumerable<string> GetLoglines(string partitionKey = null);
        IEnumerable<ScoreEvent> QueryScoreEvents(string gameName, DateTime gameDate, string dateFormat);
        IList<GameNameEntity> QueryGameNames();
    }
}
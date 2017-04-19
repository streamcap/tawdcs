using System.Collections.Generic;
using Taw.Dcs.ScoreProcessor.Models.TableEntities;

namespace Taw.Dcs.ScoreProcessor.Storage
{
    public interface ITableStorageWriteRepository
    {
        void InsertScoreEvents(IEnumerable<string> lines, char separator);
        void TryInsertGameName(ScoreEvent scoreEvent);
    }
}
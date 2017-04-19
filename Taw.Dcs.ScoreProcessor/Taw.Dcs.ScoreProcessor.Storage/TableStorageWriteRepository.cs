using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Table;
using Serilog;
using Taw.Dcs.ScoreProcessor.Models.TableEntities;

namespace Taw.Dcs.ScoreProcessor.Storage
{
    public class TableStorageWriteRepository : TableStorageRepository, ITableStorageWriteRepository
    {
        public TableStorageWriteRepository(ILogger logger)
            : base(logger)
        {
        }

        public void TryInsertGameName(ScoreEvent scoreEvent)
        {
            var query = new TableQuery<GameNameEntity>();
            if (Tables[GameNamesTableName].ExecuteQuery(query).ToList().Where(g => g.PartitionKey == scoreEvent.GameName).Any(g => g.RunTime.Date == scoreEvent.RunTime.Date))
            {
                return;
            }
            var op = TableOperation.Insert(new GameNameEntity(scoreEvent.GameName, scoreEvent.RunTime));
            Tables[GameNamesTableName].Execute(op);
        }

        public void InsertScoreEvents(IEnumerable<string> lines, char separator)
        {
            lines = CleanEventLines(lines);
            Logger.Verbose("Inserting a batch...");
            var batchEvents = lines
                .Select((line, index) => ScoreEvent.Create(line, separator))
                .ToList();
            batchEvents = batchEvents.Where(IsNotNullOrEmpty).ToList();
            RunInsertBatches(batchEvents, 100);
            Logger.Verbose("Batch inserted...");
        }

        private static bool IsNotNullOrEmpty(ScoreEvent b)
        {
            return b != null && b.Csv.Length > 0;
        }

        private void RunInsertBatches(ICollection<ScoreEvent> batchEvents, int batchSize)
        {
            var amountToRun = batchEvents.Count;
            var round = 0;
            do
            {
                var batchRound = batchEvents.Skip(round++ * batchSize).Take(batchSize).ToList();
                RunSingleInsertBatch(batchRound);
                amountToRun -= batchRound.Count;
            } while (amountToRun > 0);
        }

        private void RunSingleInsertBatch(IList<ScoreEvent> batchEvents)
        {
            var games = batchEvents.Select(b => b.PartitionKey).Distinct().ToList();
            if (games.Count > 1)
            {
                Logger.Error("För många partition keys!" + string.Join(";", games));
            }
            TryInsertGameName(batchEvents.First());
            var batchOp = new TableBatchOperation();
            foreach (var batchEvent in batchEvents)
            {
                batchOp.Insert(batchEvent);
                Logger.Verbose(
                    $"Inserting element {batchEvent.RowKey}, " +
                    $"csv length {batchEvent.Csv.Length} with separator {batchEvent.Separator} ...");
            }
            var task = Tables[EventsTableName].ExecuteBatchAsync(batchOp);
            task.Wait();
        }
    }
}
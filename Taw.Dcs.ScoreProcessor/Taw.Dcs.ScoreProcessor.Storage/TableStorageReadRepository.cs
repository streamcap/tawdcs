using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Table;
using Serilog;
using Taw.Dcs.ScoreProcessor.Models.TableEntities;

namespace Taw.Dcs.ScoreProcessor.Storage
{
    public class TableStorageReadRepository : TableStorageRepository, ITableStorageReadRepository
    {
        public TableStorageReadRepository(ILogger logger)
            : base(logger)
        {
            var a = QueryGameNames();
            if (a == null)
            {
                throw new Exception("Boom");
            }
        }

        public IEnumerable<string> GetLoglines(string partitionKey = null)
        {
            Logger.Verbose("Querying...");
            var query = new TableQuery<LogLineEntity>();
            if (partitionKey != null)
            {
                var condition = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey);
                query = query.Where(condition);
            }
            var result = Tables[EventsTableName].ExecuteQuery(query);
            var lines = result.Select(line => line.Line).ToList();
            return lines;
        }

        public IEnumerable<ScoreEvent> QueryScoreEvents(string gameName, DateTime gameDate, string dateFormat)
        {

            Logger.Verbose("Querying...");
            if (gameName == null)
            {
                throw new ArgumentNullException(nameof(gameName));
            }

            var partitionKey = gameName + ";" + gameDate.ToString(dateFormat);

            var condition = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey);
            var query = new TableQuery<ScoreEvent>().Where(condition);
            var result = Tables[EventsTableName].ExecuteQuery(query).ToList();
            return result;
        }

        public IList<GameNameEntity> QueryGameNames()
        {
            Logger.Verbose("Getting game names");

            var query = new TableQuery<GameNameEntity>();
            var table = Tables[GameNamesTableName];
            var result = table.ExecuteQuery(query).ToArray();
            return result.ToList();
        }
    }
}
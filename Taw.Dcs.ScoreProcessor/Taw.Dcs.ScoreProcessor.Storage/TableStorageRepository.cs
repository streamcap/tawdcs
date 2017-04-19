using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Serilog;

namespace Taw.Dcs.ScoreProcessor.Storage
{
    public abstract class TableStorageRepository
    {
        protected readonly IDictionary<string, CloudTable> Tables;
        protected readonly ILogger Logger;

        protected const string EventsTableName = "events";
        private const string LogsTableName = "logs";
        protected const string GameNamesTableName = "gamenames";

        protected TableStorageRepository(ILogger logger)
        {
            Logger = logger;
            Tables = GetTables(new[] { EventsTableName, LogsTableName, GameNamesTableName });
        }

        private IDictionary<string, CloudTable> GetTables(IEnumerable<string> names)
        {
            Logger.Verbose("Parsing connection string...");
            var account = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            var tableClient = account.CreateCloudTableClient();
            Logger.Verbose("Cloud client created...");
            Logger.Verbose("Getting table reference...");
            var tables = names.ToDictionary(n => n, n => tableClient.GetTableReference(n));
            foreach (var cloudTable in tables)
            {
                cloudTable.Value.CreateIfNotExists();
            }
            Logger.Information("Tables created...");
            return tables;
        }

        protected static IEnumerable<string> CleanEventLines(IEnumerable<string> lines)
        {
            var eventLines = lines
                .Where(line => !line.StartsWith("#"))
                .Select(line => line.Replace("\"", ""));
            return eventLines;
        }
    }
}
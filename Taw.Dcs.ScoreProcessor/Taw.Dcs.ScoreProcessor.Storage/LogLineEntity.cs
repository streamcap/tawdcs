using System;
using System.Globalization;
using Microsoft.WindowsAzure.Storage.Table;

namespace Taw.Dcs.ScoreProcessor.Storage
{
    class LogLineEntity : TableEntity
    {
        public string Line { get; }

        public LogLineEntity()
        {
        }

        public LogLineEntity(string line)
        {
            Line = line;
            PartitionKey = DateTime.Today.ToString(CultureInfo.InvariantCulture);
            RowKey = Guid.NewGuid().ToString();
        }
    }
}
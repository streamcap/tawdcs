using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Taw.Dcs.ScoreProcessor.Models.TableEntities
{
    public class GameNameEntity : TableEntity
    {
        public string GameName { get; set; }
        public DateTime RunTime { get; set; }

        public GameNameEntity() { }

        public GameNameEntity(string gameName, DateTime runtime)
        {
            PartitionKey = gameName;
            RowKey = runtime.ToShortDateString();
            GameName = gameName;
            RunTime = runtime;
        }
    }
}
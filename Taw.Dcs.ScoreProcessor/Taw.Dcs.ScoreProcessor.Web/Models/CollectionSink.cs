using System.Collections.Generic;
using Serilog.Core;
using Serilog.Events;

namespace Taw.Dcs.ScoreProcessor.Web.Models
{
    public class CollectionSink : ILogEventSink
    {
        public ICollection<LogEvent> Events { get; } = new List<LogEvent>();

        public void Emit(LogEvent le)
        {
            Events.Add(le);
        }


    }
}
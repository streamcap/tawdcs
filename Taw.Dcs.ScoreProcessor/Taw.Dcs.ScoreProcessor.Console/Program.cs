using System.Collections.Generic;
using System.IO;
using System.Linq;
using Serilog;
using Taw.Dcs.ScoreProcessor.Engine;
using Taw.Dcs.ScoreProcessor.Models.TableEntities;

namespace Taw.Dcs.ScoreProcessor.Console
{
    static class Program
    {
        static void Main()
        {
            var folder = new DirectoryInfo(@"C:\Temp\Csvs");
            ILogger logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
            foreach (var filePath in folder.GetFiles().Select(f => f.FullName))
            {
                RunFile(logger, filePath);
            }
        }

        private static void RunFile(ILogger logger, string filePath)
        {
            var parser = new FileParser(logger);
            IList<ScoreEvent> events;
            if (!parser.TryParseEventFile(filePath, ';', out events))
            {
                return;
            }
            events = new LogLinesCleaner(logger).CleanScoreEvents(events, true, KillScoreAlgorithm.DoNothing).ToList();
            var lines = ScoreInformationTextBuilder.GetAllReportLines(events);

            foreach (var line in lines)
            {
                System.Console.WriteLine(line);
            }
        }
    }
}

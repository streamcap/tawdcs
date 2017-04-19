using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Serilog;
using Taw.Dcs.ScoreProcessor.Models;
using Taw.Dcs.ScoreProcessor.Models.TableEntities;

namespace Taw.Dcs.ScoreProcessor.Engine
{
    public class FileParser
    {
        private readonly ILogger _logger;

        public FileParser(ILogger logger)
        {
            _logger = logger;
        }

        public bool TryParseEventFile(string filePath, char columnSeparator, out IList<ScoreEvent> events)
        {
            var parsedEvents = new List<ScoreEvent>();
            using (var reader = new StreamReader(filePath))
            {
                var lineCount = 0;
                while (!reader.EndOfStream)
                {
                    try
                    {
                        var line = reader.ReadLine();
                        if (line == null)
                        {
                            break;
                        }
                        line = line.Replace("\"", "");
                        if (!line.StartsWith("Game"))
                        {
                            parsedEvents.Add(ScoreEvent.Create(line, columnSeparator));
                        }
                        lineCount += 1;
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, $"Line {lineCount} could not be parsed. Check the file at {filePath}.");
                        events = null;
                        return false;
                    }
                }

            }
            events = parsedEvents.Where(line => Constants.Events.Contains(line.ScoreType)).ToList();
            return true;
        }
    }
}

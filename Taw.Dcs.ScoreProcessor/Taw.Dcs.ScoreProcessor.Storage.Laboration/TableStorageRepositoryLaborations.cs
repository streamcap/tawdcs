using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
using Taw.Dcs.ScoreProcessor.Models.TableEntities;

namespace Taw.Dcs.ScoreProcessor.Storage.Laboration
{
    [TestClass]
    public class TableStorageRepositoryLaborations
    {
        [TestMethod]
        public void RunFolder()
        {
            const char separator = ';';
            var folder = new DirectoryInfo(@"C:\temp\taw_events");
            ILogger logger = new LoggerConfiguration()
                .WriteTo.Console()
                .MinimumLevel.Information()
                .CreateLogger();
            var writeRepository = new TableStorageWriteRepository(logger);
            var readRepository = new TableStorageReadRepository(logger);
            foreach (var filePath in folder.GetFiles().Select(f => f.FullName))
            {
                logger.Information($"Running file {filePath}...");
                var lines = GetFileLines(filePath);
                if (AreFromDifferentGames(lines, separator))
                {
                    logger.Warning($"The file {filePath} has lines from several games. They must be divided to be processed. Skipping file...");
                    continue;
                }
                if (IsExistingGame(lines.First(), readRepository, separator))
                {
                    logger.Warning($"The game in file {filePath} has already been uploaded. Skipping file...");
                    continue;
                }
                var scoreEvents = lines.Select(l => ScoreEvent.Create(l, separator)).ToList();
                var errorEvents = scoreEvents.Where(e => !string.IsNullOrEmpty(e.Error)).ToList();
                if (errorEvents.Any())
                {
                    logger.Error($"Error in file {filePath}: {errorEvents.First().Error}");
                    continue;
                }
                WriteDataToTables(writeRepository, lines, separator, logger);
            }
        }

        private static void WriteDataToTables(ITableStorageWriteRepository writeRepository, IReadOnlyCollection<string> lines, char separator, ILogger logger)
        {
            var firstLine = ScoreEvent.Create(lines.First(), separator);
            logger.Information($"Writing >{firstLine.GameName}< game at {firstLine.RunTime}, error {firstLine.Error}...");
            if (!string.IsNullOrEmpty(firstLine.Error))
            {
                logger.Error("Wait!");
                return;
            }
            writeRepository.TryInsertGameName(firstLine);
            logger.Information($"Entering {lines.Count} lines into table...");
            writeRepository.InsertScoreEvents(lines, separator);
        }

        private static bool AreFromDifferentGames(IEnumerable<string> lines, char separator)
        {
            var scoreEvents = lines.Select(e => ScoreEvent.Create(e, separator))
                .Select(e => e.GameName + e.Error)
                .Distinct()
                .ToList();

            return scoreEvents.Count > 1;
        }

        private static bool IsExistingGame(string first, ITableStorageReadRepository readRepository, char separator)
        {
            var scoreEvent = ScoreEvent.Create(first, separator);
            return readRepository.QueryGameNames()
                .Any(game => game.GameName == scoreEvent.GameName && game.RunTime == scoreEvent.RunTime);
        }

        [TestMethod]
        public void InsertGameNames()
        {
            var game = new ScoreEvent
            {
                GameName = "TAW_Operation_VENDETTA",
                RunTime = new DateTime(2017, 04, 02)
            };
            ILogger logger = new LoggerConfiguration()
                .WriteTo.Console()
                .MinimumLevel.Information()
                .CreateLogger();
            var repository = new TableStorageWriteRepository(logger);
            repository.TryInsertGameName(game);
            logger.Information("Game name " + game.GameName + " added.");
        }

        private static List<string> GetFileLines(string filePath)
        {
            var lines = new List<string>();
            using (var reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    lines.Add(reader.ReadLine());
                }
            }
            return lines;
        }


        [TestMethod]
        public void TestDateTimeParse()
        {
            DateTime time;
            var timeToParse = "00:36:22";
            Assert.IsTrue(DateTime.TryParseExact(timeToParse, "HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out time));

        }
    }
}
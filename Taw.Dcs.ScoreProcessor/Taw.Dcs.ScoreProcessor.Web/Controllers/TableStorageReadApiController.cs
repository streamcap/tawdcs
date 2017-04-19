using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web.Http;
using Serilog;
using Taw.Dcs.ScoreProcessor.Engine;
using Taw.Dcs.ScoreProcessor.Models.TableEntities;
using Taw.Dcs.ScoreProcessor.Storage;

namespace Taw.Dcs.ScoreProcessor.Web.Controllers
{
    public abstract class TableStorageReadApiController : ApiController
    {
        protected ILogger Logger { get; }
        protected readonly ITableStorageReadRepository ReadRepository;
        protected readonly ILogLinesCleaner Cleaner;
        protected readonly string DateFormat;
        protected TableStorageReadApiController(ILogger logger, ITableStorageReadRepository readRepository, ILogLinesCleaner cleaner)
        {
            Logger = logger;
            ReadRepository = readRepository;
            Cleaner = cleaner;
            DateFormat = ConfigurationManager.AppSettings["DateFormat"];
        }

        protected IEnumerable<string> GetLogCollection()
        {
            return ReadRepository.GetLoglines();
        }

        private bool TryParseDate(string line, out DateTime time, out string name)
        {
            var parts = line.Split(';');
            name = parts[0];
            return DateTime.TryParseExact(parts[1], DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out time);
        }

        protected List<ScoreEvent> GetScoreEvents(string gameName)
        {
            DateTime time;
            string name;
            if (!TryParseDate(gameName, out time, out name))
            {
                throw new ArgumentException(nameof(gameName));
            }
            var gameEvents = ReadRepository.QueryScoreEvents(name, time, DateFormat).ToList();
            return gameEvents;
        }

    }
}
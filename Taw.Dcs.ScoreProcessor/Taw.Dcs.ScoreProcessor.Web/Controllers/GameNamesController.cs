using System.Linq;
using Serilog;
using Taw.Dcs.ScoreProcessor.Engine;
using Taw.Dcs.ScoreProcessor.Storage;

namespace Taw.Dcs.ScoreProcessor.Web.Controllers
{
    public class GameNamesController : TableStorageReadApiController
    {
        public string[] Get()
        {
            Logger.Verbose("Getting game names from table...");
            var games = ReadRepository.QueryGameNames().OrderBy(g => g.RunTime).ToList();
            var names = games.Select(e => e.GameName + "; " + e.RunTime.ToString(DateFormat)).ToArray();
            Logger.Verbose($"{names.Length} games returned...");
            return names;
        }

        public GameNamesController(ILogger logger, ITableStorageReadRepository readRepository, ILogLinesCleaner cleaner)
            : base(logger, readRepository, cleaner)
        {
        }
    }
}

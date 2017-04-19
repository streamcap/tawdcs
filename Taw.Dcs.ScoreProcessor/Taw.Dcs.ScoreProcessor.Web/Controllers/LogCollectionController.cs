using System.Linq;
using Serilog;
using Taw.Dcs.ScoreProcessor.Engine;
using Taw.Dcs.ScoreProcessor.Storage;

namespace Taw.Dcs.ScoreProcessor.Web.Controllers
{
    public class LogCollectionController
        : TableStorageReadApiController
    {
        public string[] Get()
        {
            return GetLogCollection().ToArray();
        }

        public LogCollectionController(ITableStorageReadRepository readRepository, ILogLinesCleaner cleaner, ILogger logger)
            : base(logger, readRepository, cleaner)
        {
        }
    }
}

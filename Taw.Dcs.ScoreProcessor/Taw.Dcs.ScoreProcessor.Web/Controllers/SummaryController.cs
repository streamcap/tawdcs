using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using Serilog;
using Taw.Dcs.ScoreProcessor.Engine;
using Taw.Dcs.ScoreProcessor.Storage;

namespace Taw.Dcs.ScoreProcessor.Web.Controllers
{
    public class SummaryController : TableStorageReadApiController
    {
        public string[] Post([FromBody]string gameName)
        {
            if (string.IsNullOrEmpty(gameName) || gameName.IndexOf(";", StringComparison.Ordinal) < 0)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
            var gameEvents = GetScoreEvents(gameName);
            var summary = ScoreInformationTextBuilder.GetTeamsSummaryLines(gameEvents).ToArray();
            return summary;
        }

        public SummaryController(ITableStorageReadRepository readRepository, ILogLinesCleaner cleaner, ILogger logger)
            : base(logger, readRepository, cleaner)
        {
        }
    }
}

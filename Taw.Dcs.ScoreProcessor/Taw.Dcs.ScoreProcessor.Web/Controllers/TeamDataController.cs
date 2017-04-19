using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web.Http;
using Serilog;
using Taw.Dcs.ScoreProcessor.Engine;
using Taw.Dcs.ScoreProcessor.Models.TableEntities;
using Taw.Dcs.ScoreProcessor.Storage;
using Taw.Dcs.ScoreProcessor.Web.Models;

namespace Taw.Dcs.ScoreProcessor.Web.Controllers
{
    public class TeamDataController : TableStorageReadApiController
    {
        public TeamDataController(ITableStorageReadRepository readRepository, ILogLinesCleaner cleaner, ILogger logger)
            : base(logger, readRepository, cleaner)
        {
        }

        public TeamData[] Post([FromBody]string gameName)
        {
            if (string.IsNullOrEmpty(gameName) || gameName.IndexOf(";", StringComparison.Ordinal) < 0)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
            var gameEvents = GetScoreEvents(gameName);
            gameEvents = FilterEvents(gameEvents).ToList();

            var redTeam = GetTeamData(gameEvents, "Red");
            var blueTeam = GetTeamData(gameEvents, "Blue");
            var teams = new[] { redTeam, blueTeam };
            return teams;
        }

        private static TeamData GetTeamData(IList<ScoreEvent> gameEvents, string teamName)
        {
            return new TeamData
            {
                Name = teamName,
                Tabs = new[] {
                    new Tab
                    {
                        Name= "Top list",
                        Items = ScoreInformationBuilder.GetToplistLines(gameEvents, teamName).ToArray()
                    },
                    new Tab
                    {
                        Name= "MVPs",
                        Items = ScoreInformationBuilder.GetMvps(gameEvents, teamName).ToArray()
                    },
                    new Tab
                    {
                        Name= "First kills",
                        Items  = ScoreInformationBuilder.GetFirstKills(gameEvents, teamName).ToArray()
                    },
                    new Tab
                    {
                        Name= "Trigger happy",
                        Items = ScoreInformationBuilder.GetTriggerHappies(gameEvents, teamName).Select(s=>s.ToString()).ToArray()
                    },
                    new Tab
                    {
                        Name = "Blue Falcons",
                        Items = ScoreInformationBuilder.GetBlueFalcons(gameEvents, teamName).Select(s=>s.ToString()).ToArray()
                    }
                }
            };
        }

        private IEnumerable<ScoreEvent> FilterEvents(IEnumerable<ScoreEvent> gameEvents)
        {
            int scoreAlgorithm;
            var appSettings = ConfigurationManager.AppSettings;
            if (!int.TryParse(appSettings["KillScoreAlgorithm"], out scoreAlgorithm))
            {
                scoreAlgorithm = 0;
            }
            var events = Cleaner.CleanScoreEvents(gameEvents, false, (KillDuplicateAlgorithm)scoreAlgorithm).ToList();
            return events.Where(e => !string.IsNullOrEmpty(e.PlayerName)).ToList();

        }
    }
}

using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Taw.Dcs.ScoreProcessor.Models.TableEntities;

namespace Taw.Dcs.ScoreProcessor.Engine.UnitTests
{
    [TestClass]
    public class ScoreInformationTextBuilderTests
    {
        private readonly List<ScoreEvent> _events;

        public ScoreInformationTextBuilderTests()
        {
            var redPenaltyEvent = new ScoreEvent { PlayerUnitCoalition = "red", ScoreType = "HIT_PENALTY" };
            var bluePenaltyEvent = new ScoreEvent { PlayerUnitCoalition = "blue", ScoreType = "HIT_PENALTY" };
            var redHitEvent = new ScoreEvent { PlayerUnitCoalition = "red", ScoreType = "HIT_SCORE" };
            var blueHitEvent = new ScoreEvent { PlayerUnitCoalition = "blue", ScoreType = "HIT_SCORE" };
            _events = new List<ScoreEvent>
            {
                redPenaltyEvent,
                bluePenaltyEvent,
                redHitEvent,
                blueHitEvent
            };
        }

        [TestMethod]
        public void TestGetTeamsSummaryLines()
        {
            var result = ScoreInformationTextBuilder.GetTeamsSummaryLines(_events);
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count());
        }

        [TestMethod]
        public void TestGetPenaltiesLines()
        {
            var redresult = ScoreInformationTextBuilder.GetPenaltiesLines(_events, "red");
            var blueResult = ScoreInformationTextBuilder.GetPenaltiesLines(_events, "blue");
            Assert.IsNotNull(redresult);
            Assert.IsNotNull(blueResult);
            Assert.AreEqual(1, redresult.Count(), "Not one line in red result");
            Assert.AreEqual(1, blueResult.Count(), "Not one line in blue result");
        }

        [TestMethod]
        public void TestGetTeamTopListLines()
        {
            var redresult = ScoreInformationTextBuilder.GetTeamTopListLines(_events, "red", true, true);
            var blueResult = ScoreInformationTextBuilder.GetTeamTopListLines(_events, "blue", true, true);
            Assert.IsNotNull(redresult);
            Assert.AreEqual(6, redresult.Count(), "not one line in red top list");
            Assert.IsNotNull(blueResult);
            Assert.AreEqual(6, blueResult.Count(), "not one line in blue top list");
        }

        [TestMethod]
        public void TestGetAllReportLines()
        {
            var result = ScoreInformationTextBuilder.GetAllReportLines(_events);
            Assert.IsNotNull(result);
            Assert.AreEqual(24, result.Count());
        }
    }
}
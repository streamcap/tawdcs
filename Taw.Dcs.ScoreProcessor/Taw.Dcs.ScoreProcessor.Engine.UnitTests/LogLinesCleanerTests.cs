using System.Linq;
using FakeItEasy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
using Taw.Dcs.ScoreProcessor.Models.TableEntities;

namespace Taw.Dcs.ScoreProcessor.Engine.UnitTests
{
    [TestClass]
    public class LogLinesCleanerTests
    {
        [TestMethod]
        public void ScrubDuplicateKillsTest()
        {
            var logger = A.Fake<ILogger>();
            var testLines = new[] {"A BRIDGE TOO FAR v4,17-03-26_18-58-55,00:37:34,TAW_AlephRo,TAW_SkyCap,HIT_SCORE,Blue,Plane,F-15C,Pilot #033,Red,Plane,Su-27,Pilot #002,1,1",
                "A BRIDGE TOO FAR v4,17-03-26_18-58-55,00:37:44,TAW_overdude,TAW_SkyCap,HIT_SCORE,Blue,Plane,F-15C,Pilot #034,Red,Plane,Su-27,Pilot #002,1,1",
                "A BRIDGE TOO FAR v4,17-03-26_18-58-55,00:37:45,TAW_AlephRo,TAW_SkyCap,DESTROY_SCORE,Blue,Plane,F-15C,Pilot #033,Red,Plane,Su-27,Pilot #002,1,5",
                "A BRIDGE TOO FAR v4,17-03-26_18-58-55,00:37:45,TAW_overdude,TAW_SkyCap,DESTROY_SCORE,Blue,Plane,F-15C,Pilot #034,Red,Plane,Su-27,Pilot #002,1,5",
                "A BRIDGE TOO FAR v4,17-03-26_18-58-55,00:37:46,TAW_AlephRo,TAW_overdude,DESTROY_SCORE,Blue,Plane,F-15C,Pilot #033,Red,Plane,Su-27,Pilot #034,1,5",
                "A BRIDGE TOO FAR v4,17-03-26_18-58-55,00:37:46,TAW_overdude,TAW_SkyCap,DESTROY_SCORE,Blue,Plane,F-15C,Pilot #034,Red,Plane,Su-27,Pilot #002,1,5"};
            var testEvents = testLines.Select(t => ScoreEvent.Create(t, ',')).ToList();
            var suspEvents = new LogLinesCleaner(logger).ScrubDuplicateKills(testEvents);
            suspEvents.Should().HaveCount(testLines.Length - 1);
        }
    }
}
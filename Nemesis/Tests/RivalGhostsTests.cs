using NUnit.Framework;
using Nemesis.Modules.RivalGhosts;

namespace Nemesis.Tests
{
    [TestFixture]
    public class RivalGhostsTests
    {
        [Test]
        public void Scorer_UsesConfiguredWeights()
        {
            var metrics = new RivalRunMetrics { Kills = 3, LootCollected = 2, RoomsCleared = 1, SessionsSurvived = 1, MonsterLootDrops = 4, ItemsSold = 5, Deaths = 1 };
            var profile = new RivalGhostScoreProfile { KillWeight = 100, LootWeight = 30, RoomClearWeight = 75, SurvivalWeight = 120, MonsterDropWeight = 20, SellWeight = 15, DeathPenalty = 25 };
            long score = RivalGhostScoring.ComputeScore(metrics, profile);
            Assert.AreEqual(3 * 100 + 2 * 30 + 1 * 75 + 1 * 120 + 4 * 20 + 5 * 15 - 25, score);
        }

        [Test]
        public void Selector_PicksClosestHigherScore()
        {
            var records = new[]
            {
                Record("Low", 100),
                Record("Mid", 250),
                Record("High", 600)
            };

            var selected = RivalGhostSelector.SelectTarget(records, 200);

            Assert.NotNull(selected);
            Assert.AreEqual("Mid", selected!.PlayerName);
        }

        [Test]
        public void Selector_FallsBackToHighestScoreWhenBeaten()
        {
            var records = new[]
            {
                Record("Low", 100),
                Record("Mid", 250),
                Record("High", 600)
            };

            var selected = RivalGhostSelector.SelectTarget(records, 700);

            Assert.NotNull(selected);
            Assert.AreEqual("High", selected!.PlayerName);
        }

        [Test]
        public void Challenge_BuildsHelpfulProgressLine()
        {
            var rival = Record("Mid", 250);
            var challenge = RivalGhostSelector.BuildChallenge(rival, 100);

            Assert.AreEqual("Mid", challenge.RivalName);
            Assert.AreEqual(0.4f, challenge.Progress, 0.0001f);
            Assert.IsTrue(challenge.StatusLine.Contains("points"));
        }

        private static RivalGhostRecord Record(string name, long score) => new RivalGhostRecord { PlayerName = name, CompositeScore = score, FinishedUtcSeconds = 1 };
    }
}

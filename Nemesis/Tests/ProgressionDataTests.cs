using NUnit.Framework;
using Nemesis.Modules.PersistentProgression;

namespace Nemesis.Tests
{
    [TestFixture]
    public class ProgressionDataTests
    {
        [Test]
        public void GetOrCreate_NewKey_CreatesEntry()
        {
            var data = new ProgressionData();
            var prog = data.GetOrCreate("player1");

            Assert.IsNotNull(prog);
            Assert.AreEqual(1, prog.Level);
            Assert.AreEqual(0, prog.XP);
            Assert.AreEqual(1, data.Players.Count);
        }

        [Test]
        public void GetOrCreate_ExistingKey_ReturnsSameInstance()
        {
            var data = new ProgressionData();
            var prog1 = data.GetOrCreate("player1");
            prog1.XP = 500;
            prog1.Level = 5;

            var prog2 = data.GetOrCreate("player1");
            Assert.AreSame(prog1, prog2);
            Assert.AreEqual(500, prog2.XP);
            Assert.AreEqual(5, prog2.Level);
        }

        [Test]
        public void GetOrCreate_DifferentKeys_DifferentInstances()
        {
            var data = new ProgressionData();
            var prog1 = data.GetOrCreate("player1");
            var prog2 = data.GetOrCreate("player2");

            Assert.AreNotSame(prog1, prog2);
            Assert.AreEqual(2, data.Players.Count);
        }

        [Test]
        public void NewPlayerProgression_HasDefaults()
        {
            var prog = new PlayerProgression();
            Assert.AreEqual("", prog.PlayerName);
            Assert.AreEqual(1, prog.Level);
            Assert.AreEqual(0, prog.XP);
            Assert.AreEqual(0, prog.TotalKills);
            Assert.AreEqual(0, prog.TotalLootCollected);
            Assert.AreEqual(0, prog.TotalRoomsCleared);
            Assert.AreEqual(0, prog.TotalSessionsSurvived);
        }

        [Test]
        public void NewProgressionData_HasEmptyPlayers()
        {
            var data = new ProgressionData();
            Assert.IsNotNull(data.Players);
            Assert.AreEqual(0, data.Players.Count);
        }
    }
}

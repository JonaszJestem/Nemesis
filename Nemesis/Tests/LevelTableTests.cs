using NUnit.Framework;
using Nemesis.Modules.PersistentProgression;

namespace Nemesis.Tests
{
    [TestFixture]
    public class LevelTableTests
    {
        [Test]
        public void Level1_RequiresZeroXP()
        {
            Assert.AreEqual(0, LevelTable.XPForLevel(1));
        }

        [Test]
        public void Level0_RequiresZeroXP()
        {
            Assert.AreEqual(0, LevelTable.XPForLevel(0));
        }

        [Test]
        public void NegativeLevel_RequiresZeroXP()
        {
            Assert.AreEqual(0, LevelTable.XPForLevel(-5));
        }

        [Test]
        public void HigherLevels_RequireMoreXP()
        {
            long prev = 0;
            for (int level = 2; level <= 50; level++)
            {
                long xp = LevelTable.XPForLevel(level);
                Assert.Greater(xp, prev, $"Level {level} should require more XP than level {level - 1}");
                prev = xp;
            }
        }

        [Test]
        public void ComputeLevel_ZeroXP_ReturnsLevel1()
        {
            Assert.AreEqual(1, LevelTable.ComputeLevel(0, 50));
        }

        [Test]
        public void ComputeLevel_ExactBoundary_ReturnsCorrectLevel()
        {
            for (int level = 1; level <= 20; level++)
            {
                long xp = LevelTable.XPForLevel(level);
                Assert.AreEqual(level, LevelTable.ComputeLevel(xp, 50),
                    $"Exact XP for level {level} should return level {level}");
            }
        }

        [Test]
        public void ComputeLevel_OneBelowBoundary_ReturnsPreviousLevel()
        {
            for (int level = 2; level <= 20; level++)
            {
                long xp = LevelTable.XPForLevel(level) - 1;
                Assert.AreEqual(level - 1, LevelTable.ComputeLevel(xp, 50),
                    $"XP one below level {level} threshold should return level {level - 1}");
            }
        }

        [Test]
        public void ComputeLevel_CappedAtMax()
        {
            int maxLevel = 10;
            long hugeXP = 999999999;
            Assert.AreEqual(maxLevel, LevelTable.ComputeLevel(hugeXP, maxLevel));
        }

        [Test]
        public void ComputeLevel_MaxLevel1_AlwaysReturns1()
        {
            Assert.AreEqual(1, LevelTable.ComputeLevel(999999, 1));
        }

        [Test]
        public void HpBonus_Level1_Returns1()
        {
            Assert.AreEqual(1.0f, LevelTable.GetHpBonus(1, 0.02f), 0.001f);
        }

        [Test]
        public void HpBonus_Level10_Returns1_18()
        {
            Assert.AreEqual(1.18f, LevelTable.GetHpBonus(10, 0.02f), 0.001f);
        }

        [Test]
        public void HpBonus_ZeroBonusPerLevel_AlwaysReturns1()
        {
            Assert.AreEqual(1.0f, LevelTable.GetHpBonus(50, 0f), 0.001f);
        }

        [Test]
        public void SpeedBonus_Level20_Returns1_095()
        {
            Assert.AreEqual(1.095f, LevelTable.GetSpeedBonus(20, 0.005f), 0.001f);
        }

        [Test]
        public void SpeedBonus_Level1_Returns1()
        {
            Assert.AreEqual(1.0f, LevelTable.GetSpeedBonus(1, 0.005f), 0.001f);
        }

        [Test]
        public void XPToNextLevel_AlwaysHigherThanCurrentLevel()
        {
            for (int level = 1; level < 50; level++)
            {
                long next = LevelTable.XPToNextLevel(level);
                long current = LevelTable.XPForLevel(level);
                Assert.Greater(next, current, $"At level {level}");
            }
        }

        [Test]
        public void CustomBaseXP_AffectsThresholds()
        {
            long defaultXP = LevelTable.XPForLevel(5, 100, 1.5f);
            long higherBase = LevelTable.XPForLevel(5, 200, 1.5f);
            Assert.AreEqual(higherBase, defaultXP * 2);
        }

        [Test]
        public void CustomExponent_AffectsScaling()
        {
            long linear = LevelTable.XPForLevel(10, 100, 1.0f);
            long exponential = LevelTable.XPForLevel(10, 100, 2.0f);
            Assert.Greater(exponential, linear);
        }

        [Test]
        public void ComputeLevel_WithCustomParams()
        {
            long xp = LevelTable.XPForLevel(5, 200, 2.0f);
            int level = LevelTable.ComputeLevel(xp, 50, 200, 2.0f);
            Assert.AreEqual(5, level);
        }
    }
}

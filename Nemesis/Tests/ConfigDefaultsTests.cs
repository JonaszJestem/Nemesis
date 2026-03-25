using NUnit.Framework;
using Nemesis.Modules.DifficultyDirector;
using Nemesis.Modules.RoleSystem;
using Nemesis.Modules.ProximityRadar;
using Nemesis.Modules.PersistentProgression;

namespace Nemesis.Tests
{
    [TestFixture]
    public class ConfigDefaultsTests
    {
        [Test]
        public void DifficultyConfig_DefaultsAreSensible()
        {
            var cfg = new DifficultyConfig();
            Assert.IsTrue(cfg.Enabled);
            Assert.IsTrue(cfg.ShowHudLabel);
            Assert.Greater(cfg.UpdateIntervalSeconds, 0f);
            Assert.Greater(cfg.PlayerCountWeight, 0f);
            Assert.Greater(cfg.PlayerCountMax, 0);
            Assert.Greater(cfg.GameDayMax, 0);
            Assert.Greater(cfg.SessionCycleMax, 0);
            Assert.Less(cfg.MinMultiplier, cfg.MaxMultiplier);
            Assert.GreaterOrEqual(cfg.MinMultiplier, 0.1f);
            Assert.IsTrue(cfg.ScaleMonsterHp);
            Assert.IsTrue(cfg.ScaleMonsterAtk);
        }

        [Test]
        public void RoleConfig_DefaultsAreSensible()
        {
            var cfg = new RoleConfig();
            Assert.IsTrue(cfg.Enabled);
            Assert.IsTrue(cfg.ShowRoleHud);
            Assert.Greater(cfg.ScoutSpeedMultiplier, 1.0f);
            Assert.Greater(cfg.TankHpMultiplier, 1.0f);
            Assert.LessOrEqual(cfg.TankSpeedPenalty, 1.0f);
            Assert.Greater(cfg.MedicInteractMultiplier, 1.0f);
            Assert.Greater(cfg.ScavengerLootMultiplier, 1.0f);
        }

        [Test]
        public void RadarConfig_DefaultsAreSensible()
        {
            var cfg = new RadarConfig();
            Assert.IsTrue(cfg.Enabled);
            Assert.Greater(cfg.Range, 0f);
            Assert.Greater(cfg.RadarSize, 0);
            Assert.Greater(cfg.RadarOpacity, 0f);
            Assert.LessOrEqual(cfg.RadarOpacity, 1.0f);
            Assert.Greater(cfg.UpdateRate, 0f);
            Assert.IsTrue(cfg.ShowMonsters);
            Assert.IsTrue(cfg.ShowLoot);
            Assert.IsTrue(cfg.ShowPlayers);
            Assert.Greater(cfg.MonsterDotSize, 0);
            Assert.Greater(cfg.LootDotSize, 0);
            Assert.Greater(cfg.PlayerDotSize, 0);
        }

        [Test]
        public void ProgressionConfig_DefaultsAreSensible()
        {
            var cfg = new ProgressionConfig();
            Assert.IsTrue(cfg.Enabled);
            Assert.IsTrue(cfg.ShowXpBar);
            Assert.Greater(cfg.KillXP, 0);
            Assert.Greater(cfg.LootCollectedXP, 0);
            Assert.Greater(cfg.RoomClearedXP, 0);
            Assert.Greater(cfg.SessionSurvivedXP, 0);
            Assert.Greater(cfg.MaxLevel, 1);
            Assert.Greater(cfg.BaseXPPerLevel, 0);
            Assert.Greater(cfg.XPScalingExponent, 0f);
            Assert.Greater(cfg.HpBonusPerLevel, 0f);
            Assert.Greater(cfg.SpeedBonusPerLevel, 0f);
            Assert.Greater(cfg.SaveIntervalSeconds, 0f);
            Assert.Greater(cfg.XpBarWidth, 0);
        }

        [Test]
        public void DifficultyConfig_WeatherThreshold_WithinMultiplierRange()
        {
            var cfg = new DifficultyConfig();
            Assert.GreaterOrEqual(cfg.WeatherThreshold, cfg.MinMultiplier);
            Assert.LessOrEqual(cfg.WeatherThreshold, cfg.MaxMultiplier);
        }
    }
}

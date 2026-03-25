using NUnit.Framework;
using Nemesis.Modules.DifficultyDirector;
using Nemesis.Modules.RoleSystem;
using Nemesis.Modules.ProximityRadar;
using Nemesis.Modules.PersistentProgression;
using Nemesis.Modules.DamageScale;
using Nemesis.Modules.MoreMimics;
using Nemesis.Modules.Stamina;
using Nemesis.Modules.Jump;
using Nemesis.Modules.Fullbright;
using Nemesis.Modules.Fov;
using Nemesis.Modules.VoiceFix;
using Nemesis.Modules.HealthIndicators;
using Nemesis.Modules.TooltipMod;
using Nemesis.Modules.Esp;
using Nemesis.Modules.EnemyDropLoot;
using Nemesis.Modules.MoreVoices;
using Nemesis.Modules.NoiseDirector;
using Nemesis.Modules.Marker;
using Nemesis.Modules.InventoryExpansion;
using Nemesis.Modules.AutoLoot;
using Nemesis.Modules.Fly;
using Nemesis.Modules.RunMutatorDraft;
using Nemesis.Modules.TramTalentTree;
using Nemesis.Modules.ContractBoard;
using Nemesis.Modules.PossessionPlus;
using Nemesis.Modules.RivalGhosts;

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
            Assert.Greater(cfg.SellXP, 0);
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

        [Test]
        public void DamageScaleConfig_DefaultsAreSensible()
        {
            var cfg = new DamageScaleConfig();
            Assert.IsFalse(cfg.Enabled);
            Assert.Greater(cfg.DamageMultiplier, 0f);
        }

        [Test]
        public void MoreMimicsConfig_DefaultsAreSensible()
        {
            var cfg = new MoreMimicsConfig();
            Assert.IsFalse(cfg.Enabled);
            Assert.Greater(cfg.SpawnRateMultiplier, 0f);
        }

        [Test]
        public void StaminaConfig_DefaultsAreSensible()
        {
            var cfg = new StaminaConfig();
            Assert.IsFalse(cfg.Enabled);
        }

        [Test]
        public void JumpConfig_DefaultsAreSensible()
        {
            var cfg = new JumpConfig();
            Assert.IsFalse(cfg.Enabled);
            Assert.Greater(cfg.JumpVelocity, 0f);
        }

        [Test]
        public void FullbrightConfig_DefaultsAreSensible()
        {
            var cfg = new FullbrightConfig();
            Assert.IsFalse(cfg.Enabled);
            Assert.Greater(cfg.AmbientIntensity, 0f);
        }

        [Test]
        public void FovConfig_DefaultsAreSensible()
        {
            var cfg = new FovConfig();
            Assert.IsFalse(cfg.Enabled);
            Assert.Greater(cfg.FieldOfView, 0f);
        }

        [Test]
        public void VoiceFixConfig_DefaultsAreSensible()
        {
            var cfg = new VoiceFixConfig();
            Assert.IsFalse(cfg.Enabled);
        }

        [Test]
        public void HealthIndicatorsConfig_DefaultsAreSensible()
        {
            var cfg = new HealthIndicatorsConfig();
            Assert.IsTrue(cfg.Enabled);
            Assert.IsTrue(cfg.ShowDamageNumbers);
            Assert.IsTrue(cfg.ShowHealthBars);
            Assert.Greater(cfg.DamageNumberScale, 0f);
        }

        [Test]
        public void TooltipConfig_DefaultsAreSensible()
        {
            var cfg = new TooltipConfig();
            Assert.IsTrue(cfg.Enabled);
            Assert.Greater(cfg.FontSize, 0);
        }

        [Test]
        public void EspConfig_DefaultsAreSensible()
        {
            var cfg = new EspConfig();
            Assert.IsFalse(cfg.Enabled);
            Assert.IsTrue(cfg.ShowDistance);
            Assert.Greater(cfg.MaxRange, 0f);
            Assert.IsTrue(cfg.ShowMonsters);
            Assert.IsTrue(cfg.ShowLoot);
            Assert.IsTrue(cfg.ShowPlayers);
        }

        [Test]
        public void LootDropConfig_DefaultsAreSensible()
        {
            var cfg = new LootDropConfig();
            Assert.IsFalse(cfg.Enabled);
            Assert.Greater(cfg.DropChance, 0f);
            Assert.LessOrEqual(cfg.DropChance, 1.0f);
            Assert.Greater(cfg.MaxDropsPerKill, 0);
        }

        [Test]
        public void MoreVoicesConfig_DefaultsAreSensible()
        {
            var cfg = new MoreVoicesConfig();
            Assert.IsFalse(cfg.Enabled);
            Assert.Greater(cfg.MaxRecordings, 0);
        }

        [Test]
        public void NoiseDirectorConfig_DefaultsAreSensible()
        {
            var cfg = new NoiseDirectorConfig();
            Assert.IsFalse(cfg.Enabled);
            Assert.Greater(cfg.GlobalVolumeMultiplier, 0f);
            Assert.Greater(cfg.AmbientVolumeMultiplier, 0f);
            Assert.Greater(cfg.EffectsVolumeMultiplier, 0f);
            Assert.Greater(cfg.VoiceVolumeMultiplier, 0f);
            Assert.Greater(cfg.UpdateIntervalSeconds, 0f);
            Assert.IsFalse(cfg.AffectUiAudio);
            Assert.IsFalse(cfg.AffectMusic);
        }

        [Test]
        public void MarkerConfig_DefaultsAreSensible()
        {
            var cfg = new MarkerConfig();
            Assert.IsFalse(cfg.Enabled);
            Assert.IsTrue(cfg.InfinitePaintballs);
            Assert.IsTrue(cfg.PermanentMarks);
            Assert.IsTrue(cfg.EnableColorCycling);
        }

        [Test]
        public void InventoryExpansionConfig_DefaultsAreSensible()
        {
            var cfg = new InventoryExpansionConfig();
            Assert.IsFalse(cfg.Enabled);
            Assert.Greater(cfg.AdditionalSlots, 0);
        }

        [Test]
        public void AutoLootConfig_DefaultsAreSensible()
        {
            var cfg = new AutoLootConfig();
            Assert.IsFalse(cfg.Enabled);
            Assert.Greater(cfg.PickupRange, 0f);
        }

        [Test]
        public void FlyConfig_DefaultsAreSensible()
        {
            var cfg = new FlyConfig();
            Assert.IsFalse(cfg.Enabled);
            Assert.Greater(cfg.FlySpeed, 0f);
        }

        [Test]
        public void RunMutatorDraftConfig_DefaultsAreSensible()
        {
            var cfg = new RunMutatorDraftConfig();
            Assert.IsFalse(cfg.Enabled);
            Assert.IsTrue(cfg.ShowHud);
            Assert.Greater(cfg.DraftChoiceCount, 0);
            Assert.Greater(cfg.ActiveMutatorCount, 0);
            Assert.Greater(cfg.RefreshIntervalSeconds, 0f);
            Assert.Greater(cfg.SpawnPressureMultiplier, 0f);
            Assert.Greater(cfg.NoiseLeakMultiplier, 0f);
            Assert.Greater(cfg.NoiseDecayMultiplier, 0f);
            Assert.Greater(cfg.HybridMultiplier, 0f);
        }

        [Test]
        public void TramTalentTreeConfig_DefaultsAreSensible()
        {
            var cfg = new TramTalentTreeConfig();
            Assert.IsFalse(cfg.Enabled);
            Assert.IsTrue(cfg.ShowHud);
            Assert.GreaterOrEqual(cfg.StartingPoints, 0);
            Assert.Greater(cfg.MaxRankPerNode, 0);
            Assert.Greater(cfg.SpeedBonusPerRank, 0f);
            Assert.Greater(cfg.NoiseReductionPerRank, 0f);
            Assert.Greater(cfg.RewardBonusPerRank, 0f);
            Assert.Greater(cfg.CapacityBonusPerRank, 0);
            Assert.Greater(cfg.DamageReductionPerRank, 0f);
        }

        [Test]
        public void ContractBoardConfig_DefaultsAreSensible()
        {
            var cfg = new ContractBoardConfig();
            Assert.IsFalse(cfg.Enabled);
            Assert.IsTrue(cfg.ShowHud);
            Assert.GreaterOrEqual(cfg.StartingActiveContracts, 0);
            Assert.Greater(cfg.MaxContractsPerRun, 0);
            Assert.Greater(cfg.RefreshIntervalSeconds, 0f);
            Assert.GreaterOrEqual(cfg.TargetVariancePercent, 0f);
            Assert.GreaterOrEqual(cfg.RewardVariancePercent, 0f);
            Assert.GreaterOrEqual(cfg.TargetRampPerIssuedContract, 0f);
            Assert.Greater(cfg.RewardMultiplier, 0f);
        }

        [Test]
        public void PossessionPlusConfig_DefaultsAreSensible()
        {
            var cfg = new PossessionPlusConfig();
            Assert.IsFalse(cfg.Enabled);
            Assert.IsTrue(cfg.ShowHud);
            Assert.Greater(cfg.MaxCharges, 0);
            Assert.Greater(cfg.BaseRechargeSeconds, 0f);
            Assert.Greater(cfg.MinimumRechargeSeconds, 0f);
            Assert.Greater(cfg.GhostLifetimeSeconds, 0f);
            Assert.Greater(cfg.PulseRadius, 0f);
            Assert.Greater(cfg.MaxMarksPerPulse, 0);
            Assert.Greater(cfg.MarkLifetimeSeconds, 0f);
        }

        [Test]
        public void RivalGhostsConfig_DefaultsAreSensible()
        {
            var cfg = new RivalGhostsConfig();
            Assert.IsFalse(cfg.Enabled);
            Assert.IsTrue(cfg.ShowHud);
            Assert.Greater(cfg.StorageLimit, 0);
            Assert.Greater(cfg.TopRecordsToShow, 0);
            Assert.Greater(cfg.AutoSaveIntervalSeconds, 0f);
            Assert.Greater(cfg.KillWeight, 0);
            Assert.GreaterOrEqual(cfg.DeathPenalty, 0);
            Assert.GreaterOrEqual(cfg.RivalLeadThreshold, 0f);
        }
    }
}

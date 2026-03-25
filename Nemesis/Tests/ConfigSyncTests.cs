using NUnit.Framework;
using Nemesis.Config;
using Nemesis.Core;
using Nemesis.Modules.DamageScale;
using Nemesis.Modules.DifficultyDirector;
using Nemesis.Modules.EnemyDropLoot;
using Nemesis.Modules.Fly;
using Nemesis.Modules.Fullbright;
using Nemesis.Modules.InventoryExpansion;
using Nemesis.Modules.Jump;
using Nemesis.Modules.Marker;
using Nemesis.Modules.MoreMimics;
using Nemesis.Modules.MoreVoices;
using Nemesis.Modules.NoiseDirector;
using Nemesis.Modules.ContractBoard;
using Nemesis.Modules.PersistentProgression;
using Nemesis.Modules.RoleSystem;
using Nemesis.Modules.RunMutatorDraft;
using Nemesis.Modules.AutoLoot;
using Nemesis.Modules.Stamina;
using Nemesis.Modules.TramTalentTree;
using Nemesis.Modules.PossessionPlus;
using Nemesis.Modules.RivalGhosts;

namespace Nemesis.Tests
{
    [TestFixture]
    public class ConfigSyncTests
    {
        /// <summary>
        /// The core test: serialize a HostConfig with non-default values,
        /// round-trip through JSON, apply to a SuiteConfig, and verify ALL fields survived.
        /// This automatically catches any new config fields that aren't serialized.
        /// </summary>
        [Test]
        public void HostConfig_RoundTrip_PreservesAllFields()
        {
            var source = new HostConfig
            {
                Difficulty = new DifficultyConfig
                {
                    Enabled = false, ShowHudLabel = false,
                    PlayerCountWeight = 77f, GameDayWeight = 33f, SessionCycleWeight = 11f,
                    PlayerCountMax = 42, GameDayMax = 99, SessionCycleMax = 7,
                    MinMultiplier = 0.3f, MaxMultiplier = 4.5f, UpdateIntervalSeconds = 30f,
                    ScaleMonsterHp = false, ScaleMonsterAtk = false,
                    WeatherEscalation = false, WeatherThreshold = 3.5f, StormWeatherId = 5
                },
                Roles = new RoleConfig
                {
                    Enabled = false, ShowRoleHud = false,
                    ScoutSpeedMultiplier = 1.5f, TankHpMultiplier = 2.0f,
                    TankSpeedPenalty = 0.8f, MedicInteractMultiplier = 1.75f,
                    ScavengerLootMultiplier = 1.9f
                },
                DamageScale = new DamageScaleConfig { Enabled = true, DamageMultiplier = 0.3f },
                MoreMimics = new MoreMimicsConfig { Enabled = true, SpawnRateMultiplier = 4.0f },
                Stamina = new StaminaConfig { Enabled = true },
                Jump = new JumpConfig { Enabled = true, JumpVelocity = 12.0f },
                Fullbright = new FullbrightConfig { Enabled = true, AmbientIntensity = 2.5f },
                LootDrop = new LootDropConfig { Enabled = true, DropChance = 0.8f, MaxDropsPerKill = 5 },
                MoreVoices = new MoreVoicesConfig { Enabled = true, MaxRecordings = 25 },
                NoiseDirector = new NoiseDirectorConfig
                {
                    Enabled = true,
                    GlobalVolumeMultiplier = 1.2f,
                    AmbientVolumeMultiplier = 0.7f,
                    EffectsVolumeMultiplier = 1.1f,
                    VoiceVolumeMultiplier = 0.9f,
                    UpdateIntervalSeconds = 1.25f,
                    AffectUiAudio = true,
                    AffectMusic = true
                },
                RunMutatorDraft = new RunMutatorDraftConfig
                {
                    Enabled = true,
                    ShowHud = false,
                    AutoPickBestAvailable = false,
                    DraftChoiceCount = 5,
                    ActiveMutatorCount = 3,
                    RefreshIntervalSeconds = 2.5f,
                    DefaultPreferredWeatherId = 8,
                    SpawnPressureMultiplier = 1.4f,
                    NoiseLeakMultiplier = 1.3f,
                    NoiseDecayMultiplier = 0.85f,
                    HybridMultiplier = 1.2f
                },
                TramTalentTree = new TramTalentTreeConfig
                {
                    Enabled = true,
                    ShowHud = false,
                    ResetOnSessionStart = false,
                    StartingPoints = 7,
                    MaxRankPerNode = 6,
                    SpeedBonusPerRank = 0.08f,
                    NoiseReductionPerRank = 0.09f,
                    RewardBonusPerRank = 0.12f,
                    CapacityBonusPerRank = 2,
                    DamageReductionPerRank = 0.06f
                },
                ContractBoard = new ContractBoardConfig
                {
                    Enabled = true,
                    ShowHud = false,
                    ResetOnSessionStart = false,
                    StartingActiveContracts = 4,
                    MaxContractsPerRun = 9,
                    ReplaceCompletedContracts = false,
                    AllowDuplicateObjectiveKinds = true,
                    RefreshIntervalSeconds = 1.2f,
                    TargetVariancePercent = 0.3f,
                    RewardVariancePercent = 0.25f,
                    TargetRampPerIssuedContract = 0.1f,
                    RewardMultiplier = 1.7f,
                    RewardBonusPoints = 3
                },
                PossessionPlus = new PossessionPlusConfig
                {
                    Enabled = true,
                    ShowHud = false,
                    ShareWithLobby = false,
                    AutoPulseOnDeath = true,
                    MaxCharges = 5,
                    BaseRechargeSeconds = 18f,
                    AllyRechargeBonusSeconds = 3f,
                    MinimumRechargeSeconds = 7f,
                    AllySupportRadius = 20f,
                    PulseCooldownSeconds = 8f,
                    GhostLifetimeSeconds = 210f,
                    PulseRadius = 35f,
                    MaxMarksPerPulse = 6,
                    MarkLifetimeSeconds = 18f,
                    RemoteBroadcastLifetimeSeconds = 24f
                },
                RivalGhosts = new RivalGhostsConfig
                {
                    Enabled = true,
                    ShowHud = false,
                    StorageLimit = 80,
                    TopRecordsToShow = 5,
                    AutoSaveIntervalSeconds = 30f,
                    KillWeight = 140,
                    LootWeight = 45,
                    RoomClearWeight = 90,
                    SurvivalWeight = 150,
                    MonsterDropWeight = 40,
                    SellWeight = 25,
                    DeathPenalty = 35,
                    RivalLeadThreshold = 0.2f
                },
                Marker = new MarkerConfig { Enabled = true, InfinitePaintballs = false, PermanentMarks = false, EnableColorCycling = false },
                InventoryExpansion = new InventoryExpansionConfig { Enabled = true, AdditionalSlots = 8 },
                AutoLoot = new AutoLootConfig { Enabled = true, PickupRange = 15.0f },
                Fly = new FlyConfig { Enabled = true, FlySpeed = 25.0f },
                Progression = new ProgressionConfig
                {
                    Enabled = false, ShowXpBar = false,
                    KillXP = 50, LootCollectedXP = 20, SellXP = 30,
                    RoomClearedXP = 100, SessionSurvivedXP = 200, MonsterLootDropXP = 35,
                    ScaleWithDifficulty = false,
                    HpBonusPerLevel = 0.05f, SpeedBonusPerLevel = 0.01f,
                    MaxLevel = 100, BaseXPPerLevel = 200, XPScalingExponent = 2.0f,
                    SaveIntervalSeconds = 120f,
                    XpBarX = 20, XpBarYFromBottom = 90, XpBarWidth = 200
                }
            };

            var result = ConfigSyncLogic.RoundTrip(source);

            // Difficulty
            Assert.AreEqual(false, result.Difficulty.Enabled);
            Assert.AreEqual(false, result.Difficulty.ShowHudLabel);
            Assert.AreEqual(77f, result.Difficulty.PlayerCountWeight);
            Assert.AreEqual(0.3f, result.Difficulty.MinMultiplier);
            Assert.AreEqual(4.5f, result.Difficulty.MaxMultiplier);
            Assert.AreEqual(false, result.Difficulty.ScaleMonsterHp);
            Assert.AreEqual(5, result.Difficulty.StormWeatherId);

            // Roles
            Assert.AreEqual(false, result.Roles.Enabled);
            Assert.AreEqual(1.5f, result.Roles.ScoutSpeedMultiplier);
            Assert.AreEqual(2.0f, result.Roles.TankHpMultiplier);

            // DamageScale
            Assert.AreEqual(true, result.DamageScale.Enabled);
            Assert.AreEqual(0.3f, result.DamageScale.DamageMultiplier, 0.001f);

            // MoreMimics
            Assert.AreEqual(true, result.MoreMimics.Enabled);
            Assert.AreEqual(4.0f, result.MoreMimics.SpawnRateMultiplier);

            // Stamina
            Assert.AreEqual(true, result.Stamina.Enabled);

            // Jump
            Assert.AreEqual(true, result.Jump.Enabled);
            Assert.AreEqual(12.0f, result.Jump.JumpVelocity);

            // Fullbright
            Assert.AreEqual(true, result.Fullbright.Enabled);
            Assert.AreEqual(2.5f, result.Fullbright.AmbientIntensity);

            // LootDrop
            Assert.AreEqual(true, result.LootDrop.Enabled);
            Assert.AreEqual(0.8f, result.LootDrop.DropChance, 0.001f);
            Assert.AreEqual(5, result.LootDrop.MaxDropsPerKill);

            // MoreVoices
            Assert.AreEqual(true, result.MoreVoices.Enabled);
            Assert.AreEqual(25, result.MoreVoices.MaxRecordings);

            // NoiseDirector
            Assert.AreEqual(true, result.NoiseDirector.Enabled);
            Assert.AreEqual(1.2f, result.NoiseDirector.GlobalVolumeMultiplier);
            Assert.AreEqual(0.7f, result.NoiseDirector.AmbientVolumeMultiplier);
            Assert.AreEqual(1.1f, result.NoiseDirector.EffectsVolumeMultiplier);
            Assert.AreEqual(0.9f, result.NoiseDirector.VoiceVolumeMultiplier);
            Assert.AreEqual(1.25f, result.NoiseDirector.UpdateIntervalSeconds);
            Assert.AreEqual(true, result.NoiseDirector.AffectUiAudio);
            Assert.AreEqual(true, result.NoiseDirector.AffectMusic);

            // RunMutatorDraft
            Assert.AreEqual(true, result.RunMutatorDraft.Enabled);
            Assert.AreEqual(5, result.RunMutatorDraft.DraftChoiceCount);
            Assert.AreEqual(8, result.RunMutatorDraft.DefaultPreferredWeatherId);
            Assert.AreEqual(1.3f, result.RunMutatorDraft.NoiseLeakMultiplier);

            // TramTalentTree
            Assert.AreEqual(true, result.TramTalentTree.Enabled);
            Assert.AreEqual(7, result.TramTalentTree.StartingPoints);
            Assert.AreEqual(6, result.TramTalentTree.MaxRankPerNode);
            Assert.AreEqual(2, result.TramTalentTree.CapacityBonusPerRank);

            // ContractBoard
            Assert.AreEqual(true, result.ContractBoard.Enabled);
            Assert.AreEqual(4, result.ContractBoard.StartingActiveContracts);
            Assert.AreEqual(9, result.ContractBoard.MaxContractsPerRun);
            Assert.AreEqual(1.7f, result.ContractBoard.RewardMultiplier);

            // PossessionPlus
            Assert.AreEqual(true, result.PossessionPlus.Enabled);
            Assert.AreEqual(5, result.PossessionPlus.MaxCharges);
            Assert.AreEqual(35f, result.PossessionPlus.PulseRadius);
            Assert.AreEqual(false, result.PossessionPlus.ShareWithLobby);

            // RivalGhosts
            Assert.AreEqual(true, result.RivalGhosts.Enabled);
            Assert.AreEqual(80, result.RivalGhosts.StorageLimit);
            Assert.AreEqual(140, result.RivalGhosts.KillWeight);
            Assert.AreEqual(0.2f, result.RivalGhosts.RivalLeadThreshold);

            // Marker
            Assert.AreEqual(true, result.Marker.Enabled);
            Assert.AreEqual(false, result.Marker.InfinitePaintballs);

            // InventoryExpansion
            Assert.AreEqual(true, result.InventoryExpansion.Enabled);
            Assert.AreEqual(8, result.InventoryExpansion.AdditionalSlots);

            // AutoLoot
            Assert.AreEqual(true, result.AutoLoot.Enabled);
            Assert.AreEqual(15.0f, result.AutoLoot.PickupRange);

            // Fly
            Assert.AreEqual(true, result.Fly.Enabled);
            Assert.AreEqual(25.0f, result.Fly.FlySpeed);

            // Progression
            Assert.AreEqual(false, result.Progression.Enabled);
            Assert.AreEqual(50, result.Progression.KillXP);
            Assert.AreEqual(0.05f, result.Progression.HpBonusPerLevel);
            Assert.AreEqual(100, result.Progression.MaxLevel);
            Assert.AreEqual(200, result.Progression.BaseXPPerLevel);
        }

        [Test]
        public void HostConfig_ApplyTo_DoesNotAffectClientOnlyConfigs()
        {
            var suite = new SuiteConfig();
            // Set client configs to custom values
            suite.Radar.Range = 999f;
            suite.Fov.FieldOfView = 120f;
            suite.Esp.MaxRange = 200f;

            // Apply host config (should NOT touch Radar, Fov, Esp, etc.)
            var hostConfig = new HostConfig { Difficulty = new DifficultyConfig { Enabled = false } };
            hostConfig.ApplyTo(suite);

            // Client configs should be unchanged
            Assert.AreEqual(999f, suite.Radar.Range);
            Assert.AreEqual(120f, suite.Fov.FieldOfView);
            Assert.AreEqual(200f, suite.Esp.MaxRange);

            // Host config should be applied
            Assert.AreEqual(false, suite.Difficulty.Enabled);
        }

        [Test]
        public void HostConfig_FromSuiteConfig_CapturesAllHostConfigs()
        {
            var suite = new SuiteConfig();
            suite.Difficulty.MaxMultiplier = 9.9f;
            suite.Stamina.Enabled = true;
            suite.Fly.FlySpeed = 42.0f;
            suite.NoiseDirector.GlobalVolumeMultiplier = 1.35f;
            suite.RunMutatorDraft.DraftChoiceCount = 6;
            suite.TramTalentTree.StartingPoints = 9;
            suite.ContractBoard.MaxContractsPerRun = 11;
            suite.PossessionPlus.MaxCharges = 4;
            suite.RivalGhosts.KillWeight = 222;

            var hostConfig = HostConfig.FromSuiteConfig(suite);

            Assert.AreEqual(9.9f, hostConfig.Difficulty.MaxMultiplier);
            Assert.AreEqual(true, hostConfig.Stamina.Enabled);
            Assert.AreEqual(42.0f, hostConfig.Fly.FlySpeed);
            Assert.AreEqual(1.35f, hostConfig.NoiseDirector.GlobalVolumeMultiplier);
            Assert.AreEqual(6, hostConfig.RunMutatorDraft.DraftChoiceCount);
            Assert.AreEqual(9, hostConfig.TramTalentTree.StartingPoints);
            Assert.AreEqual(11, hostConfig.ContractBoard.MaxContractsPerRun);
            Assert.AreEqual(4, hostConfig.PossessionPlus.MaxCharges);
            Assert.AreEqual(222, hostConfig.RivalGhosts.KillWeight);
        }

        [Test]
        public void ConfigSyncLogic_Deserialize_NullJson_ReturnsNull()
        {
            var result = ConfigSyncLogic.Deserialize("");
            Assert.IsNull(result);
        }
    }
}

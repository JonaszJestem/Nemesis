using Nemesis.Modules.AutoLoot;
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
using Nemesis.Modules.Stamina;
using Nemesis.Modules.TramTalentTree;
using Nemesis.Modules.PossessionPlus;
using Nemesis.Modules.RivalGhosts;

namespace Nemesis.Core
{
    /// <summary>
    /// All gameplay-affecting configs that are synced from host to clients.
    /// Adding a new host-only module config here automatically syncs it.
    /// No manual Apply methods, SyncPayload fields, or push/pull wiring needed.
    /// </summary>
    internal class HostConfig
    {
        public DifficultyConfig Difficulty { get; set; } = new DifficultyConfig();
        public RoleConfig Roles { get; set; } = new RoleConfig();
        public DamageScaleConfig DamageScale { get; set; } = new DamageScaleConfig();
        public MoreMimicsConfig MoreMimics { get; set; } = new MoreMimicsConfig();
        public StaminaConfig Stamina { get; set; } = new StaminaConfig();
        public JumpConfig Jump { get; set; } = new JumpConfig();
        public FullbrightConfig Fullbright { get; set; } = new FullbrightConfig();
        public LootDropConfig LootDrop { get; set; } = new LootDropConfig();
        public MoreVoicesConfig MoreVoices { get; set; } = new MoreVoicesConfig();
        public NoiseDirectorConfig NoiseDirector { get; set; } = new NoiseDirectorConfig();
        public RunMutatorDraftConfig RunMutatorDraft { get; set; } = new RunMutatorDraftConfig();
        public TramTalentTreeConfig TramTalentTree { get; set; } = new TramTalentTreeConfig();
        public ContractBoardConfig ContractBoard { get; set; } = new ContractBoardConfig();
        public PossessionPlusConfig PossessionPlus { get; set; } = new PossessionPlusConfig();
        public RivalGhostsConfig RivalGhosts { get; set; } = new RivalGhostsConfig();
        public MarkerConfig Marker { get; set; } = new MarkerConfig();
        public InventoryExpansionConfig InventoryExpansion { get; set; } = new InventoryExpansionConfig();
        public AutoLootConfig AutoLoot { get; set; } = new AutoLootConfig();
        public FlyConfig Fly { get; set; } = new FlyConfig();
        public ProgressionConfig Progression { get; set; } = new ProgressionConfig();

        /// <summary>
        /// Snapshot the current host configs from a SuiteConfig.
        /// </summary>
        public static HostConfig FromSuiteConfig(Config.SuiteConfig suite)
        {
            return new HostConfig
            {
                Difficulty = suite.Difficulty,
                Roles = suite.Roles,
                DamageScale = suite.DamageScale,
                MoreMimics = suite.MoreMimics,
                Stamina = suite.Stamina,
                Jump = suite.Jump,
                Fullbright = suite.Fullbright,
                LootDrop = suite.LootDrop,
                MoreVoices = suite.MoreVoices,
                NoiseDirector = suite.NoiseDirector,
                RunMutatorDraft = suite.RunMutatorDraft,
                TramTalentTree = suite.TramTalentTree,
                ContractBoard = suite.ContractBoard,
                PossessionPlus = suite.PossessionPlus,
                RivalGhosts = suite.RivalGhosts,
                Marker = suite.Marker,
                InventoryExpansion = suite.InventoryExpansion,
                AutoLoot = suite.AutoLoot,
                Fly = suite.Fly,
                Progression = suite.Progression
            };
        }

        /// <summary>
        /// Apply deserialized host configs onto a SuiteConfig.
        /// Replaces the config objects entirely (no field-by-field copy needed).
        /// </summary>
        public void ApplyTo(Config.SuiteConfig suite)
        {
            suite.Difficulty = Difficulty;
            suite.Roles = Roles;
            suite.DamageScale = DamageScale;
            suite.MoreMimics = MoreMimics;
            suite.Stamina = Stamina;
            suite.Jump = Jump;
            suite.Fullbright = Fullbright;
            suite.LootDrop = LootDrop;
            suite.MoreVoices = MoreVoices;
            suite.NoiseDirector = NoiseDirector;
            suite.RunMutatorDraft = RunMutatorDraft;
            suite.TramTalentTree = TramTalentTree;
            suite.ContractBoard = ContractBoard;
            suite.PossessionPlus = PossessionPlus;
            suite.RivalGhosts = RivalGhosts;
            suite.Marker = Marker;
            suite.InventoryExpansion = InventoryExpansion;
            suite.AutoLoot = AutoLoot;
            suite.Fly = Fly;
            suite.Progression = Progression;
        }
    }
}

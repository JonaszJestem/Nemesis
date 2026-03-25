using Nemesis.Modules.DifficultyDirector;
using Nemesis.Modules.ProximityRadar;
using Nemesis.Modules.RoleSystem;
using Nemesis.Modules.PersistentProgression;
using Nemesis.Modules.Stamina;
using Nemesis.Modules.Fov;
using Nemesis.Modules.VoiceFix;
using Nemesis.Modules.DamageScale;
using Nemesis.Modules.MoreMimics;
using Nemesis.Modules.HealthIndicators;
using Nemesis.Modules.Jump;
using Nemesis.Modules.EnemyDropLoot;
using Nemesis.Modules.Fullbright;
using Nemesis.Modules.MoreVoices;
using Nemesis.Modules.TooltipMod;
using Nemesis.Modules.Marker;
using Nemesis.Modules.InventoryExpansion;
using Nemesis.Modules.AutoLoot;
using Nemesis.Modules.Esp;
using Nemesis.Modules.Fly;
using Nemesis.Modules.NoiseDirector;
using Nemesis.Modules.ContractBoard;
using Nemesis.Modules.RunMutatorDraft;
using Nemesis.Modules.TramTalentTree;
using Nemesis.Modules.PossessionPlus;
using Nemesis.Modules.RivalGhosts;

namespace Nemesis.Config
{
    internal class SuiteConfig
    {
        public DifficultyConfig Difficulty { get; set; } = new DifficultyConfig();
        public RoleConfig Roles { get; set; } = new RoleConfig();
        public RadarConfig Radar { get; set; } = new RadarConfig();
        public ProgressionConfig Progression { get; set; } = new ProgressionConfig();
        public StaminaConfig Stamina { get; set; } = new StaminaConfig();
        public FovConfig Fov { get; set; } = new FovConfig();
        public VoiceFixConfig VoiceFix { get; set; } = new VoiceFixConfig();
        public DamageScaleConfig DamageScale { get; set; } = new DamageScaleConfig();
        public MoreMimicsConfig MoreMimics { get; set; } = new MoreMimicsConfig();
        public HealthIndicatorsConfig HealthIndicators { get; set; } = new HealthIndicatorsConfig();
        public JumpConfig Jump { get; set; } = new JumpConfig();
        public FullbrightConfig Fullbright { get; set; } = new FullbrightConfig();
        public LootDropConfig LootDrop { get; set; } = new LootDropConfig();
        public MoreVoicesConfig MoreVoices { get; set; } = new MoreVoicesConfig();
        public NoiseDirectorConfig NoiseDirector { get; set; } = new NoiseDirectorConfig();
        public RunMutatorDraftConfig RunMutatorDraft { get; set; } = new RunMutatorDraftConfig();
        public TramTalentTreeConfig TramTalentTree { get; set; } = new TramTalentTreeConfig();
        public TooltipConfig Tooltip { get; set; } = new TooltipConfig();
        public MarkerConfig Marker { get; set; } = new MarkerConfig();
        public InventoryExpansionConfig InventoryExpansion { get; set; } = new InventoryExpansionConfig();
        public AutoLootConfig AutoLoot { get; set; } = new AutoLootConfig();
        public EspConfig Esp { get; set; } = new EspConfig();
        public FlyConfig Fly { get; set; } = new FlyConfig();
        public ContractBoardConfig ContractBoard { get; set; } = new ContractBoardConfig();
        public PossessionPlusConfig PossessionPlus { get; set; } = new PossessionPlusConfig();
        public RivalGhostsConfig RivalGhosts { get; set; } = new RivalGhostsConfig();
    }
}

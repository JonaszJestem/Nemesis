namespace Nemesis.Modules.TramTalentTree
{
    internal sealed class TramTalentTreeConfig
    {
        public bool Enabled { get; set; } = false;
        public bool ShowHud { get; set; } = true;
        public bool ResetOnSessionStart { get; set; } = true;
        public int StartingPoints { get; set; } = 0;
        public int MaxRankPerNode { get; set; } = 5;
        public float SpeedBonusPerRank { get; set; } = 0.06f;
        public float NoiseReductionPerRank { get; set; } = 0.08f;
        public float RewardBonusPerRank { get; set; } = 0.10f;
        public int CapacityBonusPerRank { get; set; } = 1;
        public float DamageReductionPerRank { get; set; } = 0.05f;
    }
}

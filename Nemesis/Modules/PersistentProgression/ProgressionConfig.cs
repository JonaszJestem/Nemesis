namespace Nemesis.Modules.PersistentProgression
{
    internal class ProgressionConfig
    {
        public bool Enabled { get; set; } = true;
        public bool ShowXpBar { get; set; } = true;

        // XP rewards (configurable)
        public int KillXP { get; set; } = 25;
        public int LootCollectedXP { get; set; } = 10;
        public int SellXP { get; set; } = 15;
        public int RoomClearedXP { get; set; } = 50;
        public int SessionSurvivedXP { get; set; } = 100;
        public int MonsterLootDropXP { get; set; } = 15;

        // Scale XP with difficulty multiplier
        public bool ScaleWithDifficulty { get; set; } = true;

        // Level bonuses
        public float HpBonusPerLevel { get; set; } = 0.02f;
        public float SpeedBonusPerLevel { get; set; } = 0.005f;
        public int MaxLevel { get; set; } = 50;
        public long BaseXPPerLevel { get; set; } = 100;
        public float XPScalingExponent { get; set; } = 1.5f;

        // Auto-save interval
        public float SaveIntervalSeconds { get; set; } = 60f;

        // XP bar position
        public int XpBarX { get; set; } = 10;
        public int XpBarYFromBottom { get; set; } = 70;
        public int XpBarWidth { get; set; } = 150;
    }
}

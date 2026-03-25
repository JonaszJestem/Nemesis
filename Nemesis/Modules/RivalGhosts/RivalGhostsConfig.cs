namespace Nemesis.Modules.RivalGhosts
{
    internal sealed class RivalGhostsConfig
    {
        public bool Enabled { get; set; } = false;
        public bool ShowHud { get; set; } = true;
        public int StorageLimit { get; set; } = 50;
        public int TopRecordsToShow { get; set; } = 3;
        public float AutoSaveIntervalSeconds { get; set; } = 45f;
        public long KillWeight { get; set; } = 100;
        public long LootWeight { get; set; } = 30;
        public long RoomClearWeight { get; set; } = 75;
        public long SurvivalWeight { get; set; } = 120;
        public long MonsterDropWeight { get; set; } = 20;
        public long SellWeight { get; set; } = 15;
        public long DeathPenalty { get; set; } = 25;
        public float RivalLeadThreshold { get; set; } = 0.15f;
    }
}

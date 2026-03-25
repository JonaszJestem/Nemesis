namespace Nemesis.Modules.ProximityRadar
{
    internal class RadarConfig
    {
        public bool Enabled { get; set; } = true;
        public float Range { get; set; } = 50f;
        public int RadarSize { get; set; } = 180;
        public float RadarOpacity { get; set; } = 0.7f;
        public float UpdateRate { get; set; } = 0.75f;

        // Entity toggles
        public bool ShowMonsters { get; set; } = true;
        public bool ShowLoot { get; set; } = true;
        public bool ShowPlayers { get; set; } = true;

        // Position (from top-right corner)
        public int OffsetX { get; set; } = 20;
        public int OffsetY { get; set; } = 20;

        // Dot sizes
        public int MonsterDotSize { get; set; } = 4;
        public int LootDotSize { get; set; } = 3;
        public int PlayerDotSize { get; set; } = 5;
    }
}

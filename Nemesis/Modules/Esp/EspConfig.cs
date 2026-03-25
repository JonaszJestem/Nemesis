namespace Nemesis.Modules.Esp
{
    internal class EspConfig
    {
        public bool Enabled { get; set; } = false;
        public bool ShowDistance { get; set; } = true;
        public float MaxRange { get; set; } = 50f;
        public bool ShowMonsters { get; set; } = true;
        public bool ShowLoot { get; set; } = true;
        public bool ShowPlayers { get; set; } = true;
    }
}

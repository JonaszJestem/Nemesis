namespace Nemesis.Modules.EnemyDropLoot
{
    internal class LootDropConfig
    {
        public bool Enabled { get; set; } = false;
        public float DropChance { get; set; } = 0.1f;
        public int MaxDropsPerKill { get; set; } = 1;
    }
}

namespace Nemesis.Modules.RoleSystem
{
    internal class RoleConfig
    {
        public bool Enabled { get; set; } = true;
        public bool ShowRoleHud { get; set; } = true;

        public float ScoutSpeedMultiplier { get; set; } = 1.15f;
        public float TankHpMultiplier { get; set; } = 1.25f;
        public float TankSpeedPenalty { get; set; } = 0.95f;
        public float MedicInteractMultiplier { get; set; } = 1.20f;
        public float ScavengerLootMultiplier { get; set; } = 1.30f;
    }
}

namespace Nemesis.Modules.HealthIndicators
{
    internal class HealthIndicatorsConfig
    {
        public bool Enabled { get; set; } = true;
        public bool ShowDamageNumbers { get; set; } = true;
        public bool ShowHealthBars { get; set; } = true;
        public float DamageNumberScale { get; set; } = 1.0f;
    }
}

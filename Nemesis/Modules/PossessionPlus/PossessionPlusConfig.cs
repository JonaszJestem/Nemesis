namespace Nemesis.Modules.PossessionPlus
{
    internal sealed class PossessionPlusConfig
    {
        public bool Enabled { get; set; } = false;
        public bool ShowHud { get; set; } = true;
        public bool ShareWithLobby { get; set; } = true;
        public bool AutoPulseOnDeath { get; set; } = false;
        public int MaxCharges { get; set; } = 3;
        public float BaseRechargeSeconds { get; set; } = 20f;
        public float AllyRechargeBonusSeconds { get; set; } = 4f;
        public float MinimumRechargeSeconds { get; set; } = 6f;
        public float AllySupportRadius { get; set; } = 18f;
        public float PulseCooldownSeconds { get; set; } = 12f;
        public float GhostLifetimeSeconds { get; set; } = 180f;
        public float PulseRadius { get; set; } = 28f;
        public int MaxMarksPerPulse { get; set; } = 4;
        public float MarkLifetimeSeconds { get; set; } = 15f;
        public float RemoteBroadcastLifetimeSeconds { get; set; } = 20f;
    }
}

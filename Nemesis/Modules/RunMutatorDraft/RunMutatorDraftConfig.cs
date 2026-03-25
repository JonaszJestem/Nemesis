namespace Nemesis.Modules.RunMutatorDraft
{
    internal class RunMutatorDraftConfig
    {
        public bool Enabled { get; set; } = false;
        public bool ShowHud { get; set; } = true;
        public bool AutoPickBestAvailable { get; set; } = true;

        public int DraftChoiceCount { get; set; } = 4;
        public int ActiveMutatorCount { get; set; } = 2;

        public float RefreshIntervalSeconds { get; set; } = 5f;
        public int DefaultPreferredWeatherId { get; set; } = 4;

        public float SpawnPressureMultiplier { get; set; } = 1.15f;
        public float NoiseLeakMultiplier { get; set; } = 1.25f;
        public float NoiseDecayMultiplier { get; set; } = 0.9f;
        public float HybridMultiplier { get; set; } = 1.1f;
    }
}

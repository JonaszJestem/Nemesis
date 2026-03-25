namespace Nemesis.Modules.NoiseDirector
{
    internal class NoiseDirectorConfig
    {
        public bool Enabled { get; set; } = false;
        public float GlobalVolumeMultiplier { get; set; } = 1.0f;
        public float AmbientVolumeMultiplier { get; set; } = 0.90f;
        public float EffectsVolumeMultiplier { get; set; } = 1.0f;
        public float VoiceVolumeMultiplier { get; set; } = 1.05f;
        public float UpdateIntervalSeconds { get; set; } = 0.5f;
        public bool AffectUiAudio { get; set; } = false;
        public bool AffectMusic { get; set; } = false;
    }
}

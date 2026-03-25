using Nemesis.Core;

namespace Nemesis.Modules.DamageScale
{
    internal class DamageScaleModule : IModule
    {
        public string Name => "Damage Scale";

        private readonly DamageScaleConfig _config;

        public static bool IsEnabled { get; private set; }
        public static float CurrentMultiplier { get; private set; } = 1.0f;

        public DamageScaleModule(DamageScaleConfig config)
        {
            _config = config;
        }

        public void Initialize()
        {
            Log.Damage.Msg("Initialized");
        }

        public void Shutdown()
        {
            IsEnabled = false;
            CurrentMultiplier = 1.0f;
        }

        public void OnUpdate()
        {
            IsEnabled = _config.Enabled;
            CurrentMultiplier = _config.DamageMultiplier;
        }

        public void OnGUI() { }
    }
}

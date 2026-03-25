using Nemesis.Core;

namespace Nemesis.Modules.Stamina
{
    internal class StaminaModule : IModule
    {
        public string Name => "Infinite Stamina";

        private readonly StaminaConfig _config;

        public static bool IsEnabled { get; private set; }

        public StaminaModule(StaminaConfig config)
        {
            _config = config;
        }

        public void Initialize()
        {
            Log.Stamina.Msg("Initialized");
        }

        public void Shutdown()
        {
            IsEnabled = false;
        }

        public void OnUpdate()
        {
            IsEnabled = _config.Enabled;
        }

        public void OnGUI() { }
    }
}

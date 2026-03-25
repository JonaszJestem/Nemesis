using Nemesis.Core;

namespace Nemesis.Modules.VoiceFix
{
    internal class VoiceFixModule : IModule
    {
        public string Name => "Mimic Voice Fix";

        private readonly VoiceFixConfig _config;

        public static bool IsEnabled { get; private set; }

        public VoiceFixModule(VoiceFixConfig config)
        {
            _config = config;
        }

        public void Initialize()
        {
            Log.VoiceFix.Msg("Initialized");
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

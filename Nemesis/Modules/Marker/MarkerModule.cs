using Nemesis.Core;

namespace Nemesis.Modules.Marker
{
    internal class MarkerModule : IModule
    {
        public string Name => "Marker";

        private readonly MarkerConfig _config;

        public static bool IsEnabled { get; private set; }
        public static bool InfinitePaintballs { get; private set; }

        public MarkerModule(MarkerConfig config)
        {
            _config = config;
        }

        public void Initialize()
        {
            Log.Marker.Msg("Initialized");
        }

        public void Shutdown()
        {
            IsEnabled = false;
            InfinitePaintballs = false;
        }

        public void OnUpdate()
        {
            IsEnabled = _config.Enabled;
            InfinitePaintballs = _config.Enabled && _config.InfinitePaintballs;
        }

        public void OnGUI() { }
    }
}

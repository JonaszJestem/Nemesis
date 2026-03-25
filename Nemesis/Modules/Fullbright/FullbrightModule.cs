using Nemesis.Core;
using UnityEngine;

namespace Nemesis.Modules.Fullbright
{
    internal class FullbrightModule : IModule
    {
        public string Name => "Fullbright";

        private readonly FullbrightConfig _config;
        private bool _active;
        private float _originalIntensity;
        private Color _originalAmbientLight;
        private bool _storedOriginals;

        public FullbrightModule(FullbrightConfig config)
        {
            _config = config;
        }

        public void Initialize()
        {
            Log.Fullbright.Msg("Initialized");
        }

        public void Shutdown()
        {
            RestoreOriginals();
        }

        public void OnUpdate()
        {
            if (_config.Enabled && !_active)
            {
                StoreAndApply();
            }
            else if (_config.Enabled && _active)
            {
                // Keep applying in case scene changes reset it
                RenderSettings.ambientIntensity = _config.AmbientIntensity;
                RenderSettings.ambientLight = Color.white * _config.AmbientIntensity;
            }
            else if (!_config.Enabled && _active)
            {
                RestoreOriginals();
            }
        }

        private void StoreAndApply()
        {
            if (!_storedOriginals)
            {
                _originalIntensity = RenderSettings.ambientIntensity;
                _originalAmbientLight = RenderSettings.ambientLight;
                _storedOriginals = true;
            }

            RenderSettings.ambientIntensity = _config.AmbientIntensity;
            RenderSettings.ambientLight = Color.white * _config.AmbientIntensity;
            _active = true;
            Log.Fullbright.Msg($"Enabled with intensity {_config.AmbientIntensity:F1}");
        }

        private void RestoreOriginals()
        {
            if (!_active) return;

            if (_storedOriginals)
            {
                RenderSettings.ambientIntensity = _originalIntensity;
                RenderSettings.ambientLight = _originalAmbientLight;
            }

            _active = false;
            Log.Fullbright.Msg("Disabled, restored original lighting");
        }

        public void OnGUI() { }
    }
}

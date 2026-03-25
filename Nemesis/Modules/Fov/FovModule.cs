using Nemesis.Core;
using UnityEngine;

namespace Nemesis.Modules.Fov
{
    internal class FovModule : IModule
    {
        public string Name => "FOV Scale";

        private readonly FovConfig _config;
        private bool _active;
        private float _originalFov;
        private bool _storedOriginal;

        public FovModule(FovConfig config)
        {
            _config = config;
        }

        public void Initialize()
        {
            Log.Fov.Msg("Initialized");
        }

        public void Shutdown()
        {
            RestoreOriginal();
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
                var cam = Camera.main;
                if (cam != null)
                    cam.fieldOfView = _config.FieldOfView;
            }
            else if (!_config.Enabled && _active)
            {
                RestoreOriginal();
            }
        }

        private void StoreAndApply()
        {
            var cam = Camera.main;
            if (cam == null) return;

            if (!_storedOriginal)
            {
                _originalFov = cam.fieldOfView;
                _storedOriginal = true;
            }

            cam.fieldOfView = _config.FieldOfView;
            _active = true;
            Log.Fov.Msg($"Enabled with FOV {_config.FieldOfView:F1}");
        }

        private void RestoreOriginal()
        {
            if (!_active) return;

            if (_storedOriginal)
            {
                var cam = Camera.main;
                if (cam != null)
                    cam.fieldOfView = _originalFov;
            }

            _active = false;
            Log.Fov.Msg("Disabled, restored original FOV");
        }

        public void OnGUI() { }
    }
}

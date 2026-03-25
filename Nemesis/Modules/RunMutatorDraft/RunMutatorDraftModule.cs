using System;
using Nemesis.Core;
using UnityEngine;

namespace Nemesis.Modules.RunMutatorDraft
{
    internal class RunMutatorDraftModule : IModule
    {
        public string Name => "Run Mutator Draft";

        private readonly RunMutatorDraftConfig _config;
        private float _updateTimer;
        private GUIStyle? _statusStyle;
        private string _cachedStatus = "Idle";

        public RunMutatorDraftModule(RunMutatorDraftConfig config)
        {
            _config = config;
        }

        public void Initialize()
        {
            Log.Msg("RunMutatorDraft", "Initialized");
        }

        public void Shutdown()
        {
            RunMutatorDraftRuntime.Reset();
        }

        public void OnUpdate()
        {
            _updateTimer += Time.deltaTime;
            if (_updateTimer < Math.Max(0.5f, _config.RefreshIntervalSeconds))
                return;

            _updateTimer = 0f;

            try
            {
                RunMutatorDraftRuntime.Update(_config, Time.deltaTime);
                var bridge = RunMutatorDraftBridge.Current;
                _cachedStatus = bridge.ActiveMutators.Length == 0
                    ? "No active mutators"
                    : $"{bridge.DraftName}: {string.Join(", ", bridge.ActiveMutators)}";
            }
            catch (Exception ex)
            {
                Log.Warn("RunMutatorDraft", $"Update failed: {ex.Message}");
            }
        }

        public void OnGUI()
        {
            if (!_config.Enabled || !_config.ShowHud)
                return;

            if (_statusStyle == null)
            {
                _statusStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 12,
                    fontStyle = FontStyle.Bold,
                    normal = { textColor = new Color(0.9f, 0.8f, 0.4f) }
                };
            }

            var bridge = RunMutatorDraftBridge.Current;
            GUI.Label(
                new Rect(10, 54, 460, 22),
                $"Draft: {_cachedStatus}",
                _statusStyle);
        }
    }
}

using System;
using System.Text;
using Nemesis.Core;
using MimicAPI.GameAPI;
using UnityEngine;

namespace Nemesis.Modules.TramTalentTree
{
    internal sealed class TramTalentTreeModule : IModule
    {
        public string Name => "Tram Talent Tree";

        private readonly TramTalentTreeConfig _config;
        private readonly TramTalentTreeEngine _engine;
        private TramTalentTreeState _state;
        private GUIStyle? _hudStyle;
        private bool _sessionActive;

        public TramTalentTreeModule(TramTalentTreeConfig config)
        {
            _config = config;
            _engine = new TramTalentTreeEngine(config);
            _state = _engine.CreateState();
        }

        public TramTalentTreeConfig Config => _config;
        public TramTalentTreeState State => _state;
        public TramTalentTreeEngine Engine => _engine;
        public TramTalentSnapshot CurrentSnapshot => TramTalentTreeBridge.GetCurrentSnapshot();

        public void Initialize()
        {
            ModuleEventBus.OnSessionStarted += OnSessionStarted;
            TramTalentTreeBridge.PublishSnapshot(_engine.ComputeSnapshot(_state));
            Log.Msg("TramTalentTree", "Initialized");
        }

        public void Shutdown()
        {
            ModuleEventBus.OnSessionStarted -= OnSessionStarted;
            TramTalentTreeBridge.Reset();
        }

        private void OnSessionStarted()
        {
            if (!_config.Enabled || !_config.ResetOnSessionStart)
            {
                _sessionActive = _config.Enabled;
                if (_config.Enabled)
                    TramTalentTreeBridge.PublishSnapshot(_engine.ComputeSnapshot(_state));
                return;
            }

            _state = _engine.CreateState();
            _sessionActive = true;
            TramTalentTreeBridge.PublishSnapshot(_engine.ComputeSnapshot(_state));
        }

        public void OnUpdate()
        {
            if (!_config.Enabled)
                return;

            _sessionActive = true;

            int queuedPoints = _engine.DrainQueuedContractPoints(_state);
            if (queuedPoints > 0)
                Log.Msg("TramTalentTree", $"Received {queuedPoints} contract point(s).");

            TramTalentTreeBridge.PublishSnapshot(_engine.ComputeSnapshot(_state));
        }

        public void SpendPoint(TramTalentNodeId nodeId)
        {
            if (_engine.TrySpendPoint(_state, nodeId))
                TramTalentTreeBridge.PublishSnapshot(_engine.ComputeSnapshot(_state));
        }

        public void RefundPoint(TramTalentNodeId nodeId)
        {
            if (_engine.TryRefundPoint(_state, nodeId))
                TramTalentTreeBridge.PublishSnapshot(_engine.ComputeSnapshot(_state));
        }

        public void GrantPoints(int amount)
        {
            _engine.GrantPoints(_state, amount);
            TramTalentTreeBridge.PublishSnapshot(_engine.ComputeSnapshot(_state));
        }

        public void OnGUI()
        {
            if (!_config.Enabled || !_config.ShowHud)
                return;

            if (_hudStyle == null)
            {
                _hudStyle = new GUIStyle(GUI.skin.box)
                {
                    fontSize = 12,
                    padding = new RectOffset(10, 10, 8, 8),
                    normal =
                    {
                        textColor = new Color(0.88f, 0.94f, 1f)
                    }
                };
            }

            var snapshot = CurrentSnapshot;
            var rect = new Rect(10, 90, 290, 130);
            GUILayout.BeginArea(rect, _hudStyle);
            GUILayout.Label("Tram Talent Tree", GUI.skin.label);
            GUILayout.Label($"Unspent points: {_state.UnspentPoints}");
            GUILayout.Label($"Speed x{snapshot.SpeedMultiplier:F2}");
            GUILayout.Label($"Noise x{snapshot.NoiseMultiplier:F2}");
            GUILayout.Label($"Rewards x{snapshot.RewardMultiplier:F2}");
            GUILayout.EndArea();
        }
    }
}

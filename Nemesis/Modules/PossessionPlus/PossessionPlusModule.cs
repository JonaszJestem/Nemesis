using Nemesis.Core;
using Nemesis.UI;
using UnityEngine;

namespace Nemesis.Modules.PossessionPlus
{
    internal sealed class PossessionPlusModule : IModule
    {
        public string Name => "Possession++";

        private readonly PossessionPlusConfig _config;
        private GUIStyle? _titleStyle;
        private GUIStyle? _bodyStyle;
        private GUIStyle? _accentStyle;

        public PossessionPlusModule(PossessionPlusConfig config) => _config = config;

        public void Initialize()
        {
            ModuleEventBus.OnSessionStarted += OnSessionStarted;
            PossessionPlusRuntime.Initialize(_config);
            Log.Msg("PossessionPlus", "Initialized");
        }

        public void Shutdown()
        {
            ModuleEventBus.OnSessionStarted -= OnSessionStarted;
            PossessionPlusRuntime.Shutdown();
        }

        public void OnUpdate()
        {
            if (!_config.Enabled)
            {
                PossessionPlusRuntime.Shutdown();
                return;
            }

            PossessionPlusRuntime.Update(_config, Time.deltaTime);
        }

        public void OnGUI()
        {
            if (!_config.Enabled || !_config.ShowHud)
                return;

            var state = PossessionPlusRuntime.Current;
            if (!state.IsActive && state.RecentMarks.Count == 0 && state.RemoteGhosts.Count == 0)
                return;

            EnsureStyles();
            GUILayout.BeginVertical(GUIStyles.SectionBox, GUILayout.Width(390));
            GUILayout.Label("Possession++", _titleStyle!);
            GUILayout.Label(state.StatusLine, _accentStyle!);
            GUILayout.Label($"Charges: {state.Charges}/{state.MaxCharges}  Cooldown: {Mathf.CeilToInt(state.PulseCooldownRemaining)}s", _bodyStyle!);
            GUILayout.Label($"Ghost time: {Mathf.CeilToInt(state.GhostLifetimeRemaining)}s  Recharge: {state.RechargeProgressSeconds:F1}s", _bodyStyle!);

            if (state.RecentMarks.Count > 0)
            {
                GUILayout.Space(4);
                GUILayout.Label("Latest pulse", _accentStyle!);
                foreach (var mark in state.RecentMarks)
                    GUILayout.Label($"- {mark.Label} ({mark.Kind}) [{mark.Distance:F0}m]", _bodyStyle!);
            }

            if (state.RemoteGhosts.Count > 0)
            {
                GUILayout.Space(4);
                GUILayout.Label("Lobby ghosts", _accentStyle!);
                foreach (var ghost in state.RemoteGhosts)
                    GUILayout.Label($"{ghost.PlayerName}: {ghost.Charges} charge(s), {ghost.LatestMarks.Count} mark(s)", _bodyStyle!);
            }

            GUILayout.Label("Hotkey: G to pulse when ghosted", _bodyStyle!);
            GUILayout.EndVertical();
        }

        private void OnSessionStarted() => PossessionPlusRuntime.StartSession(_config);

        private void EnsureStyles()
        {
            if (_titleStyle != null) return;

            _titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 15,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.9f, 0.8f, 1f) }
            };
            _bodyStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 11,
                normal = { textColor = new Color(0.9f, 0.9f, 0.95f) }
            };
            _accentStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.55f, 0.9f, 1f) }
            };
        }
    }
}

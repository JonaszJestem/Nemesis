using System;
using MelonLoader;
using MimicAPI.GameAPI;
using Nemesis.Core;
using UnityEngine;

namespace Nemesis.Modules.RoleSystem
{
    internal class RoleSystemModule : IModule
    {
        public string Name => "Role System";

        private readonly RoleConfig _config;
        private readonly RoleAssigner _assigner;
        private bool _sessionActive;

        // Cached GUI
        private GUIStyle? _roleStyle;
        private GUIStyle? _descStyle;

        // Cache local role to avoid repeated FindObjectsByType
        private Role _cachedLocalRole = Role.None;
        private RoleStatDelta _cachedDelta;

        public RoleSystemModule(RoleConfig config)
        {
            _config = config;
            _assigner = new RoleAssigner(config);
        }

        public void Initialize()
        {
            ModuleEventBus.OnSessionStarted += OnSessionStarted;
            MelonLogger.Msg("[Nemesis] RoleSystem initialized");
        }

        public void Shutdown()
        {
            ModuleEventBus.OnSessionStarted -= OnSessionStarted;
            _assigner.RestoreAllStats();
        }

        private void OnSessionStarted()
        {
            if (!_config.Enabled) return;

            try
            {
                _assigner.AssignRoles();
                _sessionActive = true;
                _cachedLocalRole = _assigner.GetLocalRole();
                _cachedDelta = RoleStatDelta.ForRole(_cachedLocalRole, _config);

                MelonLogger.Msg($"[RoleSystem] Assigned role: {_cachedDelta.DisplayName} - {_cachedDelta.Description}");
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"[RoleSystem] Assignment error: {ex.Message}");
            }
        }

        public void OnUpdate() { }

        public void OnGUI()
        {
            if (!_config.Enabled || !_config.ShowRoleHud || !_sessionActive) return;
            if (_cachedLocalRole == Role.None) return;

            if (_roleStyle == null)
            {
                _roleStyle = new GUIStyle(GUI.skin.label) { fontSize = 16, fontStyle = FontStyle.Bold };
                _descStyle = new GUIStyle(GUI.skin.label) { fontSize = 12 };
            }

            var roleColor = GetRoleColor(_cachedLocalRole);
            _roleStyle!.normal.textColor = roleColor;
            _descStyle!.normal.textColor = new Color(roleColor.r, roleColor.g, roleColor.b, 0.7f);

            GUI.Label(new Rect(10, 10, 300, 30), $"Role: {_cachedDelta.DisplayName}", _roleStyle);
            GUI.Label(new Rect(10, 32, 300, 20), _cachedDelta.Description, _descStyle);
        }

        private static Color GetRoleColor(Role role)
        {
            switch (role)
            {
                case Role.Scout: return new Color(0.2f, 0.8f, 1f);
                case Role.Tank: return new Color(1f, 0.4f, 0.2f);
                case Role.Medic: return new Color(0.2f, 1f, 0.4f);
                case Role.Scavenger: return new Color(1f, 0.9f, 0.2f);
                default: return Color.white;
            }
        }
    }
}

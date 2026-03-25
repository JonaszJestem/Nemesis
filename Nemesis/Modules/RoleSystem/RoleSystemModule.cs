using System;
using System.Collections.Generic;
using MimicAPI.GameAPI;
using Nemesis.Core;
using Newtonsoft.Json;
using UnityEngine;

namespace Nemesis.Modules.RoleSystem
{
    internal class RoleSystemModule : IModule
    {
        public string Name => "Role System";

        private readonly RoleConfig _config;
        private readonly RoleAssigner _assigner;
        private bool _sessionActive;

        private GUIStyle? _roleStyle;
        private GUIStyle? _descStyle;

        private Role _cachedLocalRole = Role.None;
        private RoleStatDelta _cachedDelta;

        // Retry role assignment if players aren't loaded yet
        private float _roleRetryTimer;
        private int _roleRetryCount;
        private const int MaxRoleRetries = 10;
        private const float RoleRetryInterval = 1f;

        // (role sync now uses the retry mechanism above)

        public RoleSystemModule(RoleConfig config)
        {
            _config = config;
            _assigner = new RoleAssigner(config);
        }

        public void Initialize()
        {
            ModuleEventBus.OnSessionStarted += OnSessionStarted;
            Log.Roles.Msg("Initialized");
        }

        public void Shutdown()
        {
            ModuleEventBus.OnSessionStarted -= OnSessionStarted;
            _assigner.RestoreAllStats();
        }

        private void OnSessionStarted()
        {
            if (!_config.Enabled) return;
            _sessionActive = true;
            _cachedLocalRole = Role.None;
            _roleRetryCount = 0;
            _roleRetryTimer = 0f;

            // Try immediately, will retry in OnUpdate if players not loaded yet
            TryAssignRoles();
        }

        private bool TryAssignRoles()
        {
            try
            {
                bool isHost = NemesisMod.Instance?.IsHost ?? false;

                if (isHost)
                {
                    _assigner.AssignRoles();
                    _cachedLocalRole = _assigner.GetLocalRole();
                    _cachedDelta = RoleStatDelta.ForRole(_cachedLocalRole, _config);

                    if (_cachedLocalRole == Role.None)
                        return false; // Players not loaded yet

                    PushRoleAssignments();
                    Log.Roles.Msg($"Assigned role: {_cachedDelta.DisplayName}");
                    return true;
                }
                else
                {
                    // Client pulls from lobby data
                    return PullLocalRole();
                }
            }
            catch (Exception ex)
            {
                Log.Roles.Warn($"Assignment error: {ex.Message}");
                return false;
            }
        }

        public void OnUpdate()
        {
            if (!_config.Enabled || !_sessionActive) return;

            // Retry role assignment if it hasn't succeeded yet
            if (_cachedLocalRole == Role.None && _roleRetryCount < MaxRoleRetries)
            {
                _roleRetryTimer += Time.deltaTime;
                if (_roleRetryTimer >= RoleRetryInterval)
                {
                    _roleRetryTimer = 0f;
                    _roleRetryCount++;
                    TryAssignRoles();
                }
            }
        }

        private void PushRoleAssignments()
        {
            try
            {
                var players = PlayerAPI.GetAllPlayers();
                if (players == null) return;

                var assignments = new Dictionary<string, string>();
                foreach (var player in players)
                {
                    if (player == null) continue;
                    string name = PlayerAPI.GetPlayerName(player);
                    var role = _assigner.GetRole((uint)player.ActorID);
                    if (role != Role.None)
                        assignments[name] = role.ToString();
                }

                string json = JsonConvert.SerializeObject(assignments);
                SteamLobbyHelper.SetLobbyData(LobbyKeys.Roles, json);
                Log.Roles.Msg("Pushed role assignments to lobby");
            }
            catch (Exception ex)
            {
                Log.Roles.Warn($"Push roles failed: {ex.Message}");
            }
        }

        private bool PullLocalRole()
        {
            try
            {
                var player = PlayerAPI.GetLocalPlayer();
                if (player == null) return false;
                string localName = PlayerAPI.GetPlayerName(player);

                string json = SteamLobbyHelper.GetLobbyData(LobbyKeys.Roles);
                if (string.IsNullOrEmpty(json)) return false;

                var assignments = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                if (assignments == null || !assignments.ContainsKey(localName)) return false;

                if (Enum.TryParse<Role>(assignments[localName], out var role))
                {
                    _cachedLocalRole = role;
                    _cachedDelta = RoleStatDelta.ForRole(role, _config);

                    var statManager = PlayerAPI.GetStatManager(player);
                    if (statManager != null)
                    {
                        var baseStats = CachedStats.Capture(statManager);
                        baseStats.ApplyMultipliers(statManager, _cachedDelta.MaxHpMultiplier, _cachedDelta.SpeedMultiplier);
                    }

                    Log.Roles.Msg($"Received role: {_cachedDelta.DisplayName}");
                    return true;
                }
            }
            catch { }
            return false;
        }

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

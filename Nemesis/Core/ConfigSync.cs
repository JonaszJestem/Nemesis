using System;
using Nemesis.Config;
using Newtonsoft.Json;
using UnityEngine;

namespace Nemesis.Core
{
    /// <summary>
    /// Syncs host gameplay configs to all clients via Steam lobby data.
    /// Uses HostConfig for automatic serialization - no manual Apply methods needed.
    /// Adding a new host-only config to HostConfig automatically syncs it.
    /// </summary>
    internal class ConfigSync
    {
        private const float SyncInterval = 2f;

        private readonly SuiteConfig _config;
        private float _syncTimer;
        private string _lastWrittenJson = "";
        private string _lastReadJson = "";
        private bool _isHost;

        public ConfigSync(SuiteConfig config)
        {
            _config = config;
        }

        public void OnUpdate(bool isHost)
        {
            _isHost = isHost;
            _syncTimer += Time.deltaTime;
            if (_syncTimer < SyncInterval) return;
            _syncTimer = 0f;

            if (SteamLobbyHelper.GetLobbyId() == 0) return;

            if (isHost)
                PushConfigToLobby();
            else
                PullConfigFromLobby();
        }

        public void ForcePush()
        {
            if (!_isHost) return;
            if (SteamLobbyHelper.GetLobbyId() == 0) return;
            PushConfigToLobby();
        }

        private void PushConfigToLobby()
        {
            try
            {
                var hostConfig = HostConfig.FromSuiteConfig(_config);
                string json = JsonConvert.SerializeObject(hostConfig);
                if (json == _lastWrittenJson) return;

                SteamLobbyHelper.SetLobbyData(LobbyKeys.Config, json);
                _lastWrittenJson = json;
            }
            catch (Exception ex)
            {
                Log.Sync.Warn($"Config push failed: {ex.Message}");
            }
        }

        private void PullConfigFromLobby()
        {
            try
            {
                string json = SteamLobbyHelper.GetLobbyData(LobbyKeys.Config);
                if (string.IsNullOrEmpty(json) || json == _lastReadJson) return;

                _lastReadJson = json;

                var hostConfig = JsonConvert.DeserializeObject<HostConfig>(json);
                if (hostConfig == null) return;

                hostConfig.ApplyTo(_config);
                Log.Sync.Msg("Received config update from host");
            }
            catch (Exception ex)
            {
                Log.Sync.Warn($"Config pull failed: {ex.Message}");
            }
        }
    }
}

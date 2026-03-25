using System;
using System.Collections.Generic;
using System.Linq;
using MimicAPI.GameAPI;
using Nemesis.Core;
using Newtonsoft.Json;

namespace Nemesis.Modules.PossessionPlus
{
    internal static class PossessionPlusLobbyStore
    {
        private const string Prefix = "nemesis_possession_plus_";

        public static bool Publish(GhostPresenceSnapshot snapshot)
        {
            try
            {
                if (snapshot == null || string.IsNullOrWhiteSpace(snapshot.PlayerKey) || SteamLobbyHelper.GetLobbyId() == 0)
                    return false;

                return SteamLobbyHelper.SetLobbyData(Prefix + snapshot.PlayerKey, JsonConvert.SerializeObject(snapshot));
            }
            catch (Exception ex)
            {
                Log.Warn("PossessionPlus", $"Lobby publish failed: {ex.Message}");
                return false;
            }
        }

        public static List<GhostPresenceSnapshot> ReadVisibleGhosts(long nowUtcSeconds, string localPlayerKey)
        {
            var result = new List<GhostPresenceSnapshot>();
            try
            {
                var players = PlayerAPI.GetAllPlayers();
                if (players == null)
                    return result;

                foreach (var player in players)
                {
                    if (player == null || !PlayerAPI.IsPlayerValid(player))
                        continue;

                    string key = GetPlayerKey(player);
                    if (string.IsNullOrWhiteSpace(key) || key == localPlayerKey)
                        continue;

                    var snapshot = Read(key);
                    if (snapshot == null || snapshot.IsExpired(nowUtcSeconds))
                        continue;

                    if (string.IsNullOrWhiteSpace(snapshot.PlayerName))
                        snapshot.PlayerName = PlayerAPI.GetPlayerName(player);

                    result.Add(snapshot);
                }
            }
            catch (Exception ex)
            {
                Log.Warn("PossessionPlus", $"Lobby read failed: {ex.Message}");
            }

            return result
                .OrderByDescending(x => x.LastUpdatedUtcSeconds)
                .ThenBy(x => x.PlayerName, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private static GhostPresenceSnapshot? Read(string playerKey)
        {
            try
            {
                if (SteamLobbyHelper.GetLobbyId() == 0)
                    return null;

                string json = SteamLobbyHelper.GetLobbyData(Prefix + playerKey);
                return string.IsNullOrWhiteSpace(json) ? null : JsonConvert.DeserializeObject<GhostPresenceSnapshot>(json);
            }
            catch
            {
                return null;
            }
        }

        private static string GetPlayerKey(object player)
        {
            try
            {
                return player.GetType().GetProperty("ActorID")?.GetValue(player)?.ToString() ?? "";
            }
            catch
            {
                return "";
            }
        }
    }
}

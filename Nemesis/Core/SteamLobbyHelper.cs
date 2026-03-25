using MimicAPI.GameAPI;
using Steamworks;

namespace Nemesis.Core
{
    /// <summary>
    /// Shared helper for Steam lobby operations.
    /// Eliminates duplicated Hub -> SteamConnector -> JoinedLobbyID chains.
    /// </summary>
    internal static class SteamLobbyHelper
    {
        public static ulong GetLobbyId()
        {
            try
            {
                var hub = CoreAPI.GetHub();
                if (hub == null) return 0;

                var steamConnector = ReflectionHelper.GetFieldValue(hub, GameFieldNames.Hub_SteamConnector);
                if (steamConnector == null) return 0;

                var lobbyIdProp = steamConnector.GetType().GetProperty(
                    GamePropertyNames.SteamConnector_JoinedLobbyID,
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                if (lobbyIdProp == null) return 0;

                return (ulong)lobbyIdProp.GetValue(steamConnector);
            }
            catch
            {
                return 0;
            }
        }

        public static CSteamID GetLobbySteamId()
        {
            ulong id = GetLobbyId();
            return id == 0 ? default : new CSteamID(id);
        }

        public static string GetLobbyData(string key)
        {
            var lobbyId = GetLobbyId();
            if (lobbyId == 0) return "";
            return SteamMatchmaking.GetLobbyData(new CSteamID(lobbyId), key) ?? "";
        }

        public static bool SetLobbyData(string key, string value)
        {
            var lobbyId = GetLobbyId();
            if (lobbyId == 0) return false;
            return SteamMatchmaking.SetLobbyData(new CSteamID(lobbyId), key, value);
        }
    }
}

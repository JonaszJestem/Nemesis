using System;
using Nemesis.Core;
using Newtonsoft.Json;

namespace Nemesis.Modules.ContractBoard
{
    internal static class ContractBoardStore
    {
        private static readonly object Sync = new object();
        private static string _lastWrittenJson = "";
        private static string _lastReadJson = "";

        public static bool Publish(ContractBoardSnapshot snapshot)
        {
            try
            {
                if (snapshot == null || SteamLobbyHelper.GetLobbyId() == 0)
                    return false;

                string json = JsonConvert.SerializeObject(snapshot, Formatting.None);
                lock (Sync)
                {
                    if (json == _lastWrittenJson)
                        return true;

                    if (SteamLobbyHelper.SetLobbyData(LobbyKeys.ContractBoard, json))
                    {
                        _lastWrittenJson = json;
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Warn("ContractBoard", $"Lobby publish failed: {ex.Message}");
            }

            return false;
        }

        public static ContractBoardSnapshot? Read(bool force = false)
        {
            try
            {
                if (SteamLobbyHelper.GetLobbyId() == 0)
                    return null;

                string json = SteamLobbyHelper.GetLobbyData(LobbyKeys.ContractBoard);
                if (string.IsNullOrWhiteSpace(json))
                    return null;

                lock (Sync)
                {
                    if (!force && json == _lastReadJson)
                        return null;

                    _lastReadJson = json;
                }

                return JsonConvert.DeserializeObject<ContractBoardSnapshot>(json);
            }
            catch (Exception ex)
            {
                Log.Warn("ContractBoard", $"Lobby read failed: {ex.Message}");
                return null;
            }
        }

        public static void ResetCache()
        {
            lock (Sync)
            {
                _lastWrittenJson = "";
                _lastReadJson = "";
            }
        }
    }
}

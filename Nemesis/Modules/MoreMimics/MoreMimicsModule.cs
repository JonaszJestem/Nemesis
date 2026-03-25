using System;
using System.Collections.Generic;
using MimicAPI.GameAPI;
using Nemesis.Core;
using UnityEngine;

namespace Nemesis.Modules.MoreMimics
{
    internal class MoreMimicsModule : IModule
    {
        public string Name => "More Mimics";

        private readonly MoreMimicsConfig _config;
        private float _timer;
        private readonly HashSet<int> _scaledRooms = new HashSet<int>();

        private static readonly string[] SpawnFields =
        {
            GameFieldNames.DungeonMasterInfo_NormalMonsterSpawnRate,
            GameFieldNames.DungeonMasterInfo_MimicSpawnCountMin,
            GameFieldNames.DungeonMasterInfo_MimicSpawnCountMax,
            GameFieldNames.DungeonMasterInfo_MimicSpawnRate,
            GameFieldNames.DungeonMasterInfo_NormalMonsterSpawnTryCount,
            GameFieldNames.DungeonMasterInfo_MimicSpawnTryCount
        };

        public MoreMimicsModule(MoreMimicsConfig config)
        {
            _config = config;
        }

        public void Initialize()
        {
            ModuleEventBus.OnSessionStarted += OnSessionStarted;
            Log.MoreMimics.Msg("Initialized");
        }

        public void Shutdown()
        {
            ModuleEventBus.OnSessionStarted -= OnSessionStarted;
            _scaledRooms.Clear();
        }

        private void OnSessionStarted()
        {
            _scaledRooms.Clear();
        }

        public void OnUpdate()
        {
            if (!_config.Enabled) return;

            // Only host should modify spawn rates
            if (NemesisMod.Instance == null || !NemesisMod.Instance.IsHost) return;

            _timer += Time.deltaTime;
            if (_timer < 2.0f) return;
            _timer = 0f;

            try
            {
                var room = RoomAPI.GetCurrentRoom();
                if (room == null) return;

                int roomHash = room.GetHashCode();
                if (_scaledRooms.Contains(roomHash)) return;

                // Try to get DungeonMasterInfo from the room via reflection
                var dungeonInfo = ReflectionHelper.GetFieldValue(room, "DungeonMasterInfo")
                               ?? ReflectionHelper.GetFieldValue(room, "_dungeonMasterInfo")
                               ?? ReflectionHelper.GetFieldValue(room, "dungeonMasterInfo");

                if (dungeonInfo == null) return;

                // Verify it's a DungeonMasterInfo type
                if (!dungeonInfo.GetType().Name.Contains(GameTypeNames.DungeonMasterInfo)) return;

                float multiplier = _config.SpawnRateMultiplier;

                foreach (var fieldName in SpawnFields)
                {
                    try
                    {
                        var val = ReflectionHelper.GetFieldValue(dungeonInfo, fieldName);
                        if (val is int intVal)
                        {
                            int scaled = Mathf.Max(1, Mathf.RoundToInt(intVal * multiplier));
                            ReflectionHelper.SetFieldValue(dungeonInfo, fieldName, scaled);
                        }
                    }
                    catch { }
                }

                _scaledRooms.Add(roomHash);
                Log.MoreMimics.Msg($"Scaled spawn rates by x{multiplier:F1} for room {roomHash}");
            }
            catch (Exception ex)
            {
                Log.MoreMimics.Warn($"Update error: {ex.Message}");
            }
        }

        public void OnGUI() { }
    }
}

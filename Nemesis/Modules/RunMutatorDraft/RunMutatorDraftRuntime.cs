using System;
using System.Collections.Generic;
using System.Linq;
using MimicAPI.GameAPI;
using Nemesis.Core;
using UnityEngine;

namespace Nemesis.Modules.RunMutatorDraft
{
    internal sealed class RunMutatorRoomSnapshot
    {
        public object Room { get; }
        public object? DungeonMasterInfo { get; }
        public int? NormalMonsterSpawnRate { get; }
        public int? MimicSpawnRate { get; }
        public int? NormalMonsterSpawnPeriod { get; }
        public int? MimicSpawnPeriod { get; }
        public int? NormalMonsterSpawnTryCount { get; }
        public int? MimicSpawnTryCount { get; }
        public int? MimicSpawnCountMin { get; }
        public int? MimicSpawnCountMax { get; }

        private RunMutatorRoomSnapshot(
            object room,
            object? dungeonMasterInfo,
            int? normalMonsterSpawnRate,
            int? mimicSpawnRate,
            int? normalMonsterSpawnPeriod,
            int? mimicSpawnPeriod,
            int? normalMonsterSpawnTryCount,
            int? mimicSpawnTryCount,
            int? mimicSpawnCountMin,
            int? mimicSpawnCountMax)
        {
            Room = room;
            DungeonMasterInfo = dungeonMasterInfo;
            NormalMonsterSpawnRate = normalMonsterSpawnRate;
            MimicSpawnRate = mimicSpawnRate;
            NormalMonsterSpawnPeriod = normalMonsterSpawnPeriod;
            MimicSpawnPeriod = mimicSpawnPeriod;
            NormalMonsterSpawnTryCount = normalMonsterSpawnTryCount;
            MimicSpawnTryCount = mimicSpawnTryCount;
            MimicSpawnCountMin = mimicSpawnCountMin;
            MimicSpawnCountMax = mimicSpawnCountMax;
        }

        public static RunMutatorRoomSnapshot? Capture(object room)
        {
            object? dungeonMasterInfo = RunMutatorReflection.GetDungeonMasterInfo(room);
            if (dungeonMasterInfo == null)
                return null;

            return new RunMutatorRoomSnapshot(
                room,
                dungeonMasterInfo,
                RunMutatorReflection.ReadInt(dungeonMasterInfo, GameFieldNames.DungeonMasterInfo_NormalMonsterSpawnRate),
                RunMutatorReflection.ReadInt(dungeonMasterInfo, GameFieldNames.DungeonMasterInfo_MimicSpawnRate),
                RunMutatorReflection.ReadInt(dungeonMasterInfo, "NormalMonsterSpawnPeriod"),
                RunMutatorReflection.ReadInt(dungeonMasterInfo, "MimicSpawnPeriod"),
                RunMutatorReflection.ReadInt(dungeonMasterInfo, GameFieldNames.DungeonMasterInfo_NormalMonsterSpawnTryCount),
                RunMutatorReflection.ReadInt(dungeonMasterInfo, GameFieldNames.DungeonMasterInfo_MimicSpawnTryCount),
                RunMutatorReflection.ReadInt(dungeonMasterInfo, GameFieldNames.DungeonMasterInfo_MimicSpawnCountMin),
                RunMutatorReflection.ReadInt(dungeonMasterInfo, GameFieldNames.DungeonMasterInfo_MimicSpawnCountMax));
        }

        public void Restore()
        {
            if (DungeonMasterInfo == null)
                return;

            RunMutatorReflection.WriteInt(DungeonMasterInfo, GameFieldNames.DungeonMasterInfo_NormalMonsterSpawnRate, NormalMonsterSpawnRate);
            RunMutatorReflection.WriteInt(DungeonMasterInfo, GameFieldNames.DungeonMasterInfo_MimicSpawnRate, MimicSpawnRate);
            RunMutatorReflection.WriteInt(DungeonMasterInfo, "NormalMonsterSpawnPeriod", NormalMonsterSpawnPeriod);
            RunMutatorReflection.WriteInt(DungeonMasterInfo, "MimicSpawnPeriod", MimicSpawnPeriod);
            RunMutatorReflection.WriteInt(DungeonMasterInfo, GameFieldNames.DungeonMasterInfo_NormalMonsterSpawnTryCount, NormalMonsterSpawnTryCount);
            RunMutatorReflection.WriteInt(DungeonMasterInfo, GameFieldNames.DungeonMasterInfo_MimicSpawnTryCount, MimicSpawnTryCount);
            RunMutatorReflection.WriteInt(DungeonMasterInfo, GameFieldNames.DungeonMasterInfo_MimicSpawnCountMin, MimicSpawnCountMin);
            RunMutatorReflection.WriteInt(DungeonMasterInfo, GameFieldNames.DungeonMasterInfo_MimicSpawnCountMax, MimicSpawnCountMax);
        }

        public void ApplySpawnMultiplier(float multiplier)
        {
            if (DungeonMasterInfo == null)
                return;

            if (NormalMonsterSpawnRate.HasValue)
                RunMutatorReflection.WriteScaledInt(DungeonMasterInfo, GameFieldNames.DungeonMasterInfo_NormalMonsterSpawnRate, NormalMonsterSpawnRate.Value, multiplier);
            if (MimicSpawnRate.HasValue)
                RunMutatorReflection.WriteScaledInt(DungeonMasterInfo, GameFieldNames.DungeonMasterInfo_MimicSpawnRate, MimicSpawnRate.Value, multiplier);
            if (NormalMonsterSpawnPeriod.HasValue)
                RunMutatorReflection.WriteScaledInverseInt(DungeonMasterInfo, "NormalMonsterSpawnPeriod", NormalMonsterSpawnPeriod.Value, multiplier);
            if (MimicSpawnPeriod.HasValue)
                RunMutatorReflection.WriteScaledInverseInt(DungeonMasterInfo, "MimicSpawnPeriod", MimicSpawnPeriod.Value, multiplier);
            if (NormalMonsterSpawnTryCount.HasValue)
                RunMutatorReflection.WriteScaledInt(DungeonMasterInfo, GameFieldNames.DungeonMasterInfo_NormalMonsterSpawnTryCount, NormalMonsterSpawnTryCount.Value, multiplier);
            if (MimicSpawnTryCount.HasValue)
                RunMutatorReflection.WriteScaledInt(DungeonMasterInfo, GameFieldNames.DungeonMasterInfo_MimicSpawnTryCount, MimicSpawnTryCount.Value, multiplier);
            if (MimicSpawnCountMin.HasValue)
                RunMutatorReflection.WriteScaledInt(DungeonMasterInfo, GameFieldNames.DungeonMasterInfo_MimicSpawnCountMin, MimicSpawnCountMin.Value, multiplier);
            if (MimicSpawnCountMax.HasValue)
                RunMutatorReflection.WriteScaledInt(DungeonMasterInfo, GameFieldNames.DungeonMasterInfo_MimicSpawnCountMax, MimicSpawnCountMax.Value, multiplier);
        }
    }

    internal static class RunMutatorReflection
    {
        public static object? GetDungeonMasterInfo(object room)
        {
            return ReflectionHelper.GetFieldValue(room, "DungeonMasterInfo")
                ?? ReflectionHelper.GetFieldValue(room, "_dungeonMasterInfo")
                ?? ReflectionHelper.GetFieldValue(room, "dungeonMasterInfo");
        }

        public static int? ReadInt(object target, string fieldName)
        {
            object? value = ReflectionHelper.GetFieldValue(target, fieldName);
            if (value is int intValue)
                return intValue;
            if (value is short shortValue)
                return shortValue;
            if (value is long longValue)
                return (int)Math.Min(int.MaxValue, Math.Max(int.MinValue, longValue));
            return null;
        }

        public static void WriteInt(object target, string fieldName, int? value)
        {
            if (!value.HasValue)
                return;

            ReflectionHelper.SetFieldValue(target, fieldName, value.Value);
        }

        public static void WriteScaledInt(object target, string fieldName, int baseValue, float multiplier)
        {
            int value = Math.Max(1, (int)Math.Round(baseValue * multiplier));
            ReflectionHelper.SetFieldValue(target, fieldName, value);
        }

        public static void WriteScaledInverseInt(object target, string fieldName, int baseValue, float multiplier)
        {
            float safeMultiplier = multiplier <= 0.01f ? 0.01f : multiplier;
            int value = Math.Max(1, (int)Math.Round(baseValue / safeMultiplier));
            ReflectionHelper.SetFieldValue(target, fieldName, value);
        }
    }

    internal static class RunMutatorDraftRuntime
    {
        private static readonly List<RunMutatorRoomSnapshot> Snapshots = new List<RunMutatorRoomSnapshot>();
        private static string _activeRoomKey = "";
        private static RunMutatorPlan? _currentPlan;

        public static void Reset()
        {
            RestoreAll();
            _activeRoomKey = "";
            _currentPlan = null;
            RunMutatorDraftBridge.Reset();
        }

        public static void Update(RunMutatorDraftConfig config, float deltaTime)
        {
            if (config == null || !config.Enabled)
            {
                Reset();
                return;
            }

            var room = RoomAPI.GetCurrentRoom();
            if (room == null)
            {
                Reset();
                return;
            }

            if (NemesisMod.Instance?.IsHost != true)
                return;

            string roomKey = BuildRoomKey(room);
            if (!string.Equals(roomKey, _activeRoomKey, StringComparison.Ordinal))
            {
                RestoreAll();
                _activeRoomKey = roomKey;
                ApplyForRoom(room, config);
                return;
            }

            if (_currentPlan == null)
                ApplyForRoom(room, config);
        }

        private static void ApplyForRoom(object room, RunMutatorDraftConfig config)
        {
            var snapshot = RunMutatorRoomSnapshot.Capture(room);
            if (snapshot == null)
            {
                RunMutatorDraftBridge.Reset();
                return;
            }

            Snapshots.Add(snapshot);

            var context = BuildContext(room);
            var plan = RunMutatorDraftPlanner.BuildPlan(
                context,
                RunMutatorCatalog.GetDefaultDefinitions(),
                config.DraftChoiceCount,
                config.ActiveMutatorCount);

            _currentPlan = plan;
            ApplyPlan(plan, snapshot, config);
        }

        private static void ApplyPlan(RunMutatorPlan plan, RunMutatorRoomSnapshot snapshot, RunMutatorDraftConfig config)
        {
            float spawnMultiplier = config.SpawnPressureMultiplier;
            float noiseMultiplier = config.NoiseLeakMultiplier;
            float decayMultiplier = config.NoiseDecayMultiplier;
            int preferredWeatherId = config.DefaultPreferredWeatherId;

            foreach (var mutator in plan.ActiveMutators)
            {
                switch (mutator.EffectKind)
                {
                    case RunMutatorEffectKind.SpawnPressure:
                        spawnMultiplier *= config.SpawnPressureMultiplier;
                        break;
                    case RunMutatorEffectKind.WeatherBias:
                        preferredWeatherId = config.DefaultPreferredWeatherId;
                        break;
                    case RunMutatorEffectKind.NoiseLeak:
                        noiseMultiplier *= config.NoiseLeakMultiplier;
                        decayMultiplier *= config.NoiseDecayMultiplier;
                        break;
                    case RunMutatorEffectKind.Hybrid:
                        spawnMultiplier *= config.HybridMultiplier;
                        noiseMultiplier *= config.HybridMultiplier;
                        decayMultiplier *= 0.95f;
                        break;
                }
            }

            snapshot.ApplySpawnMultiplier(spawnMultiplier);

            RunMutatorDraftBridge.Set(new RunMutatorBridgeState
            {
                SpawnMultiplier = spawnMultiplier,
                NoiseMultiplier = noiseMultiplier,
                NoiseDecayMultiplier = decayMultiplier,
                PreferredWeatherId = preferredWeatherId,
                DraftName = plan.ActiveMutators.Count > 0 ? plan.ActiveMutators[0].Name : "Idle Draft",
                ActiveMutators = plan.ActiveMutators.Select(x => x.Name).ToArray()
            });

            Log.Msg("RunMutatorDraft", $"Applied {plan.ActiveMutators.Count} mutators to room {BuildRoomKey(room: snapshot.Room)}");
        }

        private static void RestoreAll()
        {
            foreach (var snapshot in Snapshots)
            {
                try
                {
                    snapshot.Restore();
                }
                catch { }
            }

            Snapshots.Clear();
            _currentPlan = null;
            RunMutatorDraftBridge.Reset();
        }

        private static RunMutatorContext BuildContext(object room)
        {
            int playerCount = RoomAPI.GetRoomPlayers(room as IVroom).Count;
            int gameDay = RoomAPI.GetCurrentGameDay(room as IVroom);
            int sessionCycle = RoomAPI.GetCurrentSessionCycle(room as IVroom);
            int weatherId = WeatherAPI.GetCurrentWeatherMasterID(room as IVroom);
            float noisePressure = 0f;

            return new RunMutatorContext(playerCount, gameDay, sessionCycle, weatherId, noisePressure);
        }

        private static string BuildRoomKey(object room)
        {
            long roomId = RoomAPI.GetRoomID(room as IVroom);
            if (roomId != 0)
                return roomId.ToString();
            return room.GetHashCode().ToString();
        }
    }
}

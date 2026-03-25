using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Nemesis.Core;

namespace Nemesis.Modules.EnemyDropLoot.Patches
{
    [HarmonyPatch]
    internal class DungeonRoomInitializePatch
    {
        static IEnumerable<MethodBase> TargetMethods()
        {
            return GameReflection.FindTargetMethods(true,
                (GameTypeNames.DungeonRoom, GameMethodNames.DungeonRoom_Initialize));
        }

        static void Postfix(object __instance)
        {
            try { LootPoolManager.ConfigureForDungeon(__instance); }
            catch (Exception ex) { Log.LootDrop.Warn($"DungeonRoomInitializePatch failed: {ex.Message}"); }
        }
    }

    [HarmonyPatch]
    internal class DungeonRoomVacatePatch
    {
        static IEnumerable<MethodBase> TargetMethods()
        {
            return GameReflection.FindTargetMethods(true,
                (GameTypeNames.IVroom, GameMethodNames.IVroom_OnVacateRoom));
        }

        static void Postfix(object __instance)
        {
            try
            {
                var dungeonRoomType = GameReflection.GetGameType(GameTypeNames.DungeonRoom);
                if (dungeonRoomType != null && dungeonRoomType.IsInstanceOfType(__instance))
                    LootPoolManager.Reset();
            }
            catch (Exception ex) { Log.LootDrop.Warn($"DungeonRoomVacatePatch failed: {ex.Message}"); }
        }
    }

    [HarmonyPatch]
    internal class VMonsterOnDeadPatch
    {
        // Deduplication: don't process same monster death twice
        private static readonly HashSet<int> _processedDeaths = new HashSet<int>();

        static IEnumerable<MethodBase> TargetMethods()
        {
            return GameReflection.FindTargetMethods(true,
                (GameTypeNames.VMonster, GameMethodNames.VMonster_OnDead));
        }

        static void Prefix(object __instance)
        {
            if (!EnemyDropLootModule.IsEnabled) return;

            try
            {
                int hash = __instance.GetHashCode();
                if (!_processedDeaths.Add(hash)) return;

                LootPoolManager.TryHandleMonsterDeath(__instance);
            }
            catch (Exception ex) { Log.LootDrop.Warn($"VMonsterOnDeadPatch failed: {ex.Message}"); }
        }

        internal static void ClearProcessedDeaths()
        {
            _processedDeaths.Clear();
        }
    }
}

using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using MimicAPI.GameAPI;
using Nemesis.Core;

namespace Nemesis.Modules.PersistentProgression.Patches
{
    [HarmonyPatch]
    internal class ClientLootPatch
    {
        static IEnumerable<MethodBase> TargetMethods()
        {
            return GameReflection.FindTargetMethods(
                (GameTypeNames.ProtoActor, GameMethodNames.ProtoActor_GrapLootingObject));
        }

        static void Postfix(object __instance)
        {
            try
            {
                if (!GameReflection.IsLocalAvatar(__instance)) return;
                ModuleEventBus.RaiseLootCollected();
            }
            catch { }
        }
    }

    [HarmonyPatch]
    internal class ClientSellPatch
    {
        static IEnumerable<MethodBase> TargetMethods()
        {
            return GameReflection.FindTargetMethods(
                (GameTypeNames.ProtoActor, GameMethodNames.ProtoActor_BarterItem));
        }

        static void Postfix(object __instance)
        {
            try
            {
                if (!GameReflection.IsLocalAvatar(__instance)) return;
                ModuleEventBus.RaiseItemSold();
            }
            catch { }
        }
    }

    [HarmonyPatch]
    internal class ClientDeathPatch
    {
        // Deduplication: prevent repeat XP for same actor death
        private static readonly HashSet<int> _recentDeaths = new HashSet<int>();

        static IEnumerable<MethodBase> TargetMethods()
        {
            return GameReflection.FindTargetMethods(
                (GameTypeNames.ProtoActor, GameMethodNames.ProtoActor_OnActorDeath));
        }

        static void Postfix(object __instance)
        {
            try
            {
                // Skip on host to avoid double XP with KillRewardPatch
                if (NemesisMod.Instance?.IsHost == true) return;

                var monsterMasterId = ReflectionHelper.GetPropertyValue<int>(
                    __instance, GamePropertyNames.ProtoActor_MonsterMasterID);
                if (monsterMasterId <= 0) return;

                // Deduplicate by instance hash
                int hash = __instance.GetHashCode();
                if (!_recentDeaths.Add(hash)) return;

                ModuleEventBus.RaiseMonsterKilled();
            }
            catch { }
        }

        internal static void ClearRecentDeaths()
        {
            _recentDeaths.Clear();
        }
    }
}

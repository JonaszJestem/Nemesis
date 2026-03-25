using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Nemesis.Core;

namespace Nemesis.Modules.PersistentProgression.Patches
{
    [HarmonyPatch]
    internal class KillRewardPatch
    {
        // Deduplication: track recently killed monster hashes to prevent repeat XP
        private static readonly HashSet<int> _recentKills = new HashSet<int>();

        static IEnumerable<MethodBase> TargetMethods()
        {
            return GameReflection.FindTargetMethods(true,
                (GameTypeNames.VMonster, GameMethodNames.VMonster_OnDead),
                (GameTypeNames.VMonster, GameMethodNames.VMonster_Die));
        }

        static void Postfix(object __instance)
        {
            try
            {
                int hash = __instance.GetHashCode();
                if (!_recentKills.Add(hash)) return; // Already processed

                ModuleEventBus.RaiseMonsterKilled();
            }
            catch { }
        }

        internal static void ClearRecentKills()
        {
            _recentKills.Clear();
        }
    }
}

using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Nemesis.Core;
using Nemesis.Modules.DifficultyDirector.Patches;
using Nemesis.Modules.EnemyDropLoot.Patches;
using Nemesis.Modules.PersistentProgression.Patches;

namespace Nemesis.Modules.RoleSystem.Patches
{
    [HarmonyPatch]
    internal class SessionStartPatch
    {
        static IEnumerable<MethodBase> TargetMethods()
        {
            var method = GameReflection.FindMethod(GameTypeNames.GameSessionInfo, GameMethodNames.GameSessionInfo_StartSession, includeStatic: true);
            if (method != null)
                return new List<MethodBase> { method };

            return GameReflection.FindTargetMethods(
                (GameTypeNames.VRoomManager, GameMethodNames.VRoomManager_EnterWaitingRoom));
        }

        static void Postfix()
        {
            try
            {
                // Clear all deduplication sets for new session
                MonsterStatPatch.ClearTracking();
                KillRewardPatch.ClearRecentKills();
                ClientDeathPatch.ClearRecentDeaths();
                VMonsterOnDeadPatch.ClearProcessedDeaths();

                ModuleEventBus.RaiseSessionStarted();
            }
            catch { }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Nemesis.Core;

namespace Nemesis.Modules.PossessionPlus.Patches
{
    [HarmonyPatch]
    internal static class LocalPlayerDeathPatch
    {
        static IEnumerable<MethodBase> TargetMethods()
        {
            return GameReflection.FindTargetMethods(true,
                (GameTypeNames.ProtoActor, GameMethodNames.ProtoActor_OnActorDeath));
        }

        static void Postfix(object __instance)
        {
            try
            {
                if (GameReflection.IsLocalAvatar(__instance))
                    PossessionPlusRuntime.NotifyLocalDeath(__instance);
            }
            catch (Exception ex)
            {
                Log.Warn("PossessionPlus", $"Death patch failed: {ex.Message}");
            }
        }
    }
}

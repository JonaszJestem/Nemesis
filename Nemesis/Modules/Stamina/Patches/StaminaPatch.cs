using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using MimicAPI.GameAPI;
using Nemesis.Core;

namespace Nemesis.Modules.Stamina.Patches
{
    /// <summary>
    /// Skip stamina consumption when infinite stamina is enabled.
    /// </summary>
    [HarmonyPatch]
    internal class StaminaConsumePatch
    {
        static IEnumerable<MethodBase> TargetMethods()
        {
            return GameReflection.FindTargetMethods(
                (GameTypeNames.StatManager, GameMethodNames.StatManager_ConsumeStamina));
        }

        static bool Prefix()
        {
            return !StaminaModule.IsEnabled;
        }
    }

    /// <summary>
    /// Force GetCurrentStamina to return max so the UI always shows full.
    /// </summary>
    [HarmonyPatch]
    internal class StaminaGetCurrentPatch
    {
        static IEnumerable<MethodBase> TargetMethods()
        {
            return GameReflection.FindTargetMethods(
                (GameTypeNames.StatManager, GameMethodNames.StatManager_GetCurrentStamina));
        }

        static void Postfix(ref long __result)
        {
            if (StaminaModule.IsEnabled)
                __result = long.MaxValue / 2;
        }
    }

    /// <summary>
    /// Skip stamina regeneration (not needed when stamina is always max).
    /// </summary>
    [HarmonyPatch]
    internal class StaminaRegenPatch
    {
        static IEnumerable<MethodBase> TargetMethods()
        {
            return GameReflection.FindTargetMethods(
                (GameTypeNames.StatManager, GameMethodNames.StatManager_RegenerateStamina));
        }

        static bool Prefix()
        {
            return !StaminaModule.IsEnabled;
        }
    }
}

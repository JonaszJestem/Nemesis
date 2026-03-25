using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Nemesis.Core;

namespace Nemesis.Modules.InventoryExpansion.Patches
{
    [HarmonyPatch]
    internal class InventoryPatch
    {
        static IEnumerable<MethodBase> TargetMethods()
        {
            return GameReflection.FindTargetMethods(
                (GameTypeNames.InventoryController, GameMethodNames.InventoryController_InvenFull));
        }

        static bool Prefix(ref bool __result)
        {
            if (InventoryExpansionModule.IsEnabled)
            {
                __result = false; // Inventory is never full
                return false;     // Skip original method
            }

            return true;
        }
    }
}

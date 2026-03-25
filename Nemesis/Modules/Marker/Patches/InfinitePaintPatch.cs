using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Nemesis.Core;

namespace Nemesis.Modules.Marker.Patches
{
    [HarmonyPatch]
    internal class InfinitePaintPatch
    {
        static IEnumerable<MethodBase> TargetMethods()
        {
            return GameReflection.FindTargetMethods(
                (GameTypeNames.EquipmentItemElement, GameMethodNames.EquipmentItemElement_SetDurability));
        }

        static bool Prefix()
        {
            if (MarkerModule.InfinitePaintballs)
                return false; // Skip durability reduction

            return true;
        }
    }
}

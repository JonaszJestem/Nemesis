using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using MimicAPI.GameAPI;
using Nemesis.Core;

namespace Nemesis.Modules.PersistentProgression.Patches
{
    [HarmonyPatch]
    internal class KillRewardPatch
    {
        static IEnumerable<MethodBase> TargetMethods()
        {
            var methods = new List<MethodBase>();

            try
            {
                var assembly = ServerNetworkAPI.GetGameAssembly();
                if (assembly == null) return methods;

                // Only patch VMonster.OnDead — not VActor — to avoid double XP
                var vMonsterType = assembly.GetType("VMonster");
                if (vMonsterType != null)
                {
                    var onDeadMethod = vMonsterType.GetMethod("OnDead",
                        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    if (onDeadMethod != null)
                    {
                        methods.Add(onDeadMethod);
                        return methods;
                    }

                    var dieMethod = vMonsterType.GetMethod("Die",
                        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    if (dieMethod != null)
                        methods.Add(dieMethod);
                }
            }
            catch { }

            return methods;
        }

        static void Postfix()
        {
            try
            {
                ModuleEventBus.RaiseMonsterKilled();
            }
            catch { }
        }
    }
}

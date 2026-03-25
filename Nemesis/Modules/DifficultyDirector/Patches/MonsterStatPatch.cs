using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using MimicAPI.GameAPI;

namespace Nemesis.Modules.DifficultyDirector.Patches
{
    [HarmonyPatch]
    internal class MonsterStatPatch
    {
        // Track scaled instances to prevent double-multiplication
        private static readonly HashSet<int> _scaledInstances = new HashSet<int>();

        static IEnumerable<MethodBase> TargetMethods()
        {
            var methods = new List<MethodBase>();

            try
            {
                var assembly = ServerNetworkAPI.GetGameAssembly();
                if (assembly == null) return methods;

                var vMonsterType = assembly.GetType("VMonster");
                if (vMonsterType != null)
                {
                    // Only patch InitStat — not SetStat — to avoid double-multiplication
                    var initMethod = vMonsterType.GetMethod("InitStat",
                        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    if (initMethod != null)
                        methods.Add(initMethod);
                }
            }
            catch { }

            return methods;
        }

        static void Postfix(object __instance)
        {
            try
            {
                float multiplier = DifficultyDirectorModule.CurrentMultiplier;
                if (Math.Abs(multiplier - 1.0f) < 0.01f) return;

                int hash = __instance.GetHashCode();
                if (_scaledInstances.Contains(hash)) return;
                _scaledInstances.Add(hash);

                var flags = BindingFlags.NonPublic | BindingFlags.Instance;

                var hpField = __instance.GetType().GetField("_hp", flags);
                if (hpField != null)
                {
                    var hp = hpField.GetValue(__instance);
                    if (hp is int intHp)
                        hpField.SetValue(__instance, (int)(intHp * multiplier));
                    else if (hp is float floatHp)
                        hpField.SetValue(__instance, floatHp * multiplier);
                }

                var atkField = __instance.GetType().GetField("_atk", flags);
                if (atkField != null)
                {
                    var atk = atkField.GetValue(__instance);
                    if (atk is int intAtk)
                        atkField.SetValue(__instance, (int)(intAtk * multiplier));
                    else if (atk is float floatAtk)
                        atkField.SetValue(__instance, floatAtk * multiplier);
                }
            }
            catch { }
        }

        // Clear tracking on session start to allow fresh scaling
        internal static void ClearTracking() => _scaledInstances.Clear();
    }
}

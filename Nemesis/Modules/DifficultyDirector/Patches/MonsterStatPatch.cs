using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using MimicAPI.GameAPI;
using Nemesis.Core;

namespace Nemesis.Modules.DifficultyDirector.Patches
{
    [HarmonyPatch]
    internal class MonsterStatPatch
    {
        private static readonly HashSet<int> _scaledInstances = new HashSet<int>();

        private static Type? _statTypeEnum;
        private static object? _hpEnumValue;
        private static object? _attackEnumValue;

        static IEnumerable<MethodBase> TargetMethods()
        {
            var methods = new List<MethodBase>();
            try
            {
                var method = GameReflection.FindMethod(GameTypeNames.StatController, GameMethodNames.StatController_LoadStats);
                if (method != null)
                    methods.Add(method);

                _statTypeEnum = GameReflection.GetGameType(GameTypeNames.StatType);
                if (_statTypeEnum != null)
                {
                    _hpEnumValue = Enum.Parse(_statTypeEnum, GameEnumValues.StatType_HP);
                    _attackEnumValue = Enum.Parse(_statTypeEnum, GameEnumValues.StatType_Attack);
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

                var self = ReflectionHelper.GetPropertyValue(__instance, GamePropertyNames.StatController_Self);
                if (self == null) return;

                var vMonsterType = GameReflection.GetGameType(GameTypeNames.VMonster);
                if (vMonsterType == null || !vMonsterType.IsInstanceOfType(self)) return;

                int hash = self.GetHashCode();
                if (_scaledInstances.Contains(hash)) return;
                _scaledInstances.Add(hash);

                var statManager = ReflectionHelper.GetFieldValue(__instance, GameFieldNames.StatController_StatManager);
                if (statManager == null) return;

                var totalStats = ReflectionHelper.GetFieldValue(statManager, GameFieldNames.StatManager_TotalStats);
                if (totalStats == null) return;

                var elements = ReflectionHelper.GetFieldValue(totalStats, GameFieldNames.StatCollection_Elements) as IDictionary;
                if (elements == null) return;

                if (_hpEnumValue != null && elements.Contains(_hpEnumValue))
                    ScaleStatElement(elements[_hpEnumValue], multiplier);

                if (_attackEnumValue != null && elements.Contains(_attackEnumValue))
                    ScaleStatElement(elements[_attackEnumValue], multiplier);
            }
            catch { }
        }

        private static void ScaleStatElement(object? statsElement, float multiplier)
        {
            if (statsElement == null) return;
            var valueProp = statsElement.GetType().GetProperty(GamePropertyNames.StatElement_Value,
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (valueProp == null) return;

            var currentValue = valueProp.GetValue(statsElement);
            if (currentValue is long longVal)
                valueProp.SetValue(statsElement, (long)(longVal * multiplier));
        }

        internal static void ClearTracking() => _scaledInstances.Clear();
    }
}

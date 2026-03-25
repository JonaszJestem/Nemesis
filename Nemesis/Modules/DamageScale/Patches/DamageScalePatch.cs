using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Nemesis.Core;

namespace Nemesis.Modules.DamageScale.Patches
{
    [HarmonyPatch]
    internal class DamageScalePatch
    {
        private static FieldInfo? _damageField;

        static IEnumerable<MethodBase> TargetMethods()
        {
            var methods = GameReflection.FindTargetMethods(
                (GameTypeNames.StatManager, GameMethodNames.StatManager_ApplyDamage));

            // Cache the damage field from ApplyDamageArgs for use in the prefix
            try
            {
                if (methods.Count > 0)
                {
                    var parameters = methods[0].GetParameters();
                    if (parameters.Length > 0)
                    {
                        var argsType = parameters[0].ParameterType;
                        _damageField = argsType.GetField("damage", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                            ?? argsType.GetField("Damage", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Damage.Warn($"Failed to cache damage field: {ex.Message}");
            }

            return methods;
        }

        static void Prefix(object[] __args)
        {
            if (!DamageScaleModule.IsEnabled) return;
            if (__args == null || __args.Length == 0) return;
            if (_damageField == null) return;

            try
            {
                float multiplier = DamageScaleModule.CurrentMultiplier;
                if (Math.Abs(multiplier - 1.0f) < 0.01f) return;

                object args = __args[0];
                object? damageValue = _damageField.GetValue(args);

                if (damageValue is long longDmg)
                {
                    _damageField.SetValue(args, (long)(longDmg * multiplier));
                    __args[0] = args;
                }
                else if (damageValue is int intDmg)
                {
                    _damageField.SetValue(args, (int)(intDmg * multiplier));
                    __args[0] = args;
                }
                else if (damageValue is float floatDmg)
                {
                    _damageField.SetValue(args, floatDmg * multiplier);
                    __args[0] = args;
                }
            }
            catch { }
        }
    }
}

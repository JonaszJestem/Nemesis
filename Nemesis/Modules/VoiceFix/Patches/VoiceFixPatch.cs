using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using MimicAPI.GameAPI;
using Nemesis.Core;

namespace Nemesis.Modules.VoiceFix.Patches
{
    /// <summary>
    /// Patches VoiceEffectPreset property getters to return zeroed-out filter presets,
    /// effectively removing all voice distortion effects from the mimic.
    /// </summary>
    [HarmonyPatch]
    internal class VoiceFixPatch
    {
        static IEnumerable<MethodBase> TargetMethods()
        {
            var methods = new List<MethodBase>();
            try
            {
                var assembly = ServerNetworkAPI.GetGameAssembly();
                if (assembly == null) return methods;

                var type = assembly.GetType(GameTypeNames.VoiceEffectPreset);
                if (type == null) return methods;

                // Patch all property getters (get_LowPass, get_HighPass, etc.)
                // These return filter preset objects - we'll zero their fields in the postfix
                foreach (var prop in type.GetProperties(
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
                {
                    var getter = prop.GetGetMethod();
                    if (getter != null)
                        methods.Add(getter);
                }

                if (methods.Count > 0)
                    Log.VoiceFix.Msg($"Found {methods.Count} VoiceEffectPreset getters to patch");
            }
            catch (Exception ex)
            {
                Log.VoiceFix.Warn($"TargetMethods error: {ex.Message}");
            }

            return methods;
        }

        /// <summary>
        /// After each filter preset getter returns, zero out all numeric fields
        /// on the returned filter object (frequency, level, gain, etc.)
        /// </summary>
        static void Postfix(ref object __result)
        {
            if (!VoiceFixModule.IsEnabled) return;
            if (__result == null) return;

            try
            {
                var type = __result.GetType();
                var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

                foreach (var field in type.GetFields(flags))
                {
                    if (field.FieldType == typeof(float))
                        field.SetValue(__result, 0f);
                    else if (field.FieldType == typeof(int))
                        field.SetValue(__result, 0);
                    else if (field.FieldType == typeof(bool))
                        field.SetValue(__result, false);
                }
            }
            catch { }
        }
    }
}

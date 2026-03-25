using Nemesis.Modules.DamageScale;
using UnityEngine;

namespace Nemesis.UI.Tabs
{
    internal static class DamageScaleTab
    {
        public static void Draw(DamageScaleConfig config)
        {
            config.Enabled = GUIStyles.LabeledToggle("Enable Damage Scaling", config.Enabled);

            GUILayout.Space(10);
            GUILayout.Label("Settings", GUIStyles.SubHeader);
            GUILayout.BeginVertical(GUIStyles.SectionBox);
            config.DamageMultiplier = GUIStyles.LabeledSlider("Damage Multiplier", config.DamageMultiplier, 0.1f, 2.0f);
            GUILayout.EndVertical();

            GUILayout.Space(10);
            GUILayout.Label("Info", GUIStyles.SubHeader);
            GUILayout.BeginVertical(GUIStyles.SectionBox);
            GUILayout.Label("Scales all incoming damage by the multiplier.", GUIStyles.Label);
            GUILayout.Label("Values below 1.0 reduce damage, above 1.0 increase it.", GUIStyles.Label);
            GUILayout.Label("This is a host-only setting affecting all players.", GUIStyles.Label);
            GUILayout.EndVertical();

            GUILayout.Space(10);
            GUILayout.Label($"Current Multiplier: x{DamageScaleModule.CurrentMultiplier:F2}", GUIStyles.SubHeader);
        }
    }
}

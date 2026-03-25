using Nemesis.Modules.MoreMimics;
using UnityEngine;

namespace Nemesis.UI.Tabs
{
    internal static class MoreMimicsTab
    {
        public static void Draw(MoreMimicsConfig config)
        {
            config.Enabled = GUIStyles.LabeledToggle("Enable More Mimics", config.Enabled);

            GUILayout.Space(10);
            GUILayout.Label("Spawn Settings (Host Only)", GUIStyles.SubHeader);
            GUILayout.BeginVertical(GUIStyles.SectionBox);
            config.SpawnRateMultiplier = GUIStyles.LabeledSlider("Spawn Rate Multiplier", config.SpawnRateMultiplier, 1.0f, 5.0f, "F1");
            GUILayout.Space(5);
            GUILayout.Label("Scales monster and mimic spawn rates, counts, and try counts.", GUIStyles.Label);
            GUILayout.Label("Changes apply when entering a new dungeon room.", GUIStyles.Label);
            GUILayout.EndVertical();
        }
    }
}

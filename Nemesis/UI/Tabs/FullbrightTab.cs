using Nemesis.Modules.Fullbright;
using UnityEngine;

namespace Nemesis.UI.Tabs
{
    internal static class FullbrightTab
    {
        public static void Draw(FullbrightConfig config)
        {
            config.Enabled = GUIStyles.LabeledToggle("Enable Fullbright", config.Enabled);

            GUILayout.Space(10);
            GUILayout.Label("Lighting Settings", GUIStyles.SubHeader);
            GUILayout.BeginVertical(GUIStyles.SectionBox);
            config.AmbientIntensity = GUIStyles.LabeledSlider("Ambient Intensity", config.AmbientIntensity, 0.5f, 3.0f, "F1");
            GUILayout.Space(5);
            GUILayout.Label("Brightens the entire scene. Useful for dark dungeons.", GUIStyles.Label);
            GUILayout.EndVertical();
        }
    }
}

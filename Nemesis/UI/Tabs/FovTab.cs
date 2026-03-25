using Nemesis.Modules.Fov;
using UnityEngine;

namespace Nemesis.UI.Tabs
{
    internal static class FovTab
    {
        public static void Draw(FovConfig config)
        {
            config.Enabled = GUIStyles.LabeledToggle("Enable FOV Override", config.Enabled);

            GUILayout.Space(10);
            GUILayout.Label("Settings", GUIStyles.SubHeader);
            GUILayout.BeginVertical(GUIStyles.SectionBox);
            config.FieldOfView = GUIStyles.LabeledSlider("Field of View", config.FieldOfView, 30f, 120f, "F0");
            GUILayout.EndVertical();
        }
    }
}

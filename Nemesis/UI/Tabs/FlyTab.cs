using Nemesis.Modules.Fly;
using UnityEngine;

namespace Nemesis.UI.Tabs
{
    internal static class FlyTab
    {
        public static void Draw(FlyConfig config)
        {
            config.Enabled = GUIStyles.LabeledToggle("Enable Fly", config.Enabled);

            GUILayout.Space(10);
            GUILayout.Label("Settings", GUIStyles.SubHeader);
            GUILayout.BeginVertical(GUIStyles.SectionBox);
            config.FlySpeed = GUIStyles.LabeledSlider("Fly Speed", config.FlySpeed, 1f, 50f, "F1");
            GUILayout.EndVertical();
        }
    }
}

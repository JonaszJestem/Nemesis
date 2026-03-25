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
            config.UseSpaceShiftVertical = GUIStyles.LabeledToggle("Use Space/Shift Vertical", config.UseSpaceShiftVertical);
            string controlsText = config.UseSpaceShiftVertical
                ? "Controls: WASD move, Space up, LeftShift down"
                : "Controls: WASD move, E up, Q down";
            GUILayout.Label(controlsText, GUIStyles.Label);
            GUILayout.EndVertical();
        }
    }
}

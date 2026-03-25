using Nemesis.Modules.Jump;
using UnityEngine;

namespace Nemesis.UI.Tabs
{
    internal static class JumpTab
    {
        public static void Draw(JumpConfig config)
        {
            config.Enabled = GUIStyles.LabeledToggle("Enable Better Jump", config.Enabled);

            GUILayout.Space(10);
            GUILayout.Label("Jump Settings", GUIStyles.SubHeader);
            GUILayout.BeginVertical(GUIStyles.SectionBox);
            config.JumpVelocity = GUIStyles.LabeledSlider("Jump Velocity", config.JumpVelocity, 1.0f, 15.0f, "F1");
            GUILayout.Space(5);
            GUILayout.Label("Press Space to jump. Higher velocity = higher jump.", GUIStyles.Label);
            GUILayout.EndVertical();
        }
    }
}

using Nemesis.Modules.MoreVoices;
using UnityEngine;

namespace Nemesis.UI.Tabs
{
    internal static class MoreVoicesTab
    {
        public static void Draw(MoreVoicesConfig config)
        {
            config.Enabled = GUIStyles.LabeledToggle("Enable More Voices", config.Enabled);

            GUILayout.Space(10);
            GUILayout.Label("Settings", GUIStyles.SubHeader);
            GUILayout.BeginVertical(GUIStyles.SectionBox);
            config.MaxRecordings = GUIStyles.LabeledIntSlider("Max Recordings", config.MaxRecordings, 5, 50);
            GUILayout.EndVertical();

            GUILayout.Space(10);
            GUILayout.Label("Info", GUIStyles.SubHeader);
            GUILayout.BeginVertical(GUIStyles.SectionBox);
            GUILayout.Label("Increases the maximum number of voice recordings per player.", GUIStyles.Label);
            GUILayout.Label("This is a host-only setting synced to all players.", GUIStyles.Label);
            GUILayout.EndVertical();
        }
    }
}

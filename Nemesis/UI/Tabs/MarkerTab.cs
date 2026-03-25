using Nemesis.Modules.Marker;
using UnityEngine;

namespace Nemesis.UI.Tabs
{
    internal static class MarkerTab
    {
        public static void Draw(MarkerConfig config)
        {
            config.Enabled = GUIStyles.LabeledToggle("Enable Marker Mod", config.Enabled);

            GUILayout.Space(10);
            GUILayout.Label("Paintball Settings", GUIStyles.SubHeader);
            GUILayout.BeginVertical(GUIStyles.SectionBox);
            config.InfinitePaintballs = GUIStyles.LabeledToggle("Infinite Paintballs", config.InfinitePaintballs);
            GUILayout.Label("Prevents paintball durability from decreasing.", GUIStyles.Label);
            GUILayout.EndVertical();

            GUILayout.Space(10);
            GUILayout.Label("Info", GUIStyles.SubHeader);
            GUILayout.BeginVertical(GUIStyles.SectionBox);
            GUILayout.Label("This is a host-only mod - settings are synced to all players.", GUIStyles.Label);
            GUILayout.EndVertical();
        }
    }
}

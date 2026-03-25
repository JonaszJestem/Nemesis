using Nemesis.Modules.ProximityRadar;
using UnityEngine;

namespace Nemesis.UI.Tabs
{
    internal static class RadarTab
    {
        public static void Draw(RadarConfig config)
        {
            config.Enabled = GUIStyles.LabeledToggle("Enable Proximity Radar", config.Enabled);

            GUILayout.Space(10);
            GUILayout.Label("Display", GUIStyles.SubHeader);
            GUILayout.BeginVertical(GUIStyles.SectionBox);
            config.Range = GUIStyles.LabeledSlider("Detection Range (m)", config.Range, 10f, 200f, "F0");
            config.RadarSize = GUIStyles.LabeledIntSlider("Radar Size (px)", config.RadarSize, 100, 400);
            config.RadarOpacity = GUIStyles.LabeledSlider("Opacity", config.RadarOpacity, 0.1f, 1.0f);
            config.UpdateRate = GUIStyles.LabeledSlider("Update Rate (s)", config.UpdateRate, 0.1f, 2.0f);
            GUILayout.EndVertical();

            GUILayout.Space(10);
            GUILayout.Label("Entity Filters", GUIStyles.SubHeader);
            GUILayout.BeginVertical(GUIStyles.SectionBox);
            config.ShowMonsters = GUIStyles.LabeledToggle("Show Monsters (Red)", config.ShowMonsters);
            config.ShowLoot = GUIStyles.LabeledToggle("Show Loot (Yellow)", config.ShowLoot);
            config.ShowPlayers = GUIStyles.LabeledToggle("Show Players (Green)", config.ShowPlayers);
            GUILayout.EndVertical();

            GUILayout.Space(10);
            GUILayout.Label("Dot Sizes", GUIStyles.SubHeader);
            GUILayout.BeginVertical(GUIStyles.SectionBox);
            config.MonsterDotSize = GUIStyles.LabeledIntSlider("Monster Dot Size", config.MonsterDotSize, 1, 10);
            config.LootDotSize = GUIStyles.LabeledIntSlider("Loot Dot Size", config.LootDotSize, 1, 10);
            config.PlayerDotSize = GUIStyles.LabeledIntSlider("Player Dot Size", config.PlayerDotSize, 1, 10);
            GUILayout.EndVertical();

            GUILayout.Space(10);
            GUILayout.Label("Position", GUIStyles.SubHeader);
            GUILayout.BeginVertical(GUIStyles.SectionBox);
            config.OffsetX = GUIStyles.LabeledIntSlider("Offset X (from right)", config.OffsetX, 0, 500);
            config.OffsetY = GUIStyles.LabeledIntSlider("Offset Y (from top)", config.OffsetY, 0, 500);
            GUILayout.EndVertical();
        }
    }
}

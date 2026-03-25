using Nemesis.Modules.Esp;
using UnityEngine;

namespace Nemesis.UI.Tabs
{
    internal static class EspTab
    {
        public static void Draw(EspConfig config)
        {
            config.Enabled = GUIStyles.LabeledToggle("Enable ESP", config.Enabled);

            GUILayout.Space(10);
            GUILayout.Label("Display Options", GUIStyles.SubHeader);
            GUILayout.BeginVertical(GUIStyles.SectionBox);
            config.ShowDistance = GUIStyles.LabeledToggle("Show Distance", config.ShowDistance);
            config.MaxRange = GUIStyles.LabeledSlider("Max Range", config.MaxRange, 10f, 200f, "F0");
            config.ShowMonsters = GUIStyles.LabeledToggle("Show Monsters", config.ShowMonsters);
            config.ShowLoot = GUIStyles.LabeledToggle("Show Loot", config.ShowLoot);
            config.ShowPlayers = GUIStyles.LabeledToggle("Show Players", config.ShowPlayers);
            GUILayout.EndVertical();
        }
    }
}

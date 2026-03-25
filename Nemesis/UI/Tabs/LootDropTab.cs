using Nemesis.Modules.EnemyDropLoot;
using UnityEngine;

namespace Nemesis.UI.Tabs
{
    internal static class LootDropTab
    {
        public static void Draw(LootDropConfig config)
        {
            config.Enabled = GUIStyles.LabeledToggle("Enable Enemy Loot Drops", config.Enabled);

            GUILayout.Space(10);
            GUILayout.Label("Settings", GUIStyles.SubHeader);
            GUILayout.BeginVertical(GUIStyles.SectionBox);
            config.DropChance = GUIStyles.LabeledSlider("Drop Chance", config.DropChance, 0f, 1f);
            config.MaxDropsPerKill = GUIStyles.LabeledIntSlider("Max Drops Per Kill", config.MaxDropsPerKill, 1, 5);
            GUILayout.EndVertical();

            GUILayout.Space(10);
            GUILayout.Label("Info", GUIStyles.SubHeader);
            GUILayout.BeginVertical(GUIStyles.SectionBox);
            GUILayout.Label("When a monster dies, rolls for bonus loot drop XP.", GUIStyles.Label);
            GUILayout.Label("Each potential drop is rolled independently against Drop Chance.", GUIStyles.Label);
            GUILayout.Label("This is a host-only setting synced to all players.", GUIStyles.Label);
            GUILayout.EndVertical();
        }
    }
}

using Nemesis.Modules.RivalGhosts;
using UnityEngine;

namespace Nemesis.UI.Tabs
{
    internal static class RivalGhostsTab
    {
        public static void Draw(RivalGhostsConfig config)
        {
            config.Enabled = GUIStyles.LabeledToggle("Enable Rival Ghosts", config.Enabled);

            GUILayout.Space(10);
            GUILayout.Label("Session Records", GUIStyles.SubHeader);
            GUILayout.BeginVertical(GUIStyles.SectionBox);
            config.ShowHud = GUIStyles.LabeledToggle("Show HUD", config.ShowHud);
            config.StorageLimit = GUIStyles.LabeledIntSlider("Storage limit", config.StorageLimit, 5, 100);
            config.TopRecordsToShow = GUIStyles.LabeledIntSlider("Top records shown", config.TopRecordsToShow, 1, 10);
            config.AutoSaveIntervalSeconds = GUIStyles.LabeledSlider("Autosave interval", config.AutoSaveIntervalSeconds, 10f, 120f, "F0");
            GUILayout.EndVertical();

            GUILayout.Space(10);
            GUILayout.Label("Scoring", GUIStyles.SubHeader);
            GUILayout.BeginVertical(GUIStyles.SectionBox);
            config.KillWeight = GUIStyles.LabeledIntSlider("Kill weight", (int)config.KillWeight, 10, 500);
            config.LootWeight = GUIStyles.LabeledIntSlider("Loot weight", (int)config.LootWeight, 0, 200);
            config.RoomClearWeight = GUIStyles.LabeledIntSlider("Room clear weight", (int)config.RoomClearWeight, 0, 300);
            config.SurvivalWeight = GUIStyles.LabeledIntSlider("Survival weight", (int)config.SurvivalWeight, 0, 500);
            config.MonsterDropWeight = GUIStyles.LabeledIntSlider("Monster drop weight", (int)config.MonsterDropWeight, 0, 200);
            config.SellWeight = GUIStyles.LabeledIntSlider("Sell weight", (int)config.SellWeight, 0, 200);
            config.DeathPenalty = GUIStyles.LabeledIntSlider("Death penalty", (int)config.DeathPenalty, 0, 200);
            GUILayout.EndVertical();

            GUILayout.Space(10);
            GUILayout.Label("Challenge Tuning", GUIStyles.SubHeader);
            GUILayout.BeginVertical(GUIStyles.SectionBox);
            config.RivalLeadThreshold = GUIStyles.LabeledSlider("Lead threshold", config.RivalLeadThreshold, 0.05f, 0.5f, "F2");
            GUILayout.EndVertical();
        }
    }
}

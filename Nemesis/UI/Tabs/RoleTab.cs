using Nemesis.Modules.RoleSystem;
using UnityEngine;

namespace Nemesis.UI.Tabs
{
    internal static class RoleTab
    {
        public static void Draw(RoleConfig config)
        {
            config.Enabled = GUIStyles.LabeledToggle("Enable Role System", config.Enabled);
            config.ShowRoleHud = GUIStyles.LabeledToggle("Show Role HUD", config.ShowRoleHud);

            GUILayout.Space(10);
            GUILayout.Label("Role Multipliers", GUIStyles.SubHeader);

            GUILayout.BeginVertical(GUIStyles.SectionBox);
            GUILayout.Label("Scout - Speed Bonus", GUIStyles.Label);
            config.ScoutSpeedMultiplier = GUIStyles.LabeledSlider("  Speed Multiplier", config.ScoutSpeedMultiplier, 1.0f, 1.5f);
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUIStyles.SectionBox);
            GUILayout.Label("Tank - Health Bonus", GUIStyles.Label);
            config.TankHpMultiplier = GUIStyles.LabeledSlider("  HP Multiplier", config.TankHpMultiplier, 1.0f, 2.0f);
            config.TankSpeedPenalty = GUIStyles.LabeledSlider("  Speed Penalty", config.TankSpeedPenalty, 0.7f, 1.0f);
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUIStyles.SectionBox);
            GUILayout.Label("Medic - Interaction Speed", GUIStyles.Label);
            config.MedicInteractMultiplier = GUIStyles.LabeledSlider("  Interact Multiplier", config.MedicInteractMultiplier, 1.0f, 2.0f);
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUIStyles.SectionBox);
            GUILayout.Label("Scavenger - Loot Range", GUIStyles.Label);
            config.ScavengerLootMultiplier = GUIStyles.LabeledSlider("  Loot Range Multiplier", config.ScavengerLootMultiplier, 1.0f, 2.0f);
            GUILayout.EndVertical();

            GUILayout.Space(10);
            GUILayout.Label("Roles are randomly assigned at session start.", GUIStyles.Label);
        }
    }
}

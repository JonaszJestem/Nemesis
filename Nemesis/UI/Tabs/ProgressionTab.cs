using System;
using Nemesis.Modules.PersistentProgression;
using UnityEngine;

namespace Nemesis.UI.Tabs
{
    internal static class ProgressionTab
    {
        private static Vector2 _scrollPos;

        public static void Draw(ProgressionConfig config, PersistentProgressionModule? module)
        {
            _scrollPos = GUILayout.BeginScrollView(_scrollPos);

            config.Enabled = GUIStyles.LabeledToggle("Enable Progression", config.Enabled);
            config.ShowXpBar = GUIStyles.LabeledToggle("Show XP Bar", config.ShowXpBar);

            GUILayout.Space(10);
            GUILayout.Label("XP Rewards", GUIStyles.SubHeader);
            GUILayout.BeginVertical(GUIStyles.SectionBox);
            config.KillXP = GUIStyles.LabeledIntSlider("Kill XP", config.KillXP, 0, 200);
            config.LootCollectedXP = GUIStyles.LabeledIntSlider("Loot Collected XP", config.LootCollectedXP, 0, 100);
            config.SellXP = GUIStyles.LabeledIntSlider("Sell/Barter XP", config.SellXP, 0, 100);
            config.RoomClearedXP = GUIStyles.LabeledIntSlider("Room Cleared XP", config.RoomClearedXP, 0, 500);
            config.SessionSurvivedXP = GUIStyles.LabeledIntSlider("Session Survived XP", config.SessionSurvivedXP, 0, 500);
            config.ScaleWithDifficulty = GUIStyles.LabeledToggle("Scale XP with Difficulty", config.ScaleWithDifficulty);
            GUILayout.EndVertical();

            GUILayout.Space(10);
            GUILayout.Label("Level Bonuses", GUIStyles.SubHeader);
            GUILayout.BeginVertical(GUIStyles.SectionBox);

            // Use local vars to avoid mid-IMGUI-pass mutation issues
            float hpDisplay = config.HpBonusPerLevel * 100f;
            hpDisplay = GUIStyles.LabeledSlider("HP Bonus Per Level (%)", hpDisplay, 0, 10, "F1");
            config.HpBonusPerLevel = hpDisplay / 100f;

            float speedDisplay = config.SpeedBonusPerLevel * 100f;
            speedDisplay = GUIStyles.LabeledSlider("Speed Bonus Per Level (%)", speedDisplay, 0, 5, "F1");
            config.SpeedBonusPerLevel = speedDisplay / 100f;

            config.MaxLevel = GUIStyles.LabeledIntSlider("Max Level", config.MaxLevel, 10, 100);
            config.BaseXPPerLevel = GUIStyles.LabeledIntSlider("Base XP Per Level", (int)config.BaseXPPerLevel, 50, 500);
            GUILayout.EndVertical();

            GUILayout.Space(10);
            GUILayout.Label("Display", GUIStyles.SubHeader);
            GUILayout.BeginVertical(GUIStyles.SectionBox);
            config.XpBarX = GUIStyles.LabeledIntSlider("XP Bar X", config.XpBarX, 0, 500);
            config.XpBarYFromBottom = GUIStyles.LabeledIntSlider("XP Bar Y (from bottom)", config.XpBarYFromBottom, 20, 200);
            config.XpBarWidth = GUIStyles.LabeledIntSlider("XP Bar Width", config.XpBarWidth, 80, 300);
            config.SaveIntervalSeconds = GUIStyles.LabeledSlider("Auto-save Interval (s)", config.SaveIntervalSeconds, 10f, 300f, "F0");
            GUILayout.EndVertical();

            var localPlayer = module?.LocalPlayer;
            if (localPlayer != null)
            {
                GUILayout.Space(10);
                GUILayout.Label("Current Player Stats", GUIStyles.SubHeader);
                GUILayout.BeginVertical(GUIStyles.SectionBox);
                GUILayout.Label($"Name: {localPlayer.PlayerName}", GUIStyles.Label);
                GUILayout.Label($"Level: {localPlayer.Level}", GUIStyles.Label);
                GUILayout.Label($"Total XP: {localPlayer.XP}", GUIStyles.Label);

                long xpToNext = Math.Max(0,
                    LevelTable.XPToNextLevel(localPlayer.Level, config.BaseXPPerLevel, config.XPScalingExponent) - localPlayer.XP);
                GUILayout.Label($"XP to Next: {xpToNext}", GUIStyles.Label);

                GUILayout.Space(5);
                GUILayout.Label($"Total Kills: {localPlayer.TotalKills}", GUIStyles.Label);
                GUILayout.Label($"Loot Collected: {localPlayer.TotalLootCollected}", GUIStyles.Label);
                GUILayout.Label($"Rooms Cleared: {localPlayer.TotalRoomsCleared}", GUIStyles.Label);
                GUILayout.Label($"Sessions Survived: {localPlayer.TotalSessionsSurvived}", GUIStyles.Label);
                GUILayout.EndVertical();
            }

            GUILayout.EndScrollView();
        }
    }
}

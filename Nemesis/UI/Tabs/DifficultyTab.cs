using Nemesis.Modules.DifficultyDirector;
using UnityEngine;

namespace Nemesis.UI.Tabs
{
    internal static class DifficultyTab
    {
        private static Vector2 _scrollPos;

        public static void Draw(DifficultyConfig config)
        {
            _scrollPos = GUILayout.BeginScrollView(_scrollPos);

            config.Enabled = GUIStyles.LabeledToggle("Enable Difficulty Director", config.Enabled);
            config.ShowHudLabel = GUIStyles.LabeledToggle("Show HUD Label", config.ShowHudLabel);

            GUILayout.Space(10);
            GUILayout.Label("Factor Weights", GUIStyles.SubHeader);
            GUILayout.BeginVertical(GUIStyles.SectionBox);
            config.PlayerCountWeight = GUIStyles.LabeledSlider("Player Count Weight", config.PlayerCountWeight, 0, 100, "F0");
            config.GameDayWeight = GUIStyles.LabeledSlider("Game Day Weight", config.GameDayWeight, 0, 100, "F0");
            config.SessionCycleWeight = GUIStyles.LabeledSlider("Session Cycle Weight", config.SessionCycleWeight, 0, 100, "F0");
            GUILayout.EndVertical();

            GUILayout.Space(10);
            GUILayout.Label("Factor Scaling", GUIStyles.SubHeader);
            GUILayout.BeginVertical(GUIStyles.SectionBox);
            config.PlayerCountMax = GUIStyles.LabeledIntSlider("Max Player Count", config.PlayerCountMax, 1, 50);
            config.GameDayMax = GUIStyles.LabeledIntSlider("Max Game Day", config.GameDayMax, 1, 100);
            config.SessionCycleMax = GUIStyles.LabeledIntSlider("Max Session Cycle", config.SessionCycleMax, 1, 30);
            GUILayout.EndVertical();

            GUILayout.Space(10);
            GUILayout.Label("Output Range", GUIStyles.SubHeader);
            GUILayout.BeginVertical(GUIStyles.SectionBox);
            config.MinMultiplier = GUIStyles.LabeledSlider("Min Multiplier", config.MinMultiplier, 0.1f, 2.0f);
            config.MaxMultiplier = GUIStyles.LabeledSlider("Max Multiplier", config.MaxMultiplier, 1.0f, 5.0f);
            config.UpdateIntervalSeconds = GUIStyles.LabeledSlider("Update Interval (s)", config.UpdateIntervalSeconds, 1f, 60f, "F0");
            GUILayout.EndVertical();

            GUILayout.Space(10);
            GUILayout.Label("Monster Scaling", GUIStyles.SubHeader);
            GUILayout.BeginVertical(GUIStyles.SectionBox);
            config.ScaleMonsterHp = GUIStyles.LabeledToggle("Scale Monster HP", config.ScaleMonsterHp);
            config.ScaleMonsterAtk = GUIStyles.LabeledToggle("Scale Monster ATK", config.ScaleMonsterAtk);
            GUILayout.EndVertical();

            GUILayout.Space(10);
            GUILayout.Label("Weather", GUIStyles.SubHeader);
            GUILayout.BeginVertical(GUIStyles.SectionBox);
            config.WeatherEscalation = GUIStyles.LabeledToggle("Weather Escalation", config.WeatherEscalation);
            config.WeatherThreshold = GUIStyles.LabeledSlider("Storm Threshold", config.WeatherThreshold, 1.0f, 4.0f);
            config.StormWeatherId = GUIStyles.LabeledIntSlider("Storm Weather ID", config.StormWeatherId, 0, 10);
            GUILayout.EndVertical();

            GUILayout.Space(10);
            GUILayout.Label($"Current Multiplier: x{DifficultyDirectorModule.CurrentMultiplier:F2}", GUIStyles.SubHeader);

            GUILayout.EndScrollView();
        }
    }
}

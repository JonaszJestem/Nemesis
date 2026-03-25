using Nemesis.Modules.TramTalentTree;
using UnityEngine;

namespace Nemesis.UI.Tabs
{
    internal static class TramTalentTreeTab
    {
        private static Vector2 _scrollPos;

        public static void Draw(TramTalentTreeConfig config)
        {
            Draw(config, null, null);
        }

        public static void Draw(TramTalentTreeConfig config, TramTalentTreeState? state, TramTalentTreeEngine? engine)
        {
            _scrollPos = GUILayout.BeginScrollView(_scrollPos);

            config.Enabled = GUIStyles.LabeledToggle("Enable Tram Talent Tree", config.Enabled);
            config.ShowHud = GUIStyles.LabeledToggle("Show HUD", config.ShowHud);
            config.ResetOnSessionStart = GUIStyles.LabeledToggle("Reset on Session Start", config.ResetOnSessionStart);

            GUILayout.Space(10);
            GUILayout.Label("Points", GUIStyles.SubHeader);
            GUILayout.BeginVertical(GUIStyles.SectionBox);
            config.StartingPoints = GUIStyles.LabeledIntSlider("Starting Points", config.StartingPoints, 0, 20);
            config.MaxRankPerNode = GUIStyles.LabeledIntSlider("Max Rank Per Node", config.MaxRankPerNode, 1, 10);
            GUILayout.EndVertical();

            GUILayout.Space(8);
            GUILayout.Label("Talents", GUIStyles.SubHeader);
            if (engine != null && state != null)
            {
                foreach (var definition in engine.GetDefinitions())
                {
                    DrawNodeRow(state, engine, definition);
                }
            }
            else
            {
                GUILayout.BeginVertical(GUIStyles.SectionBox);
                GUILayout.Label("Talent tree is configured, but the live module is not attached yet.", GUIStyles.Label);
                GUILayout.EndVertical();
            }

            GUILayout.Space(8);
            if (engine != null && state != null)
            {
                var snapshot = engine.ComputeSnapshot(state);
                GUILayout.Label(
                    $"Unspent: {state.UnspentPoints} | Speed x{snapshot.SpeedMultiplier:F2} | Noise x{snapshot.NoiseMultiplier:F2} | Reward x{snapshot.RewardMultiplier:F2}",
                    GUIStyles.Label);
            }

            GUILayout.EndScrollView();
        }

        private static void DrawNodeRow(
            TramTalentTreeState state,
            TramTalentTreeEngine engine,
            TramTalentNodeDefinition definition)
        {
            GUILayout.BeginVertical(GUIStyles.SectionBox);
            GUILayout.BeginHorizontal();
            GUILayout.Label(definition.Title, GUIStyles.SubHeader);
            GUILayout.FlexibleSpace();
            GUILayout.Label($"Rank {state.GetRank(definition.Id)}/{definition.MaxRank}", GUIStyles.ValueLabel);
            GUILayout.EndHorizontal();
            GUILayout.Label(definition.Description, GUIStyles.Label);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("-", GUILayout.Width(28)))
                engine.TryRefundPoint(state, definition.Id);

            if (GUILayout.Button("+", GUILayout.Width(28)))
                engine.TrySpendPoint(state, definition.Id);

            GUILayout.Space(8);
            GUILayout.Label("Cost: 1 point", GUIStyles.Label);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }
    }
}

using Nemesis.Modules.HealthIndicators;
using UnityEngine;

namespace Nemesis.UI.Tabs
{
    internal static class HealthIndicatorsTab
    {
        public static void Draw(HealthIndicatorsConfig config)
        {
            config.Enabled = GUIStyles.LabeledToggle("Enable Health Indicators", config.Enabled);

            GUILayout.Space(10);
            GUILayout.Label("Display Options", GUIStyles.SubHeader);
            GUILayout.BeginVertical(GUIStyles.SectionBox);
            config.ShowHealthBars = GUIStyles.LabeledToggle("Show Health Bars", config.ShowHealthBars);
            config.ShowDamageNumbers = GUIStyles.LabeledToggle("Show Damage Numbers", config.ShowDamageNumbers);
            config.DamageNumberScale = GUIStyles.LabeledSlider("Damage Number Scale", config.DamageNumberScale, 0.5f, 3.0f, "F1");
            GUILayout.EndVertical();
        }
    }
}

using Nemesis.Modules.RunMutatorDraft;
using UnityEngine;

namespace Nemesis.UI.Tabs
{
    internal static class RunMutatorDraftTab
    {
        public static void Draw(RunMutatorDraftConfig config)
        {
            config.Enabled = GUIStyles.LabeledToggle("Enable Run Mutator Draft", config.Enabled);

            GUILayout.Space(10);
            GUILayout.Label("Session Draft", GUIStyles.SubHeader);
            GUILayout.BeginVertical(GUIStyles.SectionBox);
            config.ShowHud = GUIStyles.LabeledToggle("Show HUD", config.ShowHud);
            config.AutoPickBestAvailable = GUIStyles.LabeledToggle("Auto Pick Best", config.AutoPickBestAvailable);
            config.DraftChoiceCount = GUIStyles.LabeledIntSlider("Draft Choices", config.DraftChoiceCount, 1, 6);
            config.ActiveMutatorCount = GUIStyles.LabeledIntSlider("Active Mutators", config.ActiveMutatorCount, 1, 4);
            config.RefreshIntervalSeconds = GUIStyles.LabeledSlider("Refresh Interval", config.RefreshIntervalSeconds, 1f, 15f, "F1");
            GUILayout.EndVertical();

            GUILayout.Space(8);
            GUILayout.Label("Pressure Tuning", GUIStyles.SubHeader);
            GUILayout.BeginVertical(GUIStyles.SectionBox);
            config.DefaultPreferredWeatherId = GUIStyles.LabeledIntSlider("Preferred Weather ID", config.DefaultPreferredWeatherId, 0, 10);
            config.SpawnPressureMultiplier = GUIStyles.LabeledSlider("Spawn Pressure Multiplier", config.SpawnPressureMultiplier, 1.0f, 2.0f, "F2");
            config.NoiseLeakMultiplier = GUIStyles.LabeledSlider("Noise Leak Multiplier", config.NoiseLeakMultiplier, 1.0f, 2.0f, "F2");
            config.NoiseDecayMultiplier = GUIStyles.LabeledSlider("Noise Decay Multiplier", config.NoiseDecayMultiplier, 0.5f, 1.5f, "F2");
            config.HybridMultiplier = GUIStyles.LabeledSlider("Hybrid Multiplier", config.HybridMultiplier, 1.0f, 2.0f, "F2");
            GUILayout.EndVertical();
        }
    }
}

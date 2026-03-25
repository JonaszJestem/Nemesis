using Nemesis.Modules.NoiseDirector;
using UnityEngine;

namespace Nemesis.UI.Tabs
{
    internal static class NoiseDirectorTab
    {
        public static void Draw(NoiseDirectorConfig config)
        {
            config.Enabled = GUIStyles.LabeledToggle("Enable Noise Director", config.Enabled);

            GUILayout.Space(10);
            GUILayout.Label("Noise Mix", GUIStyles.SubHeader);
            GUILayout.BeginVertical(GUIStyles.SectionBox);
            config.GlobalVolumeMultiplier = GUIStyles.LabeledSlider("Global Volume", config.GlobalVolumeMultiplier, 0f, 2f, "F2");
            config.AmbientVolumeMultiplier = GUIStyles.LabeledSlider("Ambient Volume", config.AmbientVolumeMultiplier, 0f, 2f, "F2");
            config.EffectsVolumeMultiplier = GUIStyles.LabeledSlider("Effects Volume", config.EffectsVolumeMultiplier, 0f, 2f, "F2");
            config.VoiceVolumeMultiplier = GUIStyles.LabeledSlider("Voice Volume", config.VoiceVolumeMultiplier, 0f, 2f, "F2");
            GUILayout.EndVertical();

            GUILayout.Space(10);
            GUILayout.Label("Scope", GUIStyles.SubHeader);
            GUILayout.BeginVertical(GUIStyles.SectionBox);
            config.AffectUiAudio = GUIStyles.LabeledToggle("Affect UI Audio", config.AffectUiAudio);
            config.AffectMusic = GUIStyles.LabeledToggle("Affect Music", config.AffectMusic);
            config.UpdateIntervalSeconds = GUIStyles.LabeledSlider("Refresh Interval (s)", config.UpdateIntervalSeconds, 0.1f, 2.0f, "F1");
            GUILayout.EndVertical();

            GUILayout.Space(10);
            GUILayout.Label("Heuristic", GUIStyles.SubHeader);
            GUILayout.Label("Name-based filters keep music and UI stable unless you opt in.", GUIStyles.Label);
        }
    }
}

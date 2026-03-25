using Nemesis.Modules.VoiceFix;
using UnityEngine;

namespace Nemesis.UI.Tabs
{
    internal static class VoiceFixTab
    {
        public static void Draw(VoiceFixConfig config)
        {
            config.Enabled = GUIStyles.LabeledToggle("Enable Voice Fix", config.Enabled);

            GUILayout.Space(10);
            GUILayout.Label("Info", GUIStyles.SubHeader);
            GUILayout.BeginVertical(GUIStyles.SectionBox);
            GUILayout.Label("Removes audio filter effects (distortion, reverb, echo, etc.)", GUIStyles.Label);
            GUILayout.Label("from mimic voice presets for clearer voice chat.", GUIStyles.Label);
            GUILayout.EndVertical();
        }
    }
}

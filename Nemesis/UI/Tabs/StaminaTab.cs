using Nemesis.Modules.Stamina;
using UnityEngine;

namespace Nemesis.UI.Tabs
{
    internal static class StaminaTab
    {
        public static void Draw(StaminaConfig config)
        {
            config.Enabled = GUIStyles.LabeledToggle("Enable Infinite Stamina", config.Enabled);

            GUILayout.Space(10);
            GUILayout.Label("Info", GUIStyles.SubHeader);
            GUILayout.BeginVertical(GUIStyles.SectionBox);
            GUILayout.Label("When enabled, stamina consumption is completely prevented.", GUIStyles.Label);
            GUILayout.Label("This is a client-side cheat - only affects you.", GUIStyles.Label);
            GUILayout.EndVertical();
        }
    }
}

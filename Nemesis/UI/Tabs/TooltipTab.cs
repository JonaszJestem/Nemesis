using Nemesis.Modules.TooltipMod;
using UnityEngine;

namespace Nemesis.UI.Tabs
{
    internal static class TooltipTab
    {
        public static void Draw(TooltipConfig config)
        {
            config.Enabled = GUIStyles.LabeledToggle("Enable Item Tooltip", config.Enabled);

            GUILayout.Space(10);
            GUILayout.Label("Settings", GUIStyles.SubHeader);
            GUILayout.BeginVertical(GUIStyles.SectionBox);
            config.FontSize = GUIStyles.LabeledIntSlider("Font Size", config.FontSize, 10, 24);
            GUILayout.EndVertical();

            GUILayout.Space(10);
            GUILayout.Label("Info", GUIStyles.SubHeader);
            GUILayout.BeginVertical(GUIStyles.SectionBox);
            GUILayout.Label("Shows the name of your selected inventory item near the hotbar.", GUIStyles.Label);
            GUILayout.Label("This is a client-only visual overlay.", GUIStyles.Label);
            GUILayout.EndVertical();
        }
    }
}

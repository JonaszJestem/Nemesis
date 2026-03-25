using Nemesis.Modules.InventoryExpansion;
using UnityEngine;

namespace Nemesis.UI.Tabs
{
    internal static class InventoryExpansionTab
    {
        public static void Draw(InventoryExpansionConfig config)
        {
            config.Enabled = GUIStyles.LabeledToggle("Enable Inventory Expansion", config.Enabled);

            GUILayout.Space(10);
            GUILayout.Label("Settings", GUIStyles.SubHeader);
            GUILayout.BeginVertical(GUIStyles.SectionBox);
            GUILayout.Label($"Additional Slots: {config.AdditionalSlots}", GUIStyles.Label);
            config.AdditionalSlots = (int)GUILayout.HorizontalSlider(config.AdditionalSlots, 0, 8);
            GUILayout.Label("Expands inventory capacity by preventing the full check.", GUIStyles.Label);
            GUILayout.EndVertical();

            GUILayout.Space(10);
            GUILayout.Label("Info", GUIStyles.SubHeader);
            GUILayout.BeginVertical(GUIStyles.SectionBox);
            GUILayout.Label("This is a host-only mod - settings are synced to all players.", GUIStyles.Label);
            GUILayout.EndVertical();
        }
    }
}

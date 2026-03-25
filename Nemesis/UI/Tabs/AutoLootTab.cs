using Nemesis.Modules.AutoLoot;
using UnityEngine;

namespace Nemesis.UI.Tabs
{
    internal static class AutoLootTab
    {
        public static void Draw(AutoLootConfig config)
        {
            config.Enabled = GUIStyles.LabeledToggle("Enable Auto Loot", config.Enabled);

            GUILayout.Space(10);
            GUILayout.Label("Settings", GUIStyles.SubHeader);
            GUILayout.BeginVertical(GUIStyles.SectionBox);
            config.PickupRange = GUIStyles.LabeledSlider("Pickup Range", config.PickupRange, 1f, 20f, "F1");
            GUILayout.EndVertical();
        }
    }
}

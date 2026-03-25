using Nemesis.Modules.PossessionPlus;
using UnityEngine;

namespace Nemesis.UI.Tabs
{
    internal static class PossessionPlusTab
    {
        public static void Draw(PossessionPlusConfig config)
        {
            config.Enabled = GUIStyles.LabeledToggle("Enable Possession++", config.Enabled);

            GUILayout.Space(10);
            GUILayout.Label("Ghost Support", GUIStyles.SubHeader);
            GUILayout.BeginVertical(GUIStyles.SectionBox);
            config.ShowHud = GUIStyles.LabeledToggle("Show HUD", config.ShowHud);
            config.ShareWithLobby = GUIStyles.LabeledToggle("Share ghost pulses in lobby", config.ShareWithLobby);
            config.AutoPulseOnDeath = GUIStyles.LabeledToggle("Auto pulse on death", config.AutoPulseOnDeath);
            config.MaxCharges = GUIStyles.LabeledIntSlider("Max charges", config.MaxCharges, 1, 6);
            config.PulseCooldownSeconds = GUIStyles.LabeledSlider("Pulse cooldown", config.PulseCooldownSeconds, 1f, 30f, "F1");
            config.GhostLifetimeSeconds = GUIStyles.LabeledSlider("Ghost lifetime", config.GhostLifetimeSeconds, 30f, 360f, "F0");
            GUILayout.EndVertical();

            GUILayout.Space(10);
            GUILayout.Label("Recharge", GUIStyles.SubHeader);
            GUILayout.BeginVertical(GUIStyles.SectionBox);
            config.BaseRechargeSeconds = GUIStyles.LabeledSlider("Base recharge", config.BaseRechargeSeconds, 5f, 60f, "F0");
            config.AllyRechargeBonusSeconds = GUIStyles.LabeledSlider("Ally recharge bonus", config.AllyRechargeBonusSeconds, 0f, 10f, "F1");
            config.MinimumRechargeSeconds = GUIStyles.LabeledSlider("Minimum recharge", config.MinimumRechargeSeconds, 1f, 20f, "F0");
            config.AllySupportRadius = GUIStyles.LabeledSlider("Ally support radius", config.AllySupportRadius, 5f, 40f, "F0");
            GUILayout.EndVertical();

            GUILayout.Space(10);
            GUILayout.Label("Pulse", GUIStyles.SubHeader);
            GUILayout.BeginVertical(GUIStyles.SectionBox);
            config.PulseRadius = GUIStyles.LabeledSlider("Pulse radius", config.PulseRadius, 10f, 60f, "F0");
            config.MaxMarksPerPulse = GUIStyles.LabeledIntSlider("Marks per pulse", config.MaxMarksPerPulse, 1, 8);
            config.MarkLifetimeSeconds = GUIStyles.LabeledSlider("Mark lifetime", config.MarkLifetimeSeconds, 3f, 45f, "F0");
            config.RemoteBroadcastLifetimeSeconds = GUIStyles.LabeledSlider("Lobby broadcast lifetime", config.RemoteBroadcastLifetimeSeconds, 5f, 60f, "F0");
            GUILayout.EndVertical();
        }
    }
}

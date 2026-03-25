using System;
using Nemesis.Modules.ContractBoard;
using UnityEngine;

namespace Nemesis.UI.Tabs
{
    internal static class ContractBoardTab
    {
        private static Vector2 _scrollPos;

        public static void Draw(ContractBoardConfig config)
        {
            Draw(config, null);
        }

        public static void Draw(ContractBoardConfig config, ContractBoardModule? module)
        {
            _scrollPos = GUILayout.BeginScrollView(_scrollPos);

            config.Enabled = GUIStyles.LabeledToggle("Enable Contract Board", config.Enabled);
            config.ShowHud = GUIStyles.LabeledToggle("Show HUD", config.ShowHud);
            config.ResetOnSessionStart = GUIStyles.LabeledToggle("Reset on Session Start", config.ResetOnSessionStart);

            GUILayout.Space(10);
            GUILayout.Label("Board Setup", GUIStyles.SubHeader);
            GUILayout.BeginVertical(GUIStyles.SectionBox);
            config.StartingActiveContracts = GUIStyles.LabeledIntSlider("Starting active contracts", config.StartingActiveContracts, 0, 6);
            config.MaxContractsPerRun = GUIStyles.LabeledIntSlider("Max contracts per run", config.MaxContractsPerRun, 1, 12);
            config.ReplaceCompletedContracts = GUIStyles.LabeledToggle("Replace completed contracts", config.ReplaceCompletedContracts);
            config.AllowDuplicateObjectiveKinds = GUIStyles.LabeledToggle("Allow duplicate objective kinds", config.AllowDuplicateObjectiveKinds);
            GUILayout.EndVertical();

            GUILayout.Space(10);
            GUILayout.Label("Contract Scaling", GUIStyles.SubHeader);
            GUILayout.BeginVertical(GUIStyles.SectionBox);
            config.RefreshIntervalSeconds = GUIStyles.LabeledSlider("Refresh interval", config.RefreshIntervalSeconds, 0.25f, 5f, "F2");
            config.TargetVariancePercent = GUIStyles.LabeledSlider("Target variance", config.TargetVariancePercent, 0f, 0.5f, "F2");
            config.RewardVariancePercent = GUIStyles.LabeledSlider("Reward variance", config.RewardVariancePercent, 0f, 0.5f, "F2");
            config.TargetRampPerIssuedContract = GUIStyles.LabeledSlider("Target ramp per issued", config.TargetRampPerIssuedContract, 0f, 0.25f, "F2");
            config.RewardMultiplier = GUIStyles.LabeledSlider("Reward multiplier", config.RewardMultiplier, 0.5f, 3f, "F2");
            config.RewardBonusPoints = GUIStyles.LabeledIntSlider("Reward bonus points", config.RewardBonusPoints, 0, 12);
            GUILayout.EndVertical();

            GUILayout.Space(10);
            GUILayout.Label("Live Board", GUIStyles.SubHeader);
            DrawSnapshot(module?.CurrentSnapshot ?? ContractBoardRuntime.CurrentSnapshot);

            GUILayout.EndScrollView();
        }

        private static void DrawSnapshot(ContractBoardSnapshot snapshot)
        {
            GUILayout.BeginVertical(GUIStyles.SectionBox);
            GUILayout.Label(snapshot.StatusLine, GUIStyles.Label);
            GUILayout.Label($"Session: {DisplaySessionKey(snapshot.SessionKey)}", GUIStyles.Label);
            GUILayout.Label($"Issued: {snapshot.IssuedContracts} | Completed: {snapshot.CompletedContracts} | Reward points: {snapshot.TotalQueuedTalentPoints}", GUIStyles.Label);

            if (snapshot.ActiveContracts.Count > 0)
            {
                GUILayout.Space(4);
                GUILayout.Label("Active contracts", GUIStyles.SubHeader);
                foreach (var contract in snapshot.ActiveContracts)
                    DrawContract(contract);
            }
            else
            {
                GUILayout.Label("No active contracts right now.", GUIStyles.Label);
            }

            if (snapshot.CompletionHistory.Count > 0)
            {
                GUILayout.Space(4);
                GUILayout.Label("Completion history", GUIStyles.SubHeader);
                int start = Math.Max(0, snapshot.CompletionHistory.Count - 5);
                for (int i = start; i < snapshot.CompletionHistory.Count; i++)
                {
                    var completion = snapshot.CompletionHistory[i];
                    GUILayout.Label($"+{completion.RewardPoints} pts - {completion.Title} ({completion.Progress}/{completion.Target})", GUIStyles.Label);
                }
            }

            GUILayout.EndVertical();
        }

        private static void DrawContract(ContractBoardContractSnapshot contract)
        {
            GUILayout.BeginVertical(GUIStyles.SectionBox);
            GUILayout.Label(contract.Title, GUIStyles.SubHeader);
            GUILayout.Label(contract.Description, GUIStyles.Label);
            GUILayout.Label(
                $"{contract.Progress}/{contract.Target} {contract.ProgressUnit} | Reward {contract.RewardPoints} pts",
                GUIStyles.Label);
            var barRect = GUILayoutUtility.GetRect(300, 14);
            float progress = contract.Target <= 0 ? 1f : Mathf.Clamp01((float)contract.Progress / contract.Target);
            GUI.Box(barRect, GUIContent.none);
            GUI.Box(new Rect(barRect.x, barRect.y, barRect.width * progress, barRect.height), GUIContent.none);
            GUILayout.EndVertical();
        }

        private static string DisplaySessionKey(string sessionKey)
        {
            return string.IsNullOrWhiteSpace(sessionKey) ? "offline" : sessionKey;
        }
    }
}

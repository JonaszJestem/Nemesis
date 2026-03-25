using System;
using Nemesis.Core;
using Nemesis.Modules.TramTalentTree;
using Nemesis.UI;
using UnityEngine;

namespace Nemesis.Modules.ContractBoard
{
    internal sealed class ContractBoardModule : IModule
    {
        public string Name => "Contract Board";

        private readonly ContractBoardConfig _config;
        private GUIStyle? _titleStyle;
        private GUIStyle? _bodyStyle;
        private GUIStyle? _accentStyle;

        public ContractBoardModule(ContractBoardConfig config)
        {
            _config = config;
        }

        public ContractBoardSnapshot CurrentSnapshot => ContractBoardRuntime.CurrentSnapshot;

        public void Initialize()
        {
            ModuleEventBus.OnSessionStarted += OnSessionStarted;
            ModuleEventBus.OnMonsterKilled += OnMonsterKilled;
            ModuleEventBus.OnLootCollected += OnLootCollected;
            ModuleEventBus.OnItemSold += OnItemSold;
            ModuleEventBus.OnRoomCleared += OnRoomCleared;
            ModuleEventBus.OnMonsterLootDrop += OnMonsterLootDrop;
            ContractBoardRuntime.Initialize(_config);
            Log.Msg("ContractBoard", "Initialized");
        }

        public void Shutdown()
        {
            ModuleEventBus.OnSessionStarted -= OnSessionStarted;
            ModuleEventBus.OnMonsterKilled -= OnMonsterKilled;
            ModuleEventBus.OnLootCollected -= OnLootCollected;
            ModuleEventBus.OnItemSold -= OnItemSold;
            ModuleEventBus.OnRoomCleared -= OnRoomCleared;
            ModuleEventBus.OnMonsterLootDrop -= OnMonsterLootDrop;
            ContractBoardRuntime.Shutdown();
        }

        public void OnUpdate()
        {
            ContractBoardRuntime.Update(_config, Time.deltaTime, NemesisMod.Instance?.IsHost == true);
        }

        public void OnGUI()
        {
            var snapshot = CurrentSnapshot;
            if (!_config.Enabled || !_config.ShowHud)
                return;

            if (!snapshot.Enabled && snapshot.ActiveContracts.Count == 0 && snapshot.CompletionHistory.Count == 0)
                return;

            EnsureStyles();

            GUILayout.BeginVertical(GUIStyles.SectionBox, GUILayout.Width(430));
            GUILayout.Label("Contract Board", _titleStyle!);
            GUILayout.Label(snapshot.StatusLine, _accentStyle!);
            GUILayout.Label(
                $"Active: {snapshot.ActiveContracts.Count}  Completed: {snapshot.CompletedContracts}  Issued: {snapshot.IssuedContracts}/{Math.Max(1, snapshot.MaxContractsPerRun)}",
                _bodyStyle!);
            GUILayout.Label($"Queued talent points: {snapshot.TotalQueuedTalentPoints}", _bodyStyle!);

            if (snapshot.ActiveContracts.Count > 0)
            {
                GUILayout.Space(4);
                GUILayout.Label("Active contracts", _accentStyle!);
                foreach (var contract in snapshot.ActiveContracts)
                    DrawContract(contract);
            }

            if (snapshot.CompletionHistory.Count > 0)
            {
                GUILayout.Space(4);
                GUILayout.Label("Recent completions", _accentStyle!);
                foreach (var completion in snapshot.CompletionHistory.Count > 3
                    ? snapshot.CompletionHistory.GetRange(Math.Max(0, snapshot.CompletionHistory.Count - 3), Math.Min(3, snapshot.CompletionHistory.Count))
                    : snapshot.CompletionHistory)
                {
                    GUILayout.Label($"+{completion.RewardPoints} pts - {completion.Title}", _bodyStyle!);
                }
            }

            GUILayout.EndVertical();
        }

        private void OnSessionStarted()
        {
            ContractBoardRuntime.NotifySessionStarted(NemesisMod.Instance?.IsHost == true);
        }

        private void OnMonsterKilled()
        {
            if (NemesisMod.Instance?.IsHost == true)
                ContractBoardRuntime.ReportObjective(ContractBoardObjectiveKind.MonsterKills);
        }

        private void OnLootCollected()
        {
            if (NemesisMod.Instance?.IsHost == true)
                ContractBoardRuntime.ReportObjective(ContractBoardObjectiveKind.LootCollected);
        }

        private void OnItemSold()
        {
            if (NemesisMod.Instance?.IsHost == true)
                ContractBoardRuntime.ReportObjective(ContractBoardObjectiveKind.ItemsSold);
        }

        private void OnRoomCleared()
        {
            if (NemesisMod.Instance?.IsHost == true)
                ContractBoardRuntime.ReportObjective(ContractBoardObjectiveKind.RoomsCleared);
        }

        private void OnMonsterLootDrop()
        {
            if (NemesisMod.Instance?.IsHost == true)
                ContractBoardRuntime.ReportObjective(ContractBoardObjectiveKind.MonsterLootDrops);
        }

        private void DrawContract(ContractBoardContractSnapshot contract)
        {
            GUILayout.BeginVertical(GUIStyles.SectionBox);
            GUILayout.Label(contract.Title, _accentStyle!);
            GUILayout.Label(contract.Description, _bodyStyle!);
            GUILayout.Label(
                $"{contract.Progress}/{contract.Target} {contract.ProgressUnit} | Reward {contract.RewardPoints} points",
                _bodyStyle!);

            float progress = contract.Target <= 0 ? 1f : Mathf.Clamp01((float)contract.Progress / contract.Target);
            var barRect = GUILayoutUtility.GetRect(320, 14);
            GUI.Box(barRect, GUIContent.none);
            GUI.Box(new Rect(barRect.x, barRect.y, barRect.width * progress, barRect.height), GUIContent.none);
            GUILayout.EndVertical();
        }

        private void EnsureStyles()
        {
            if (_titleStyle != null)
                return;

            _titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 15,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.95f, 0.84f, 0.5f) }
            };

            _bodyStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 11,
                normal = { textColor = new Color(0.92f, 0.92f, 0.96f) }
            };

            _accentStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.6f, 0.9f, 1f) }
            };
        }
    }
}

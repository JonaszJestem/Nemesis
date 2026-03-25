using System;
using System.Collections.Generic;
using System.Linq;
using Nemesis.Core;
using Nemesis.UI;
using UnityEngine;

namespace Nemesis.Modules.RivalGhosts
{
    internal sealed class RivalGhostsModule : IModule
    {
        public string Name => "Rival Ghosts";

        private readonly RivalGhostsConfig _config;
        private readonly List<RivalGhostRecord> _records;
        private RivalRunMetrics _metrics = new RivalRunMetrics();
        private RivalGhostChallenge _challenge = new RivalGhostChallenge { RivalName = "No rival yet", Progress = 1f, StatusLine = "No rival data yet." };
        private float _autoSaveTimer;
        private float _challengeRefreshTimer;
        private bool _runActive;
        private bool _dirty;
        private string _runId = "";
        private long _runStartedUtcSeconds;

        private GUIStyle? _titleStyle;
        private GUIStyle? _bodyStyle;
        private GUIStyle? _accentStyle;

        public RivalGhostsModule(RivalGhostsConfig config)
        {
            _config = config;
            _records = RivalGhostStore.Load();
        }

        public void Initialize()
        {
            ModuleEventBus.OnSessionStarted += OnSessionStarted;
            ModuleEventBus.OnMonsterKilled += OnMonsterKilled;
            ModuleEventBus.OnLootCollected += OnLootCollected;
            ModuleEventBus.OnItemSold += OnItemSold;
            ModuleEventBus.OnRoomCleared += OnRoomCleared;
            ModuleEventBus.OnMonsterLootDrop += OnMonsterLootDrop;
            BeginRun();
            Log.Msg("RivalGhosts", "Initialized");
        }

        public void Shutdown()
        {
            ModuleEventBus.OnSessionStarted -= OnSessionStarted;
            ModuleEventBus.OnMonsterKilled -= OnMonsterKilled;
            ModuleEventBus.OnLootCollected -= OnLootCollected;
            ModuleEventBus.OnItemSold -= OnItemSold;
            ModuleEventBus.OnRoomCleared -= OnRoomCleared;
            ModuleEventBus.OnMonsterLootDrop -= OnMonsterLootDrop;
            FinalizeCurrentRun();
            SaveRecords();
        }

        public void OnUpdate()
        {
            if (!_config.Enabled)
            {
                if (_runActive)
                {
                    FinalizeCurrentRun();
                    SaveRecords();
                }

                _runActive = false;
                return;
            }

            if (!_runActive)
                BeginRun();

            _metrics.ActiveSeconds += Time.deltaTime;
            _autoSaveTimer += Time.deltaTime;
            _challengeRefreshTimer += Time.deltaTime;

            if (_challengeRefreshTimer >= 0.25f)
            {
                _challengeRefreshTimer = 0f;
                RefreshChallenge();
            }

            if (_dirty && _autoSaveTimer >= Math.Max(5f, _config.AutoSaveIntervalSeconds))
            {
                _autoSaveTimer = 0f;
                SaveRecords();
            }
        }

        public void OnGUI()
        {
            if (!_config.Enabled || !_config.ShowHud)
                return;

            EnsureStyles();
            GUILayout.BeginVertical(GUIStyles.SectionBox, GUILayout.Width(390));
            GUILayout.Label("Rival Ghosts", _titleStyle!);
            GUILayout.Label($"Current score: {_challenge.CurrentScore}", _accentStyle!);
            GUILayout.Label(_challenge.StatusLine, _bodyStyle!);

            var barRect = GUILayoutUtility.GetRect(320, 14);
            GUI.Box(barRect, GUIContent.none);
            GUI.Box(new Rect(barRect.x, barRect.y, barRect.width * Mathf.Clamp01(_challenge.Progress), barRect.height), GUIContent.none);

            GUILayout.Label(string.IsNullOrWhiteSpace(_challenge.RivalName) || _challenge.RivalName == "No rival yet"
                ? "No rival selected."
                : $"Rival: {_challenge.RivalName} ({_challenge.RivalScore})", _bodyStyle!);

            GUILayout.Space(4);
            GUILayout.Label("Top records", _accentStyle!);
            foreach (var record in _records.OrderByDescending(x => x.CompositeScore).ThenByDescending(x => x.FinishedUtcSeconds).Take(Math.Max(1, _config.TopRecordsToShow)))
                GUILayout.Label($"{record.PlayerName}: {record.CompositeScore} pts", _bodyStyle!);

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
                normal = { textColor = new Color(1f, 0.82f, 0.55f) }
            };
            _bodyStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 11,
                normal = { textColor = new Color(0.92f, 0.92f, 0.95f) }
            };
            _accentStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.85f, 0.95f, 0.55f) }
            };
        }

        private void OnSessionStarted()
        {
            if (!_config.Enabled)
                return;

            FinalizeCurrentRun();
            BeginRun();
        }

        private void OnMonsterKilled()
        {
            if (!_config.Enabled || !_runActive)
                return;

            _metrics.Kills++;
            MarkDirty();
        }

        private void OnLootCollected()
        {
            if (!_config.Enabled || !_runActive)
                return;

            _metrics.LootCollected++;
            MarkDirty();
        }

        private void OnItemSold()
        {
            if (!_config.Enabled || !_runActive)
                return;

            _metrics.ItemsSold++;
            MarkDirty();
        }

        private void OnRoomCleared()
        {
            if (!_config.Enabled || !_runActive)
                return;

            _metrics.RoomsCleared++;
            MarkDirty();
        }

        private void OnMonsterLootDrop()
        {
            if (!_config.Enabled || !_runActive)
                return;

            _metrics.MonsterLootDrops++;
            MarkDirty();
        }

        private void MarkDirty()
        {
            _dirty = true;
            _challengeRefreshTimer = 0f;
            RefreshChallenge();
        }

        private void BeginRun()
        {
            _metrics = new RivalRunMetrics();
            _metrics.SessionsSurvived = 1;
            _challenge = new RivalGhostChallenge { RivalName = "No rival yet", Progress = 1f, StatusLine = "No rival data yet." };
            _runId = Guid.NewGuid().ToString("N");
            _runStartedUtcSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            _runActive = true;
            _dirty = false;
            _autoSaveTimer = 0f;
            _challengeRefreshTimer = 0f;
        }

        private void FinalizeCurrentRun()
        {
            if (!_runActive || !HasMeaningfulActivity(_metrics))
                return;

            var record = new RivalGhostRecord
            {
                RunId = _runId,
                PlayerName = GetLocalPlayerName(),
                StartedUtcSeconds = _runStartedUtcSeconds,
                FinishedUtcSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                Metrics = CloneMetrics(_metrics),
                CompositeScore = RivalGhostScoring.ComputeScore(_metrics, RivalGhostScoring.FromConfig(_config))
            };

            record.Metrics.SessionsSurvived = Math.Max(1, record.Metrics.SessionsSurvived);
            _records.Add(record);
            TrimRecords();
            _runActive = false;
            _dirty = true;
        }

        private void RefreshChallenge()
        {
            long currentScore = RivalGhostScoring.ComputeScore(_metrics, RivalGhostScoring.FromConfig(_config));
            var rival = RivalGhostSelector.SelectTarget(_records, currentScore, _config.RivalLeadThreshold);
            _challenge = RivalGhostSelector.BuildChallenge(rival, currentScore);
        }

        private void SaveRecords()
        {
            try
            {
                RivalGhostStore.Save(_records);
                _dirty = false;
            }
            catch (Exception ex)
            {
                Log.Warn("RivalGhosts", $"Save failed: {ex.Message}");
            }
        }

        private void TrimRecords()
        {
            int limit = Math.Max(1, _config.StorageLimit);
            if (_records.Count <= limit)
                return;

            var trimmed = _records.OrderByDescending(x => x.CompositeScore).ThenByDescending(x => x.FinishedUtcSeconds).Take(limit).ToList();
            _records.Clear();
            _records.AddRange(trimmed);
        }

        private static bool HasMeaningfulActivity(RivalRunMetrics metrics)
        {
            return metrics.Kills > 0 || metrics.LootCollected > 0 || metrics.RoomsCleared > 0 || metrics.ItemsSold > 0 || metrics.MonsterLootDrops > 0 || metrics.ActiveSeconds > 5f;
        }

        private static RivalRunMetrics CloneMetrics(RivalRunMetrics metrics)
        {
            return new RivalRunMetrics
            {
                Kills = metrics.Kills,
                LootCollected = metrics.LootCollected,
                RoomsCleared = metrics.RoomsCleared,
                SessionsSurvived = metrics.SessionsSurvived,
                MonsterLootDrops = metrics.MonsterLootDrops,
                ItemsSold = metrics.ItemsSold,
                Deaths = metrics.Deaths,
                ActiveSeconds = metrics.ActiveSeconds
            };
        }

        private static string GetLocalPlayerName()
        {
            try
            {
                var player = MimicAPI.GameAPI.PlayerAPI.GetLocalPlayer();
                if (player != null)
                    return MimicAPI.GameAPI.PlayerAPI.GetPlayerName(player);
            }
            catch { }

            return "Unknown Runner";
        }
    }
}

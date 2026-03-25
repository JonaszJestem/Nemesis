using System;
using MelonLoader;
using MimicAPI.GameAPI;
using Nemesis.Core;
using Nemesis.Modules.DifficultyDirector;
using UnityEngine;

namespace Nemesis.Modules.PersistentProgression
{
    internal class PersistentProgressionModule : IModule
    {
        public string Name => "Persistent Progression";

        private readonly ProgressionConfig _config;
        private ProgressionData _data;
        private PlayerProgression? _localPlayer;
        private float _saveTimer;
        private bool _dirty;
        private int _previousLevel;

        // Base stats cached once per session to avoid compounding
        private CachedStats _baseStats;
        private bool _baseStatsCaptured;

        // Cached XP boundaries — only recompute on level change
        private long _cachedCurrentLevelXP;
        private long _cachedNextLevelXP;

        // Cached GUI
        private GUIStyle? _levelStyle;
        private GUIStyle? _xpStyle;

        public PersistentProgressionModule(ProgressionConfig config)
        {
            _config = config;
            _data = ProgressionStore.Load();
        }

        public PlayerProgression? LocalPlayer => _localPlayer;
        public ProgressionData Data => _data;

        public void Initialize()
        {
            ModuleEventBus.OnMonsterKilled += OnMonsterKilled;
            ModuleEventBus.OnLootCollected += OnLootCollected;
            ModuleEventBus.OnRoomCleared += OnRoomCleared;
            ModuleEventBus.OnSessionStarted += OnSessionStarted;

            MelonLogger.Msg("[Nemesis] PersistentProgression initialized");
        }

        public void Shutdown()
        {
            ModuleEventBus.OnMonsterKilled -= OnMonsterKilled;
            ModuleEventBus.OnLootCollected -= OnLootCollected;
            ModuleEventBus.OnRoomCleared -= OnRoomCleared;
            ModuleEventBus.OnSessionStarted -= OnSessionStarted;

            if (_dirty) ProgressionStore.Save(_data);
        }

        private void EnsureLocalPlayer()
        {
            if (_localPlayer != null) return;

            var player = PlayerAPI.GetLocalPlayer();
            if (player == null) return;

            string key = player.ActorID.ToString();
            string name = PlayerAPI.GetPlayerName(player);
            _localPlayer = _data.GetOrCreate(key);
            _localPlayer.PlayerName = name;
            _previousLevel = _localPlayer.Level;
            RecacheLevelBoundaries();
        }

        private void RecacheLevelBoundaries()
        {
            if (_localPlayer == null) return;
            _cachedCurrentLevelXP = LevelTable.XPForLevel(_localPlayer.Level, _config.BaseXPPerLevel, _config.XPScalingExponent);
            _cachedNextLevelXP = LevelTable.XPToNextLevel(_localPlayer.Level, _config.BaseXPPerLevel, _config.XPScalingExponent);
        }

        private void CaptureBaseStats()
        {
            if (_baseStatsCaptured) return;
            try
            {
                var player = PlayerAPI.GetLocalPlayer();
                if (player == null) return;
                var statManager = PlayerAPI.GetStatManager(player);
                if (statManager == null) return;
                _baseStats = CachedStats.Capture(statManager);
                _baseStatsCaptured = true;
            }
            catch { }
        }

        private void OnSessionStarted()
        {
            if (!_config.Enabled) return;
            EnsureLocalPlayer();
            CaptureBaseStats();
            ApplyLevelBonuses();
        }

        private void OnMonsterKilled()
        {
            if (!_config.Enabled) return;
            EnsureLocalPlayer();
            if (_localPlayer == null) return;

            int xp = _config.KillXP;
            if (_config.ScaleWithDifficulty)
                xp = (int)(xp * DifficultyDirectorModule.CurrentMultiplier);

            AddXP(xp);
            _localPlayer.TotalKills++;
        }

        private void OnLootCollected()
        {
            if (!_config.Enabled) return;
            EnsureLocalPlayer();
            if (_localPlayer == null) return;

            AddXP(_config.LootCollectedXP);
            _localPlayer.TotalLootCollected++;
        }

        private void OnRoomCleared()
        {
            if (!_config.Enabled) return;
            EnsureLocalPlayer();
            if (_localPlayer == null) return;

            int xp = _config.RoomClearedXP;
            if (_config.ScaleWithDifficulty)
                xp = (int)(xp * DifficultyDirectorModule.CurrentMultiplier);

            AddXP(xp);
            _localPlayer.TotalRoomsCleared++;
        }

        public void AwardSessionSurvived()
        {
            if (!_config.Enabled) return;
            EnsureLocalPlayer();
            if (_localPlayer == null) return;

            AddXP(_config.SessionSurvivedXP);
            _localPlayer.TotalSessionsSurvived++;
        }

        private void AddXP(int amount)
        {
            if (_localPlayer == null || amount <= 0) return;

            _localPlayer.XP += amount;
            _localPlayer.Level = LevelTable.ComputeLevel(
                _localPlayer.XP, _config.MaxLevel, _config.BaseXPPerLevel, _config.XPScalingExponent);

            if (_localPlayer.Level > _previousLevel)
            {
                MelonLogger.Msg($"[Progression] Level up! Now level {_localPlayer.Level}");
                _previousLevel = _localPlayer.Level;
                RecacheLevelBoundaries();
                ApplyLevelBonuses();
            }

            _dirty = true;
        }

        private void ApplyLevelBonuses()
        {
            if (_localPlayer == null || _localPlayer.Level <= 1) return;
            if (!_baseStatsCaptured) return;

            try
            {
                var player = PlayerAPI.GetLocalPlayer();
                if (player == null) return;
                var statManager = PlayerAPI.GetStatManager(player);
                if (statManager == null) return;

                float hpMult = LevelTable.GetHpBonus(_localPlayer.Level, _config.HpBonusPerLevel);
                float speedMult = LevelTable.GetSpeedBonus(_localPlayer.Level, _config.SpeedBonusPerLevel);

                _baseStats.ApplyMultipliers(statManager, hpMult, speedMult);
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"[Progression] Failed to apply bonuses: {ex.Message}");
            }
        }

        public void OnUpdate()
        {
            if (!_config.Enabled) return;

            _saveTimer += Time.deltaTime;
            if (_saveTimer >= _config.SaveIntervalSeconds && _dirty)
            {
                _saveTimer -= _config.SaveIntervalSeconds;
                ProgressionStore.Save(_data);
                _dirty = false;
            }
        }

        public void OnGUI()
        {
            if (!_config.Enabled || !_config.ShowXpBar) return;

            EnsureLocalPlayer();
            if (_localPlayer == null) return;

            if (_levelStyle == null)
            {
                _levelStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 14, fontStyle = FontStyle.Bold,
                    normal = { textColor = new Color(0.6f, 0.8f, 1f) }
                };
                _xpStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 10, alignment = TextAnchor.MiddleCenter,
                    normal = { textColor = Color.white }
                };
            }

            long xpIntoLevel = _localPlayer.XP - _cachedCurrentLevelXP;
            long xpSpan = _cachedNextLevelXP - _cachedCurrentLevelXP;
            float progress = xpSpan > 0 ? Mathf.Clamp01((float)xpIntoLevel / xpSpan) : 1f;

            float yPos = Screen.height - _config.XpBarYFromBottom;
            int x = _config.XpBarX;

            GUI.Label(new Rect(x, yPos, 200, 20), $"Lv.{_localPlayer.Level}", _levelStyle);

            var barRect = new Rect(x + 50, yPos + 2, _config.XpBarWidth, 14);
            GUI.DrawTexture(barRect, Texture2D.whiteTexture, ScaleMode.StretchToFill, true, 0,
                new Color(0.2f, 0.2f, 0.2f, 0.7f), 0, 0);

            var fillRect = new Rect(barRect.x, barRect.y, barRect.width * progress, barRect.height);
            GUI.DrawTexture(fillRect, Texture2D.whiteTexture, ScaleMode.StretchToFill, true, 0,
                new Color(0.3f, 0.7f, 1f, 0.8f), 0, 0);

            GUI.Label(barRect, $"{_localPlayer.XP} XP", _xpStyle);
        }
    }
}

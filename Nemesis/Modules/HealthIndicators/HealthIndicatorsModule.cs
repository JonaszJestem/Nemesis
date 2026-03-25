using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using MimicAPI.GameAPI;
using Nemesis.Core;
using UnityEngine;

namespace Nemesis.Modules.HealthIndicators
{
    internal class HealthIndicatorsModule : IModule
    {
        public string Name => "Health Indicators";

        private readonly HealthIndicatorsConfig _config;
        private float _updateTimer;
        private readonly List<MonsterHealthInfo> _monsterInfos = new List<MonsterHealthInfo>();

        private Texture2D? _barBg;
        private Texture2D? _barFg;
        private Texture2D? _barBorder;

        private Type? _statTypeEnum;
        private object? _hpEnumValue;

        private struct MonsterHealthInfo
        {
            public Vector3 WorldPosition;
            public float HealthPercent;
            public long CurrentHp;
            public long MaxHp;
        }

        public HealthIndicatorsModule(HealthIndicatorsConfig config)
        {
            _config = config;
        }

        public void Initialize()
        {
            try
            {
                _statTypeEnum = GameReflection.GetGameType(GameTypeNames.StatType);
                if (_statTypeEnum != null)
                    _hpEnumValue = Enum.Parse(_statTypeEnum, GameEnumValues.StatType_HP);
            }
            catch { }

            Log.Health.Msg("Initialized");
        }

        public void Shutdown()
        {
            if (_barBg != null) UnityEngine.Object.Destroy(_barBg);
            if (_barFg != null) UnityEngine.Object.Destroy(_barFg);
            if (_barBorder != null) UnityEngine.Object.Destroy(_barBorder);
        }

        public void OnUpdate()
        {
            if (!_config.Enabled || !_config.ShowHealthBars) return;

            _updateTimer += Time.deltaTime;
            if (_updateTimer < 0.25f) return;
            _updateTimer = 0f;

            RefreshMonsterHealth();
        }

        private void RefreshMonsterHealth()
        {
            _monsterInfos.Clear();

            try
            {
                var room = RoomAPI.GetCurrentRoom();
                if (room == null) return;

                var monsters = ActorAPI.GetAliveMonstersInRoom(room);
                foreach (var monster in monsters)
                {
                    if (monster == null) continue;

                    try
                    {
                        var posObj = ReflectionHelper.GetPropertyValue(monster, GamePropertyNames.VActor_PositionVector);
                        if (posObj == null) continue;
                        var pos = (Vector3)posObj;

                        // Get HP via StatController chain
                        var statController = ReflectionHelper.GetFieldValue(monster, GameFieldNames.StatController_StatManager);
                        if (statController == null)
                        {
                            // Try getting StatManager directly
                            statController = ReflectionHelper.GetFieldValue(monster, "StatManager")
                                          ?? ReflectionHelper.GetFieldValue(monster, "_statManager");
                        }
                        if (statController == null) continue;

                        // For StatController, get StatManager
                        var statManager = ReflectionHelper.GetFieldValue(statController, GameFieldNames.StatController_StatManager);
                        var targetManager = statManager ?? statController;

                        var totalStats = ReflectionHelper.GetFieldValue(targetManager, GameFieldNames.StatManager_TotalStats);
                        if (totalStats == null) continue;

                        var elements = ReflectionHelper.GetFieldValue(totalStats, GameFieldNames.StatCollection_Elements) as IDictionary;
                        if (elements == null || _hpEnumValue == null || !elements.Contains(_hpEnumValue)) continue;

                        var hpElement = elements[_hpEnumValue];
                        if (hpElement == null) continue;

                        var valueProp = hpElement.GetType().GetProperty(GamePropertyNames.StatElement_Value,
                            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                        if (valueProp == null) continue;

                        var currentVal = valueProp.GetValue(hpElement);
                        if (currentVal == null) continue;

                        long currentHp = Convert.ToInt64(currentVal);

                        var maxHpObj = ReflectionHelper.GetFieldValue(targetManager, GameFieldNames.StatManager_MaxHp);
                        long maxHp = maxHpObj != null ? Convert.ToInt64(maxHpObj) : currentHp;
                        if (maxHp <= 0) maxHp = 1;

                        float percent = Mathf.Clamp01((float)currentHp / maxHp);

                        _monsterInfos.Add(new MonsterHealthInfo
                        {
                            WorldPosition = pos + Vector3.up * 2.2f,
                            HealthPercent = percent,
                            CurrentHp = currentHp,
                            MaxHp = maxHp
                        });
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                Log.Health.Warn($"Refresh error: {ex.Message}");
            }
        }

        public void OnGUI()
        {
            if (!_config.Enabled || !_config.ShowHealthBars) return;
            if (_monsterInfos.Count == 0) return;

            EnsureTextures();

            var cam = Camera.main;
            if (cam == null) return;

            foreach (var info in _monsterInfos)
            {
                Vector3 screenPos = cam.WorldToScreenPoint(info.WorldPosition);
                if (screenPos.z <= 0) continue; // Behind camera

                // Unity GUI Y is inverted
                float x = screenPos.x;
                float y = Screen.height - screenPos.y;

                float barWidth = 60f;
                float barHeight = 8f;
                float bx = x - barWidth / 2f;
                float by = y - barHeight / 2f;

                // Background
                GUI.DrawTexture(new Rect(bx - 1, by - 1, barWidth + 2, barHeight + 2), _barBorder!);
                GUI.DrawTexture(new Rect(bx, by, barWidth, barHeight), _barBg!);

                // Health fill
                Color healthColor = Color.Lerp(Color.red, Color.green, info.HealthPercent);
                GUI.color = healthColor;
                GUI.DrawTexture(new Rect(bx, by, barWidth * info.HealthPercent, barHeight), _barFg!);
                GUI.color = Color.white;
            }
        }

        private void EnsureTextures()
        {
            if (_barBg != null) return;

            _barBg = MakeTexture(new Color(0.15f, 0.15f, 0.15f, 0.8f));
            _barFg = MakeTexture(Color.white);
            _barBorder = MakeTexture(new Color(0f, 0f, 0f, 0.9f));
        }

        private static Texture2D MakeTexture(Color color)
        {
            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, color);
            tex.Apply();
            return tex;
        }
    }
}

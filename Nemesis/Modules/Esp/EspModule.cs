using System;
using System.Collections.Generic;
using MimicAPI.GameAPI;
using Nemesis.Core;
using UnityEngine;

namespace Nemesis.Modules.Esp
{
    internal class EspModule : IModule
    {
        public string Name => "ESP";

        private readonly EspConfig _config;
        private float _updateTimer;

        private readonly List<EspEntry> _entries = new List<EspEntry>();

        private GUIStyle? _redStyle;
        private GUIStyle? _greenStyle;
        private GUIStyle? _yellowStyle;

        private struct EspEntry
        {
            public Vector3 WorldPosition;
            public string Label;
            public Color Color;
        }

        public EspModule(EspConfig config)
        {
            _config = config;
        }

        public void Initialize()
        {
            Log.Esp.Msg("Initialized");
        }

        public void Shutdown() { }

        public void OnUpdate()
        {
            if (!_config.Enabled) return;

            _updateTimer += Time.deltaTime;
            if (_updateTimer < 0.25f) return;
            _updateTimer = 0f;

            RefreshEntries();
        }

        private void RefreshEntries()
        {
            _entries.Clear();

            try
            {
                var localPos = PlayerAPI.GetLocalPlayerPosition();
                if (localPos == Vector3.zero && PlayerAPI.GetLocalPlayer() == null) return;

                // Players
                if (_config.ShowPlayers)
                {
                    var others = PlayerAPI.GetOtherPlayers();
                    if (others != null)
                    {
                        foreach (var p in others)
                        {
                            if (p == null || !PlayerAPI.IsPlayerValid(p)) continue;
                            var pos = p.transform.position;
                            float dist = Vector3.Distance(localPos, pos);
                            if (dist > _config.MaxRange) continue;

                            string label = _config.ShowDistance
                                ? $"{PlayerAPI.GetPlayerName(p)} [{dist:F0}m]"
                                : PlayerAPI.GetPlayerName(p);

                            _entries.Add(new EspEntry
                            {
                                WorldPosition = pos + Vector3.up * 2.0f,
                                Label = label,
                                Color = Color.green
                            });
                        }
                    }
                }

                // Monsters
                if (_config.ShowMonsters)
                {
                    try
                    {
                        var room = RoomAPI.GetCurrentRoom();
                        if (room != null)
                        {
                            var monsters = ActorAPI.GetAliveMonstersInRoom(room);
                            foreach (var monster in monsters)
                            {
                                if (monster == null) continue;
                                try
                                {
                                    var posObj = ReflectionHelper.GetPropertyValue(monster, GamePropertyNames.VActor_PositionVector);
                                    if (posObj == null) continue;
                                    var pos = (Vector3)posObj;
                                    float dist = Vector3.Distance(localPos, pos);
                                    if (dist > _config.MaxRange) continue;

                                    string name = monster.GetType().Name;
                                    string label = _config.ShowDistance
                                        ? $"{name} [{dist:F0}m]"
                                        : name;

                                    _entries.Add(new EspEntry
                                    {
                                        WorldPosition = pos + Vector3.up * 2.2f,
                                        Label = label,
                                        Color = Color.red
                                    });
                                }
                                catch { }
                            }
                        }
                    }
                    catch { }
                }

                // Loot
                if (_config.ShowLoot)
                {
                    var loot = LootAPI.GetLootNearby(_config.MaxRange);
                    if (loot != null)
                    {
                        foreach (var l in loot)
                        {
                            if (l == null) continue;
                            var pos = l.transform.position;
                            float dist = Vector3.Distance(localPos, pos);

                            string name = l.gameObject.name;
                            string label = _config.ShowDistance
                                ? $"{name} [{dist:F0}m]"
                                : name;

                            _entries.Add(new EspEntry
                            {
                                WorldPosition = pos + Vector3.up * 1.0f,
                                Label = label,
                                Color = Color.yellow
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Esp.Warn($"Refresh error: {ex.Message}");
            }
        }

        public void OnGUI()
        {
            if (!_config.Enabled || _entries.Count == 0) return;

            EnsureStyles();

            var cam = Camera.main;
            if (cam == null) return;

            foreach (var entry in _entries)
            {
                Vector3 screenPos = cam.WorldToScreenPoint(entry.WorldPosition);
                if (screenPos.z <= 0) continue; // Behind camera

                float x = screenPos.x;
                float y = Screen.height - screenPos.y;

                GUIStyle style;
                if (entry.Color == Color.red) style = _redStyle!;
                else if (entry.Color == Color.green) style = _greenStyle!;
                else style = _yellowStyle!;

                var content = new GUIContent(entry.Label);
                var size = style.CalcSize(content);
                GUI.Label(new Rect(x - size.x / 2f, y - size.y / 2f, size.x, size.y), content, style);
            }
        }

        private void EnsureStyles()
        {
            if (_redStyle != null) return;

            _redStyle = MakeStyle(Color.red);
            _greenStyle = MakeStyle(Color.green);
            _yellowStyle = MakeStyle(Color.yellow);
        }

        private static GUIStyle MakeStyle(Color color)
        {
            return new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = color }
            };
        }
    }
}

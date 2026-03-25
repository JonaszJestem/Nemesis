using System;
using System.Collections.Generic;
using MelonLoader;
using MimicAPI.GameAPI;
using Nemesis.Core;
using UnityEngine;

namespace Nemesis.Modules.ProximityRadar
{
    internal class ProximityRadarModule : IModule
    {
        public string Name => "Proximity Radar";

        private readonly RadarConfig _config;
        private readonly RadarRenderer _renderer = new RadarRenderer();
        private readonly List<RadarRenderer.RadarEntity> _entities = new List<RadarRenderer.RadarEntity>();
        private float _updateTimer;

        public ProximityRadarModule(RadarConfig config)
        {
            _config = config;
        }

        public void Initialize()
        {
            MelonLogger.Msg("[Nemesis] ProximityRadar initialized");
        }

        public void Shutdown()
        {
            _renderer.Destroy();
        }

        public void OnUpdate()
        {
            if (!_config.Enabled) return;

            _updateTimer += Time.deltaTime;
            if (_updateTimer < _config.UpdateRate) return;
            _updateTimer -= _config.UpdateRate;

            RefreshEntities();
        }

        private void RefreshEntities()
        {
            _entities.Clear();

            try
            {
                var localPlayer = PlayerAPI.GetLocalPlayer();
                if (localPlayer == null) return;

                Vector3 center = localPlayer.transform.position;
                float range = _config.Range;

                if (_config.ShowPlayers)
                {
                    var others = PlayerAPI.GetOtherPlayers();
                    if (others != null)
                    {
                        foreach (var p in others)
                        {
                            if (p == null || !PlayerAPI.IsPlayerValid(p)) continue;
                            if (Vector3.Distance(p.transform.position, center) > range) continue;

                            _entities.Add(new RadarRenderer.RadarEntity
                            {
                                WorldPosition = p.transform.position,
                                Type = RadarRenderer.EntityType.Player
                            });
                        }
                    }
                }

                if (_config.ShowMonsters)
                {
                    var room = RoomAPI.GetCurrentRoom();
                    if (room != null)
                    {
                        var monsters = ActorAPI.GetAliveMonstersInRoom(room);
                        foreach (var monster in monsters)
                        {
                            if (monster == null) continue;

                            // VActor extends MonoBehaviour — use transform directly
                            var comp = monster as Component;
                            if (comp == null) continue;

                            var pos = comp.transform.position;
                            if (Vector3.Distance(pos, center) > range) continue;

                            _entities.Add(new RadarRenderer.RadarEntity
                            {
                                WorldPosition = pos,
                                Type = RadarRenderer.EntityType.Monster
                            });
                        }
                    }
                }

                if (_config.ShowLoot)
                {
                    var loot = LootAPI.GetLootNearby(range);
                    if (loot != null)
                    {
                        foreach (var l in loot)
                        {
                            if (l == null) continue;
                            _entities.Add(new RadarRenderer.RadarEntity
                            {
                                WorldPosition = l.transform.position,
                                Type = RadarRenderer.EntityType.Loot
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"[Radar] Refresh error: {ex.Message}");
            }
        }

        public void OnGUI()
        {
            if (!_config.Enabled) return;
            if (!PlayerAPI.HasLocalPlayer()) return;

            int size = _config.RadarSize;
            var radarRect = new Rect(
                Screen.width - size - _config.OffsetX,
                _config.OffsetY,
                size, size);

            Vector3 playerPos = PlayerAPI.GetLocalPlayerPosition();
            _renderer.Draw(radarRect, playerPos, _config.Range, _entities, _config);
        }
    }
}

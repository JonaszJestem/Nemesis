using System;
using MimicAPI.GameAPI;
using Nemesis.Core;
using UnityEngine;

namespace Nemesis.Modules.AutoLoot
{
    internal class AutoLootModule : IModule
    {
        public string Name => "Auto Loot";

        private readonly AutoLootConfig _config;
        private float _updateTimer;

        public AutoLootModule(AutoLootConfig config)
        {
            _config = config;
        }

        public void Initialize()
        {
            Log.AutoLoot.Msg("Initialized");
        }

        public void Shutdown() { }

        public void OnUpdate()
        {
            if (!_config.Enabled) return;

            _updateTimer += Time.deltaTime;
            if (_updateTimer < 0.5f) return;
            _updateTimer = 0f;

            try
            {
                var player = PlayerAPI.GetLocalPlayer();
                if (player == null) return;

                var lootObjects = LootAPI.GetLootNearby(_config.PickupRange);
                if (lootObjects == null || lootObjects.Length == 0) return;

                foreach (var loot in lootObjects)
                {
                    if (loot == null) continue;

                    try
                    {
                        // Get the loot object's actor ID via reflection
                        var actorIdObj = ReflectionHelper.GetPropertyValue(loot, "ActorID")
                                      ?? ReflectionHelper.GetFieldValue(loot, "ActorID")
                                      ?? ReflectionHelper.GetPropertyValue(loot, "ObjectID")
                                      ?? ReflectionHelper.GetFieldValue(loot, "ObjectID");
                        if (actorIdObj == null) continue;

                        int actorId = Convert.ToInt32(actorIdObj);
                        ReflectionHelper.InvokeMethod(player, GameMethodNames.ProtoActor_GrapLootingObject, actorId);
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                Log.AutoLoot.Warn($"AutoLoot error: {ex.Message}");
            }
        }

        public void OnGUI() { }
    }
}

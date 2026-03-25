using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using MimicAPI.GameAPI;
using Nemesis.Core;
using UnityEngine;

namespace Nemesis.Modules.EnemyDropLoot
{
    internal class EnemyDropLootModule : IModule
    {
        public string Name => "Enemy Drop Loot";

        private readonly LootDropConfig _config;

        public static bool IsEnabled { get; private set; }
        public static float DropChance { get; private set; }
        public static int MaxDropsPerKill { get; private set; }

        public EnemyDropLootModule(LootDropConfig config)
        {
            _config = config;
        }

        public void Initialize()
        {
            LootPoolManager.Reset();
            Log.LootDrop.Msg("Initialized");
        }

        public void Shutdown()
        {
            IsEnabled = false;
            LootPoolManager.Reset();
        }

        public void OnUpdate()
        {
            IsEnabled = _config.Enabled;
            DropChance = Mathf.Clamp01(_config.DropChance);
            MaxDropsPerKill = Mathf.Clamp(_config.MaxDropsPerKill, 0, 100);
        }

        public void OnGUI() { }

        /// <summary>
        /// Roll drop count using per-roll drop chance, matching the community mod's logic.
        /// </summary>
        internal static int RollDropCount()
        {
            if (!IsEnabled) return 0;

            int rolls = Math.Max(1, MaxDropsPerKill);
            int successes = 0;
            for (int i = 0; i < rolls; i++)
            {
                if (UnityEngine.Random.value <= DropChance)
                    successes++;
            }
            return successes;
        }
    }
}

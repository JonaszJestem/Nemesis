using System;
using System.Collections.Generic;
using HarmonyLib;
using MelonLoader;
using MimicAPI.GameAPI;
using Nemesis.Config;
using Nemesis.Modules.DamageScale;
using Nemesis.Modules.DifficultyDirector;
using Nemesis.Modules.Fov;
using Nemesis.Modules.PersistentProgression;
using Nemesis.Modules.ProximityRadar;
using Nemesis.Modules.RoleSystem;
using Nemesis.Modules.Stamina;
using Nemesis.Modules.VoiceFix;
using Nemesis.Modules.MoreMimics;
using Nemesis.Modules.HealthIndicators;
using Nemesis.Modules.Jump;
using Nemesis.Modules.Fullbright;
using Nemesis.Modules.EnemyDropLoot;
using Nemesis.Modules.MoreVoices;
using Nemesis.Modules.NoiseDirector;
using Nemesis.Modules.TooltipMod;
using Nemesis.Modules.Marker;
using Nemesis.Modules.InventoryExpansion;
using Nemesis.Modules.AutoLoot;
using Nemesis.Modules.Esp;
using Nemesis.Modules.Fly;
using Nemesis.Modules.ContractBoard;
using Nemesis.Modules.RunMutatorDraft;
using Nemesis.Modules.TramTalentTree;
using Nemesis.Modules.PossessionPlus;
using Nemesis.Modules.RivalGhosts;
using Nemesis.UI;

[assembly: MelonInfo(typeof(Nemesis.Core.NemesisMod), "Nemesis", "1.0.0", "JonaszJestem")]
[assembly: MelonGame("ReLUGames", "MIMESIS")]

namespace Nemesis.Core
{
    public class NemesisMod : MelonMod
    {
        internal const string HarmonyId = "com.jonaszjestem.nemesis";

        public static NemesisMod? Instance { get; private set; }

        private readonly List<IModule> _modules = new List<IModule>();
        private AdminPanel? _adminPanel;
        private SuiteConfig? _config;
        private ConfigSync? _configSync;

        private bool _isHost;
        private bool _hostChecked;
        private float _hostCheckTimer;

        public bool IsHost => _isHost;

        public override void OnInitializeMelon()
        {
            Instance = this;

            var harmony = new HarmonyLib.Harmony(HarmonyId);
            try
            {
                harmony.PatchAll(typeof(NemesisMod).Assembly);
            }
            catch (Exception ex)
            {
                Log.Warn($"Some Harmony patches failed: {ex.Message}");
            }

            _config = ConfigManager.Load();
            _configSync = new ConfigSync(_config);

            _modules.Add(new DifficultyDirectorModule(_config.Difficulty));
            _modules.Add(new RoleSystemModule(_config.Roles));
            _modules.Add(new ProximityRadarModule(_config.Radar));
            _modules.Add(new PersistentProgressionModule(_config.Progression));
            _modules.Add(new StaminaModule(_config.Stamina));
            _modules.Add(new FovModule(_config.Fov));
            _modules.Add(new VoiceFixModule(_config.VoiceFix));
            _modules.Add(new DamageScaleModule(_config.DamageScale));
            _modules.Add(new MoreMimicsModule(_config.MoreMimics));
            _modules.Add(new HealthIndicatorsModule(_config.HealthIndicators));
            _modules.Add(new JumpModule(_config.Jump));
            _modules.Add(new FullbrightModule(_config.Fullbright));
            _modules.Add(new EnemyDropLootModule(_config.LootDrop));
            _modules.Add(new MoreVoicesModule(_config.MoreVoices));
            _modules.Add(new NoiseDirectorModule(_config.NoiseDirector));
            _modules.Add(new RunMutatorDraftModule(_config.RunMutatorDraft));
            _modules.Add(new TramTalentTreeModule(_config.TramTalentTree));
            _modules.Add(new TooltipModule(_config.Tooltip));
            _modules.Add(new MarkerModule(_config.Marker));
            _modules.Add(new InventoryExpansionModule(_config.InventoryExpansion));
            _modules.Add(new AutoLootModule(_config.AutoLoot));
            _modules.Add(new EspModule(_config.Esp));
            _modules.Add(new FlyModule(_config.Fly));
            _modules.Add(new ContractBoardModule(_config.ContractBoard));
            _modules.Add(new PossessionPlusModule(_config.PossessionPlus));
            _modules.Add(new RivalGhostsModule(_config.RivalGhosts));

            _adminPanel = new AdminPanel(_config, _modules);

            foreach (var module in _modules)
            {
                try { module.Initialize(); }
                catch (Exception ex)
                {
                    Log.Warn($"Failed to initialize {module.Name}: {ex.Message}");
                }
            }

            Log.Msg("All modules loaded. Press F10 for admin panel.");
        }

        public override void OnUpdate()
        {
            _hostCheckTimer += UnityEngine.Time.deltaTime;
            if (!_hostChecked || _hostCheckTimer > 5f)
            {
                _hostCheckTimer = 0f;
                try
                {
                    var player = PlayerAPI.GetLocalPlayer();
                    if (player != null)
                    {
                        _isHost = player.IsHost;
                        _hostChecked = true;
                    }
                }
                catch { }
            }

            try { _configSync?.OnUpdate(_isHost); }
            catch { }

            foreach (var module in _modules)
            {
                try { module.OnUpdate(); }
                catch { }
            }
            _adminPanel?.OnUpdate();
        }

        public override void OnGUI()
        {
            foreach (var module in _modules)
            {
                try { module.OnGUI(); }
                catch { }
            }
            _adminPanel?.OnGUI();
        }

        public override void OnApplicationQuit()
        {
            foreach (var module in _modules)
            {
                try { module.Shutdown(); }
                catch { }
            }
            if (_config != null) ConfigManager.Save(_config);
            GUIStyles.Dispose();
        }

        public void NotifyConfigChanged() => _configSync?.ForcePush();
    }
}

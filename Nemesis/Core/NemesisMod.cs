using System;
using System.Collections.Generic;
using HarmonyLib;
using MelonLoader;
using Nemesis.Config;
using Nemesis.Modules.DifficultyDirector;
using Nemesis.Modules.PersistentProgression;
using Nemesis.Modules.ProximityRadar;
using Nemesis.Modules.RoleSystem;
using Nemesis.UI;

[assembly: MelonInfo(typeof(Nemesis.Core.NemesisMod), "Nemesis", "1.0.0", "NeoMimicry")]
[assembly: MelonGame("ReLUGames", "MIMESIS")]

namespace Nemesis.Core
{
    public class NemesisMod : MelonMod
    {
        public static NemesisMod? Instance { get; private set; }

        private readonly List<IModule> _modules = new List<IModule>();
        private AdminPanel? _adminPanel;
        private SuiteConfig? _config;

        public override void OnInitializeMelon()
        {
            Instance = this;

            var harmony = new HarmonyLib.Harmony("com.neomimicry.nemesis");
            harmony.PatchAll(typeof(NemesisMod).Assembly);

            _config = ConfigManager.Load();

            _modules.Add(new DifficultyDirectorModule(_config.Difficulty));
            _modules.Add(new RoleSystemModule(_config.Roles));
            _modules.Add(new ProximityRadarModule(_config.Radar));
            _modules.Add(new PersistentProgressionModule(_config.Progression));

            _adminPanel = new AdminPanel(_config, _modules);

            foreach (var module in _modules)
            {
                try
                {
                    module.Initialize();
                }
                catch (Exception ex)
                {
                    MelonLogger.Warning($"[Nemesis] Failed to initialize {module.Name}: {ex.Message}");
                }
            }

            MelonLogger.Msg("[Nemesis] All modules loaded. Press F10 for admin panel.");
        }

        public override void OnUpdate()
        {
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
    }
}

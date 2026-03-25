using System;
using System.Collections.Generic;
using Nemesis.Config;
using Nemesis.Core;
using Nemesis.Modules.ContractBoard;
using Nemesis.Modules.PersistentProgression;
using Nemesis.Modules.TramTalentTree;
using Nemesis.UI.Tabs;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Nemesis.UI
{
    internal class AdminPanel
    {
        private readonly SuiteConfig _config;
        private readonly List<IModule> _modules;
        private bool _visible;
        private int _activeTab;
        private Rect _windowRect = new Rect(50, 50, 550, 600);
        private CursorLockMode _previousLockState;
        private bool _previousCursorVisible;
        private Vector2 _navScroll;
        private Vector2 _contentScroll;
        private string _moduleSearch = string.Empty;
        private bool _enabledOnly;

        // Size presets
        private int _sizePreset = 1; // 0=Small, 1=Medium, 2=Large, 3=XL, 4=2X, 5=3X
        private static readonly string[] SizeNames = { "S", "M", "L", "XL", "2X", "3X" };
        private static readonly int[] PresetWidths = { 400, 550, 750, 1000, 1100, 1650 };
        private static readonly int[] PresetHeights = { 420, 600, 800, 1100, 1200, 1800 };
        private static readonly float[] PresetScales = { 0.85f, 1.0f, 1.2f, 1.3f, 1.45f, 1.6f };

        // Host-aware tabs
        // Host tabs: gameplay-affecting settings synced to all players
        // Client tabs: visual/audio-only settings local to each player
        private static readonly string[] HostTabNames =
        {
            "Difficulty",
            "Roles",
            "Damage",
            "Mimics",
            "Stamina",
            "Jump",
            "Fullbright",
            "LootDrop",
            "Voices",
            "NoiseDir",
            "RunDraft",
            "TramTree",
            "Contracts",
            "Possession",
            "Rivals",
            "Marker",
            "Inventory",
            "AutoLoot",
            "Fly",
            "Progression",
            "Radar",
            "FOV",
            "VoiceFix",
            "Health",
            "Tooltip",
            "ESP"
        };

        private static readonly string[] ClientTabNames =
        {
            "Radar",
            "FOV",
            "VoiceFix",
            "Health",
            "Tooltip",
            "ESP"
        };

        // Window background
        private Texture2D? _windowBg;

        public AdminPanel(SuiteConfig config, List<IModule> modules)
        {
            _config = config;
            _modules = modules;
        }

        public void OnUpdate()
        {
            var keyboard = Keyboard.current;
            if (keyboard != null && keyboard.f10Key.wasPressedThisFrame)
            {
                _visible = !_visible;
                if (_visible)
                {
                    _previousLockState = Cursor.lockState;
                    _previousCursorVisible = Cursor.visible;
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    // Host status is checked live via NemesisMod.Instance
                }
                else
                {
                    Cursor.lockState = _previousLockState;
                    Cursor.visible = _previousCursorVisible;
                }
            }
        }

        public void OnGUI()
        {
            if (!_visible)
                return;

            GUIStyles.EnsureInitialized();
            EnsureWindowBg();

            // Apply window background style
            var windowStyle = new GUIStyle(GUI.skin.window)
            {
                normal = { background = _windowBg, textColor = Color.clear },
                onNormal = { background = _windowBg, textColor = Color.clear },
                padding = new RectOffset(10, 10, 10, 10),
                border = new RectOffset(0, 0, 0, 0)
            };

            _windowRect = GUI.Window(9876, _windowRect, DrawWindow, "", windowStyle);
        }

        private void EnsureWindowBg()
        {
            if (_windowBg != null)
                return;

            _windowBg = new Texture2D(1, 1);
            _windowBg.SetPixel(0, 0, new Color(0.08f, 0.08f, 0.12f, 0.96f));
            _windowBg.Apply();
        }

        private void DrawWindow(int windowId)
        {
            bool isHost = NemesisMod.Instance?.IsHost ?? false;
            string[] tabNames = isHost ? HostTabNames : ClientTabNames;
            if (_activeTab >= tabNames.Length)
                _activeTab = 0;

            DrawTopBar(isHost);

            GUILayout.Space(6);

            float bodyHeight = Mathf.Max(190f, _windowRect.height - 180f);
            GUILayout.BeginHorizontal(GUILayout.Height(bodyHeight));
            DrawModuleRail(isHost, tabNames, bodyHeight);
            GUILayout.Space(8);
            DrawActiveModulePanel(isHost, tabNames, bodyHeight);
            GUILayout.EndHorizontal();

            GUILayout.Space(8);

            // Bottom actions
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save Config", GUILayout.Height(30)))
            {
                ConfigManager.Save(_config);
                NemesisMod.Instance?.NotifyConfigChanged();
            }

            if (GUILayout.Button("Close [F10]", GUILayout.Height(30)))
            {
                _visible = false;
                Cursor.lockState = _previousLockState;
                Cursor.visible = _previousCursorVisible;
            }
            GUILayout.EndHorizontal();

            GUI.DragWindow();
        }

        private void DrawTopBar(bool isHost)
        {
            GUILayout.BeginVertical(GUIStyles.SectionBox);
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();
            GUILayout.Label("Nemesis Control Center", GUIStyles.Header);
            GUILayout.Label(
                isHost
                    ? "Host mode: gameplay changes are synced to all players."
                    : "Client mode: visual and audio changes are local.",
                GUIStyles.MutedLabel);
            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();

            GUILayout.BeginVertical();
            GUILayout.Label("Scale", GUIStyles.MutedLabel, GUILayout.Width(150));
            GUILayout.BeginHorizontal();
            for (int i = 0; i < SizeNames.Length; i++)
            {
                var style = i == _sizePreset ? GUIStyles.TabActive : GUIStyles.TabInactive;
                if (GUILayout.Button(SizeNames[i], style, GUILayout.Width(38), GUILayout.Height(22)))
                {
                    _sizePreset = i;
                    ApplySizePreset(i);
                    GUIStyles.SetScale(GetFontScale());
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private void DrawModuleRail(bool isHost, string[] tabNames, float bodyHeight)
        {
            float railWidth = Mathf.Clamp(_windowRect.width * 0.34f, 220f, 380f);

            GUILayout.BeginVertical(GUIStyles.SectionBox, GUILayout.Width(railWidth), GUILayout.Height(bodyHeight));
            GUILayout.Label("Modules", GUIStyles.SubHeader);
            GUILayout.Label("Search and switch quickly", GUIStyles.MutedLabel);

            _moduleSearch = GUILayout.TextField(_moduleSearch ?? string.Empty, GUIStyles.SearchField, GUILayout.Height(24));
            _enabledOnly = GUILayout.Toggle(_enabledOnly, "Enabled only", GUIStyles.ToggleCompact);

            GUILayout.Space(6);

            _navScroll = GUILayout.BeginScrollView(_navScroll, false, true);
            var entries = BuildTabEntries(isHost, tabNames);
            string currentGroup = string.Empty;
            bool anyVisible = false;

            foreach (var entry in entries)
            {
                if (!ShouldShowEntry(entry))
                    continue;

                if (!string.Equals(currentGroup, entry.Group, StringComparison.Ordinal))
                {
                    currentGroup = entry.Group;
                    GUILayout.Space(4);
                    GUILayout.Label(currentGroup, GUIStyles.NavGroup);
                }

                var style = entry.Index == _activeTab ? GUIStyles.NavItemActive : GUIStyles.NavItemInactive;
                string label = $"{(entry.Enabled ? "[ON]" : "[OFF]")} {entry.Name}";
                if (GUILayout.Button(label, style, GUILayout.Height(26)))
                {
                    _activeTab = entry.Index;
                    _contentScroll = Vector2.zero;
                }

                anyVisible = true;
            }

            if (!anyVisible)
            {
                GUILayout.Space(8);
                GUILayout.Label("No modules match the current filter.", GUIStyles.MutedLabel);
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        private void DrawActiveModulePanel(bool isHost, string[] tabNames, float bodyHeight)
        {
            bool isEnabled = IsTabEnabled(_activeTab, tabNames.Length);

            GUILayout.BeginVertical(GUIStyles.SectionBox, GUILayout.ExpandWidth(true), GUILayout.Height(bodyHeight));
            GUILayout.Label(tabNames[_activeTab], GUIStyles.ModuleTitle);
            GUILayout.Label(isEnabled ? "Status: Enabled" : "Status: Disabled", isEnabled ? GUIStyles.StatusOn : GUIStyles.StatusOff);
            GUILayout.Label("Adjust settings below, then save config.", GUIStyles.MutedLabel);

            GUILayout.Space(6);

            float contentHeight = Mathf.Max(120f, bodyHeight - 100f);
            _contentScroll = GUILayout.BeginScrollView(_contentScroll, false, true, GUILayout.Height(contentHeight));
            DrawActiveTabContent(isHost);
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        private void DrawActiveTabContent(bool isHost)
        {
            if (isHost)
            {
                switch (_activeTab)
                {
                    // Host-only gameplay tabs (synced to clients)
                    case 0:
                        DifficultyTab.Draw(_config.Difficulty);
                        break;
                    case 1:
                        RoleTab.Draw(_config.Roles);
                        break;
                    case 2:
                        DamageScaleTab.Draw(_config.DamageScale);
                        break;
                    case 3:
                        MoreMimicsTab.Draw(_config.MoreMimics);
                        break;
                    case 4:
                        StaminaTab.Draw(_config.Stamina);
                        break;
                    case 5:
                        JumpTab.Draw(_config.Jump);
                        break;
                    case 6:
                        FullbrightTab.Draw(_config.Fullbright);
                        break;
                    case 7:
                        LootDropTab.Draw(_config.LootDrop);
                        break;
                    case 8:
                        MoreVoicesTab.Draw(_config.MoreVoices);
                        break;
                    case 9:
                        NoiseDirectorTab.Draw(_config.NoiseDirector);
                        break;
                    case 10:
                        RunMutatorDraftTab.Draw(_config.RunMutatorDraft);
                        break;
                    case 11:
                        var tramModule = FindModule<TramTalentTreeModule>();
                        if (tramModule != null)
                            TramTalentTreeTab.Draw(_config.TramTalentTree, tramModule.State, tramModule.Engine);
                        else
                            TramTalentTreeTab.Draw(_config.TramTalentTree);
                        break;
                    case 12:
                        ContractBoardTab.Draw(_config.ContractBoard, FindModule<ContractBoardModule>());
                        break;
                    case 13:
                        PossessionPlusTab.Draw(_config.PossessionPlus);
                        break;
                    case 14:
                        RivalGhostsTab.Draw(_config.RivalGhosts);
                        break;
                    case 15:
                        MarkerTab.Draw(_config.Marker);
                        break;
                    case 16:
                        InventoryExpansionTab.Draw(_config.InventoryExpansion);
                        break;
                    case 17:
                        AutoLootTab.Draw(_config.AutoLoot);
                        break;
                    case 18:
                        FlyTab.Draw(_config.Fly);
                        break;
                    case 19:
                        var progressionModule = FindModule<PersistentProgressionModule>();
                        ProgressionTab.Draw(_config.Progression, progressionModule);
                        break;
                    // Client-visible tabs (local settings)
                    case 20:
                        RadarTab.Draw(_config.Radar);
                        break;
                    case 21:
                        FovTab.Draw(_config.Fov);
                        break;
                    case 22:
                        VoiceFixTab.Draw(_config.VoiceFix);
                        break;
                    case 23:
                        HealthIndicatorsTab.Draw(_config.HealthIndicators);
                        break;
                    case 24:
                        TooltipTab.Draw(_config.Tooltip);
                        break;
                    case 25:
                        EspTab.Draw(_config.Esp);
                        break;
                }
            }
            else
            {
                switch (_activeTab)
                {
                    case 0:
                        RadarTab.Draw(_config.Radar);
                        break;
                    case 1:
                        FovTab.Draw(_config.Fov);
                        break;
                    case 2:
                        VoiceFixTab.Draw(_config.VoiceFix);
                        break;
                    case 3:
                        HealthIndicatorsTab.Draw(_config.HealthIndicators);
                        break;
                    case 4:
                        TooltipTab.Draw(_config.Tooltip);
                        break;
                    case 5:
                        EspTab.Draw(_config.Esp);
                        break;
                }
            }
        }

        private List<TabEntry> BuildTabEntries(bool isHost, string[] tabNames)
        {
            var entries = new List<TabEntry>(tabNames.Length);
            for (int i = 0; i < tabNames.Length; i++)
            {
                entries.Add(new TabEntry(
                    i,
                    tabNames[i],
                    GetTabGroup(isHost, i),
                    IsTabEnabled(i, tabNames.Length)));
            }

            return entries;
        }

        private bool ShouldShowEntry(TabEntry entry)
        {
            if (_enabledOnly && !entry.Enabled)
                return false;

            if (string.IsNullOrWhiteSpace(_moduleSearch))
                return true;

            string term = _moduleSearch.Trim();
            return entry.Name.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0
                   || entry.Group.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static string GetTabGroup(bool isHost, int tabIndex)
        {
            if (!isHost)
                return "Client";

            if (tabIndex <= 7)
                return "Gameplay";
            if (tabIndex <= 14)
                return "Systems";
            if (tabIndex <= 19)
                return "Utility";
            return "Client";
        }

        private float GetFontScale()
        {
            if (_sizePreset < 0 || _sizePreset >= PresetScales.Length)
                return 1.0f;

            return PresetScales[_sizePreset];
        }

        private void ApplySizePreset(int presetIndex)
        {
            if (presetIndex < 0 || presetIndex >= PresetWidths.Length || presetIndex >= PresetHeights.Length)
                return;

            float maxWidth = Mathf.Max(360f, Screen.width * 0.96f);
            float maxHeight = Mathf.Max(320f, Screen.height * 0.94f);
            float width = Mathf.Min(PresetWidths[presetIndex], maxWidth);
            float height = Mathf.Min(PresetHeights[presetIndex], maxHeight);

            _windowRect.width = width;
            _windowRect.height = height;

            float maxX = Mathf.Max(0f, Screen.width - _windowRect.width);
            float maxY = Mathf.Max(0f, Screen.height - _windowRect.height);
            _windowRect.x = Mathf.Clamp(_windowRect.x, 0f, maxX);
            _windowRect.y = Mathf.Clamp(_windowRect.y, 0f, maxY);
        }

        private bool IsTabEnabled(int tabIndex, int visibleTabCount)
        {
            // In client mode, only client-visible tabs exist (6 entries).
            if (visibleTabCount == ClientTabNames.Length)
            {
                return tabIndex switch
                {
                    0 => _config.Radar.Enabled,
                    1 => _config.Fov.Enabled,
                    2 => _config.VoiceFix.Enabled,
                    3 => _config.HealthIndicators.Enabled,
                    4 => _config.Tooltip.Enabled,
                    5 => _config.Esp.Enabled,
                    _ => false
                };
            }

            // Host mode tab order.
            return tabIndex switch
            {
                0 => _config.Difficulty.Enabled,
                1 => _config.Roles.Enabled,
                2 => _config.DamageScale.Enabled,
                3 => _config.MoreMimics.Enabled,
                4 => _config.Stamina.Enabled,
                5 => _config.Jump.Enabled,
                6 => _config.Fullbright.Enabled,
                7 => _config.LootDrop.Enabled,
                8 => _config.MoreVoices.Enabled,
                9 => _config.NoiseDirector.Enabled,
                10 => _config.RunMutatorDraft.Enabled,
                11 => _config.TramTalentTree.Enabled,
                12 => _config.ContractBoard.Enabled,
                13 => _config.PossessionPlus.Enabled,
                14 => _config.RivalGhosts.Enabled,
                15 => _config.Marker.Enabled,
                16 => _config.InventoryExpansion.Enabled,
                17 => _config.AutoLoot.Enabled,
                18 => _config.Fly.Enabled,
                19 => _config.Progression.Enabled,
                20 => _config.Radar.Enabled,
                21 => _config.Fov.Enabled,
                22 => _config.VoiceFix.Enabled,
                23 => _config.HealthIndicators.Enabled,
                24 => _config.Tooltip.Enabled,
                25 => _config.Esp.Enabled,
                _ => false
            };
        }

        private T? FindModule<T>() where T : class, IModule
        {
            foreach (var module in _modules)
            {
                if (module is T typed)
                    return typed;
            }

            return null;
        }

        private readonly struct TabEntry
        {
            public TabEntry(int index, string name, string group, bool enabled)
            {
                Index = index;
                Name = name;
                Group = group;
                Enabled = enabled;
            }

            public int Index { get; }
            public string Name { get; }
            public string Group { get; }
            public bool Enabled { get; }
        }
    }
}

using System.Collections.Generic;
using System;
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
        private bool _showTabDropdown;
        private Vector2 _tabDropdownScroll;
        private Vector2 _contentScroll;

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
        private static readonly string[] ClientTabNames = { "Radar", "FOV", "VoiceFix", "Health", "Tooltip", "ESP" };

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
            if (!_visible) return;

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
            if (_windowBg != null) return;
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

            // Title
            GUILayout.Label("Nemesis Admin Panel", GUIStyles.Header);

            // Size preset buttons
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Size:", GUIStyles.Label, GUILayout.Width(35));
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
            if (!isHost)
            {
                GUILayout.Space(10);
                GUILayout.Label("(Client mode)", GUIStyles.Label);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            DrawTabSelector(tabNames);

            GUILayout.Space(8);

            // Draw active tab content
            float contentHeight = Mathf.Max(150f, _windowRect.height - (_showTabDropdown ? 330f : 230f));
            _contentScroll = GUILayout.BeginScrollView(_contentScroll, false, true, GUILayout.Height(contentHeight));
            if (isHost)
            {
                switch (_activeTab)
                {
                    // Host-only gameplay tabs (synced to clients)
                    case 0: DifficultyTab.Draw(_config.Difficulty); break;
                    case 1: RoleTab.Draw(_config.Roles); break;
                    case 2: DamageScaleTab.Draw(_config.DamageScale); break;
                    case 3: MoreMimicsTab.Draw(_config.MoreMimics); break;
                    case 4: StaminaTab.Draw(_config.Stamina); break;
                    case 5: JumpTab.Draw(_config.Jump); break;
                    case 6: FullbrightTab.Draw(_config.Fullbright); break;
                    case 7: LootDropTab.Draw(_config.LootDrop); break;
                    case 8: MoreVoicesTab.Draw(_config.MoreVoices); break;
                    case 9: NoiseDirectorTab.Draw(_config.NoiseDirector); break;
                    case 10: RunMutatorDraftTab.Draw(_config.RunMutatorDraft); break;
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
                    case 13: PossessionPlusTab.Draw(_config.PossessionPlus); break;
                    case 14: RivalGhostsTab.Draw(_config.RivalGhosts); break;
                    case 15: MarkerTab.Draw(_config.Marker); break;
                    case 16: InventoryExpansionTab.Draw(_config.InventoryExpansion); break;
                    case 17: AutoLootTab.Draw(_config.AutoLoot); break;
                    case 18: FlyTab.Draw(_config.Fly); break;
                    case 19:
                        var pm = FindModule<PersistentProgressionModule>();
                        ProgressionTab.Draw(_config.Progression, pm);
                        break;
                    // Client-visible tabs (local settings)
                    case 20: RadarTab.Draw(_config.Radar); break;
                    case 21: FovTab.Draw(_config.Fov); break;
                    case 22: VoiceFixTab.Draw(_config.VoiceFix); break;
                    case 23: HealthIndicatorsTab.Draw(_config.HealthIndicators); break;
                    case 24: TooltipTab.Draw(_config.Tooltip); break;
                    case 25: EspTab.Draw(_config.Esp); break;
                }
            }
            else
            {
                switch (_activeTab)
                {
                    case 0: RadarTab.Draw(_config.Radar); break;
                    case 1: FovTab.Draw(_config.Fov); break;
                    case 2: VoiceFixTab.Draw(_config.VoiceFix); break;
                    case 3: HealthIndicatorsTab.Draw(_config.HealthIndicators); break;
                    case 4: TooltipTab.Draw(_config.Tooltip); break;
                    case 5: EspTab.Draw(_config.Esp); break;
                }
            }
            GUILayout.EndScrollView();

            GUILayout.FlexibleSpace();

            // Bottom buttons
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save Config", GUILayout.Height(28)))
            {
                ConfigManager.Save(_config);
                NemesisMod.Instance?.NotifyConfigChanged();
            }
            if (GUILayout.Button("Close [F10]", GUILayout.Height(28)))
            {
                _visible = false;
                Cursor.lockState = _previousLockState;
                Cursor.visible = _previousCursorVisible;
            }
            GUILayout.EndHorizontal();

            GUI.DragWindow();
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

        private void DrawTabSelector(string[] tabNames)
        {
            if (tabNames.Length == 0)
                return;

            GUILayout.BeginHorizontal();
            GUILayout.Label("Module:", GUIStyles.Label, GUILayout.Width(52));

            if (GUILayout.Button(tabNames[_activeTab], GUIStyles.TabActive, GUILayout.Height(28)))
                _showTabDropdown = !_showTabDropdown;
            if (GUILayout.Button(_showTabDropdown ? "▲" : "▼", GUILayout.Width(34), GUILayout.Height(28)))
                _showTabDropdown = !_showTabDropdown;

            GUILayout.Space(6);
            if (GUILayout.Button("<", GUILayout.Width(30), GUILayout.Height(28)))
                SelectPreviousTab(tabNames);
            if (GUILayout.Button(">", GUILayout.Width(30), GUILayout.Height(28)))
                SelectNextTab(tabNames);
            GUILayout.EndHorizontal();

            if (!_showTabDropdown)
                return;

            GUILayout.BeginVertical(GUIStyles.SectionBox);
            float dropdownHeight = Mathf.Clamp(_windowRect.height * 0.33f, 120f, 260f);
            _tabDropdownScroll = GUILayout.BeginScrollView(_tabDropdownScroll, false, true, GUILayout.Height(dropdownHeight));
            for (int i = 0; i < tabNames.Length; i++)
            {
                var style = i == _activeTab ? GUIStyles.TabActive : GUIStyles.TabInactive;
                if (GUILayout.Button(tabNames[i], style, GUILayout.Height(26)))
                {
                    _activeTab = i;
                    _showTabDropdown = false;
                    _contentScroll = Vector2.zero;
                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        private void SelectPreviousTab(string[] tabNames)
        {
            if (tabNames.Length == 0)
                return;

            _activeTab = (_activeTab - 1 + tabNames.Length) % tabNames.Length;
            _contentScroll = Vector2.zero;
        }

        private void SelectNextTab(string[] tabNames)
        {
            if (tabNames.Length == 0)
                return;

            _activeTab = (_activeTab + 1) % tabNames.Length;
            _contentScroll = Vector2.zero;
        }

        private T? FindModule<T>() where T : class, IModule
        {
            foreach (var m in _modules)
            {
                if (m is T typed) return typed;
            }
            return null;
        }
    }
}

using System.Collections.Generic;
using Nemesis.Config;
using Nemesis.Core;
using Nemesis.Modules.PersistentProgression;
using Nemesis.UI.Tabs;
using UnityEngine;

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

        private static readonly string[] TabNames = { "Difficulty", "Roles", "Radar", "Progression" };

        public AdminPanel(SuiteConfig config, List<IModule> modules)
        {
            _config = config;
            _modules = modules;
        }

        public void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.F10))
            {
                _visible = !_visible;
                if (_visible)
                {
                    _previousLockState = Cursor.lockState;
                    _previousCursorVisible = Cursor.visible;
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
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
            _windowRect = GUI.Window(9876, _windowRect, DrawWindow, "");
        }

        private void DrawWindow(int windowId)
        {
            GUILayout.Label("Nemesis Admin Panel", GUIStyles.Header);
            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            for (int i = 0; i < TabNames.Length; i++)
            {
                var style = i == _activeTab ? GUIStyles.TabActive : GUIStyles.TabInactive;
                if (GUILayout.Button(TabNames[i], style, GUILayout.Height(30)))
                    _activeTab = i;
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            switch (_activeTab)
            {
                case 0:
                    DifficultyTab.Draw(_config.Difficulty);
                    break;
                case 1:
                    RoleTab.Draw(_config.Roles);
                    break;
                case 2:
                    RadarTab.Draw(_config.Radar);
                    break;
                case 3:
                    var progressionModule = FindModule<PersistentProgressionModule>();
                    ProgressionTab.Draw(_config.Progression, progressionModule);
                    break;
            }

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save Config", GUILayout.Height(30)))
                ConfigManager.Save(_config);
            if (GUILayout.Button("Close [F10]", GUILayout.Height(30)))
            {
                _visible = false;
                Cursor.lockState = _previousLockState;
                Cursor.visible = _previousCursorVisible;
            }
            GUILayout.EndHorizontal();

            GUI.DragWindow();
        }

        private T FindModule<T>() where T : class, IModule
        {
            foreach (var m in _modules)
            {
                if (m is T typed) return typed;
            }
            return null;
        }
    }
}

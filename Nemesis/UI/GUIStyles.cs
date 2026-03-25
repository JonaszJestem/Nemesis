using UnityEngine;

namespace Nemesis.UI
{
    internal static class GUIStyles
    {
        private static bool _initialized;
        private static GUIStyle? _header;
        private static GUIStyle? _subHeader;
        private static GUIStyle? _label;
        private static GUIStyle? _valueLabel;
        private static GUIStyle? _tabActive;
        private static GUIStyle? _tabInactive;
        private static GUIStyle? _sectionBox;
        private static Texture2D? _darkBg;
        private static Texture2D? _accentBg;
        private static Texture2D? _sectionBg;

        public static GUIStyle Header => _header!;
        public static GUIStyle SubHeader => _subHeader!;
        public static GUIStyle Label => _label!;
        public static GUIStyle ValueLabel => _valueLabel!;
        public static GUIStyle TabActive => _tabActive!;
        public static GUIStyle TabInactive => _tabInactive!;
        public static GUIStyle SectionBox => _sectionBox!;

        public static void EnsureInitialized()
        {
            if (_initialized) return;

            _darkBg = MakeTexture(new Color(0.12f, 0.12f, 0.15f, 0.95f));
            _accentBg = MakeTexture(new Color(0.2f, 0.35f, 0.55f, 1f));
            _sectionBg = MakeTexture(new Color(0.15f, 0.15f, 0.18f, 0.9f));

            _header = new GUIStyle(GUI.skin.label)
            {
                fontSize = 18, fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = new Color(0.8f, 0.9f, 1f) }
            };

            _subHeader = new GUIStyle(GUI.skin.label)
            {
                fontSize = 14, fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.6f, 0.75f, 0.9f) }
            };

            _label = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                normal = { textColor = new Color(0.8f, 0.8f, 0.85f) }
            };

            _valueLabel = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12, alignment = TextAnchor.MiddleRight,
                normal = { textColor = new Color(0.5f, 0.8f, 1f) }
            };

            _tabActive = new GUIStyle(GUI.skin.button)
            {
                fontSize = 13, fontStyle = FontStyle.Bold,
                normal = { background = _accentBg, textColor = Color.white },
                hover = { background = _accentBg, textColor = Color.white },
                active = { background = _accentBg, textColor = Color.white }
            };

            _tabInactive = new GUIStyle(GUI.skin.button)
            {
                fontSize = 13,
                normal = { textColor = new Color(0.6f, 0.6f, 0.65f) }
            };

            _sectionBox = new GUIStyle(GUI.skin.box)
            {
                normal = { background = _sectionBg },
                padding = new RectOffset(10, 10, 8, 8),
                margin = new RectOffset(0, 0, 5, 5)
            };

            _initialized = true;
        }

        public static float LabeledSlider(string label, float value, float min, float max, string format = "F2")
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, _label, GUILayout.Width(200));
            value = GUILayout.HorizontalSlider(value, min, max, GUILayout.Width(200));
            GUILayout.Label(value.ToString(format), _valueLabel, GUILayout.Width(60));
            GUILayout.EndHorizontal();
            return value;
        }

        public static int LabeledIntSlider(string label, int value, int min, int max)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, _label, GUILayout.Width(200));
            value = (int)GUILayout.HorizontalSlider(value, min, max, GUILayout.Width(200));
            GUILayout.Label(value.ToString(), _valueLabel, GUILayout.Width(60));
            GUILayout.EndHorizontal();
            return value;
        }

        public static bool LabeledToggle(string label, bool value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, _label, GUILayout.Width(200));
            value = GUILayout.Toggle(value, value ? "ON" : "OFF", GUILayout.Width(60));
            GUILayout.EndHorizontal();
            return value;
        }

        public static void Dispose()
        {
            if (_darkBg != null) Object.Destroy(_darkBg);
            if (_accentBg != null) Object.Destroy(_accentBg);
            if (_sectionBg != null) Object.Destroy(_sectionBg);
            _darkBg = null;
            _accentBg = null;
            _sectionBg = null;
            _initialized = false;
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

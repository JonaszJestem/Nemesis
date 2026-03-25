using UnityEngine;

namespace Nemesis.UI
{
    internal static class GUIStyles
    {
        private static bool _initialized;
        private static float _scale = 1.0f;
        private static GUIStyle? _header;
        private static GUIStyle? _moduleTitle;
        private static GUIStyle? _subHeader;
        private static GUIStyle? _navGroup;
        private static GUIStyle? _mutedLabel;
        private static GUIStyle? _label;
        private static GUIStyle? _valueLabel;
        private static GUIStyle? _tabActive;
        private static GUIStyle? _tabInactive;
        private static GUIStyle? _navItemActive;
        private static GUIStyle? _navItemInactive;
        private static GUIStyle? _searchField;
        private static GUIStyle? _toggleCompact;
        private static GUIStyle? _statusOn;
        private static GUIStyle? _statusOff;
        private static GUIStyle? _sectionBox;
        private static Texture2D? _accentBg;
        private static Texture2D? _sectionBg;
        private static Texture2D? _navActiveBg;

        public static GUIStyle Header => _header!;
        public static GUIStyle ModuleTitle => _moduleTitle!;
        public static GUIStyle SubHeader => _subHeader!;
        public static GUIStyle NavGroup => _navGroup!;
        public static GUIStyle MutedLabel => _mutedLabel!;
        public static GUIStyle Label => _label!;
        public static GUIStyle ValueLabel => _valueLabel!;
        public static GUIStyle TabActive => _tabActive!;
        public static GUIStyle TabInactive => _tabInactive!;
        public static GUIStyle NavItemActive => _navItemActive!;
        public static GUIStyle NavItemInactive => _navItemInactive!;
        public static GUIStyle SearchField => _searchField!;
        public static GUIStyle ToggleCompact => _toggleCompact!;
        public static GUIStyle StatusOn => _statusOn!;
        public static GUIStyle StatusOff => _statusOff!;
        public static GUIStyle SectionBox => _sectionBox!;

        public static void SetScale(float scale)
        {
            if (System.Math.Abs(_scale - scale) < 0.01f)
                return;

            _scale = scale;
            _initialized = false;
        }

        public static void EnsureInitialized()
        {
            if (_initialized)
                return;

            _accentBg = MakeTexture(new Color(0.18f, 0.43f, 0.63f, 1f));
            _sectionBg = MakeTexture(new Color(0.12f, 0.13f, 0.18f, 0.96f));
            _navActiveBg = MakeTexture(new Color(0.16f, 0.24f, 0.36f, 0.96f));

            int headerSize = Mathf.RoundToInt(20 * _scale);
            int moduleTitleSize = Mathf.RoundToInt(17 * _scale);
            int subHeaderSize = Mathf.RoundToInt(14 * _scale);
            int bodySize = Mathf.RoundToInt(12 * _scale);
            int smallSize = Mathf.RoundToInt(11 * _scale);
            int tabSize = Mathf.RoundToInt(12 * _scale);

            _header = new GUIStyle(GUI.skin.label)
            {
                fontSize = headerSize,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft,
                normal = { textColor = new Color(0.9f, 0.95f, 1f) }
            };

            _moduleTitle = new GUIStyle(GUI.skin.label)
            {
                fontSize = moduleTitleSize,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft,
                normal = { textColor = new Color(0.86f, 0.92f, 1f) }
            };

            _subHeader = new GUIStyle(GUI.skin.label)
            {
                fontSize = subHeaderSize,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.64f, 0.79f, 0.96f) }
            };

            _navGroup = new GUIStyle(GUI.skin.label)
            {
                fontSize = smallSize,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.53f, 0.64f, 0.78f) },
                margin = new RectOffset(2, 2, 6, 2)
            };

            _mutedLabel = new GUIStyle(GUI.skin.label)
            {
                fontSize = smallSize,
                alignment = TextAnchor.MiddleLeft,
                normal = { textColor = new Color(0.62f, 0.66f, 0.74f) }
            };

            _label = new GUIStyle(GUI.skin.label)
            {
                fontSize = bodySize,
                normal = { textColor = new Color(0.9f, 0.9f, 0.94f) }
            };

            _valueLabel = new GUIStyle(GUI.skin.label)
            {
                fontSize = bodySize,
                alignment = TextAnchor.MiddleRight,
                normal = { textColor = new Color(0.44f, 0.86f, 0.98f) }
            };

            _statusOn = new GUIStyle(GUI.skin.label)
            {
                fontSize = smallSize,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.52f, 0.92f, 0.66f) }
            };

            _statusOff = new GUIStyle(GUI.skin.label)
            {
                fontSize = smallSize,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.96f, 0.58f, 0.58f) }
            };

            _tabActive = new GUIStyle(GUI.skin.button)
            {
                fontSize = tabSize,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = { background = _accentBg, textColor = Color.white },
                hover = { background = _accentBg, textColor = Color.white },
                active = { background = _accentBg, textColor = Color.white }
            };

            _tabInactive = new GUIStyle(GUI.skin.button)
            {
                fontSize = tabSize,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = new Color(0.74f, 0.74f, 0.79f) }
            };

            _navItemActive = new GUIStyle(GUI.skin.button)
            {
                fontSize = tabSize,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft,
                padding = new RectOffset(10, 10, 4, 4),
                normal = { background = _navActiveBg, textColor = Color.white },
                hover = { background = _navActiveBg, textColor = Color.white },
                active = { background = _navActiveBg, textColor = Color.white }
            };

            _navItemInactive = new GUIStyle(GUI.skin.button)
            {
                fontSize = tabSize,
                alignment = TextAnchor.MiddleLeft,
                padding = new RectOffset(10, 10, 4, 4),
                normal = { textColor = new Color(0.78f, 0.8f, 0.85f) }
            };

            _searchField = new GUIStyle(GUI.skin.textField)
            {
                fontSize = bodySize,
                padding = new RectOffset(8, 8, 5, 5),
                normal = { textColor = new Color(0.95f, 0.95f, 0.97f) }
            };

            _toggleCompact = new GUIStyle(GUI.skin.toggle)
            {
                fontSize = smallSize,
                normal = { textColor = new Color(0.78f, 0.82f, 0.88f) }
            };

            _sectionBox = new GUIStyle(GUI.skin.box)
            {
                normal = { background = _sectionBg },
                padding = new RectOffset(12, 12, 10, 10),
                margin = new RectOffset(0, 0, 0, 0)
            };

            _initialized = true;
        }

        public static float LabeledSlider(string label, float value, float min, float max, string format = "F2")
        {
            int minLabelW = Mathf.RoundToInt(120 * _scale);
            int maxLabelW = Mathf.RoundToInt(230 * _scale);
            int valW = Mathf.RoundToInt(60 * _scale);

            GUILayout.BeginHorizontal();
            GUILayout.Label(label, _label, GUILayout.MinWidth(minLabelW), GUILayout.MaxWidth(maxLabelW));
            value = GUILayout.HorizontalSlider(value, min, max, GUILayout.ExpandWidth(true));
            GUILayout.Label(value.ToString(format), _valueLabel, GUILayout.Width(valW));
            GUILayout.EndHorizontal();
            return value;
        }

        public static int LabeledIntSlider(string label, int value, int min, int max)
        {
            int minLabelW = Mathf.RoundToInt(120 * _scale);
            int maxLabelW = Mathf.RoundToInt(230 * _scale);
            int valW = Mathf.RoundToInt(60 * _scale);

            GUILayout.BeginHorizontal();
            GUILayout.Label(label, _label, GUILayout.MinWidth(minLabelW), GUILayout.MaxWidth(maxLabelW));
            value = (int)GUILayout.HorizontalSlider(value, min, max, GUILayout.ExpandWidth(true));
            GUILayout.Label(value.ToString(), _valueLabel, GUILayout.Width(valW));
            GUILayout.EndHorizontal();
            return value;
        }

        public static bool LabeledToggle(string label, bool value)
        {
            int minLabelW = Mathf.RoundToInt(120 * _scale);
            int maxLabelW = Mathf.RoundToInt(230 * _scale);

            GUILayout.BeginHorizontal();
            GUILayout.Label(label, _label, GUILayout.MinWidth(minLabelW), GUILayout.MaxWidth(maxLabelW));
            value = GUILayout.Toggle(value, value ? "ON" : "OFF", GUILayout.Width(Mathf.RoundToInt(65 * _scale)));
            GUILayout.EndHorizontal();
            return value;
        }

        public static void Dispose()
        {
            if (_accentBg != null)
                Object.Destroy(_accentBg);
            if (_sectionBg != null)
                Object.Destroy(_sectionBg);
            if (_navActiveBg != null)
                Object.Destroy(_navActiveBg);

            _accentBg = null;
            _sectionBg = null;
            _navActiveBg = null;
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

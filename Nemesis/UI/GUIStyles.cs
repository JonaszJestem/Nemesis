using UnityEngine;

namespace Nemesis.UI
{
    internal static class GUIStyles
    {
        private static bool _initialized;
        private static float _scale = 1.0f;
        private static GUIStyle? _header;
        private static GUIStyle? _subHeader;
        private static GUIStyle? _label;
        private static GUIStyle? _valueLabel;
        private static GUIStyle? _tabActive;
        private static GUIStyle? _tabInactive;
        private static GUIStyle? _sectionBox;
        private static Texture2D? _accentBg;
        private static Texture2D? _sectionBg;

        public static GUIStyle Header => _header!;
        public static GUIStyle SubHeader => _subHeader!;
        public static GUIStyle Label => _label!;
        public static GUIStyle ValueLabel => _valueLabel!;
        public static GUIStyle TabActive => _tabActive!;
        public static GUIStyle TabInactive => _tabInactive!;
        public static GUIStyle SectionBox => _sectionBox!;

        public static void SetScale(float scale)
        {
            if (System.Math.Abs(_scale - scale) < 0.01f) return;
            _scale = scale;
            _initialized = false;
        }

        public static void EnsureInitialized()
        {
            if (_initialized) return;

            _accentBg = MakeTexture(new Color(0.2f, 0.35f, 0.55f, 1f));
            _sectionBg = MakeTexture(new Color(0.13f, 0.13f, 0.17f, 0.95f));

            int headerSize = Mathf.RoundToInt(18 * _scale);
            int subHeaderSize = Mathf.RoundToInt(14 * _scale);
            int bodySize = Mathf.RoundToInt(12 * _scale);
            int tabSize = Mathf.RoundToInt(13 * _scale);

            _header = new GUIStyle(GUI.skin.label)
            {
                fontSize = headerSize, fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = new Color(0.85f, 0.92f, 1f) }
            };

            _subHeader = new GUIStyle(GUI.skin.label)
            {
                fontSize = subHeaderSize, fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.55f, 0.75f, 1f) }
            };

            _label = new GUIStyle(GUI.skin.label)
            {
                fontSize = bodySize,
                normal = { textColor = new Color(0.88f, 0.88f, 0.92f) }
            };

            _valueLabel = new GUIStyle(GUI.skin.label)
            {
                fontSize = bodySize, alignment = TextAnchor.MiddleRight,
                normal = { textColor = new Color(0.4f, 0.85f, 1f) }
            };

            _tabActive = new GUIStyle(GUI.skin.button)
            {
                fontSize = tabSize, fontStyle = FontStyle.Bold,
                normal = { background = _accentBg, textColor = Color.white },
                hover = { background = _accentBg, textColor = Color.white },
                active = { background = _accentBg, textColor = Color.white }
            };

            _tabInactive = new GUIStyle(GUI.skin.button)
            {
                fontSize = tabSize,
                normal = { textColor = new Color(0.65f, 0.65f, 0.7f) }
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
            int labelW = Mathf.RoundToInt(200 * _scale);
            int sliderW = Mathf.RoundToInt(200 * _scale);
            int valW = Mathf.RoundToInt(60 * _scale);

            GUILayout.BeginHorizontal();
            GUILayout.Label(label, _label, GUILayout.Width(labelW));
            value = GUILayout.HorizontalSlider(value, min, max, GUILayout.Width(sliderW));
            GUILayout.Label(value.ToString(format), _valueLabel, GUILayout.Width(valW));
            GUILayout.EndHorizontal();
            return value;
        }

        public static int LabeledIntSlider(string label, int value, int min, int max)
        {
            int labelW = Mathf.RoundToInt(200 * _scale);
            int sliderW = Mathf.RoundToInt(200 * _scale);
            int valW = Mathf.RoundToInt(60 * _scale);

            GUILayout.BeginHorizontal();
            GUILayout.Label(label, _label, GUILayout.Width(labelW));
            value = (int)GUILayout.HorizontalSlider(value, min, max, GUILayout.Width(sliderW));
            GUILayout.Label(value.ToString(), _valueLabel, GUILayout.Width(valW));
            GUILayout.EndHorizontal();
            return value;
        }

        public static bool LabeledToggle(string label, bool value)
        {
            int labelW = Mathf.RoundToInt(200 * _scale);

            GUILayout.BeginHorizontal();
            GUILayout.Label(label, _label, GUILayout.Width(labelW));
            value = GUILayout.Toggle(value, value ? "ON" : "OFF", GUILayout.Width(60));
            GUILayout.EndHorizontal();
            return value;
        }

        public static void Dispose()
        {
            if (_accentBg != null) Object.Destroy(_accentBg);
            if (_sectionBg != null) Object.Destroy(_sectionBg);
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

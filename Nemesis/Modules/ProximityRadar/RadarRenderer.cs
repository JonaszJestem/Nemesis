using System.Collections.Generic;
using UnityEngine;

namespace Nemesis.Modules.ProximityRadar
{
    internal class RadarRenderer
    {
        private Texture2D? _bgTexture;
        private Texture2D? _dotRed;
        private Texture2D? _dotYellow;
        private Texture2D? _dotGreen;
        private Texture2D? _dotWhite;
        private Texture2D? _borderTexture;
        private GUIStyle? _labelStyle;
        private bool _initialized;

        internal struct RadarEntity
        {
            public Vector3 WorldPosition;
            public EntityType Type;
        }

        internal enum EntityType
        {
            Monster,
            Loot,
            Player
        }

        public void EnsureInitialized()
        {
            if (_initialized) return;

            _bgTexture = MakeTexture(new Color(0.05f, 0.05f, 0.1f, 0.75f));
            _borderTexture = MakeTexture(new Color(0.3f, 0.4f, 0.5f, 0.8f));
            _dotRed = MakeTexture(new Color(1f, 0.2f, 0.2f, 1f));
            _dotYellow = MakeTexture(new Color(1f, 0.85f, 0.1f, 1f));
            _dotGreen = MakeTexture(new Color(0.2f, 1f, 0.4f, 1f));
            _dotWhite = MakeTexture(Color.white);
            _labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 10,
                alignment = TextAnchor.LowerCenter,
                normal = { textColor = new Color(0.7f, 0.7f, 0.7f, 0.6f) }
            };

            _initialized = true;
        }

        public void Draw(Rect radarRect, Vector3 playerPos, float range,
            List<RadarEntity> entities, RadarConfig config)
        {
            EnsureInitialized();

            Color prevColor = GUI.color;
            GUI.color = new Color(1, 1, 1, config.RadarOpacity);

            // Background
            GUI.DrawTexture(radarRect, _bgTexture);

            // Border (2px)
            GUI.DrawTexture(new Rect(radarRect.x - 2, radarRect.y - 2, radarRect.width + 4, 2), _borderTexture);
            GUI.DrawTexture(new Rect(radarRect.x - 2, radarRect.yMax, radarRect.width + 4, 2), _borderTexture);
            GUI.DrawTexture(new Rect(radarRect.x - 2, radarRect.y, 2, radarRect.height), _borderTexture);
            GUI.DrawTexture(new Rect(radarRect.xMax, radarRect.y, 2, radarRect.height), _borderTexture);

            // Center crosshair
            float cx = radarRect.x + radarRect.width / 2f;
            float cy = radarRect.y + radarRect.height / 2f;
            GUI.DrawTexture(new Rect(cx - 3, cy, 7, 1), _dotWhite);
            GUI.DrawTexture(new Rect(cx, cy - 3, 1, 7), _dotWhite);

            float halfSize = radarRect.width / 2f;

            foreach (var entity in entities)
            {
                float dx = entity.WorldPosition.x - playerPos.x;
                float dz = entity.WorldPosition.z - playerPos.z;

                float nx = (dx / range) * halfSize;
                float ny = -(dz / range) * halfSize;

                float dist = Mathf.Sqrt(nx * nx + ny * ny);
                if (dist > halfSize - 4)
                {
                    float scale = (halfSize - 4) / dist;
                    nx *= scale;
                    ny *= scale;
                }

                float px = cx + nx;
                float py = cy + ny;

                Texture2D dot;
                int dotSize;

                switch (entity.Type)
                {
                    case EntityType.Monster:
                        dot = _dotRed!;
                        dotSize = config.MonsterDotSize;
                        break;
                    case EntityType.Loot:
                        dot = _dotYellow!;
                        dotSize = config.LootDotSize;
                        break;
                    case EntityType.Player:
                        dot = _dotGreen!;
                        dotSize = config.PlayerDotSize;
                        break;
                    default:
                        dot = _dotWhite!;
                        dotSize = 3;
                        break;
                }

                GUI.DrawTexture(new Rect(px - dotSize / 2f, py - dotSize / 2f, dotSize, dotSize), dot);
            }

            GUI.Label(new Rect(radarRect.x, radarRect.yMax - 16, radarRect.width, 16),
                $"{range:F0}m", _labelStyle);

            GUI.color = prevColor;
        }

        public void Destroy()
        {
            if (_bgTexture != null) Object.Destroy(_bgTexture);
            if (_borderTexture != null) Object.Destroy(_borderTexture);
            if (_dotRed != null) Object.Destroy(_dotRed);
            if (_dotYellow != null) Object.Destroy(_dotYellow);
            if (_dotGreen != null) Object.Destroy(_dotGreen);
            if (_dotWhite != null) Object.Destroy(_dotWhite);
            _bgTexture = null;
            _borderTexture = null;
            _dotRed = null;
            _dotYellow = null;
            _dotGreen = null;
            _dotWhite = null;
            _labelStyle = null;
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

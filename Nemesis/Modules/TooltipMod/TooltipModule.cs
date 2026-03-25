using System;
using MimicAPI.GameAPI;
using Nemesis.Core;
using UnityEngine;

namespace Nemesis.Modules.TooltipMod
{
    internal class TooltipModule : IModule
    {
        public string Name => "Tooltip";

        private readonly TooltipConfig _config;
        private GUIStyle? _tooltipStyle;
        private int _lastFontSize;

        public TooltipModule(TooltipConfig config)
        {
            _config = config;
        }

        public void Initialize()
        {
            Log.Tooltip.Msg("Initialized");
        }

        public void Shutdown() { }

        public void OnUpdate() { }

        public void OnGUI()
        {
            if (!_config.Enabled) return;

            try
            {
                var player = PlayerAPI.GetLocalPlayer();
                if (player == null) return;

                var result = ReflectionHelper.InvokeMethod(player,
                    GameMethodNames.ProtoActor_GetSelectedInventoryItem);
                if (result == null) return;

                string itemName = result.ToString();
                if (string.IsNullOrEmpty(itemName)) return;

                EnsureStyle();

                float labelWidth = 300f;
                float labelHeight = 30f;
                float x = (Screen.width - labelWidth) / 2f;
                float y = Screen.height - 120f;

                GUI.Label(new Rect(x, y, labelWidth, labelHeight), itemName, _tooltipStyle);
            }
            catch { }
        }

        private void EnsureStyle()
        {
            if (_tooltipStyle != null && _lastFontSize == _config.FontSize) return;

            _tooltipStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = _config.FontSize,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = new Color(1f, 1f, 0.85f) }
            };
            _lastFontSize = _config.FontSize;
        }
    }
}

using System;
using MimicAPI.GameAPI;
using Nemesis.Core;
using UnityEngine;

namespace Nemesis.Modules.DifficultyDirector
{
    internal class DifficultyDirectorModule : IModule
    {
        public string Name => "Difficulty Director";
        public static float CurrentMultiplier { get; private set; } = 1.0f;

        private readonly DifficultyConfig _config;
        private float _timer;

        // Cached GUI
        private GUIStyle? _difficultyStyle;
        private float _cachedMultiplierForColor;
        private Color _cachedColor = Color.green;

        public DifficultyDirectorModule(DifficultyConfig config)
        {
            _config = config;
        }

        public void Initialize()
        {
            Log.Difficulty.Msg("Initialized");
        }

        public void Shutdown()
        {
            CurrentMultiplier = 1.0f;
        }

        public void OnUpdate()
        {
            if (!_config.Enabled) return;

            _timer += Time.deltaTime;
            if (_timer < _config.UpdateIntervalSeconds) return;
            _timer -= _config.UpdateIntervalSeconds;

            try
            {
                var room = RoomAPI.GetCurrentRoom();
                if (room == null) return;

                int playerCount = RoomAPI.GetRoomPlayers(room).Count;
                int gameDay = RoomAPI.GetCurrentGameDay(room);
                int sessionCycle = RoomAPI.GetCurrentSessionCycle(room);

                float newMult = DifficultyCalculator.ComputeMultiplier(
                    playerCount, gameDay, sessionCycle, _config);

                if (Math.Abs(newMult - CurrentMultiplier) > 0.001f)
                {
                    CurrentMultiplier = newMult;
                    float t = Mathf.InverseLerp(_config.MinMultiplier, _config.MaxMultiplier, CurrentMultiplier);
                    _cachedColor = Color.Lerp(Color.green, Color.red, t);
                    _cachedMultiplierForColor = CurrentMultiplier;
                }

                if (_config.WeatherEscalation && CurrentMultiplier >= _config.WeatherThreshold)
                {
                    WeatherAPI.UpdateWeatherAll(room, _config.StormWeatherId);
                }
            }
            catch (Exception ex)
            {
                Log.Difficulty.Warn($"Update error: {ex.Message}");
            }
        }

        public void OnGUI()
        {
            if (!_config.Enabled || !_config.ShowHudLabel) return;

            if (_difficultyStyle == null)
            {
                _difficultyStyle = new GUIStyle(GUI.skin.label) { fontSize = 14 };
            }

            _difficultyStyle.normal.textColor = _cachedColor;
            GUI.Label(new Rect(10, Screen.height - 30, 250, 25),
                $"Difficulty: x{CurrentMultiplier:F2}", _difficultyStyle);
        }
    }
}

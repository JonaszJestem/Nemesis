using System;
using System.Reflection;
using Nemesis.Core;

namespace Nemesis.Modules.MoreVoices
{
    internal class MoreVoicesModule : IModule
    {
        public string Name => "More Voices";

        private readonly MoreVoicesConfig _config;
        private bool _applied;

        public MoreVoicesModule(MoreVoicesConfig config)
        {
            _config = config;
        }

        public void Initialize()
        {
            Log.Voice.Msg("Initialized");
        }

        public void Shutdown()
        {
            _applied = false;
        }

        public void OnUpdate()
        {
            if (!_config.Enabled)
            {
                _applied = false;
                return;
            }

            if (_applied) return;

            try
            {
                var archiveType = GameReflection.GetGameType(GameTypeNames.SpeechEventArchive);
                if (archiveType == null)
                {
                    Log.Voice.Warn("SpeechEventArchive type not found - voice recording limit unchanged");
                    _applied = true;
                    return;
                }

                // Try to find and set the max recordings field via reflection
                var field = archiveType.GetField(GameFieldNames.SpeechEventArchive_MaxRecordings,
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);

                if (field == null)
                {
                    Log.Voice.Warn("Max recordings field not found on SpeechEventArchive - voice recording limit unchanged");
                    _applied = true;
                    return;
                }

                // Find active instances via UnityEngine
                var instances = UnityEngine.Object.FindObjectsByType(archiveType, UnityEngine.FindObjectsSortMode.None);
                if (instances == null || instances.Length == 0)
                {
                    // Not yet available, retry next frame
                    return;
                }

                foreach (var instance in instances)
                {
                    field.SetValue(instance, _config.MaxRecordings);
                }

                Log.Voice.Msg($"Set max voice recordings to {_config.MaxRecordings}");
                _applied = true;
            }
            catch (Exception ex)
            {
                Log.Voice.Warn($"Failed to apply voice recording limit: {ex.Message}");
                _applied = true;
            }
        }

        public void OnGUI() { }
    }
}

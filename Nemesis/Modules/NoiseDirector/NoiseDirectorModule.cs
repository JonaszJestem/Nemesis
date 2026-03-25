using System;
using System.Collections.Generic;
using Nemesis.Core;
using UnityEngine;

namespace Nemesis.Modules.NoiseDirector
{
    internal class NoiseDirectorModule : IModule
    {
        private sealed class SourceSnapshot
        {
            public AudioSource? Source { get; set; }
            public float BaseVolume { get; set; }
            public float LastAppliedVolume { get; set; }
        }

        private readonly NoiseDirectorConfig _config;
        private readonly Dictionary<int, SourceSnapshot> _sources = new Dictionary<int, SourceSnapshot>();
        private readonly List<int> _staleSourceIds = new List<int>();
        private float _timer;
        private bool _isActive;

        public string Name => "Noise Director";

        public NoiseDirectorModule(NoiseDirectorConfig config)
        {
            _config = config;
        }

        public void Initialize()
        {
            Log.Msg("NoiseDirector", "Initialized");
        }

        public void Shutdown()
        {
            RestoreAllSources();
        }

        public void OnUpdate()
        {
            if (!_config.Enabled)
            {
                if (_isActive)
                {
                    RestoreAllSources();
                }

                _timer = 0f;
                return;
            }

            _timer += Time.deltaTime;
            if (_timer < _config.UpdateIntervalSeconds)
            {
                return;
            }

            _timer -= _config.UpdateIntervalSeconds;

            try
            {
                var sources = UnityEngine.Object.FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
                if (sources == null || sources.Length == 0)
                {
                    PruneMissingSources();
                    _isActive = _sources.Count > 0;
                    return;
                }

                var seen = new HashSet<int>();
                foreach (var source in sources)
                {
                    if (source == null) continue;

                    int instanceId = source.GetInstanceID();
                    seen.Add(instanceId);

                    if (!_sources.TryGetValue(instanceId, out var snapshot))
                    {
                        snapshot = new SourceSnapshot
                        {
                            Source = source,
                            BaseVolume = source.volume,
                            LastAppliedVolume = source.volume
                        };
                        _sources[instanceId] = snapshot;
                    }
                    else
                    {
                        snapshot.Source = source;

                        // If another system touched the source, treat the current value as the new base.
                        if (!Mathf.Approximately(source.volume, snapshot.LastAppliedVolume))
                        {
                            snapshot.BaseVolume = source.volume;
                        }
                    }

                    float targetVolume = CalculateTargetVolume(source, snapshot.BaseVolume);
                    if (!Mathf.Approximately(source.volume, targetVolume))
                    {
                        source.volume = targetVolume;
                    }

                    snapshot.LastAppliedVolume = targetVolume;
                }

                PruneMissingSources(seen);
                _isActive = _sources.Count > 0;
            }
            catch (Exception ex)
            {
                Log.Warn("NoiseDirector", $"Update failed: {ex.Message}");
            }
        }

        public void OnGUI() { }

        private float CalculateTargetVolume(AudioSource source, float baseVolume)
        {
            string signature = BuildSignature(source);

            if (!_config.AffectMusic && ContainsAny(signature, "music", "theme", "song", "ost"))
            {
                return baseVolume;
            }

            if (!_config.AffectUiAudio && ContainsAny(signature, "ui", "menu", "hud", "button", "click", "interface"))
            {
                return baseVolume;
            }

            float categoryMultiplier = _config.EffectsVolumeMultiplier;
            if (ContainsAny(signature, "voice", "speech", "dialog", "radio", "talk", "npc"))
            {
                categoryMultiplier = _config.VoiceVolumeMultiplier;
            }
            else if (ContainsAny(signature, "ambient", "ambience", "background", "wind", "rain", "weather", "room", "cave", "hall", "machine", "drone"))
            {
                categoryMultiplier = _config.AmbientVolumeMultiplier;
            }

            float targetVolume = baseVolume * _config.GlobalVolumeMultiplier * categoryMultiplier;
            return Mathf.Clamp(targetVolume, 0f, 2f);
        }

        private static string BuildSignature(AudioSource source)
        {
            string clipName = source.clip != null ? source.clip.name : string.Empty;
            string groupName = source.outputAudioMixerGroup != null ? source.outputAudioMixerGroup.name : string.Empty;
            return $"{source.name} {source.gameObject.name} {clipName} {groupName}";
        }

        private static bool ContainsAny(string value, params string[] needles)
        {
            foreach (var needle in needles)
            {
                if (value.IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }
            }

            return false;
        }

        private void PruneMissingSources()
        {
            _staleSourceIds.Clear();
            foreach (var pair in _sources)
            {
                if (pair.Value.Source == null)
                {
                    _staleSourceIds.Add(pair.Key);
                }
            }

            foreach (int id in _staleSourceIds)
            {
                _sources.Remove(id);
            }
        }

        private void PruneMissingSources(HashSet<int> seen)
        {
            _staleSourceIds.Clear();
            foreach (var pair in _sources)
            {
                if (!seen.Contains(pair.Key) || pair.Value.Source == null)
                {
                    _staleSourceIds.Add(pair.Key);
                }
            }

            foreach (int id in _staleSourceIds)
            {
                _sources.Remove(id);
            }
        }

        private void RestoreAllSources()
        {
            try
            {
                foreach (var snapshot in _sources.Values)
                {
                    if (snapshot.Source == null) continue;
                    snapshot.Source.volume = snapshot.BaseVolume;
                }
            }
            catch (Exception ex)
            {
                Log.Warn("NoiseDirector", $"Restore failed: {ex.Message}");
            }
            finally
            {
                _sources.Clear();
                _staleSourceIds.Clear();
                _timer = 0f;
                _isActive = false;
            }
        }
    }
}

using Nemesis.Config;
using Nemesis.Core;
using Nemesis.Modules.NoiseDirector;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Nemesis.Tests
{
    [TestFixture]
    public class NoiseDirectorTests
    {
        [Test]
        public void NoiseDirectorConfig_RoundTripsThroughJson()
        {
            var source = new NoiseDirectorConfig
            {
                Enabled = true,
                GlobalVolumeMultiplier = 1.4f,
                AmbientVolumeMultiplier = 0.6f,
                EffectsVolumeMultiplier = 1.15f,
                VoiceVolumeMultiplier = 0.95f,
                UpdateIntervalSeconds = 1.5f,
                AffectUiAudio = true,
                AffectMusic = true
            };

            string json = JsonConvert.SerializeObject(source);
            var result = JsonConvert.DeserializeObject<NoiseDirectorConfig>(json);

            Assert.IsNotNull(result);
            Assert.AreEqual(source.Enabled, result!.Enabled);
            Assert.AreEqual(source.GlobalVolumeMultiplier, result.GlobalVolumeMultiplier);
            Assert.AreEqual(source.AmbientVolumeMultiplier, result.AmbientVolumeMultiplier);
            Assert.AreEqual(source.EffectsVolumeMultiplier, result.EffectsVolumeMultiplier);
            Assert.AreEqual(source.VoiceVolumeMultiplier, result.VoiceVolumeMultiplier);
            Assert.AreEqual(source.UpdateIntervalSeconds, result.UpdateIntervalSeconds);
            Assert.AreEqual(source.AffectUiAudio, result.AffectUiAudio);
            Assert.AreEqual(source.AffectMusic, result.AffectMusic);
        }

        [Test]
        public void HostConfig_RoundTrip_PreservesNoiseDirectorConfig()
        {
            var source = new HostConfig
            {
                NoiseDirector = new NoiseDirectorConfig
                {
                    Enabled = true,
                    GlobalVolumeMultiplier = 1.3f,
                    AmbientVolumeMultiplier = 0.8f,
                    EffectsVolumeMultiplier = 1.1f,
                    VoiceVolumeMultiplier = 1.2f,
                    UpdateIntervalSeconds = 0.75f,
                    AffectUiAudio = true,
                    AffectMusic = false
                }
            };

            var result = ConfigSyncLogic.RoundTrip(source);

            Assert.AreEqual(true, result.NoiseDirector.Enabled);
            Assert.AreEqual(1.3f, result.NoiseDirector.GlobalVolumeMultiplier);
            Assert.AreEqual(0.8f, result.NoiseDirector.AmbientVolumeMultiplier);
            Assert.AreEqual(1.1f, result.NoiseDirector.EffectsVolumeMultiplier);
            Assert.AreEqual(1.2f, result.NoiseDirector.VoiceVolumeMultiplier);
            Assert.AreEqual(0.75f, result.NoiseDirector.UpdateIntervalSeconds);
            Assert.AreEqual(true, result.NoiseDirector.AffectUiAudio);
            Assert.AreEqual(false, result.NoiseDirector.AffectMusic);
        }
    }
}

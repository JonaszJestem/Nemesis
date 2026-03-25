using NUnit.Framework;
using Nemesis.Modules.DifficultyDirector;

namespace Nemesis.Tests
{
    [TestFixture]
    public class DifficultyCalculatorTests
    {
        private static DifficultyConfig DefaultConfig() => new DifficultyConfig
        {
            PlayerCountWeight = 50f,
            GameDayWeight = 30f,
            SessionCycleWeight = 20f,
            PlayerCountMax = 10,
            GameDayMax = 30,
            SessionCycleMax = 10,
            MinMultiplier = 0.5f,
            MaxMultiplier = 3.0f
        };

        [Test]
        public void ZeroAllFactors_ReturnsMinMultiplier()
        {
            var cfg = DefaultConfig();
            float result = DifficultyCalculator.ComputeMultiplier(0, 0, 0, cfg);
            Assert.AreEqual(cfg.MinMultiplier, result, 0.01f);
        }

        [Test]
        public void MaxAllFactors_ReturnsMaxMultiplier()
        {
            var cfg = DefaultConfig();
            float result = DifficultyCalculator.ComputeMultiplier(
                cfg.PlayerCountMax, cfg.GameDayMax, cfg.SessionCycleMax, cfg);
            Assert.AreEqual(cfg.MaxMultiplier, result, 0.01f);
        }

        [Test]
        public void OverMaxFactors_ClampedToMaxMultiplier()
        {
            var cfg = DefaultConfig();
            float result = DifficultyCalculator.ComputeMultiplier(100, 100, 100, cfg);
            Assert.AreEqual(cfg.MaxMultiplier, result, 0.01f);
        }

        [Test]
        public void HalfFactors_ReturnsMidpoint()
        {
            var cfg = DefaultConfig();
            float result = DifficultyCalculator.ComputeMultiplier(5, 15, 5, cfg);
            float expected = cfg.MinMultiplier + 0.5f * (cfg.MaxMultiplier - cfg.MinMultiplier);
            Assert.AreEqual(expected, result, 0.01f);
        }

        [Test]
        public void SingleDominantFactor_PlayerCountOnly()
        {
            var cfg = DefaultConfig();
            cfg.PlayerCountWeight = 100f;
            cfg.GameDayWeight = 0f;
            cfg.SessionCycleWeight = 0f;

            float result = DifficultyCalculator.ComputeMultiplier(5, 0, 0, cfg);
            float expected = cfg.MinMultiplier + 0.5f * (cfg.MaxMultiplier - cfg.MinMultiplier);
            Assert.AreEqual(expected, result, 0.01f);
        }

        [Test]
        public void SingleDominantFactor_GameDayOnly()
        {
            var cfg = DefaultConfig();
            cfg.PlayerCountWeight = 0f;
            cfg.GameDayWeight = 100f;
            cfg.SessionCycleWeight = 0f;

            float result = DifficultyCalculator.ComputeMultiplier(10, 15, 0, cfg);
            float expected = cfg.MinMultiplier + 0.5f * (cfg.MaxMultiplier - cfg.MinMultiplier);
            Assert.AreEqual(expected, result, 0.01f);
        }

        [Test]
        public void AllZeroWeights_ReturnsMinMultiplier()
        {
            var cfg = DefaultConfig();
            cfg.PlayerCountWeight = 0f;
            cfg.GameDayWeight = 0f;
            cfg.SessionCycleWeight = 0f;

            float result = DifficultyCalculator.ComputeMultiplier(5, 15, 5, cfg);
            Assert.AreEqual(cfg.MinMultiplier, result, 0.01f);
        }

        [Test]
        public void EmptyFactorArray_ReturnsMin()
        {
            float result = DifficultyCalculator.ComputeMultiplier(
                new DifficultyCalculator.FactorInput[0], 0.5f, 3.0f);
            Assert.AreEqual(0.5f, result, 0.01f);
        }

        [Test]
        public void NullFactorArray_ReturnsMin()
        {
            float result = DifficultyCalculator.ComputeMultiplier(null!, 0.5f, 3.0f);
            Assert.AreEqual(0.5f, result, 0.01f);
        }

        [Test]
        public void MaxValueZero_TreatedAsZeroContribution()
        {
            var factors = new[]
            {
                new DifficultyCalculator.FactorInput { Value = 10, MaxValue = 0, Weight = 100f }
            };
            float result = DifficultyCalculator.ComputeMultiplier(factors, 0.5f, 3.0f);
            Assert.AreEqual(0.5f, result, 0.01f);
        }

        [Test]
        public void NegativeWeight_TreatedAsZero()
        {
            var factors = new[]
            {
                new DifficultyCalculator.FactorInput { Value = 5, MaxValue = 10, Weight = -50f }
            };
            float result = DifficultyCalculator.ComputeMultiplier(factors, 0.5f, 3.0f);
            Assert.AreEqual(0.5f, result, 0.01f);
        }

        [Test]
        public void EasyConfig_LowRange()
        {
            var cfg = DefaultConfig();
            cfg.MinMultiplier = 0.3f;
            cfg.MaxMultiplier = 0.8f;

            float result = DifficultyCalculator.ComputeMultiplier(
                cfg.PlayerCountMax, cfg.GameDayMax, cfg.SessionCycleMax, cfg);
            Assert.AreEqual(0.8f, result, 0.01f);
        }

        [Test]
        public void SuperHardConfig_HighRange()
        {
            var cfg = DefaultConfig();
            cfg.MinMultiplier = 2.0f;
            cfg.MaxMultiplier = 5.0f;

            float result = DifficultyCalculator.ComputeMultiplier(
                cfg.PlayerCountMax, cfg.GameDayMax, cfg.SessionCycleMax, cfg);
            Assert.AreEqual(5.0f, result, 0.01f);
        }

        [Test]
        public void ResultAlwaysBetweenMinAndMax()
        {
            var cfg = DefaultConfig();
            for (int p = 0; p <= 20; p += 2)
            for (int d = 0; d <= 40; d += 10)
            for (int s = 0; s <= 15; s += 3)
            {
                float result = DifficultyCalculator.ComputeMultiplier(p, d, s, cfg);
                Assert.GreaterOrEqual(result, cfg.MinMultiplier, $"p={p}, d={d}, s={s}");
                Assert.LessOrEqual(result, cfg.MaxMultiplier, $"p={p}, d={d}, s={s}");
            }
        }

        [Test]
        public void HigherPlayerCount_HigherMultiplier()
        {
            var cfg = DefaultConfig();
            float low = DifficultyCalculator.ComputeMultiplier(2, 5, 1, cfg);
            float high = DifficultyCalculator.ComputeMultiplier(8, 5, 1, cfg);
            Assert.Greater(high, low);
        }

        [Test]
        public void IncreasingWeight_IncreasesFactorInfluence()
        {
            var cfg = DefaultConfig();
            cfg.PlayerCountWeight = 10f;
            cfg.GameDayWeight = 10f;
            cfg.SessionCycleWeight = 10f;
            float low = DifficultyCalculator.ComputeMultiplier(10, 0, 0, cfg);

            cfg.PlayerCountWeight = 90f;
            float high = DifficultyCalculator.ComputeMultiplier(10, 0, 0, cfg);

            Assert.Greater(high, low);
        }

        [Test]
        public void SinglePlayer_MinDay_MinCycle_NearMinMultiplier()
        {
            var cfg = DefaultConfig();
            float result = DifficultyCalculator.ComputeMultiplier(1, 1, 1, cfg);
            Assert.Less(result, cfg.MinMultiplier + 0.5f);
        }

        [Test]
        public void EqualMinAndMax_AlwaysReturnsThatValue()
        {
            var cfg = DefaultConfig();
            cfg.MinMultiplier = 1.5f;
            cfg.MaxMultiplier = 1.5f;

            float result = DifficultyCalculator.ComputeMultiplier(5, 10, 3, cfg);
            Assert.AreEqual(1.5f, result, 0.01f);
        }
    }
}

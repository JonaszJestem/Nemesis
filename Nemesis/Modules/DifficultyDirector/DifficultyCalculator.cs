using System;

namespace Nemesis.Modules.DifficultyDirector
{
    internal static class DifficultyCalculator
    {
        internal struct FactorInput
        {
            public int Value;
            public int MaxValue;
            public float Weight;
        }

        public static float ComputeMultiplier(FactorInput[] factors, float minMultiplier, float maxMultiplier)
        {
            if (factors == null || factors.Length == 0)
                return minMultiplier;

            float totalWeight = 0f;
            float weightedSum = 0f;

            for (int i = 0; i < factors.Length; i++)
            {
                float weight = Math.Max(0f, factors[i].Weight);
                totalWeight += weight;

                float normalized = factors[i].MaxValue > 0
                    ? Math.Min(1f, (float)factors[i].Value / factors[i].MaxValue)
                    : 0f;

                weightedSum += normalized * weight;
            }

            if (totalWeight <= 0f)
                return minMultiplier;

            float score = weightedSum / totalWeight; // 0.0 to 1.0
            return minMultiplier + score * (maxMultiplier - minMultiplier);
        }

        public static float ComputeMultiplier(
            int playerCount, int gameDay, int sessionCycle,
            DifficultyConfig cfg)
        {
            var factors = new FactorInput[]
            {
                new FactorInput { Value = playerCount, MaxValue = cfg.PlayerCountMax, Weight = cfg.PlayerCountWeight },
                new FactorInput { Value = gameDay, MaxValue = cfg.GameDayMax, Weight = cfg.GameDayWeight },
                new FactorInput { Value = sessionCycle, MaxValue = cfg.SessionCycleMax, Weight = cfg.SessionCycleWeight }
            };

            return ComputeMultiplier(factors, cfg.MinMultiplier, cfg.MaxMultiplier);
        }
    }
}

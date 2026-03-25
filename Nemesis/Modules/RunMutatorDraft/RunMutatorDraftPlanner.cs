using System;
using System.Collections.Generic;
using System.Linq;

namespace Nemesis.Modules.RunMutatorDraft
{
    internal static class RunMutatorDraftPlanner
    {
        public static RunMutatorPlan BuildPlan(
            RunMutatorContext context,
            IReadOnlyList<RunMutatorDefinition> definitions,
            int draftChoiceCount,
            int activeMutatorCount)
        {
            if (definitions == null || definitions.Count == 0)
                return new RunMutatorPlan(Array.Empty<RunMutatorDefinition>(), Array.Empty<RunMutatorDefinition>());

            int draftCount = Math.Max(1, draftChoiceCount);
            int activeCount = Math.Max(0, activeMutatorCount);

            var ranked = definitions
                .Select(def => new RankedDefinition(def, Score(def, context)))
                .OrderByDescending(x => x.Score)
                .ThenBy(x => x.Definition.Name, StringComparer.OrdinalIgnoreCase)
                .ToList();

            var draftOptions = ranked
                .Take(Math.Min(draftCount, ranked.Count))
                .Select(x => x.Definition)
                .ToArray();

            var activeMutators = draftOptions
                .Select(def => new RankedDefinition(def, Score(def, context)))
                .OrderByDescending(x => x.Score)
                .ThenBy(x => x.Definition.Name, StringComparer.OrdinalIgnoreCase)
                .Take(Math.Min(activeCount, draftOptions.Length))
                .Select(x => x.Definition)
                .ToArray();

            return new RunMutatorPlan(draftOptions, activeMutators);
        }

        public static int Score(RunMutatorDefinition definition, RunMutatorContext context)
        {
            if (definition == null)
                return 0;

            int score = definition.BaseWeight;
            score += context.PlayerCount * 12;
            score += context.GameDay * 2;
            score += context.SessionCycle * 10;

            switch (definition.EffectKind)
            {
                case RunMutatorEffectKind.SpawnPressure:
                    score += context.PlayerCount * 20;
                    score += context.SessionCycle * 8;
                    break;
                case RunMutatorEffectKind.WeatherBias:
                    score += context.GameDay * 3;
                    score += context.CurrentWeatherId == 1 ? 18 : 6;
                    break;
                case RunMutatorEffectKind.NoiseLeak:
                    score += (int)Math.Round(context.CurrentNoisePressure * 4f);
                    score += context.PlayerCount * 10;
                    break;
                case RunMutatorEffectKind.Hybrid:
                    score += context.PlayerCount * 14;
                    score += context.GameDay * 2;
                    score += (int)Math.Round(context.CurrentNoisePressure * 2f);
                    break;
            }

            return score;
        }

        private sealed class RankedDefinition
        {
            public RunMutatorDefinition Definition { get; }
            public int Score { get; }

            public RankedDefinition(RunMutatorDefinition definition, int score)
            {
                Definition = definition;
                Score = score;
            }
        }
    }
}

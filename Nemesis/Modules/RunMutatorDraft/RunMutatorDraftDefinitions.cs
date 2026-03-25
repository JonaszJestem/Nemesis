using System.Collections.Generic;

namespace Nemesis.Modules.RunMutatorDraft
{
    internal enum RunMutatorEffectKind
    {
        SpawnPressure,
        WeatherBias,
        NoiseLeak,
        Hybrid
    }

    internal sealed class RunMutatorDefinition
    {
        public string Id { get; }
        public string Name { get; }
        public string Description { get; }
        public RunMutatorEffectKind EffectKind { get; }
        public int BaseWeight { get; }

        public RunMutatorDefinition(
            string id,
            string name,
            string description,
            RunMutatorEffectKind effectKind,
            int baseWeight)
        {
            Id = id;
            Name = name;
            Description = description;
            EffectKind = effectKind;
            BaseWeight = baseWeight;
        }
    }

    internal sealed class RunMutatorContext
    {
        public int PlayerCount { get; }
        public int GameDay { get; }
        public int SessionCycle { get; }
        public int CurrentWeatherId { get; }
        public float CurrentNoisePressure { get; }

        public RunMutatorContext(
            int playerCount,
            int gameDay,
            int sessionCycle,
            int currentWeatherId,
            float currentNoisePressure)
        {
            PlayerCount = playerCount;
            GameDay = gameDay;
            SessionCycle = sessionCycle;
            CurrentWeatherId = currentWeatherId;
            CurrentNoisePressure = currentNoisePressure;
        }
    }

    internal sealed class RunMutatorPlan
    {
        public IReadOnlyList<RunMutatorDefinition> DraftOptions { get; }
        public IReadOnlyList<RunMutatorDefinition> ActiveMutators { get; }

        public RunMutatorPlan(
            IReadOnlyList<RunMutatorDefinition> draftOptions,
            IReadOnlyList<RunMutatorDefinition> activeMutators)
        {
            DraftOptions = draftOptions;
            ActiveMutators = activeMutators;
        }
    }

    internal static class RunMutatorCatalog
    {
        private static readonly IReadOnlyList<RunMutatorDefinition> DefaultDefinitions = new[]
        {
            new RunMutatorDefinition(
                "static-leak",
                "Static Leak",
                "Noise lingers longer and leaks out of the team.",
                RunMutatorEffectKind.NoiseLeak,
                120),
            new RunMutatorDefinition(
                "crowded-hunt",
                "Crowded Hunt",
                "Monster pressure scales harder with player count.",
                RunMutatorEffectKind.SpawnPressure,
                115),
            new RunMutatorDefinition(
                "storm-front",
                "Storm Front",
                "Weather is biased toward harsher conditions.",
                RunMutatorEffectKind.WeatherBias,
                100),
            new RunMutatorDefinition(
                "false-silence",
                "False Silence",
                "Quiet stretches decay slower but spikes hit harder.",
                RunMutatorEffectKind.NoiseLeak,
                85),
            new RunMutatorDefinition(
                "predator-rhythm",
                "Predator Rhythm",
                "The run gets louder and the hunt gets tighter.",
                RunMutatorEffectKind.Hybrid,
                105),
            new RunMutatorDefinition(
                "blackout-drive",
                "Blackout Drive",
                "Bad weather and pressure build in tandem.",
                RunMutatorEffectKind.Hybrid,
                95)
        };

        public static IReadOnlyList<RunMutatorDefinition> GetDefaultDefinitions()
        {
            return DefaultDefinitions;
        }
    }
}

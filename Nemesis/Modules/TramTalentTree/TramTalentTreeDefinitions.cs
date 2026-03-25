using System;
using System.Collections.Generic;

namespace Nemesis.Modules.TramTalentTree
{
    internal enum TramTalentNodeId
    {
        TurboCoupler,
        SignalBoost,
        ReinforcedCarriage,
        FreightLedger
    }

    internal sealed class TramTalentNodeDefinition
    {
        public TramTalentNodeDefinition(TramTalentNodeId id, string title, string description)
        {
            Id = id;
            Title = title;
            Description = description;
        }

        public TramTalentNodeId Id { get; }
        public string Title { get; }
        public string Description { get; }
        public int MaxRank { get; set; }
    }

    internal sealed class TramTalentSnapshot
    {
        public TramTalentSnapshot(
            int unspentPoints,
            float speedMultiplier,
            float noiseMultiplier,
            float rewardMultiplier,
            int capacityBonus,
            float damageTakenMultiplier)
        {
            UnspentPoints = unspentPoints;
            SpeedMultiplier = speedMultiplier;
            NoiseMultiplier = noiseMultiplier;
            RewardMultiplier = rewardMultiplier;
            CapacityBonus = capacityBonus;
            DamageTakenMultiplier = damageTakenMultiplier;
        }

        public int UnspentPoints { get; }
        public float SpeedMultiplier { get; }
        public float NoiseMultiplier { get; }
        public float RewardMultiplier { get; }
        public int CapacityBonus { get; }
        public float DamageTakenMultiplier { get; }

        public static TramTalentSnapshot Empty { get; } =
            new TramTalentSnapshot(0, 1f, 1f, 1f, 0, 1f);
    }

    internal static class TramTalentCatalog
    {
        public static IReadOnlyList<TramTalentNodeDefinition> Build(TramTalentTreeConfig config)
        {
            int maxRank = config.MaxRankPerNode < 1 ? 1 : config.MaxRankPerNode;

            return new List<TramTalentNodeDefinition>
            {
                new TramTalentNodeDefinition(
                    TramTalentNodeId.TurboCoupler,
                    "Turbo Coupler",
                    "Improves tram speed and acceleration.")
                {
                    MaxRank = maxRank
                },
                new TramTalentNodeDefinition(
                    TramTalentNodeId.SignalBoost,
                    "Signal Boost",
                    "Reduces noise spill and improves contract rewards.")
                {
                    MaxRank = maxRank
                },
                new TramTalentNodeDefinition(
                    TramTalentNodeId.ReinforcedCarriage,
                    "Reinforced Carriage",
                    "Adds capacity and lowers incoming damage.")
                {
                    MaxRank = maxRank
                },
                new TramTalentNodeDefinition(
                    TramTalentNodeId.FreightLedger,
                    "Freight Ledger",
                    "Turns route efficiency into stronger contract rewards.")
                {
                    MaxRank = maxRank
                }
            };
        }

        public static TramTalentNodeDefinition? GetDefinition(
            TramTalentNodeId id,
            TramTalentTreeConfig config)
        {
            foreach (var definition in Build(config))
            {
                if (definition.Id == id)
                    return definition;
            }

            return null;
        }
    }
}

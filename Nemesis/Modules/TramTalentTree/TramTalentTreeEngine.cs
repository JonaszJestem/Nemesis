using System;
using System.Collections.Generic;

namespace Nemesis.Modules.TramTalentTree
{
    internal sealed class TramTalentTreeEngine
    {
        private readonly TramTalentTreeConfig _config;

        public TramTalentTreeEngine(TramTalentTreeConfig config)
        {
            _config = config;
        }

        public TramTalentTreeState CreateState()
        {
            var state = new TramTalentTreeState();
            state.Reset(_config.StartingPoints);
            return state;
        }

        public TramTalentSnapshot ComputeSnapshot(TramTalentTreeState state)
        {
            if (state == null)
                return TramTalentSnapshot.Empty;

            int turboRank = GetClampedRank(state, TramTalentNodeId.TurboCoupler);
            int signalRank = GetClampedRank(state, TramTalentNodeId.SignalBoost);
            int carriageRank = GetClampedRank(state, TramTalentNodeId.ReinforcedCarriage);
            int ledgerRank = GetClampedRank(state, TramTalentNodeId.FreightLedger);

            float speedMultiplier = 1f + (turboRank * _config.SpeedBonusPerRank);
            float noiseMultiplier = Clamp01(1f - (signalRank * _config.NoiseReductionPerRank));
            float rewardMultiplier = 1f + (ledgerRank * _config.RewardBonusPerRank);
            int capacityBonus = carriageRank * _config.CapacityBonusPerRank;
            float damageTakenMultiplier = Clamp01(1f - (carriageRank * _config.DamageReductionPerRank));

            return new TramTalentSnapshot(
                state.UnspentPoints,
                speedMultiplier,
                noiseMultiplier,
                rewardMultiplier,
                capacityBonus,
                damageTakenMultiplier);
        }

        public void PublishSnapshot(TramTalentTreeState state)
        {
            TramTalentTreeBridge.PublishSnapshot(ComputeSnapshot(state));
        }

        public int DrainQueuedContractPoints(TramTalentTreeState state)
        {
            int points = TramTalentTreeBridge.DrainContractPoints();
            if (points > 0)
                GrantPoints(state, points);

            return points;
        }

        public void GrantPoints(TramTalentTreeState state, int amount)
        {
            if (state == null || amount <= 0)
                return;

            state.AddPoints(amount);
        }

        public bool TrySpendPoint(TramTalentTreeState state, TramTalentNodeId nodeId)
        {
            if (state == null)
                return false;

            var definition = TramTalentCatalog.GetDefinition(nodeId, _config);
            if (definition == null)
                return false;

            if (!state.CanSpend(nodeId, definition.MaxRank))
                return false;

            state.SpendPoint(nodeId);
            return true;
        }

        public bool TryRefundPoint(TramTalentTreeState state, TramTalentNodeId nodeId)
        {
            if (state == null)
                return false;

            if (!state.CanRefund(nodeId))
                return false;

            state.RefundPoint(nodeId);
            return true;
        }

        public bool TrySetRank(TramTalentTreeState state, TramTalentNodeId nodeId, int rank)
        {
            if (state == null)
                return false;

            var definition = TramTalentCatalog.GetDefinition(nodeId, _config);
            if (definition == null)
                return false;

            if (rank < 0)
                rank = 0;

            if (rank > definition.MaxRank)
                rank = definition.MaxRank;

            int currentRank = state.GetRank(nodeId);
            int delta = rank - currentRank;
            if (delta == 0)
                return true;

            if (delta > 0)
            {
                if (delta > state.UnspentPoints)
                    return false;

                for (int i = 0; i < delta; i++)
                    state.SpendPoint(nodeId);
            }
            else
            {
                for (int i = 0; i < -delta; i++)
                    state.RefundPoint(nodeId);
            }

            return true;
        }

        public IReadOnlyList<TramTalentNodeDefinition> GetDefinitions()
        {
            return TramTalentCatalog.Build(_config);
        }

        public TramTalentNodeDefinition? GetDefinition(TramTalentNodeId nodeId)
        {
            return TramTalentCatalog.GetDefinition(nodeId, _config);
        }

        public int CalculateContractRewardPoints(int basePoints, TramTalentSnapshot? snapshot)
        {
            if (basePoints <= 0)
                return 0;

            float multiplier = snapshot?.RewardMultiplier ?? 1f;
            int reward = (int)Math.Round(basePoints * multiplier, MidpointRounding.AwayFromZero);
            return reward < 1 ? 1 : reward;
        }

        private int GetClampedRank(TramTalentTreeState state, TramTalentNodeId nodeId)
        {
            var definition = TramTalentCatalog.GetDefinition(nodeId, _config);
            if (definition == null)
                return 0;

            int rank = state.GetRank(nodeId);
            if (rank < 0)
                return 0;

            return rank > definition.MaxRank ? definition.MaxRank : rank;
        }

        private static float Clamp01(float value)
        {
            if (value < 0f)
                return 0f;
            if (value > 1f)
                return 1f;
            return value;
        }
    }
}

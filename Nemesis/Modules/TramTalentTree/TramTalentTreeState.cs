using System;
using System.Collections.Generic;

namespace Nemesis.Modules.TramTalentTree
{
    internal sealed class TramTalentTreeState
    {
        private readonly Dictionary<TramTalentNodeId, int> _ranks =
            new Dictionary<TramTalentNodeId, int>();

        public int UnspentPoints { get; private set; }
        public int TotalPointsEarned { get; private set; }
        public int TotalPointsSpent { get; private set; }

        public IReadOnlyDictionary<TramTalentNodeId, int> Ranks => _ranks;

        public void Reset(int startingPoints)
        {
            _ranks.Clear();
            UnspentPoints = startingPoints < 0 ? 0 : startingPoints;
            TotalPointsEarned = UnspentPoints;
            TotalPointsSpent = 0;
        }

        public int GetRank(TramTalentNodeId nodeId)
        {
            return _ranks.TryGetValue(nodeId, out int rank) ? rank : 0;
        }

        public bool CanSpend(TramTalentNodeId nodeId, int maxRank)
        {
            return UnspentPoints > 0 && GetRank(nodeId) < maxRank;
        }

        public bool CanRefund(TramTalentNodeId nodeId)
        {
            return GetRank(nodeId) > 0;
        }

        internal void AddPoints(int amount)
        {
            if (amount <= 0)
                return;

            checked
            {
                TotalPointsEarned += amount;
                UnspentPoints += amount;
            }
        }

        internal void SpendPoint(TramTalentNodeId nodeId)
        {
            if (UnspentPoints <= 0)
                return;

            UnspentPoints--;
            TotalPointsSpent++;
            _ranks[nodeId] = GetRank(nodeId) + 1;
        }

        internal void RefundPoint(TramTalentNodeId nodeId)
        {
            int current = GetRank(nodeId);
            if (current <= 0)
                return;

            _ranks[nodeId] = current - 1;
            UnspentPoints++;
            if (TotalPointsSpent > 0)
                TotalPointsSpent--;
        }

        internal void SetRank(TramTalentNodeId nodeId, int rank)
        {
            if (rank <= 0)
            {
                _ranks.Remove(nodeId);
                return;
            }

            _ranks[nodeId] = rank;
        }
    }
}

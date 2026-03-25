using System;

namespace Nemesis.Modules.TramTalentTree
{
    /// <summary>
    /// Tiny bridge for contract rewards and current talent snapshot.
    /// ContractBoard writes points here; the talent tree module drains them.
    /// </summary>
    internal static class TramTalentTreeBridge
    {
        private static readonly object Sync = new object();
        private static TramTalentSnapshot _currentSnapshot = TramTalentSnapshot.Empty;
        private static int _pendingContractPoints;

        public static TramTalentSnapshot GetCurrentSnapshot()
        {
            lock (Sync)
            {
                return _currentSnapshot;
            }
        }

        public static void PublishSnapshot(TramTalentSnapshot snapshot)
        {
            lock (Sync)
            {
                _currentSnapshot = snapshot ?? TramTalentSnapshot.Empty;
            }
        }

        public static void QueueContractPoints(int points)
        {
            if (points <= 0)
                return;

            lock (Sync)
            {
                checked
                {
                    _pendingContractPoints += points;
                }
            }
        }

        public static int DrainContractPoints()
        {
            lock (Sync)
            {
                int points = _pendingContractPoints;
                _pendingContractPoints = 0;
                return points;
            }
        }

        public static void Reset()
        {
            lock (Sync)
            {
                _currentSnapshot = TramTalentSnapshot.Empty;
                _pendingContractPoints = 0;
            }
        }
    }
}

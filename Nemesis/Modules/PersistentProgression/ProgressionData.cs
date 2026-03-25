using System.Collections.Generic;

namespace Nemesis.Modules.PersistentProgression
{
    internal class PlayerProgression
    {
        public string PlayerName { get; set; } = "";
        public int Level { get; set; } = 1;
        public long XP { get; set; } = 0;
        public int TotalKills { get; set; } = 0;
        public int TotalLootCollected { get; set; } = 0;
        public int TotalRoomsCleared { get; set; } = 0;
        public int TotalSessionsSurvived { get; set; } = 0;
    }

    internal class ProgressionData
    {
        public Dictionary<string, PlayerProgression> Players { get; set; } = new Dictionary<string, PlayerProgression>();

        public PlayerProgression GetOrCreate(string key)
        {
            if (!Players.TryGetValue(key, out var prog))
            {
                prog = new PlayerProgression();
                Players[key] = prog;
            }
            return prog;
        }
    }
}

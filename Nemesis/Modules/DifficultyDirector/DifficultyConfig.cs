namespace Nemesis.Modules.DifficultyDirector
{
    internal class DifficultyConfig
    {
        public bool Enabled { get; set; } = true;
        public bool ShowHudLabel { get; set; } = true;
        public float UpdateIntervalSeconds { get; set; } = 10f;

        // Factor weights (0-100)
        public float PlayerCountWeight { get; set; } = 50f;
        public float GameDayWeight { get; set; } = 30f;
        public float SessionCycleWeight { get; set; } = 20f;

        // Factor scaling: how many units = max difficulty contribution
        public int PlayerCountMax { get; set; } = 10;
        public int GameDayMax { get; set; } = 30;
        public int SessionCycleMax { get; set; } = 10;

        // Output multiplier range
        public float MinMultiplier { get; set; } = 0.5f;
        public float MaxMultiplier { get; set; } = 3.0f;

        // Weather escalation
        public bool WeatherEscalation { get; set; } = true;
        public float WeatherThreshold { get; set; } = 2.0f;
        public int StormWeatherId { get; set; } = 3;

        // Monster stat scaling
        public bool ScaleMonsterHp { get; set; } = true;
        public bool ScaleMonsterAtk { get; set; } = true;
    }
}

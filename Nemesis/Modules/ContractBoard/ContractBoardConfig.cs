namespace Nemesis.Modules.ContractBoard
{
    internal sealed class ContractBoardConfig
    {
        public bool Enabled { get; set; } = false;
        public bool ShowHud { get; set; } = true;
        public bool ResetOnSessionStart { get; set; } = true;

        public int StartingActiveContracts { get; set; } = 3;
        public int MaxContractsPerRun { get; set; } = 6;
        public bool ReplaceCompletedContracts { get; set; } = true;
        public bool AllowDuplicateObjectiveKinds { get; set; } = false;

        public float RefreshIntervalSeconds { get; set; } = 0.5f;
        public float TargetVariancePercent { get; set; } = 0.20f;
        public float RewardVariancePercent { get; set; } = 0.15f;
        public float TargetRampPerIssuedContract { get; set; } = 0.08f;
        public float RewardMultiplier { get; set; } = 1f;
        public int RewardBonusPoints { get; set; } = 0;
    }
}

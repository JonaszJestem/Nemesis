using System;
using System.Collections.Generic;
using System.Linq;

namespace Nemesis.Modules.ContractBoard
{
    internal enum ContractBoardObjectiveKind
    {
        MonsterKills = 0,
        LootCollected = 1,
        ItemsSold = 2,
        RoomsCleared = 3,
        MonsterLootDrops = 4
    }

    internal sealed class ContractBoardObjectiveTemplate
    {
        public ContractBoardObjectiveTemplate(
            ContractBoardObjectiveKind kind,
            string title,
            string description,
            string progressUnit,
            int baseTarget,
            int baseRewardPoints)
        {
            Kind = kind;
            Title = title;
            Description = description;
            ProgressUnit = progressUnit;
            BaseTarget = baseTarget;
            BaseRewardPoints = baseRewardPoints;
        }

        public ContractBoardObjectiveKind Kind { get; }
        public string Title { get; }
        public string Description { get; }
        public string ProgressUnit { get; }
        public int BaseTarget { get; }
        public int BaseRewardPoints { get; }
    }

    internal sealed class ContractBoardContractSnapshot
    {
        public string ContractId { get; set; } = "";
        public ContractBoardObjectiveKind Kind { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string ProgressUnit { get; set; } = "";
        public int Target { get; set; }
        public int Progress { get; set; }
        public int RewardPoints { get; set; }
        public bool IsCompleted { get; set; }
        public long CompletionSequence { get; set; }
        public long CompletedUtcSeconds { get; set; }

        public ContractBoardContractSnapshot Clone()
        {
            return new ContractBoardContractSnapshot
            {
                ContractId = ContractId,
                Kind = Kind,
                Title = Title,
                Description = Description,
                ProgressUnit = ProgressUnit,
                Target = Target,
                Progress = Progress,
                RewardPoints = RewardPoints,
                IsCompleted = IsCompleted,
                CompletionSequence = CompletionSequence,
                CompletedUtcSeconds = CompletedUtcSeconds
            };
        }
    }

    internal sealed class ContractBoardCompletionSnapshot
    {
        public string ContractId { get; set; } = "";
        public ContractBoardObjectiveKind Kind { get; set; }
        public string Title { get; set; } = "";
        public string ProgressUnit { get; set; } = "";
        public int Target { get; set; }
        public int Progress { get; set; }
        public int RewardPoints { get; set; }
        public long Sequence { get; set; }
        public long CompletedUtcSeconds { get; set; }

        public ContractBoardCompletionSnapshot Clone()
        {
            return new ContractBoardCompletionSnapshot
            {
                ContractId = ContractId,
                Kind = Kind,
                Title = Title,
                ProgressUnit = ProgressUnit,
                Target = Target,
                Progress = Progress,
                RewardPoints = RewardPoints,
                Sequence = Sequence,
                CompletedUtcSeconds = CompletedUtcSeconds
            };
        }
    }

    internal sealed class ContractBoardSnapshot
    {
        public bool Enabled { get; set; }
        public bool IsHost { get; set; }
        public string SessionKey { get; set; } = "";
        public int IssuedContracts { get; set; }
        public int CompletedContracts { get; set; }
        public int MaxContractsPerRun { get; set; }
        public int TotalQueuedTalentPoints { get; set; }
        public long LastCompletionSequence { get; set; }
        public string StatusLine { get; set; } = "Idle";
        public List<ContractBoardContractSnapshot> ActiveContracts { get; set; } = new List<ContractBoardContractSnapshot>();
        public List<ContractBoardCompletionSnapshot> CompletionHistory { get; set; } = new List<ContractBoardCompletionSnapshot>();

        public static ContractBoardSnapshot Empty
        {
            get
            {
                return new ContractBoardSnapshot
                {
                    Enabled = false,
                    IsHost = false,
                    StatusLine = "Idle",
                    ActiveContracts = new List<ContractBoardContractSnapshot>(),
                    CompletionHistory = new List<ContractBoardCompletionSnapshot>()
                };
            }
        }

        public ContractBoardSnapshot Clone()
        {
            var activeContracts = ActiveContracts ?? new List<ContractBoardContractSnapshot>();
            var completionHistory = CompletionHistory ?? new List<ContractBoardCompletionSnapshot>();

            return new ContractBoardSnapshot
            {
                Enabled = Enabled,
                IsHost = IsHost,
                SessionKey = SessionKey,
                IssuedContracts = IssuedContracts,
                CompletedContracts = CompletedContracts,
                MaxContractsPerRun = MaxContractsPerRun,
                TotalQueuedTalentPoints = TotalQueuedTalentPoints,
                LastCompletionSequence = LastCompletionSequence,
                StatusLine = StatusLine,
                ActiveContracts = activeContracts.Select(x => x.Clone()).ToList(),
                CompletionHistory = completionHistory.Select(x => x.Clone()).ToList()
            };
        }
    }

    internal sealed class ContractBoardContractState
    {
        public string ContractId { get; set; } = "";
        public ContractBoardObjectiveKind Kind { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string ProgressUnit { get; set; } = "";
        public int Target { get; set; }
        public int Progress { get; set; }
        public int RewardPoints { get; set; }
        public int IssuedIndex { get; set; }
        public bool IsCompleted { get; set; }
        public long CompletionSequence { get; set; }
        public long CompletedUtcSeconds { get; set; }

        public ContractBoardContractSnapshot ToSnapshot()
        {
            return new ContractBoardContractSnapshot
            {
                ContractId = ContractId,
                Kind = Kind,
                Title = Title,
                Description = Description,
                ProgressUnit = ProgressUnit,
                Target = Target,
                Progress = Progress,
                RewardPoints = RewardPoints,
                IsCompleted = IsCompleted,
                CompletionSequence = CompletionSequence,
                CompletedUtcSeconds = CompletedUtcSeconds
            };
        }

        public ContractBoardCompletionSnapshot ToCompletionSnapshot()
        {
            return new ContractBoardCompletionSnapshot
            {
                ContractId = ContractId,
                Kind = Kind,
                Title = Title,
                ProgressUnit = ProgressUnit,
                Target = Target,
                Progress = Progress,
                RewardPoints = RewardPoints,
                Sequence = CompletionSequence,
                CompletedUtcSeconds = CompletedUtcSeconds
            };
        }
    }

    internal sealed class ContractBoardState
    {
        public string SessionKey { get; set; } = "";
        public ulong SessionSeed { get; set; }
        public int IssuedContracts { get; set; }
        public int TotalQueuedTalentPoints { get; set; }
        public long CompletionSequence { get; set; }
        public string StatusLine { get; set; } = "Idle";
        public List<ContractBoardContractState> ActiveContracts { get; } = new List<ContractBoardContractState>();
        public List<ContractBoardCompletionSnapshot> CompletionHistory { get; } = new List<ContractBoardCompletionSnapshot>();
    }

    internal sealed class ContractBoardEngine
    {
        private static readonly IReadOnlyList<ContractBoardObjectiveTemplate> Catalog = new List<ContractBoardObjectiveTemplate>
        {
            new ContractBoardObjectiveTemplate(
                ContractBoardObjectiveKind.MonsterKills,
                "Hunter's Ledger",
                "Eliminate hostiles and keep the threat count down.",
                "kills",
                5,
                4),
            new ContractBoardObjectiveTemplate(
                ContractBoardObjectiveKind.LootCollected,
                "Recovery Run",
                "Recover nearby loot and bring the haul home.",
                "loot",
                4,
                3),
            new ContractBoardObjectiveTemplate(
                ContractBoardObjectiveKind.ItemsSold,
                "Merchant's Cut",
                "Sell surplus gear to keep the run profitable.",
                "sales",
                3,
                4),
            new ContractBoardObjectiveTemplate(
                ContractBoardObjectiveKind.RoomsCleared,
                "Sweep Order",
                "Clear secured spaces and push the line forward.",
                "rooms",
                2,
                5),
            new ContractBoardObjectiveTemplate(
                ContractBoardObjectiveKind.MonsterLootDrops,
                "Spoils Audit",
                "Collect rare drops before they disappear.",
                "drops",
                2,
                4)
        };

        public IReadOnlyList<ContractBoardObjectiveTemplate> GetCatalog()
        {
            return Catalog;
        }

        public ContractBoardState CreateState(ContractBoardConfig config, string sessionKey)
        {
            var state = new ContractBoardState
            {
                SessionKey = sessionKey ?? "",
                SessionSeed = ContractBoardCatalog.BuildSeed(sessionKey ?? "")
            };

            IssueStartingContracts(state, config);
            state.StatusLine = state.ActiveContracts.Count > 0
                ? "Board ready"
                : "No contracts available";
            return state;
        }

        public List<ContractBoardCompletionSnapshot> ApplyProgress(
            ContractBoardState state,
            ContractBoardConfig config,
            ContractBoardObjectiveKind kind,
            int amount)
        {
            var completions = new List<ContractBoardCompletionSnapshot>();
            if (state == null || config == null || amount <= 0)
                return completions;

            for (int i = 0; i < amount; i++)
            {
                var contract = FindNextMatchingContract(state, kind);
                if (contract == null)
                    break;

                contract.Progress++;
                state.StatusLine = $"{contract.Title}: {contract.Progress}/{contract.Target} {contract.ProgressUnit}";

                if (contract.Progress < contract.Target)
                    continue;

                completions.Add(CompleteContract(state, contract));
            }

            if (completions.Count > 0 && config.ReplaceCompletedContracts)
                MaintainActiveContracts(state, config);

            if (state.ActiveContracts.Count == 0 && completions.Count == 0)
                state.StatusLine = "No active contracts";

            return completions;
        }

        public ContractBoardSnapshot BuildSnapshot(ContractBoardState state, ContractBoardConfig config, bool isHost)
        {
            if (state == null || config == null)
                return ContractBoardSnapshot.Empty;

            return new ContractBoardSnapshot
            {
                Enabled = config.Enabled,
                IsHost = isHost,
                SessionKey = state.SessionKey,
                IssuedContracts = state.IssuedContracts,
                CompletedContracts = state.CompletionHistory.Count,
                MaxContractsPerRun = Math.Max(0, config.MaxContractsPerRun),
                TotalQueuedTalentPoints = state.TotalQueuedTalentPoints,
                LastCompletionSequence = state.CompletionSequence,
                StatusLine = string.IsNullOrWhiteSpace(state.StatusLine) ? "Idle" : state.StatusLine,
                ActiveContracts = state.ActiveContracts.Select(x => x.ToSnapshot()).ToList(),
                CompletionHistory = state.CompletionHistory.Select(x => x.Clone()).ToList()
            };
        }

        private void IssueStartingContracts(ContractBoardState state, ContractBoardConfig config)
        {
            if (state == null || config == null)
                return;

            int desired = ClampNonNegative(config.StartingActiveContracts);
            int maxIssued = Math.Max(0, config.MaxContractsPerRun);
            desired = Math.Min(desired, maxIssued);

            while (state.ActiveContracts.Count < desired && state.IssuedContracts < maxIssued)
            {
                if (!TryIssueContract(state, config))
                    break;
            }
        }

        private void MaintainActiveContracts(ContractBoardState state, ContractBoardConfig config)
        {
            if (state == null || config == null || !config.ReplaceCompletedContracts)
                return;

            int desired = ClampNonNegative(config.StartingActiveContracts);
            int maxIssued = Math.Max(0, config.MaxContractsPerRun);
            desired = Math.Min(desired, maxIssued);

            while (state.ActiveContracts.Count < desired && state.IssuedContracts < maxIssued)
            {
                if (!TryIssueContract(state, config))
                    break;
            }
        }

        private ContractBoardCompletionSnapshot CompleteContract(ContractBoardState state, ContractBoardContractState contract)
        {
            contract.IsCompleted = true;
            contract.CompletedUtcSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            state.ActiveContracts.Remove(contract);

            state.CompletionSequence++;
            contract.CompletionSequence = state.CompletionSequence;

            var completion = contract.ToCompletionSnapshot();
            state.CompletionHistory.Add(completion);
            state.TotalQueuedTalentPoints += contract.RewardPoints;
            state.StatusLine = $"{contract.Title} completed (+{contract.RewardPoints} pts)";

            return completion;
        }

        private bool TryIssueContract(ContractBoardState state, ContractBoardConfig config)
        {
            var templates = BuildEligibleTemplates(state, config);
            if (templates.Count == 0)
                return false;

            int issuedIndex = state.IssuedContracts;
            int templateIndex = (int)(new DeterministicRng(MixSeed(state.SessionSeed, (ulong)issuedIndex, 0xC0FFEEUL)).NextInt((uint)templates.Count));
            var template = templates[templateIndex];

            var contract = BuildContract(state, config, template, issuedIndex);
            state.ActiveContracts.Add(contract);
            state.IssuedContracts++;
            return true;
        }

        private List<ContractBoardObjectiveTemplate> BuildEligibleTemplates(ContractBoardState state, ContractBoardConfig config)
        {
            var activeKinds = new HashSet<ContractBoardObjectiveKind>(state.ActiveContracts.Select(x => x.Kind));
            var available = new List<ContractBoardObjectiveTemplate>();

            foreach (var template in Catalog)
            {
                if (!config.AllowDuplicateObjectiveKinds && activeKinds.Contains(template.Kind))
                    continue;

                available.Add(template);
            }

            return available;
        }

        private static ContractBoardContractState BuildContract(
            ContractBoardState state,
            ContractBoardConfig config,
            ContractBoardObjectiveTemplate template,
            int issuedIndex)
        {
            var rng = new DeterministicRng(MixSeed(state.SessionSeed, (ulong)issuedIndex, (ulong)template.Kind));

            float targetMultiplier = 1f + (Math.Max(0f, config.TargetRampPerIssuedContract) * issuedIndex);
            targetMultiplier *= rng.NextVariance(config.TargetVariancePercent);

            float rewardMultiplier = Math.Max(0f, config.RewardMultiplier);
            rewardMultiplier *= rng.NextVariance(config.RewardVariancePercent);

            int target = Math.Max(1, RoundToInt(template.BaseTarget * targetMultiplier));
            int reward = Math.Max(1, RoundToInt(template.BaseRewardPoints * rewardMultiplier) + config.RewardBonusPoints);

            return new ContractBoardContractState
            {
                ContractId = BuildContractId(state.SessionSeed, issuedIndex, template.Kind),
                Kind = template.Kind,
                Title = template.Title,
                Description = template.Description,
                ProgressUnit = template.ProgressUnit,
                Target = target,
                Progress = 0,
                RewardPoints = reward,
                IssuedIndex = issuedIndex
            };
        }

        private static ContractBoardContractState? FindNextMatchingContract(ContractBoardState state, ContractBoardObjectiveKind kind)
        {
            return state.ActiveContracts
                .Where(x => x != null && !x.IsCompleted && x.Kind == kind)
                .OrderBy(x => x.IssuedIndex)
                .FirstOrDefault();
        }

        private static int ClampNonNegative(int value)
        {
            return value < 0 ? 0 : value;
        }

        private static int RoundToInt(float value)
        {
            return (int)Math.Round(value, MidpointRounding.AwayFromZero);
        }

        private static string BuildContractId(ulong sessionSeed, int issuedIndex, ContractBoardObjectiveKind kind)
        {
            return $"{sessionSeed:x16}:{issuedIndex}:{(int)kind}";
        }

        private static ulong MixSeed(ulong seed, ulong issueIndex, ulong salt)
        {
            unchecked
            {
                ulong mixed = seed ^ (issueIndex + 0x9E3779B97F4A7C15UL + (seed << 6) + (seed >> 2));
                mixed ^= salt + 0x94D049BB133111EBUL + (mixed << 7) + (mixed >> 3);
                return mixed == 0 ? 0xA511E9B3C6B8D1B1UL : mixed;
            }
        }
    }

    internal static class ContractBoardCatalog
    {
        public static IReadOnlyList<ContractBoardObjectiveTemplate> Build()
        {
            return new ContractBoardEngine().GetCatalog();
        }

        public static ulong BuildSeed(string sessionKey)
        {
            unchecked
            {
                const ulong offset = 14695981039346656037UL;
                const ulong prime = 1099511628211UL;
                ulong hash = offset;
                string key = string.IsNullOrWhiteSpace(sessionKey) ? "contract-board" : sessionKey;

                for (int i = 0; i < key.Length; i++)
                {
                    hash ^= key[i];
                    hash *= prime;
                }

                return hash == 0 ? 0x9E3779B97F4A7C15UL : hash;
            }
        }
    }

    internal struct DeterministicRng
    {
        private ulong _state;

        public DeterministicRng(ulong seed)
        {
            _state = seed == 0 ? 0x9E3779B97F4A7C15UL : seed;
        }

        private ulong NextRaw()
        {
            unchecked
            {
                _state += 0x9E3779B97F4A7C15UL;
                ulong z = _state;
                z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9UL;
                z = (z ^ (z >> 27)) * 0x94D049BB133111EBUL;
                return z ^ (z >> 31);
            }
        }

        public uint NextUInt()
        {
            return (uint)(NextRaw() >> 32);
        }

        public int NextInt(uint maxExclusive)
        {
            if (maxExclusive == 0)
                return 0;

            return (int)(NextRaw() % maxExclusive);
        }

        public float NextFloat()
        {
            return (NextRaw() >> 40) * (1f / 16777216f);
        }

        public float NextVariance(float variancePercent)
        {
            if (variancePercent <= 0f)
                return 1f;

            float offset = (NextFloat() * 2f) - 1f;
            return 1f + (offset * variancePercent);
        }
    }
}

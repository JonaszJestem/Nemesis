using System;
using System.Collections.Generic;
using System.Linq;

namespace Nemesis.Modules.RivalGhosts
{
    internal sealed class RivalRunMetrics
    {
        public int Kills { get; set; }
        public int LootCollected { get; set; }
        public int RoomsCleared { get; set; }
        public int SessionsSurvived { get; set; }
        public int MonsterLootDrops { get; set; }
        public int ItemsSold { get; set; }
        public int Deaths { get; set; }
        public float ActiveSeconds { get; set; }
    }

    internal sealed class RivalGhostRecord
    {
        public string RunId { get; set; } = "";
        public string PlayerName { get; set; } = "";
        public long StartedUtcSeconds { get; set; }
        public long FinishedUtcSeconds { get; set; }
        public RivalRunMetrics Metrics { get; set; } = new RivalRunMetrics();
        public long CompositeScore { get; set; }

        public string Summary => $"{PlayerName} - {CompositeScore} pts";
    }

    internal sealed class RivalGhostScoreProfile
    {
        public long KillWeight { get; set; } = 100;
        public long LootWeight { get; set; } = 30;
        public long RoomClearWeight { get; set; } = 75;
        public long SurvivalWeight { get; set; } = 120;
        public long MonsterDropWeight { get; set; } = 20;
        public long SellWeight { get; set; } = 15;
        public long DeathPenalty { get; set; } = 25;
    }

    internal sealed class RivalGhostChallenge
    {
        public string RivalRunId { get; set; } = "";
        public string RivalName { get; set; } = "";
        public long RivalScore { get; set; }
        public long CurrentScore { get; set; }
        public float Progress { get; set; }
        public string StatusLine { get; set; } = "";
    }

    internal static class RivalGhostScoring
    {
        public static long ComputeScore(RivalRunMetrics metrics, RivalGhostScoreProfile profile)
        {
            if (metrics == null || profile == null)
                return 0;

            long score = 0;
            score += (long)metrics.Kills * profile.KillWeight;
            score += (long)metrics.LootCollected * profile.LootWeight;
            score += (long)metrics.RoomsCleared * profile.RoomClearWeight;
            score += (long)metrics.SessionsSurvived * profile.SurvivalWeight;
            score += (long)metrics.MonsterLootDrops * profile.MonsterDropWeight;
            score += (long)metrics.ItemsSold * profile.SellWeight;
            score -= (long)metrics.Deaths * profile.DeathPenalty;
            return Math.Max(0, score);
        }

        public static RivalGhostScoreProfile FromConfig(RivalGhostsConfig config)
        {
            return new RivalGhostScoreProfile
            {
                KillWeight = config.KillWeight,
                LootWeight = config.LootWeight,
                RoomClearWeight = config.RoomClearWeight,
                SurvivalWeight = config.SurvivalWeight,
                MonsterDropWeight = config.MonsterDropWeight,
                SellWeight = config.SellWeight,
                DeathPenalty = config.DeathPenalty
            };
        }
    }

    internal static class RivalGhostSelector
    {
        public static RivalGhostRecord? SelectTarget(
            IEnumerable<RivalGhostRecord> records,
            long currentScore,
            float leadThresholdRatio = 0f)
        {
            if (records == null)
                return null;

            var ordered = records
                .Where(r => r != null)
                .OrderBy(r => r.CompositeScore)
                .ThenByDescending(r => r.FinishedUtcSeconds)
                .ToList();

            if (ordered.Count == 0)
                return null;

            long preferredTarget = currentScore;
            if (leadThresholdRatio > 0f)
            {
                preferredTarget = (long)Math.Ceiling(currentScore * (1f + leadThresholdRatio));
            }

            var target = ordered
                .Where(r => r.CompositeScore >= preferredTarget)
                .OrderBy(r => r.CompositeScore - preferredTarget)
                .ThenBy(r => r.FinishedUtcSeconds)
                .FirstOrDefault();

            if (target != null)
                return target;

            var aboveCurrent = ordered
                .Where(r => r.CompositeScore > currentScore)
                .OrderBy(r => r.CompositeScore - currentScore)
                .ThenBy(r => r.FinishedUtcSeconds)
                .FirstOrDefault();

            return aboveCurrent ?? ordered.Last();
        }

        public static RivalGhostChallenge BuildChallenge(RivalGhostRecord? rival, long currentScore)
        {
            if (rival == null)
            {
                return new RivalGhostChallenge
                {
                    RivalName = "No rival yet",
                    RivalScore = 0,
                    CurrentScore = currentScore,
                    Progress = 1f,
                    StatusLine = "Collect a few runs to build a rival."
                };
            }

            float progress = rival.CompositeScore <= 0
                ? 1f
                : Math.Max(0f, Math.Min(1f, (float)currentScore / rival.CompositeScore));

            string statusLine = currentScore >= rival.CompositeScore
                ? "You have beaten this ghost."
                : $"{rival.CompositeScore - currentScore} points to pass.";

            return new RivalGhostChallenge
            {
                RivalRunId = rival.RunId,
                RivalName = rival.PlayerName,
                RivalScore = rival.CompositeScore,
                CurrentScore = currentScore,
                Progress = progress,
                StatusLine = statusLine
            };
        }
    }
}

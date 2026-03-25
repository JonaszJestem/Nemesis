using System;
using System.Collections.Generic;
using System.Linq;

namespace Nemesis.Modules.PossessionPlus
{
    internal readonly struct WorldPoint
    {
        public readonly float X;
        public readonly float Y;
        public readonly float Z;

        public WorldPoint(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public float DistanceTo(WorldPoint other)
        {
            float dx = X - other.X;
            float dy = Y - other.Y;
            float dz = Z - other.Z;
            return (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }
    }

    internal enum GhostEntityKind
    {
        Monster,
        Loot,
        Ally,
        Objective,
        RallyPoint
    }

    internal sealed class GhostEntitySnapshot
    {
        public string Label { get; set; } = "";
        public GhostEntityKind Kind { get; set; }
        public WorldPoint Position { get; set; }
        public float Priority { get; set; }
        public float Distance { get; set; }
    }

    internal sealed class GhostPulseMark
    {
        public string Label { get; set; } = "";
        public GhostEntityKind Kind { get; set; }
        public WorldPoint Position { get; set; }
        public float Distance { get; set; }
        public long ExpiresUtcSeconds { get; set; }
    }

    internal sealed class GhostPresenceSnapshot
    {
        public string PlayerKey { get; set; } = "";
        public string PlayerName { get; set; } = "";
        public bool IsDead { get; set; }
        public int Charges { get; set; }
        public long GhostExpiresUtcSeconds { get; set; }
        public long CooldownEndsUtcSeconds { get; set; }
        public long LastUpdatedUtcSeconds { get; set; }
        public long LastPulseUtcSeconds { get; set; }
        public List<GhostPulseMark> LatestMarks { get; set; } = new List<GhostPulseMark>();

        public bool IsExpired(long nowUtcSeconds)
        {
            return GhostExpiresUtcSeconds > 0 && nowUtcSeconds >= GhostExpiresUtcSeconds;
        }
    }

    internal sealed class PossessionPlusLobbyState
    {
        public long UpdatedUtcSeconds { get; set; }
        public Dictionary<string, GhostPresenceSnapshot> Ghosts { get; set; } = new Dictionary<string, GhostPresenceSnapshot>();

        public void PruneExpired(long nowUtcSeconds)
        {
            var expiredKeys = Ghosts
                .Where(kvp => kvp.Value == null || kvp.Value.IsExpired(nowUtcSeconds))
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var key in expiredKeys)
                Ghosts.Remove(key);
        }
    }

    internal sealed class GhostRechargeProfile
    {
        public float BaseRechargeSeconds { get; set; } = 20f;
        public float AllyRechargeBonusSeconds { get; set; } = 4f;
        public float MinimumRechargeSeconds { get; set; } = 6f;

        public float GetEffectiveRechargeSeconds(int nearbyAllies)
        {
            float seconds = BaseRechargeSeconds - Math.Max(0, nearbyAllies) * AllyRechargeBonusSeconds;
            return Math.Max(MinimumRechargeSeconds, seconds);
        }
    }

    internal static class GhostPulsePlanner
    {
        public static List<GhostPulseMark> BuildMarks(
            IEnumerable<GhostEntitySnapshot> candidates,
            int maxMarks,
            long nowUtcSeconds,
            float markLifetimeSeconds)
        {
            if (maxMarks <= 0)
                return new List<GhostPulseMark>();

            var selected = candidates
                .Where(c => c != null && !string.IsNullOrWhiteSpace(c.Label))
                .OrderByDescending(c => c.Priority)
                .ThenBy(c => c.Distance)
                .ThenBy(c => c.Label, StringComparer.OrdinalIgnoreCase)
                .GroupBy(c => c.Label, StringComparer.OrdinalIgnoreCase)
                .Select(g => g.First())
                .Take(maxMarks)
                .ToList();

            if (selected.Count == 0)
            {
                return new List<GhostPulseMark>
                {
                    new GhostPulseMark
                    {
                        Label = "Rally Point",
                        Kind = GhostEntityKind.RallyPoint,
                        Position = new WorldPoint(0f, 0f, 0f),
                        Distance = 0f,
                        ExpiresUtcSeconds = nowUtcSeconds + Math.Max(1, (int)markLifetimeSeconds)
                    }
                };
            }

            var marks = new List<GhostPulseMark>(selected.Count);
            foreach (var entity in selected)
            {
                marks.Add(new GhostPulseMark
                {
                    Label = entity.Label,
                    Kind = entity.Kind,
                    Position = entity.Position,
                    Distance = entity.Distance,
                    ExpiresUtcSeconds = nowUtcSeconds + Math.Max(1, (int)markLifetimeSeconds)
                });
            }

            return marks;
        }
    }
}

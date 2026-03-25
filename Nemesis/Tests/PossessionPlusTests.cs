using System.Collections.Generic;
using NUnit.Framework;
using Nemesis.Modules.PossessionPlus;

namespace Nemesis.Tests
{
    [TestFixture]
    public class PossessionPlusTests
    {
        [Test]
        public void RechargeProfile_RespectsMinimumRecharge()
        {
            var profile = new GhostRechargeProfile
            {
                BaseRechargeSeconds = 20f,
                AllyRechargeBonusSeconds = 4f,
                MinimumRechargeSeconds = 6f
            };

            Assert.AreEqual(20f, profile.GetEffectiveRechargeSeconds(0), 0.001f);
            Assert.AreEqual(12f, profile.GetEffectiveRechargeSeconds(2), 0.001f);
            Assert.AreEqual(6f, profile.GetEffectiveRechargeSeconds(10), 0.001f);
        }

        [Test]
        public void PulsePlanner_SelectsHighestPriorityMarks()
        {
            var candidates = new List<GhostEntitySnapshot>
            {
                new GhostEntitySnapshot { Label = "Loot A", Kind = GhostEntityKind.Loot, Distance = 12f, Priority = 10f },
                new GhostEntitySnapshot { Label = "Monster B", Kind = GhostEntityKind.Monster, Distance = 8f, Priority = 30f },
                new GhostEntitySnapshot { Label = "Monster B", Kind = GhostEntityKind.Monster, Distance = 7f, Priority = 40f },
                new GhostEntitySnapshot { Label = "Ally C", Kind = GhostEntityKind.Ally, Distance = 5f, Priority = 20f }
            };

            var marks = GhostPulsePlanner.BuildMarks(candidates, 2, 100L, 15f);

            Assert.AreEqual(2, marks.Count);
            Assert.AreEqual("Monster B", marks[0].Label);
            Assert.AreEqual("Ally C", marks[1].Label);
        }

        [Test]
        public void PulsePlanner_FallsBackToRallyPointWhenNoCandidates()
        {
            var marks = GhostPulsePlanner.BuildMarks(new GhostEntitySnapshot[0], 4, 100L, 10f);

            Assert.AreEqual(1, marks.Count);
            Assert.AreEqual("Rally Point", marks[0].Label);
            Assert.AreEqual(GhostEntityKind.RallyPoint, marks[0].Kind);
        }

        [Test]
        public void LobbyState_PrunesExpiredGhosts()
        {
            var state = new PossessionPlusLobbyState
            {
                Ghosts = new Dictionary<string, GhostPresenceSnapshot>
                {
                    ["a"] = new GhostPresenceSnapshot { GhostExpiresUtcSeconds = 10 },
                    ["b"] = new GhostPresenceSnapshot { GhostExpiresUtcSeconds = 100 }
                }
            };

            state.PruneExpired(50);

            Assert.IsFalse(state.Ghosts.ContainsKey("a"));
            Assert.IsTrue(state.Ghosts.ContainsKey("b"));
        }
    }
}

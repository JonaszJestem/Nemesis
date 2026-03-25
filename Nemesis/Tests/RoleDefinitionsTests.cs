using NUnit.Framework;
using Nemesis.Modules.RoleSystem;

namespace Nemesis.Tests
{
    [TestFixture]
    public class RoleDefinitionsTests
    {
        private static RoleConfig DefaultConfig() => new RoleConfig();

        [Test]
        public void NoneRole_AllMultipliersAreOne()
        {
            var delta = RoleStatDelta.ForRole(Role.None, DefaultConfig());
            Assert.AreEqual(1.0f, delta.SpeedMultiplier, 0.001f);
            Assert.AreEqual(1.0f, delta.MaxHpMultiplier, 0.001f);
            Assert.AreEqual("None", delta.DisplayName);
        }

        [Test]
        public void ScoutRole_HasSpeedBoost()
        {
            var cfg = DefaultConfig();
            var delta = RoleStatDelta.ForRole(Role.Scout, cfg);
            Assert.AreEqual(cfg.ScoutSpeedMultiplier, delta.SpeedMultiplier, 0.001f);
            Assert.AreEqual(1.0f, delta.MaxHpMultiplier, 0.001f);
            Assert.AreEqual("Scout", delta.DisplayName);
        }

        [Test]
        public void TankRole_HasHpBoostAndSpeedPenalty()
        {
            var cfg = DefaultConfig();
            var delta = RoleStatDelta.ForRole(Role.Tank, cfg);
            Assert.AreEqual(cfg.TankHpMultiplier, delta.MaxHpMultiplier, 0.001f);
            Assert.AreEqual(cfg.TankSpeedPenalty, delta.SpeedMultiplier, 0.001f);
            Assert.AreEqual("Tank", delta.DisplayName);
        }

        [Test]
        public void MedicRole_HasNoStatChanges()
        {
            var delta = RoleStatDelta.ForRole(Role.Medic, DefaultConfig());
            Assert.AreEqual(1.0f, delta.SpeedMultiplier, 0.001f);
            Assert.AreEqual(1.0f, delta.MaxHpMultiplier, 0.001f);
            Assert.AreEqual("Medic", delta.DisplayName);
        }

        [Test]
        public void ScavengerRole_HasNoStatChanges()
        {
            var delta = RoleStatDelta.ForRole(Role.Scavenger, DefaultConfig());
            Assert.AreEqual(1.0f, delta.SpeedMultiplier, 0.001f);
            Assert.AreEqual(1.0f, delta.MaxHpMultiplier, 0.001f);
            Assert.AreEqual("Scavenger", delta.DisplayName);
        }

        [Test]
        public void AllRoles_HaveNonEmptyDisplayName()
        {
            var cfg = DefaultConfig();
            foreach (Role role in new[] { Role.None, Role.Scout, Role.Tank, Role.Medic, Role.Scavenger })
            {
                var delta = RoleStatDelta.ForRole(role, cfg);
                Assert.IsNotEmpty(delta.DisplayName, $"Role {role} should have a display name");
            }
        }

        [Test]
        public void CustomConfig_AffectsMultipliers()
        {
            var cfg = new RoleConfig
            {
                ScoutSpeedMultiplier = 1.5f,
                TankHpMultiplier = 2.0f,
                TankSpeedPenalty = 0.8f
            };

            var scout = RoleStatDelta.ForRole(Role.Scout, cfg);
            Assert.AreEqual(1.5f, scout.SpeedMultiplier, 0.001f);

            var tank = RoleStatDelta.ForRole(Role.Tank, cfg);
            Assert.AreEqual(2.0f, tank.MaxHpMultiplier, 0.001f);
            Assert.AreEqual(0.8f, tank.SpeedMultiplier, 0.001f);
        }

        [Test]
        public void AllActiveRoles_HaveDescription()
        {
            var cfg = DefaultConfig();
            foreach (Role role in new[] { Role.Scout, Role.Tank, Role.Medic, Role.Scavenger })
            {
                var delta = RoleStatDelta.ForRole(role, cfg);
                Assert.IsNotEmpty(delta.Description, $"Role {role} should have a description");
            }
        }
    }
}

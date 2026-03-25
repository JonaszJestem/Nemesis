using NUnit.Framework;
using Nemesis.Core;

namespace Nemesis.Tests
{
    [TestFixture]
    public class LobbyKeysTests
    {
        [Test]
        public void LobbyKeys_AreNotEmpty()
        {
            Assert.IsNotEmpty(LobbyKeys.Config);
            Assert.IsNotEmpty(LobbyKeys.Roles);
            Assert.IsNotEmpty(LobbyKeys.RolePrefix);
            Assert.IsNotEmpty(LobbyKeys.ContractBoard);
        }

        [Test]
        public void LobbyKeys_AreDistinct()
        {
            Assert.AreNotEqual(LobbyKeys.Config, LobbyKeys.Roles);
            Assert.AreNotEqual(LobbyKeys.Config, LobbyKeys.RolePrefix);
            Assert.AreNotEqual(LobbyKeys.Config, LobbyKeys.ContractBoard);
            Assert.AreNotEqual(LobbyKeys.Roles, LobbyKeys.RolePrefix);
            Assert.AreNotEqual(LobbyKeys.Roles, LobbyKeys.ContractBoard);
        }

        [Test]
        public void LobbyKeys_HaveNemesisPrefix()
        {
            Assert.IsTrue(LobbyKeys.Config.StartsWith("nemesis_"));
            Assert.IsTrue(LobbyKeys.Roles.StartsWith("nemesis_"));
            Assert.IsTrue(LobbyKeys.RolePrefix.StartsWith("nemesis_"));
            Assert.IsTrue(LobbyKeys.ContractBoard.StartsWith("nemesis_"));
        }
    }
}

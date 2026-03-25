using System.Linq;
using NUnit.Framework;
using Nemesis.Modules.ContractBoard;

namespace Nemesis.Tests
{
    [TestFixture]
    public class ContractBoardTests
    {
        [Test]
        public void Catalog_ContainsUniqueObjectiveKinds()
        {
            var engine = new ContractBoardEngine();
            var catalog = engine.GetCatalog();

            Assert.AreEqual(5, catalog.Count);
            Assert.AreEqual(catalog.Count, catalog.Select(x => x.Kind).Distinct().Count());
        }

        [Test]
        public void CreateState_IsDeterministicForSameSession()
        {
            var config = BuildConfig();
            var engine = new ContractBoardEngine();

            var first = engine.CreateState(config, "session-1");
            var second = engine.CreateState(config, "session-1");

            Assert.AreEqual(first.ActiveContracts.Count, second.ActiveContracts.Count);
            for (int i = 0; i < first.ActiveContracts.Count; i++)
            {
                Assert.AreEqual(first.ActiveContracts[i].ContractId, second.ActiveContracts[i].ContractId);
                Assert.AreEqual(first.ActiveContracts[i].Kind, second.ActiveContracts[i].Kind);
                Assert.AreEqual(first.ActiveContracts[i].Target, second.ActiveContracts[i].Target);
                Assert.AreEqual(first.ActiveContracts[i].RewardPoints, second.ActiveContracts[i].RewardPoints);
            }
        }

        [Test]
        public void ApplyProgress_CompletesAndReplacesContract()
        {
            var config = BuildConfig();
            config.StartingActiveContracts = 1;
            config.MaxContractsPerRun = 2;
            config.ReplaceCompletedContracts = true;

            var engine = new ContractBoardEngine();
            var state = engine.CreateState(config, "session-2");

            Assert.AreEqual(1, state.ActiveContracts.Count);
            var contract = state.ActiveContracts[0];

            var completions = engine.ApplyProgress(state, config, contract.Kind, contract.Target);

            Assert.AreEqual(1, completions.Count);
            Assert.AreEqual(1, state.CompletionHistory.Count);
            Assert.AreEqual(1, state.ActiveContracts.Count);
            Assert.AreEqual(2, state.IssuedContracts);
            Assert.AreEqual(contract.RewardPoints, state.TotalQueuedTalentPoints);
        }

        private static ContractBoardConfig BuildConfig()
        {
            return new ContractBoardConfig
            {
                Enabled = true,
                ShowHud = true,
                ResetOnSessionStart = true,
                StartingActiveContracts = 3,
                MaxContractsPerRun = 6,
                ReplaceCompletedContracts = true,
                AllowDuplicateObjectiveKinds = false,
                RefreshIntervalSeconds = 0.5f,
                TargetVariancePercent = 0f,
                RewardVariancePercent = 0f,
                TargetRampPerIssuedContract = 0f,
                RewardMultiplier = 1f,
                RewardBonusPoints = 0
            };
        }
    }
}

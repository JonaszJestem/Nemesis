using NUnit.Framework;
using Nemesis.Core;

namespace Nemesis.Tests
{
    [TestFixture]
    public class ModuleEventBusTests
    {
        private int _callCount;

        [SetUp]
        public void SetUp()
        {
            _callCount = 0;
        }

        [Test]
        public void SessionStarted_FiresSubscribers()
        {
            void Handler() => _callCount++;
            ModuleEventBus.OnSessionStarted += Handler;
            try
            {
                ModuleEventBus.RaiseSessionStarted();
                Assert.AreEqual(1, _callCount);
            }
            finally
            {
                ModuleEventBus.OnSessionStarted -= Handler;
            }
        }

        [Test]
        public void MonsterKilled_FiresSubscribers()
        {
            void Handler() => _callCount++;
            ModuleEventBus.OnMonsterKilled += Handler;
            try
            {
                ModuleEventBus.RaiseMonsterKilled();
                Assert.AreEqual(1, _callCount);
            }
            finally
            {
                ModuleEventBus.OnMonsterKilled -= Handler;
            }
        }

        [Test]
        public void LootCollected_FiresSubscribers()
        {
            void Handler() => _callCount++;
            ModuleEventBus.OnLootCollected += Handler;
            try
            {
                ModuleEventBus.RaiseLootCollected();
                Assert.AreEqual(1, _callCount);
            }
            finally
            {
                ModuleEventBus.OnLootCollected -= Handler;
            }
        }

        [Test]
        public void RoomCleared_FiresSubscribers()
        {
            void Handler() => _callCount++;
            ModuleEventBus.OnRoomCleared += Handler;
            try
            {
                ModuleEventBus.RaiseRoomCleared();
                Assert.AreEqual(1, _callCount);
            }
            finally
            {
                ModuleEventBus.OnRoomCleared -= Handler;
            }
        }

        [Test]
        public void Unsubscribe_StopsReceivingEvents()
        {
            void Handler() => _callCount++;
            ModuleEventBus.OnSessionStarted += Handler;
            ModuleEventBus.RaiseSessionStarted();
            Assert.AreEqual(1, _callCount);

            ModuleEventBus.OnSessionStarted -= Handler;
            ModuleEventBus.RaiseSessionStarted();
            Assert.AreEqual(1, _callCount); // no change
        }

        [Test]
        public void MultipleSubscribers_AllFire()
        {
            int count1 = 0, count2 = 0;
            void Handler1() => count1++;
            void Handler2() => count2++;

            ModuleEventBus.OnMonsterKilled += Handler1;
            ModuleEventBus.OnMonsterKilled += Handler2;
            try
            {
                ModuleEventBus.RaiseMonsterKilled();
                Assert.AreEqual(1, count1);
                Assert.AreEqual(1, count2);
            }
            finally
            {
                ModuleEventBus.OnMonsterKilled -= Handler1;
                ModuleEventBus.OnMonsterKilled -= Handler2;
            }
        }

        [Test]
        public void RaiseWithNoSubscribers_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => ModuleEventBus.RaiseSessionStarted());
            Assert.DoesNotThrow(() => ModuleEventBus.RaiseMonsterKilled());
            Assert.DoesNotThrow(() => ModuleEventBus.RaiseLootCollected());
            Assert.DoesNotThrow(() => ModuleEventBus.RaiseRoomCleared());
        }
    }
}

using System;

namespace Nemesis.Core
{
    // Typed events — no object boxing
    internal static class ModuleEventBus
    {
        public static event Action? OnSessionStarted;
        public static event Action? OnMonsterKilled;
        public static event Action? OnLootCollected;
        public static event Action? OnRoomCleared;

        public static void RaiseSessionStarted() => OnSessionStarted?.Invoke();
        public static void RaiseMonsterKilled() => OnMonsterKilled?.Invoke();
        public static void RaiseLootCollected() => OnLootCollected?.Invoke();
        public static void RaiseRoomCleared() => OnRoomCleared?.Invoke();
    }
}

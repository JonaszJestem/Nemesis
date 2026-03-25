using System;

namespace Nemesis.Modules.PersistentProgression
{
    internal static class LevelTable
    {
        public static long XPForLevel(int level, long baseXP = 100, float exponent = 1.5f)
        {
            if (level <= 1) return 0;
            return (long)(baseXP * Math.Pow(level, exponent));
        }

        public static long XPToNextLevel(int currentLevel, long baseXP = 100, float exponent = 1.5f)
        {
            return XPForLevel(currentLevel + 1, baseXP, exponent);
        }

        public static int ComputeLevel(long totalXP, int maxLevel, long baseXP = 100, float exponent = 1.5f)
        {
            int level = 1;
            while (level < maxLevel && totalXP >= XPForLevel(level + 1, baseXP, exponent))
            {
                level++;
            }
            return level;
        }

        public static float GetHpBonus(int level, float bonusPerLevel)
        {
            return 1.0f + (level - 1) * bonusPerLevel;
        }

        public static float GetSpeedBonus(int level, float bonusPerLevel)
        {
            return 1.0f + (level - 1) * bonusPerLevel;
        }
    }
}

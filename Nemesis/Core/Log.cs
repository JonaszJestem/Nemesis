using MelonLoader;

namespace Nemesis.Core
{
    /// <summary>
    /// Centralized logging with consistent module prefixes.
    /// MelonLoader already prefixes with "[Nemesis]", so we only add the module name.
    /// </summary>
    internal static class Log
    {
        public static void Msg(string message) => MelonLogger.Msg(message);
        public static void Warn(string message) => MelonLogger.Warning(message);

        public static void Msg(string module, string message) => MelonLogger.Msg($"[{module}] {message}");
        public static void Warn(string module, string message) => MelonLogger.Warning($"[{module}] {message}");

        // Module-specific shorthand
        public static class Difficulty
        {
            private const string Module = "Difficulty";
            public static void Msg(string message) => Log.Msg(Module, message);
            public static void Warn(string message) => Log.Warn(Module, message);
        }

        public static class Roles
        {
            private const string Module = "Roles";
            public static void Msg(string message) => Log.Msg(Module, message);
            public static void Warn(string message) => Log.Warn(Module, message);
        }

        public static class Radar
        {
            private const string Module = "Radar";
            public static void Msg(string message) => Log.Msg(Module, message);
            public static void Warn(string message) => Log.Warn(Module, message);
        }

        public static class Progression
        {
            private const string Module = "Progression";
            public static void Msg(string message) => Log.Msg(Module, message);
            public static void Warn(string message) => Log.Warn(Module, message);
        }

        public static class Sync
        {
            private const string Module = "Sync";
            public static void Msg(string message) => Log.Msg(Module, message);
            public static void Warn(string message) => Log.Warn(Module, message);
        }

        public static class Stamina
        {
            private const string Module = "Stamina";
            public static void Msg(string message) => Log.Msg(Module, message);
            public static void Warn(string message) => Log.Warn(Module, message);
        }

        public static class Fov
        {
            private const string Module = "Fov";
            public static void Msg(string message) => Log.Msg(Module, message);
            public static void Warn(string message) => Log.Warn(Module, message);
        }

        public static class VoiceFix
        {
            private const string Module = "VoiceFix";
            public static void Msg(string message) => Log.Msg(Module, message);
            public static void Warn(string message) => Log.Warn(Module, message);
        }

        public static class Damage
        {
            private const string Module = "Damage";
            public static void Msg(string message) => Log.Msg(Module, message);
            public static void Warn(string message) => Log.Warn(Module, message);
        }

        public static class MoreMimics
        {
            private const string Module = "MoreMimics";
            public static void Msg(string message) => Log.Msg(Module, message);
            public static void Warn(string message) => Log.Warn(Module, message);
        }

        public static class Health
        {
            private const string Module = "Health";
            public static void Msg(string message) => Log.Msg(Module, message);
            public static void Warn(string message) => Log.Warn(Module, message);
        }

        public static class Jump
        {
            private const string Module = "Jump";
            public static void Msg(string message) => Log.Msg(Module, message);
            public static void Warn(string message) => Log.Warn(Module, message);
        }

        public static class Fullbright
        {
            private const string Module = "Fullbright";
            public static void Msg(string message) => Log.Msg(Module, message);
            public static void Warn(string message) => Log.Warn(Module, message);
        }

        public static class LootDrop
        {
            private const string Module = "LootDrop";
            public static void Msg(string message) => Log.Msg(Module, message);
            public static void Warn(string message) => Log.Warn(Module, message);
        }

        public static class Voice
        {
            private const string Module = "Voice";
            public static void Msg(string message) => Log.Msg(Module, message);
            public static void Warn(string message) => Log.Warn(Module, message);
        }

        public static class Tooltip
        {
            private const string Module = "Tooltip";
            public static void Msg(string message) => Log.Msg(Module, message);
            public static void Warn(string message) => Log.Warn(Module, message);
        }

        public static class Marker
        {
            private const string Module = "Marker";
            public static void Msg(string message) => Log.Msg(Module, message);
            public static void Warn(string message) => Log.Warn(Module, message);
        }

        public static class Inventory
        {
            private const string Module = "Inventory";
            public static void Msg(string message) => Log.Msg(Module, message);
            public static void Warn(string message) => Log.Warn(Module, message);
        }

        public static class AutoLoot
        {
            private const string Module = "AutoLoot";
            public static void Msg(string message) => Log.Msg(Module, message);
            public static void Warn(string message) => Log.Warn(Module, message);
        }

        public static class Esp
        {
            private const string Module = "Esp";
            public static void Msg(string message) => Log.Msg(Module, message);
            public static void Warn(string message) => Log.Warn(Module, message);
        }

        public static class Fly
        {
            private const string Module = "Fly";
            public static void Msg(string message) => Log.Msg(Module, message);
            public static void Warn(string message) => Log.Warn(Module, message);
        }
    }
}

using System;
using System.IO;

namespace Nemesis.Core
{
    internal static class Paths
    {
        private static string? _userDataDir;

        public static string UserDataDir
        {
            get
            {
                if (_userDataDir != null) return _userDataDir;

                // Try MelonEnvironment first (newer MelonLoader)
                try
                {
                    var envType = Type.GetType("MelonLoader.Utils.MelonEnvironment, MelonLoader");
                    if (envType != null)
                    {
                        var prop = envType.GetProperty("UserDataDirectory",
                            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                        if (prop != null)
                        {
                            _userDataDir = prop.GetValue(null) as string;
                            if (_userDataDir != null) return _userDataDir;
                        }
                    }
                }
                catch { }

                // Try MelonUtils (older MelonLoader)
                try
                {
                    var utilsType = Type.GetType("MelonLoader.MelonUtils, MelonLoader");
                    if (utilsType != null)
                    {
                        var prop = utilsType.GetProperty("UserDataDirectory",
                            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                        if (prop != null)
                        {
                            _userDataDir = prop.GetValue(null) as string;
                            if (_userDataDir != null) return _userDataDir;
                        }
                    }
                }
                catch { }

                // Fallback: derive from assembly location
                var asmDir = Path.GetDirectoryName(typeof(Paths).Assembly.Location) ?? ".";
                _userDataDir = Path.Combine(Path.GetDirectoryName(asmDir) ?? asmDir, "UserData");
                return _userDataDir;
            }
        }

        public static string NemesisDir => Path.Combine(UserDataDir, "Nemesis");
    }
}

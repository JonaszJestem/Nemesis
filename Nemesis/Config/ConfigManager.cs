using System;
using System.IO;
using MelonLoader;
using Nemesis.Core;
using Newtonsoft.Json;

namespace Nemesis.Config
{
    internal static class ConfigManager
    {
        private static readonly string ConfigPath = Path.Combine(Paths.NemesisDir, "config.json");

        public static SuiteConfig Load()
        {
            try
            {
                if (File.Exists(ConfigPath))
                {
                    string json = File.ReadAllText(ConfigPath);
                    return JsonConvert.DeserializeObject<SuiteConfig>(json) ?? new SuiteConfig();
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"[Nemesis] Failed to load config: {ex.Message}");
            }
            return new SuiteConfig();
        }

        public static void Save(SuiteConfig config)
        {
            try
            {
                Directory.CreateDirectory(Paths.NemesisDir);
                string json = JsonConvert.SerializeObject(config, Formatting.Indented);
                File.WriteAllText(ConfigPath, json);
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"[Nemesis] Failed to save config: {ex.Message}");
            }
        }
    }
}

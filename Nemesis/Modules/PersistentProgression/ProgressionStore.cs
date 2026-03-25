using System;
using System.IO;
using MelonLoader;
using Newtonsoft.Json;

namespace Nemesis.Modules.PersistentProgression
{
    internal static class ProgressionStore
    {
        private static readonly string DataDir = Path.Combine(MelonUtils.UserDataDirectory, "Nemesis");
        private static readonly string DataPath = Path.Combine(DataDir, "progression.json");

        public static ProgressionData Load()
        {
            try
            {
                if (File.Exists(DataPath))
                {
                    string json = File.ReadAllText(DataPath);
                    return JsonConvert.DeserializeObject<ProgressionData>(json) ?? new ProgressionData();
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"[Progression] Failed to load data: {ex.Message}");
            }
            return new ProgressionData();
        }

        public static void Save(ProgressionData data)
        {
            try
            {
                Directory.CreateDirectory(DataDir);
                string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                File.WriteAllText(DataPath, json);
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"[Progression] Failed to save data: {ex.Message}");
            }
        }
    }
}

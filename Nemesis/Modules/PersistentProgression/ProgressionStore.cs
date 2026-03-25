using System;
using System.IO;
using Nemesis.Core;
using Newtonsoft.Json;

namespace Nemesis.Modules.PersistentProgression
{
    internal static class ProgressionStore
    {
        private static readonly string DataPath = Path.Combine(Paths.NemesisDir, "progression.json");

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
                Log.Progression.Warn($"Failed to load data: {ex.Message}");
            }
            return new ProgressionData();
        }

        public static void Save(ProgressionData data)
        {
            try
            {
                Directory.CreateDirectory(Paths.NemesisDir);
                string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                File.WriteAllText(DataPath, json);
            }
            catch (Exception ex)
            {
                Log.Progression.Warn($"Failed to save data: {ex.Message}");
            }
        }
    }
}

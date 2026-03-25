using System;
using System.Collections.Generic;
using System.IO;
using Nemesis.Core;
using Newtonsoft.Json;

namespace Nemesis.Modules.RivalGhosts
{
    internal sealed class RivalGhostArchive
    {
        public int Version { get; set; } = 1;
        public List<RivalGhostRecord> Records { get; set; } = new List<RivalGhostRecord>();
    }

    internal static class RivalGhostStore
    {
        private static readonly string DataPath = Path.Combine(Paths.NemesisDir, "rival_ghosts.json");

        public static List<RivalGhostRecord> Load()
        {
            try
            {
                if (!File.Exists(DataPath))
                    return new List<RivalGhostRecord>();

                var archive = JsonConvert.DeserializeObject<RivalGhostArchive>(File.ReadAllText(DataPath));
                return archive?.Records ?? new List<RivalGhostRecord>();
            }
            catch (Exception ex)
            {
                Log.Warn("RivalGhosts", $"Load failed: {ex.Message}");
                return new List<RivalGhostRecord>();
            }
        }

        public static void Save(List<RivalGhostRecord> records)
        {
            try
            {
                Directory.CreateDirectory(Paths.NemesisDir);
                var archive = new RivalGhostArchive { Version = 1, Records = records ?? new List<RivalGhostRecord>() };
                File.WriteAllText(DataPath, JsonConvert.SerializeObject(archive, Formatting.Indented));
            }
            catch (Exception ex)
            {
                Log.Warn("RivalGhosts", $"Save failed: {ex.Message}");
            }
        }
    }
}

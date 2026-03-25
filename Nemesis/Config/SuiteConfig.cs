using Nemesis.Modules.DifficultyDirector;
using Nemesis.Modules.ProximityRadar;
using Nemesis.Modules.RoleSystem;
using Nemesis.Modules.PersistentProgression;

namespace Nemesis.Config
{
    internal class SuiteConfig
    {
        public DifficultyConfig Difficulty { get; set; } = new DifficultyConfig();
        public RoleConfig Roles { get; set; } = new RoleConfig();
        public RadarConfig Radar { get; set; } = new RadarConfig();
        public ProgressionConfig Progression { get; set; } = new ProgressionConfig();
    }
}

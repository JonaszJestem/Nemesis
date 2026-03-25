using Nemesis.Config;
using Newtonsoft.Json;

namespace Nemesis.Core
{
    /// <summary>
    /// Pure logic for config sync testing (no Unity/Steam dependencies).
    /// The actual sync uses HostConfig directly - this provides testable helpers.
    /// </summary>
    internal static class ConfigSyncLogic
    {
        /// <summary>
        /// Serialize a HostConfig to JSON (same as what gets pushed to lobby).
        /// </summary>
        public static string Serialize(HostConfig hostConfig)
        {
            return JsonConvert.SerializeObject(hostConfig);
        }

        /// <summary>
        /// Deserialize a HostConfig from JSON (same as what gets pulled from lobby).
        /// </summary>
        public static HostConfig? Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<HostConfig>(json);
        }

        /// <summary>
        /// Round-trip test: serialize and deserialize, then apply to a fresh SuiteConfig.
        /// </summary>
        public static SuiteConfig RoundTrip(HostConfig source)
        {
            string json = Serialize(source);
            var deserialized = Deserialize(json)!;
            var suite = new SuiteConfig();
            deserialized.ApplyTo(suite);
            return suite;
        }
    }
}

using Nemesis.Core;

namespace Nemesis.Modules.InventoryExpansion
{
    internal class InventoryExpansionModule : IModule
    {
        public string Name => "Inventory Expansion";

        private readonly InventoryExpansionConfig _config;

        public static bool IsEnabled { get; private set; }

        public InventoryExpansionModule(InventoryExpansionConfig config)
        {
            _config = config;
        }

        public void Initialize()
        {
            Log.Inventory.Msg("Initialized");
        }

        public void Shutdown()
        {
            IsEnabled = false;
        }

        public void OnUpdate()
        {
            IsEnabled = _config.Enabled;
        }

        public void OnGUI() { }
    }
}

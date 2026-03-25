namespace Nemesis.Core
{
    /// <summary>
    /// Type names resolved via reflection from game assemblies.
    /// </summary>
    internal static class GameTypeNames
    {
        public const string GameAssembly = "Assembly-CSharp";
        public const string FishySteamworks = "FishySteamworks";

        // Server types
        public const string ServerSocket = "ServerSocket";
        public const string VMonster = "VMonster";
        public const string VRoomManager = "VRoomManager";
        public const string GameSessionInfo = "GameSessionInfo";
        public const string StatController = "StatController";
        public const string StatType = "StatType";
        public const string IVroom = "IVroom";
        public const string VWaitingRoom = "VWaitingRoom";
        public const string MaintenanceRoom = "MaintenanceRoom";
        public const string MsgErrorCode = "MsgErrorCode";

        // Client types
        public const string ProtoActor = "Mimic.Actors.ProtoActor";

        // StatManager / Stamina / Damage
        public const string StatManager = "StatManager";

        // Audio
        public const string VoiceEffectPreset = "Mimic.Audio.VoiceEffectPreset";

        // Audio / Voice
        public const string SpeechEventArchive = "SpeechEventArchive";

        // Equipment / Marker
        public const string EquipmentItemElement = "EquipmentItemElement";

        // Inventory
        public const string InventoryController = "InventoryController";

        // Dungeon
        public const string DungeonRoom = "DungeonRoom";
        public const string DungeonMasterInfo = "DungeonMasterInfo";

        // Loot / Spawn
        public const string RandomSpawnedItemActorData = "RandomSpawnedItemActorData";
        public const string SpawnedActorData = "SpawnedActorData";
        public const string ItemElement = "ItemElement";
        public const string PosWithRot = "PosWithRot";
        public const string MapMarkerType = "MapMarkerType";
        public const string ReasonOfSpawn = "ReasonOfSpawn";

        // MelonLoader
        public const string MelonEnvironment = "MelonLoader.Utils.MelonEnvironment, MelonLoader";
        public const string MelonUtils = "MelonLoader.MelonUtils, MelonLoader";
    }
}

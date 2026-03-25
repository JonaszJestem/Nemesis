namespace Nemesis.Core
{
    /// <summary>
    /// Field names used in reflection to access game internals.
    /// Centralizes magic strings so breakage from game updates is caught in one place.
    /// </summary>
    internal static class GameFieldNames
    {
        // Hub
        public const string Hub_VWorld = "vworld";
        public const string Hub_PersistentData = "pdata";
        public const string Hub_SteamConnector = "SteamConnector";

        // VWorld
        public const string VWorld_VRoomManager = "VRoomManager";

        // VRoom
        public const string VRoom_VPlayerDict = "_vPlayerDict";
        public const string VRoom_VActorDict = "_vActorDict";
        public const string VRoom_MaxPlayers = "_maxPlayers";
        public const string VRoom_CurrentDay = "_currentDay";
        public const string VRoom_CurrentSessionCount = "_currentSessionCount";
        public const string VRoom_LevelObjects = "_levelObjects";
        public const string VRoom_RoomID = "RoomID";
        public const string VRoom_MasterID = "MasterID";
        public const string VRoom_DungeonWeather = "_dungeonWeather";

        // VRoomManager
        public const string VRoomManager_VRooms = "_vrooms";
        public const string VRoomManager_RoomDict = "_roomDict";

        // StatController / StatManager
        public const string StatController_StatManager = "StatManager";
        public const string StatManager_TotalStats = "_totalStats";
        public const string StatManager_MaxHp = "_maxHp";
        public const string StatManager_MoveSpeed = "_moveSpeed";
        public const string StatCollection_Elements = "elements";

        // ProtoActor
        public const string ProtoActor_Inventory = "inventory";
        public const string Inventory_SlotItems = "SlotItems";

        // DungeonRoom
        public const string DungeonRoom_SpawnedActorDatas = "_spawnedActorDatas";

        // DungeonMasterInfo
        public const string DungeonMasterInfo_NormalMonsterSpawnRate = "NormalMonsterSpawnRate";
        public const string DungeonMasterInfo_MimicSpawnCountMin = "MimicSpawnCountMin";
        public const string DungeonMasterInfo_MimicSpawnCountMax = "MimicSpawnCountMax";
        public const string DungeonMasterInfo_MimicSpawnRate = "MimicSpawnRate";
        public const string DungeonMasterInfo_NormalMonsterSpawnTryCount = "NormalMonsterSpawnTryCount";
        public const string DungeonMasterInfo_MimicSpawnTryCount = "MimicSpawnTryCount";

        // SpeechEventArchive
        public const string SpeechEventArchive_MaxRecordings = "_maxRecordings";

        // ServerSocket
        public const string ServerSocket_MaximumClients = "_maximumClients";
        public const string ServerSocket_Connections = "_connections";
    }
}

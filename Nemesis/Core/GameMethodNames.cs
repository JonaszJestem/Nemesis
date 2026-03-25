namespace Nemesis.Core
{
    /// <summary>
    /// Method and property names resolved via reflection from game assemblies.
    /// </summary>
    internal static class GameMethodNames
    {
        // StatController
        public const string StatController_LoadStats = "LoadStats";

        // VMonster
        public const string VMonster_OnDead = "OnDead";
        public const string VMonster_Die = "Die";

        // GameSessionInfo
        public const string GameSessionInfo_StartSession = "StartSession";

        // VRoomManager
        public const string VRoomManager_EnterWaitingRoom = "EnterWaitingRoom";

        // StatManager
        public const string StatManager_ConsumeStamina = "ConsumeStamina";
        public const string StatManager_GetCurrentStamina = "GetCurrentStamina";
        public const string StatManager_RegenerateStamina = "RegenerateStamina";
        public const string StatManager_ApplyDamage = "ApplyDamage";

        // EquipmentItemElement
        public const string EquipmentItemElement_SetDurability = "SetDurability";

        // IVroom / DungeonRoom - Loot spawning
        public const string IVroom_GetNewItemElement = "GetNewItemElement";
        public const string IVroom_FindNearestPoly = "FindNearestPoly";
        public const string IVroom_SpawnLootingObject = "SpawnLootingObject";
        public const string IVroom_OnVacateRoom = "OnVacateRoom";
        public const string DungeonRoom_Initialize = "Initialize";

        // RandomSpawnedItemActorData
        public const string RandomSpawnedItemActorData_GetPickedItemValue = "GetPickedItemValue";
        public const string RandomSpawnedItemActorData_Candidates = "Candidates";

        // InventoryController
        public const string InventoryController_InvenFull = "InvenFull";

        // ProtoActor
        public const string ProtoActor_GrapLootingObject = "GrapLootingObject";
        public const string ProtoActor_BarterItem = "BarterItem";
        public const string ProtoActor_OnActorDeath = "OnActorDeath";
        public const string ProtoActor_AmIAvatar = "AmIAvatar";
        public const string ProtoActor_ProcessJumpKey = "ProcessJumpKey";
        public const string ProtoActor_GetSelectedInventoryItem = "GetSelectedInventoryItem";
    }

    internal static class GamePropertyNames
    {
        public const string StatController_Self = "Self";
        public const string StatElement_Value = "Value";
        public const string VActor_PositionVector = "PositionVector";
        public const string SteamConnector_JoinedLobbyID = "JoinedLobbyID";
        public const string ProtoActor_MonsterMasterID = "monsterMasterID";
        public const string VMonster_IsIndoor = "IsIndoor";
        public const string VMonster_MasterID = "MasterID";
        public const string VMonster_Position = "Position";
        public const string VMonster_VRoom = "VRoom";
        public const string SpawnedActorData_MarkerType = "MarkerType";
        public const string MelonEnvironment_UserDataDirectory = "UserDataDirectory";
    }

    internal static class GameEnumValues
    {
        public const string StatType_HP = "HP";
        public const string StatType_Attack = "Attack";
        public const string MsgErrorCode_Success = "Success";
        public const string MapMarkerType_LootingObject = "LootingObject";
        public const string ReasonOfSpawn_ActorDying = "ActorDying";
    }
}

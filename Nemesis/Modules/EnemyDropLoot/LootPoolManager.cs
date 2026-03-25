using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using MimicAPI.GameAPI;
using Nemesis.Core;
using UnityEngine;

namespace Nemesis.Modules.EnemyDropLoot
{
    /// <summary>
    /// Captures the active dungeon's loot pool when a DungeonRoom is initialized,
    /// and handles monster death loot spawning.
    /// Mirrors the community mod's LootPoolManager + LootSpawnService.
    /// </summary>
    internal static class LootPoolManager
    {
        private const float DropScatterRadius = 2f;

        // Cached loot sources from the dungeon room
        private static readonly List<object> _lootSources = new List<object>();
        private static readonly List<int> _fallbackItemIds = new List<int>();

        private static object? _activeRoom;
        private static bool _hasActiveRoom;

        // Cached reflection info
        private static Type? _randomSpawnedItemActorDataType;
        private static Type? _mapMarkerTypeEnum;
        private static Type? _reasonOfSpawnEnum;
        private static object? _lootingObjectEnumValue;
        private static object? _actorDyingEnumValue;
        private static MethodInfo? _getPickedItemValueMethod;
        private static PropertyInfo? _candidatesProp;
        private static PropertyInfo? _markerTypeProp;
        private static bool _reflectionInitialized;

        internal static void Reset()
        {
            _lootSources.Clear();
            _fallbackItemIds.Clear();
            _activeRoom = null;
            _hasActiveRoom = false;
        }

        private static bool EnsureReflection()
        {
            if (_reflectionInitialized) return true;

            try
            {
                var assembly = ServerNetworkAPI.GetGameAssembly();
                if (assembly == null) return false;

                _randomSpawnedItemActorDataType = assembly.GetType(GameTypeNames.RandomSpawnedItemActorData);
                var spawnedActorDataType = assembly.GetType(GameTypeNames.SpawnedActorData);
                _mapMarkerTypeEnum = assembly.GetType(GameTypeNames.MapMarkerType);
                _reasonOfSpawnEnum = assembly.GetType(GameTypeNames.ReasonOfSpawn);

                if (_mapMarkerTypeEnum != null)
                    _lootingObjectEnumValue = Enum.Parse(_mapMarkerTypeEnum, GameEnumValues.MapMarkerType_LootingObject);

                if (_reasonOfSpawnEnum != null)
                    _actorDyingEnumValue = Enum.Parse(_reasonOfSpawnEnum, GameEnumValues.ReasonOfSpawn_ActorDying);

                if (_randomSpawnedItemActorDataType != null)
                {
                    _getPickedItemValueMethod = _randomSpawnedItemActorDataType.GetMethod(
                        GameMethodNames.RandomSpawnedItemActorData_GetPickedItemValue,
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                    _candidatesProp = _randomSpawnedItemActorDataType.GetProperty(
                        GameMethodNames.RandomSpawnedItemActorData_Candidates,
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                }

                if (spawnedActorDataType != null)
                {
                    _markerTypeProp = spawnedActorDataType.GetProperty(
                        GamePropertyNames.SpawnedActorData_MarkerType,
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                }

                _reflectionInitialized = true;
                return true;
            }
            catch (Exception ex)
            {
                Log.LootDrop.Warn($"Reflection init failed: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Called from the DungeonRoom.Initialize postfix patch.
        /// Reads _spawnedActorDatas to collect loot-eligible spawn data.
        /// </summary>
        internal static void ConfigureForDungeon(object room)
        {
            Reset();

            if (!EnsureReflection()) return;

            _activeRoom = room;
            _hasActiveRoom = true;

            try
            {
                var spawnDict = ReflectionHelper.GetFieldValue(room, GameFieldNames.DungeonRoom_SpawnedActorDatas)
                    as IDictionary;

                if (spawnDict == null)
                {
                    Log.LootDrop.Warn("Unable to fetch spawn data for current dungeon.");
                    return;
                }

                foreach (DictionaryEntry entry in spawnDict)
                {
                    var data = entry.Value;
                    if (data == null) continue;

                    // Check MarkerType == LootingObject
                    if (_markerTypeProp != null && _lootingObjectEnumValue != null)
                    {
                        var markerType = _markerTypeProp.GetValue(data);
                        if (markerType == null || !markerType.Equals(_lootingObjectEnumValue))
                            continue;
                    }

                    // Only collect RandomSpawnedItemActorData instances
                    if (_randomSpawnedItemActorDataType != null &&
                        _randomSpawnedItemActorDataType.IsInstanceOfType(data))
                    {
                        _lootSources.Add(data);

                        // Collect candidate item IDs for fallback
                        if (_candidatesProp != null)
                        {
                            var candidates = _candidatesProp.GetValue(data);
                            if (candidates is IDictionary candidateDict)
                            {
                                foreach (var key in candidateDict.Keys)
                                {
                                    if (key is int itemId && !_fallbackItemIds.Contains(itemId))
                                        _fallbackItemIds.Add(itemId);
                                }
                            }
                        }
                    }
                }

                Log.LootDrop.Msg($"Loaded {_lootSources.Count} loot spawn bundles with {_fallbackItemIds.Count} unique items.");
            }
            catch (Exception ex)
            {
                Log.LootDrop.Warn($"Failed to read dungeon loot pool: {ex.Message}");
            }
        }

        /// <summary>
        /// Called from VMonster.OnDead prefix patch.
        /// Rolls for drops and spawns loot at the monster's position.
        /// </summary>
        internal static void TryHandleMonsterDeath(object monster)
        {
            if (!EnemyDropLootModule.IsEnabled || _lootSources.Count == 0)
                return;

            if (!_hasActiveRoom)
                return;

            // Verify monster is in the active room
            var monsterRoom = ReflectionHelper.GetPropertyValue(monster, GamePropertyNames.VMonster_VRoom);
            if (monsterRoom == null || monsterRoom != _activeRoom)
                return;

            int dropCount = EnemyDropLootModule.RollDropCount();
            if (dropCount <= 0)
                return;

            for (int i = 0; i < dropCount; i++)
            {
                if (TryPickItem(out int itemMasterId))
                {
                    TrySpawnLoot(monster, monsterRoom, itemMasterId);
                }
            }
        }

        private static bool TryPickItem(out int itemMasterId)
        {
            itemMasterId = 0;
            if (_lootSources.Count == 0) return false;

            // Pick a random loot source and call GetPickedItemValue()
            var source = _lootSources[UnityEngine.Random.Range(0, _lootSources.Count)];

            if (_getPickedItemValueMethod != null)
            {
                var result = _getPickedItemValueMethod.Invoke(source, null);
                if (result is int pickedId)
                    itemMasterId = pickedId;
            }

            // Fallback: pick a random item from the collected candidates
            if (itemMasterId == 0 && _fallbackItemIds.Count > 0)
                itemMasterId = _fallbackItemIds[UnityEngine.Random.Range(0, _fallbackItemIds.Count)];

            return itemMasterId != 0;
        }

        /// <summary>
        /// Spawns a loot item at the monster's position, replicating the community mod's
        /// LootSpawnService: GetNewItemElement -> FindNearestPoly -> SpawnLootingObject.
        /// </summary>
        private static void TrySpawnLoot(object monster, object vRoom, int itemMasterId)
        {
            try
            {
                // ItemElement itemElement = vRoom.GetNewItemElement(itemMasterId, false)
                var itemElement = ReflectionHelper.InvokeMethod(
                    vRoom, GameMethodNames.IVroom_GetNewItemElement, itemMasterId, false);

                if (itemElement == null)
                {
                    Log.LootDrop.Warn($"GetNewItemElement returned null for item {itemMasterId}.");
                    return;
                }

                // Get monster position for scatter
                var positionVector = ReflectionHelper.GetPropertyValue(
                    monster, GamePropertyNames.VActor_PositionVector);

                Vector3 spawnPos;
                if (positionVector is Vector3 monsterPos)
                    spawnPos = monsterPos;
                else
                {
                    Log.LootDrop.Warn("Could not read monster PositionVector.");
                    return;
                }

                // Apply scatter offset
                Vector2 offset2D = UnityEngine.Random.insideUnitCircle * DropScatterRadius;
                spawnPos += new Vector3(offset2D.x, 0f, offset2D.y);

                // FindNearestPoly to snap to navmesh
                float searchRadius = Mathf.Max(1.5f, DropScatterRadius);
                Vector3 nearestPos = spawnPos;

                try
                {
                    // bool FindNearestPoly(Vector3 pos, out Vector3 nearest, float radius)
                    var findMethod = vRoom.GetType().GetMethod(
                        GameMethodNames.IVroom_FindNearestPoly,
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                    if (findMethod != null)
                    {
                        var args = new object[] { spawnPos, Vector3.zero, searchRadius };
                        var found = findMethod.Invoke(vRoom, args);
                        if (found is bool b && b)
                            nearestPos = (Vector3)args[1]; // out parameter
                    }
                }
                catch { /* Fall back to unsnapped position */ }

                // Clone the monster's PosWithRot and update coordinates
                var monsterPosRot = ReflectionHelper.GetPropertyValue(
                    monster, GamePropertyNames.VMonster_Position);

                object? dropPos = null;
                if (monsterPosRot != null)
                {
                    var cloneMethod = monsterPosRot.GetType().GetMethod("Clone",
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                    if (cloneMethod != null)
                    {
                        dropPos = cloneMethod.Invoke(monsterPosRot, null);
                        if (dropPos != null)
                        {
                            ReflectionHelper.SetFieldValue(dropPos, "x", nearestPos.x);
                            ReflectionHelper.SetFieldValue(dropPos, "y", nearestPos.y);
                            ReflectionHelper.SetFieldValue(dropPos, "z", nearestPos.z);
                        }
                    }
                }

                if (dropPos == null)
                {
                    Log.LootDrop.Warn("Could not create drop position.");
                    return;
                }

                // Get IsIndoor
                var isIndoorObj = ReflectionHelper.GetPropertyValue(
                    monster, GamePropertyNames.VMonster_IsIndoor);
                bool isIndoor = isIndoorObj is bool b2 && b2;

                // SpawnLootingObject(itemElement, dropPos, isIndoor, ReasonOfSpawn.ActorDying)
                object[] spawnArgs;
                if (_actorDyingEnumValue != null)
                    spawnArgs = new object[] { itemElement, dropPos, isIndoor, _actorDyingEnumValue };
                else
                    spawnArgs = new object[] { itemElement, dropPos, isIndoor };

                var spawnResult = ReflectionHelper.InvokeMethod(
                    vRoom, GameMethodNames.IVroom_SpawnLootingObject, spawnArgs);

                if (spawnResult is int resultInt && resultInt == 0)
                {
                    Log.LootDrop.Warn($"SpawnLootingObject failed for item {itemMasterId}.");
                    return;
                }

                var monsterMasterId = ReflectionHelper.GetPropertyValue(
                    monster, GamePropertyNames.VMonster_MasterID);
                Log.LootDrop.Msg($"Spawned loot item {itemMasterId} from monster {monsterMasterId}.");
            }
            catch (Exception ex)
            {
                Log.LootDrop.Warn($"TrySpawnLoot failed for item {itemMasterId}: {ex.Message}");
            }
        }
    }
}

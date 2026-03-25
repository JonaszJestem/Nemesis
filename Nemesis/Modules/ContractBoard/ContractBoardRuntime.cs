using System;
using System.Linq;
using MimicAPI.GameAPI;
using Nemesis.Core;
using Nemesis.Modules.TramTalentTree;
using UnityEngine;

namespace Nemesis.Modules.ContractBoard
{
    internal static class ContractBoardRuntime
    {
        private static readonly object Sync = new object();
        private static readonly ContractBoardEngine Engine = new ContractBoardEngine();

        private static ContractBoardConfig? _config;
        private static ContractBoardState? _hostState;
        private static ContractBoardSnapshot _currentSnapshot = ContractBoardSnapshot.Empty;
        private static float _refreshTimer;
        private static bool _hostDirty;
        private static bool _pendingHostReset;
        private static bool _forceClientRefresh;
        private static string _currentSessionKey = "";
        private static long _lastAppliedCompletionSequence;

        public static ContractBoardSnapshot CurrentSnapshot
        {
            get
            {
                lock (Sync)
                {
                    return _currentSnapshot.Clone();
                }
            }
        }

        public static void Initialize(ContractBoardConfig config)
        {
            lock (Sync)
            {
                _config = config;
                if (_config == null || !_config.Enabled)
                {
                    ClearHostStateLocked(clearLobby: false);
                    ClearClientStateLocked();
                }
            }
        }

        public static void Shutdown()
        {
            lock (Sync)
            {
                ClearHostStateLocked(clearLobby: false);
                ClearClientStateLocked();
                ContractBoardStore.ResetCache();
            }
        }

        public static void NotifySessionStarted(bool isHost)
        {
            lock (Sync)
            {
                if (_config?.ResetOnSessionStart != true)
                    return;

                if (isHost)
                    _pendingHostReset = true;
                else
                    _forceClientRefresh = true;
            }
        }

        public static void Update(ContractBoardConfig config, float deltaTime, bool isHost)
        {
            lock (Sync)
            {
                _config = config;
                if (_config == null || !_config.Enabled)
                {
                    if (isHost)
                        ClearHostStateLocked(clearLobby: true);
                    else
                        ClearClientStateLocked();
                    return;
                }

                _refreshTimer += deltaTime;
                if (isHost)
                    UpdateHostLocked();
                else
                    UpdateClientLocked();
            }
        }

        public static void ReportObjective(ContractBoardObjectiveKind kind)
        {
            lock (Sync)
            {
                if (_config == null || !_config.Enabled)
                    return;

                EnsureHostStateLocked();
                if (_hostState == null)
                    return;

                var completions = Engine.ApplyProgress(_hostState, _config, kind, 1);
                if (completions.Count > 0)
                {
                    foreach (var completion in completions)
                    {
                        TramTalentTreeBridge.QueueContractPoints(Math.Max(0, completion.RewardPoints));
                        _lastAppliedCompletionSequence = completion.Sequence;
                    }
                }

                _hostDirty = true;
                PublishHostSnapshotLocked();
            }
        }

        private static void UpdateHostLocked()
        {
            var room = RoomAPI.GetCurrentRoom();
            if (room == null)
            {
                ClearHostStateLocked(clearLobby: true);
                return;
            }

            string sessionKey = BuildSessionKey(room);
            if (string.IsNullOrWhiteSpace(sessionKey))
            {
                ClearHostStateLocked(clearLobby: true);
                return;
            }

            bool sessionChanged = !string.Equals(sessionKey, _currentSessionKey, StringComparison.Ordinal);
            if (_hostState == null || sessionChanged || _pendingHostReset)
            {
                _pendingHostReset = false;
                StartHostSessionLocked(sessionKey);
                return;
            }

            float interval = Math.Max(0.25f, _config?.RefreshIntervalSeconds ?? 0.5f);
            if (_hostDirty || _refreshTimer >= interval)
            {
                PublishHostSnapshotLocked();
                _refreshTimer = 0f;
            }
        }

        private static void UpdateClientLocked()
        {
            if (SteamLobbyHelper.GetLobbyId() == 0)
            {
                ClearClientStateLocked();
                return;
            }

            if (_forceClientRefresh)
            {
                _forceClientRefresh = false;
                var forcedSnapshot = ContractBoardStore.Read(force: true);
                if (forcedSnapshot == null)
                {
                    ClearClientStateLocked();
                    return;
                }

                ApplyClientSnapshotLocked(forcedSnapshot);
                return;
            }

            float interval = Math.Max(0.25f, _config?.RefreshIntervalSeconds ?? 0.5f);
            if (_refreshTimer < interval)
                return;

            _refreshTimer = 0f;
            var snapshot = ContractBoardStore.Read();
            if (snapshot == null)
                return;

            ApplyClientSnapshotLocked(snapshot);
        }

        private static void StartHostSessionLocked(string sessionKey)
        {
            _currentSessionKey = sessionKey;
            _hostState = Engine.CreateState(_config!, sessionKey);
            _lastAppliedCompletionSequence = 0;
            _hostDirty = false;
            _refreshTimer = 0f;
            _currentSnapshot = Engine.BuildSnapshot(_hostState, _config!, isHost: true);
            ContractBoardStore.Publish(_currentSnapshot);
        }

        private static void EnsureHostStateLocked()
        {
            if (_hostState != null || _config == null || !_config.Enabled)
                return;

            var room = RoomAPI.GetCurrentRoom();
            if (room == null)
                return;

            string sessionKey = BuildSessionKey(room);
            if (string.IsNullOrWhiteSpace(sessionKey))
                return;

            StartHostSessionLocked(sessionKey);
        }

        private static void PublishHostSnapshotLocked()
        {
            if (_hostState == null || _config == null)
                return;

            _currentSnapshot = Engine.BuildSnapshot(_hostState, _config, isHost: true);
            ContractBoardStore.Publish(_currentSnapshot);
            _hostDirty = false;
        }

        private static void ApplyClientSnapshotLocked(ContractBoardSnapshot snapshot)
        {
            if (snapshot == null)
                return;

            if (!string.Equals(snapshot.SessionKey, _currentSnapshot.SessionKey, StringComparison.Ordinal) ||
                snapshot.LastCompletionSequence < _lastAppliedCompletionSequence)
            {
                _lastAppliedCompletionSequence = 0;
            }

            var history = snapshot.CompletionHistory ?? Enumerable.Empty<ContractBoardCompletionSnapshot>();
            foreach (var completion in history.OrderBy(x => x.Sequence))
            {
                if (completion.Sequence <= _lastAppliedCompletionSequence)
                    continue;

                TramTalentTreeBridge.QueueContractPoints(Math.Max(0, completion.RewardPoints));
                _lastAppliedCompletionSequence = completion.Sequence;
            }

            _currentSnapshot = snapshot.Clone();
        }

        private static void ClearHostStateLocked(bool clearLobby)
        {
            _hostState = null;
            _currentSessionKey = "";
            _lastAppliedCompletionSequence = 0;
            _pendingHostReset = false;
            _hostDirty = false;
            _refreshTimer = 0f;
            _currentSnapshot = ContractBoardSnapshot.Empty;

            if (clearLobby)
                ContractBoardStore.Publish(_currentSnapshot);
        }

        private static void ClearClientStateLocked()
        {
            _lastAppliedCompletionSequence = 0;
            _forceClientRefresh = false;
            _refreshTimer = 0f;
            _currentSnapshot = ContractBoardSnapshot.Empty;
        }

        private static string BuildSessionKey(object room)
        {
            var vroom = room as IVroom;
            if (vroom == null)
                return "";

            long roomId = RoomAPI.GetRoomID(vroom);
            int gameDay = RoomAPI.GetCurrentGameDay(vroom);
            int sessionCycle = RoomAPI.GetCurrentSessionCycle(vroom);
            int playerCount = RoomAPI.GetRoomPlayers(vroom).Count;

            if (roomId > 0)
                return $"{roomId}:{gameDay}:{sessionCycle}:{playerCount}";

            return $"{vroom.GetHashCode()}:{gameDay}:{sessionCycle}:{playerCount}";
        }
    }
}

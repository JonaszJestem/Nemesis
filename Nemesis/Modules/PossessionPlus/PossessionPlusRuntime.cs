using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Mimic.Actors;
using MimicAPI.GameAPI;
using Nemesis.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Nemesis.Modules.PossessionPlus
{
    internal sealed class PossessionPlusRuntimeState
    {
        public bool IsActive { get; set; }
        public bool IsDead { get; set; }
        public string PlayerKey { get; set; } = "";
        public string PlayerName { get; set; } = "";
        public WorldPoint LastKnownPosition { get; set; }
        public int Charges { get; set; }
        public int MaxCharges { get; set; }
        public float PulseCooldownRemaining { get; set; }
        public float GhostLifetimeRemaining { get; set; }
        public float RechargeProgressSeconds { get; set; }
        public long LastUpdatedUtcSeconds { get; set; }
        public long LastPulseUtcSeconds { get; set; }
        public string StatusLine { get; set; } = "Idle";
        public List<GhostPulseMark> RecentMarks { get; set; } = new List<GhostPulseMark>();
        public List<GhostPresenceSnapshot> RemoteGhosts { get; set; } = new List<GhostPresenceSnapshot>();

        public PossessionPlusRuntimeState Clone()
        {
            return new PossessionPlusRuntimeState
            {
                IsActive = IsActive,
                IsDead = IsDead,
                PlayerKey = PlayerKey,
                PlayerName = PlayerName,
                LastKnownPosition = LastKnownPosition,
                Charges = Charges,
                MaxCharges = MaxCharges,
                PulseCooldownRemaining = PulseCooldownRemaining,
                GhostLifetimeRemaining = GhostLifetimeRemaining,
                RechargeProgressSeconds = RechargeProgressSeconds,
                LastUpdatedUtcSeconds = LastUpdatedUtcSeconds,
                LastPulseUtcSeconds = LastPulseUtcSeconds,
                StatusLine = StatusLine,
                RecentMarks = RecentMarks.Select(CloneMark).ToList(),
                RemoteGhosts = RemoteGhosts.Select(CloneGhost).ToList()
            };
        }

        private static GhostPulseMark CloneMark(GhostPulseMark mark) => new GhostPulseMark
        {
            Label = mark.Label,
            Kind = mark.Kind,
            Position = mark.Position,
            Distance = mark.Distance,
            ExpiresUtcSeconds = mark.ExpiresUtcSeconds
        };

        private static GhostPresenceSnapshot CloneGhost(GhostPresenceSnapshot snapshot) => new GhostPresenceSnapshot
        {
            PlayerKey = snapshot.PlayerKey,
            PlayerName = snapshot.PlayerName,
            IsDead = snapshot.IsDead,
            Charges = snapshot.Charges,
            GhostExpiresUtcSeconds = snapshot.GhostExpiresUtcSeconds,
            CooldownEndsUtcSeconds = snapshot.CooldownEndsUtcSeconds,
            LastUpdatedUtcSeconds = snapshot.LastUpdatedUtcSeconds,
            LastPulseUtcSeconds = snapshot.LastPulseUtcSeconds,
            LatestMarks = snapshot.LatestMarks.Select(CloneMark).ToList()
        };
    }

    internal static class PossessionPlusRuntime
    {
        private const float RemoteRefreshSeconds = 1.25f;
        private static readonly object Sync = new object();
        private static readonly GhostRechargeProfile RechargeProfile = new GhostRechargeProfile();
        private static readonly PossessionPlusRuntimeState State = new PossessionPlusRuntimeState();

        private static PossessionPlusConfig? _config;
        private static float _remoteRefreshTimer;
        private static bool _dirtyLobbyState;

        public static PossessionPlusRuntimeState Current
        {
            get { lock (Sync) return State.Clone(); }
        }

        public static void Initialize(PossessionPlusConfig config)
        {
            lock (Sync)
            {
                _config = config;
                if (!config.Enabled) ResetUnlocked();
            }
        }

        public static void Shutdown()
        {
            lock (Sync)
            {
                PublishLobbyStateUnlocked(force: true);
                ResetUnlocked();
            }
        }

        public static void StartSession(PossessionPlusConfig config)
        {
            lock (Sync)
            {
                _config = config;
                ResetGhostUnlocked();
                State.StatusLine = "Session ready";
            }
        }

        public static void NotifyLocalDeath(object avatar)
        {
            lock (Sync)
            {
                if (_config == null || !_config.Enabled)
                    return;

                CaptureLocalIdentity(avatar);
                CapturePosition(avatar);

                State.IsActive = true;
                State.IsDead = true;
                State.MaxCharges = Math.Max(1, _config.MaxCharges);
                State.Charges = State.MaxCharges;
                State.GhostLifetimeRemaining = Math.Max(1f, _config.GhostLifetimeSeconds);
                State.PulseCooldownRemaining = 0f;
                State.RechargeProgressSeconds = 0f;
                State.LastPulseUtcSeconds = 0;
                State.StatusLine = "Ghost mode active";
                State.RecentMarks = new List<GhostPulseMark>();
                _dirtyLobbyState = true;

                if (_config.AutoPulseOnDeath)
                    TryPulseUnlocked();
            }
        }

        public static void Update(PossessionPlusConfig config, float deltaTime)
        {
            lock (Sync)
            {
                _config = config;
                if (_config == null || !_config.Enabled)
                {
                    ResetUnlocked();
                    return;
                }

                State.MaxCharges = Math.Max(1, _config.MaxCharges);
                State.LastUpdatedUtcSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                if (!State.IsActive)
                {
                    RefreshRemoteGhostsUnlocked();
                    State.StatusLine = "Waiting for ghost state";
                    return;
                }

                State.GhostLifetimeRemaining = Math.Max(0f, State.GhostLifetimeRemaining - deltaTime);
                State.PulseCooldownRemaining = Math.Max(0f, State.PulseCooldownRemaining - deltaTime);

                if (State.GhostLifetimeRemaining <= 0f)
                {
                    State.StatusLine = "Ghost time expired";
                    ResetGhostUnlocked();
                    return;
                }

                if (TryDetectRespawnUnlocked())
                {
                    State.StatusLine = "Respawn detected";
                    ResetGhostUnlocked();
                    return;
                }

                var players = PlayerAPI.GetOtherPlayers() ?? Array.Empty<ProtoActor>();
                int nearbyAllies = CountNearbyAlliesUnlocked(players);
                RechargeProfile.BaseRechargeSeconds = _config.BaseRechargeSeconds;
                RechargeProfile.AllyRechargeBonusSeconds = _config.AllyRechargeBonusSeconds;
                RechargeProfile.MinimumRechargeSeconds = _config.MinimumRechargeSeconds;

                State.RechargeProgressSeconds += deltaTime;
                float recharge = RechargeProfile.GetEffectiveRechargeSeconds(nearbyAllies);
                while (State.Charges < State.MaxCharges && State.RechargeProgressSeconds >= recharge)
                {
                    State.RechargeProgressSeconds -= recharge;
                    State.Charges++;
                    recharge = RechargeProfile.GetEffectiveRechargeSeconds(nearbyAllies);
                }

                State.StatusLine = State.Charges >= State.MaxCharges
                    ? "Ghost ready"
                    : $"Recharging with {nearbyAllies} ally bonus";

                if (Keyboard.current?.gKey.wasPressedThisFrame == true)
                    TryPulseUnlocked();

                RefreshRemoteGhostsUnlocked();
                PublishLobbyStateUnlocked(force: false);
            }
        }

        private static void TryPulseUnlocked()
        {
            if (_config == null || !_config.Enabled || !State.IsActive || !State.IsDead)
                return;
            if (State.Charges <= 0)
            {
                State.StatusLine = "No charges available";
                return;
            }
            if (State.PulseCooldownRemaining > 0f)
            {
                State.StatusLine = $"Pulse cooling down ({Mathf.CeilToInt(State.PulseCooldownRemaining)}s)";
                return;
            }

            State.RecentMarks = GhostPulsePlanner.BuildMarks(
                GatherCandidatesUnlocked(),
                Math.Max(1, _config.MaxMarksPerPulse),
                DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                Math.Max(1f, _config.MarkLifetimeSeconds));

            State.Charges = Math.Max(0, State.Charges - 1);
            State.PulseCooldownRemaining = Math.Max(0.1f, _config.PulseCooldownSeconds);
            State.LastPulseUtcSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            State.StatusLine = State.RecentMarks.Count > 0 ? $"Pulse sent with {State.RecentMarks.Count} mark(s)" : "Pulse sent";
            _dirtyLobbyState = true;
            PublishLobbyStateUnlocked(force: true);
        }

        private static IEnumerable<GhostEntitySnapshot> GatherCandidatesUnlocked()
        {
            var origin = State.LastKnownPosition;
            var candidates = new List<GhostEntitySnapshot>();

            try
            {
                var players = PlayerAPI.GetOtherPlayers();
                if (players != null)
                {
                    foreach (var player in players)
                    {
                        if (player == null || !PlayerAPI.IsPlayerValid(player))
                            continue;

                        var pos = TryReadPosition(player);
                        if (pos == null) continue;
                        float distance = origin.DistanceTo(pos.Value);
                        if (distance > _config!.PulseRadius) continue;

                        candidates.Add(new GhostEntitySnapshot
                        {
                            Label = string.IsNullOrWhiteSpace(PlayerAPI.GetPlayerName(player)) ? "Ally" : PlayerAPI.GetPlayerName(player),
                            Kind = GhostEntityKind.Ally,
                            Position = pos.Value,
                            Distance = distance,
                            Priority = 200f - distance
                        });
                    }
                }
            }
            catch { }

            try
            {
                var room = RoomAPI.GetCurrentRoom();
                if (room != null)
                {
                    var monsters = ActorAPI.GetAliveMonstersInRoom(room);
                    foreach (var monster in monsters)
                    {
                        if (monster == null) continue;
                        var pos = TryReadPosition(monster);
                        if (pos == null) continue;
                        float distance = origin.DistanceTo(pos.Value);
                        if (distance > _config!.PulseRadius) continue;

                        candidates.Add(new GhostEntitySnapshot
                        {
                            Label = monster.GetType().Name,
                            Kind = GhostEntityKind.Monster,
                            Position = pos.Value,
                            Distance = distance,
                            Priority = 160f - distance
                        });
                    }
                }
            }
            catch { }

            try
            {
                var loot = LootAPI.GetLootNearby(_config!.PulseRadius);
                if (loot != null)
                {
                    foreach (var item in loot)
                    {
                        if (item == null) continue;
                        var p = item.transform.position;
                        var pos = new WorldPoint(p.x, p.y, p.z);
                        float distance = origin.DistanceTo(pos);
                        if (distance > _config.PulseRadius) continue;

                        candidates.Add(new GhostEntitySnapshot
                        {
                            Label = item.gameObject.name,
                            Kind = GhostEntityKind.Loot,
                            Position = pos,
                            Distance = distance,
                            Priority = 90f - distance
                        });
                    }
                }
            }
            catch { }

            return candidates;
        }

        private static void RefreshRemoteGhostsUnlocked()
        {
            if (_config == null || !_config.ShareWithLobby)
            {
                State.RemoteGhosts = new List<GhostPresenceSnapshot>();
                return;
            }

            _remoteRefreshTimer += Time.deltaTime;
            if (_remoteRefreshTimer < RemoteRefreshSeconds && State.RemoteGhosts.Count > 0)
                return;

            _remoteRefreshTimer = 0f;
            try
            {
                State.RemoteGhosts = PossessionPlusLobbyStore.ReadVisibleGhosts(
                    DateTimeOffset.UtcNow.ToUnixTimeSeconds(), State.PlayerKey);
            }
            catch
            {
                State.RemoteGhosts = new List<GhostPresenceSnapshot>();
            }
        }

        private static void PublishLobbyStateUnlocked(bool force)
        {
            if (_config == null || !_config.ShareWithLobby)
                return;
            if (!force && !_dirtyLobbyState)
                return;

            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var snapshot = new GhostPresenceSnapshot
            {
                PlayerKey = State.PlayerKey,
                PlayerName = State.PlayerName,
                IsDead = State.IsDead,
                Charges = State.Charges,
                GhostExpiresUtcSeconds = now + Math.Max(1, (int)Math.Ceiling(State.GhostLifetimeRemaining)),
                CooldownEndsUtcSeconds = now + Math.Max(0, (int)Math.Ceiling(State.PulseCooldownRemaining)),
                LastUpdatedUtcSeconds = now,
                LastPulseUtcSeconds = State.LastPulseUtcSeconds,
                LatestMarks = State.RecentMarks.Select(CloneMark).ToList()
            };

            if (PossessionPlusLobbyStore.Publish(snapshot))
                _dirtyLobbyState = false;
        }

        private static void CaptureLocalIdentity(object avatar)
        {
            try
            {
                var localPlayer = PlayerAPI.GetLocalPlayer();
                if (localPlayer != null)
                {
                    State.PlayerKey = localPlayer.ActorID.ToString(CultureInfo.InvariantCulture);
                    State.PlayerName = PlayerAPI.GetPlayerName(localPlayer);
                    return;
                }
            }
            catch { }

            var actorId = ReflectionHelper.GetPropertyValue(avatar, "ActorID");
            if (actorId != null)
                State.PlayerKey = actorId.ToString() ?? "";

            if (string.IsNullOrWhiteSpace(State.PlayerKey))
                State.PlayerKey = "local";

            if (string.IsNullOrWhiteSpace(State.PlayerName))
                State.PlayerName = "Ghost";
        }

        private static void CapturePosition(object avatar)
        {
            try
            {
                var pos = PlayerAPI.GetLocalPlayerPosition();
                if (pos != Vector3.zero || PlayerAPI.GetLocalPlayer() != null)
                {
                    State.LastKnownPosition = new WorldPoint(pos.x, pos.y, pos.z);
                    return;
                }
            }
            catch { }

            var fromAvatar = TryReadPosition(avatar);
            if (fromAvatar != null)
                State.LastKnownPosition = fromAvatar.Value;
        }

        private static bool TryDetectRespawnUnlocked()
        {
            try
            {
                var localPlayer = PlayerAPI.GetLocalPlayer();
                if (localPlayer == null)
                    return false;

                bool? alive = TryReadBoolean(localPlayer, "IsAlive", "Alive");
                if (alive.HasValue) return alive.Value;

                bool? dead = TryReadBoolean(localPlayer, "IsDead", "Dead", "IsDowned");
                if (dead.HasValue) return !dead.Value;
            }
            catch { }

            return false;
        }

        private static int CountNearbyAlliesUnlocked(IEnumerable<ProtoActor> players)
        {
            int count = 0;
            foreach (var player in players)
            {
                if (player == null || !PlayerAPI.IsPlayerValid(player))
                    continue;

                var pos = TryReadPosition(player);
                if (pos == null)
                    continue;

                if (State.LastKnownPosition.DistanceTo(pos.Value) <= _config!.AllySupportRadius)
                    count++;
            }
            return count;
        }

        private static WorldPoint? TryReadPosition(object target)
        {
            var pos = ReflectionHelper.GetPropertyValue(target, GamePropertyNames.VActor_PositionVector);
            if (pos is Vector3 vector)
                return new WorldPoint(vector.x, vector.y, vector.z);

            var transform = ReflectionHelper.GetPropertyValue(target, "transform");
            if (transform is Transform unityTransform)
            {
                var v = unityTransform.position;
                return new WorldPoint(v.x, v.y, v.z);
            }

            return null;
        }

        private static bool? TryReadBoolean(object target, params string[] names)
        {
            foreach (var name in names)
            {
                var value = ReflectionHelper.GetPropertyValue(target, name) ?? ReflectionHelper.GetFieldValue(target, name);
                if (value is bool flag)
                    return flag;
            }
            return null;
        }

        private static GhostPulseMark CloneMark(GhostPulseMark mark) => new GhostPulseMark
        {
            Label = mark.Label,
            Kind = mark.Kind,
            Position = mark.Position,
            Distance = mark.Distance,
            ExpiresUtcSeconds = mark.ExpiresUtcSeconds
        };

        private static void ResetGhostUnlocked()
        {
            State.IsActive = false;
            State.IsDead = false;
            State.Charges = 0;
            State.MaxCharges = Math.Max(1, _config?.MaxCharges ?? 1);
            State.PulseCooldownRemaining = 0f;
            State.GhostLifetimeRemaining = 0f;
            State.RechargeProgressSeconds = 0f;
            State.RecentMarks = new List<GhostPulseMark>();
            _remoteRefreshTimer = 0f;
            _dirtyLobbyState = true;
            State.StatusLine = "Idle";
            State.RemoteGhosts = new List<GhostPresenceSnapshot>();
        }

        private static void ResetUnlocked()
        {
            ResetGhostUnlocked();
            State.PlayerKey = "";
            State.PlayerName = "";
            State.LastKnownPosition = default;
            State.LastUpdatedUtcSeconds = 0;
            State.LastPulseUtcSeconds = 0;
        }
    }
}

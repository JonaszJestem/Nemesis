using System;
using System.Collections.Generic;
using System.Linq;
using MimicAPI.GameAPI;
using Nemesis.Core;
using Mimic.Actors;

namespace Nemesis.Modules.RoleSystem
{
    internal class RoleAssigner
    {
        private readonly Dictionary<uint, Role> _assignments = new Dictionary<uint, Role>();
        private readonly Dictionary<uint, CachedStats> _originalStats = new Dictionary<uint, CachedStats>();
        private readonly RoleConfig _config;
        private static readonly Random _rng = new Random();

        public RoleAssigner(RoleConfig config)
        {
            _config = config;
        }

        public Role GetRole(uint actorId)
        {
            return _assignments.TryGetValue(actorId, out var role) ? role : Role.None;
        }

        public Role GetLocalRole()
        {
            var player = PlayerAPI.GetLocalPlayer();
            return player != null ? GetRole(player.ActorID) : Role.None;
        }

        public void AssignRoles()
        {
            RestoreAllStats();
            _assignments.Clear();

            var players = PlayerAPI.GetAllPlayers();
            if (players == null || players.Length == 0) return;

            var roles = new[] { Role.Scout, Role.Tank, Role.Medic, Role.Scavenger };
            var shuffled = roles.OrderBy(_ => _rng.Next()).ToArray();

            for (int i = 0; i < players.Length; i++)
            {
                var player = players[i];
                if (player == null) continue;

                var role = shuffled[i % shuffled.Length];
                _assignments[player.ActorID] = role;

                // Only modify local player stats — cannot modify remote clients
                if (player.AmIAvatar())
                {
                    ApplyStatModifiers(player, role);
                }
            }
        }

        private void ApplyStatModifiers(ProtoActor player, Role role)
        {
            try
            {
                var statManager = PlayerAPI.GetStatManager(player);
                if (statManager == null) return;

                var baseStats = CachedStats.Capture(statManager);
                _originalStats[player.ActorID] = baseStats;

                var delta = RoleStatDelta.ForRole(role, _config);
                baseStats.ApplyMultipliers(statManager, delta.MaxHpMultiplier, delta.SpeedMultiplier);
            }
            catch { }
        }

        public void RestoreAllStats()
        {
            foreach (var kvp in _originalStats)
            {
                try
                {
                    var player = PlayerAPI.GetPlayerByID(kvp.Key);
                    if (player == null) continue;

                    var statManager = PlayerAPI.GetStatManager(player);
                    if (statManager == null) continue;

                    kvp.Value.Restore(statManager);
                }
                catch { }
            }
            _originalStats.Clear();
        }
    }
}

namespace Nemesis.Modules.RoleSystem
{
    internal enum Role
    {
        None,
        Scout,
        Tank,
        Medic,
        Scavenger
    }

    internal struct RoleStatDelta
    {
        public float SpeedMultiplier;
        public float MaxHpMultiplier;
        public string DisplayName;
        public string Description;

        public static RoleStatDelta ForRole(Role role, RoleConfig config)
        {
            switch (role)
            {
                case Role.Scout:
                    return new RoleStatDelta
                    {
                        SpeedMultiplier = config.ScoutSpeedMultiplier,
                        MaxHpMultiplier = 1.0f,
                        DisplayName = "Scout",
                        Description = "Faster movement speed"
                    };
                case Role.Tank:
                    return new RoleStatDelta
                    {
                        SpeedMultiplier = config.TankSpeedPenalty,
                        MaxHpMultiplier = config.TankHpMultiplier,
                        DisplayName = "Tank",
                        Description = "Increased health"
                    };
                case Role.Medic:
                    return new RoleStatDelta
                    {
                        SpeedMultiplier = 1.0f,
                        MaxHpMultiplier = 1.0f,
                        DisplayName = "Medic",
                        Description = "Faster interactions"
                    };
                case Role.Scavenger:
                    return new RoleStatDelta
                    {
                        SpeedMultiplier = 1.0f,
                        MaxHpMultiplier = 1.0f,
                        DisplayName = "Scavenger",
                        Description = "Better loot detection range"
                    };
                default:
                    return new RoleStatDelta
                    {
                        SpeedMultiplier = 1.0f,
                        MaxHpMultiplier = 1.0f,
                        DisplayName = "None",
                        Description = ""
                    };
            }
        }
    }
}

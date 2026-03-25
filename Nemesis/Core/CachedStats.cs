namespace Nemesis.Core
{
    internal struct CachedStats
    {
        public float? MoveSpeed;
        public float? MaxHpFloat;
        public int? MaxHpInt;
        public bool IsHpFloat;

        public static CachedStats Capture(object statManager)
        {
            var stats = new CachedStats();

            var hpVal = MimicAPI.GameAPI.ReflectionHelper.GetFieldValue(statManager, GameFieldNames.StatManager_MaxHp);
            if (hpVal is float fHp)
            {
                stats.MaxHpFloat = fHp;
                stats.IsHpFloat = true;
            }
            else if (hpVal is int iHp)
            {
                stats.MaxHpInt = iHp;
                stats.IsHpFloat = false;
            }

            var speedVal = MimicAPI.GameAPI.ReflectionHelper.GetFieldValue(statManager, GameFieldNames.StatManager_MoveSpeed);
            if (speedVal is float spd)
                stats.MoveSpeed = spd;

            return stats;
        }

        public void Restore(object statManager)
        {
            if (IsHpFloat && MaxHpFloat.HasValue)
                MimicAPI.GameAPI.ReflectionHelper.SetFieldValue(statManager, GameFieldNames.StatManager_MaxHp, MaxHpFloat.Value);
            else if (!IsHpFloat && MaxHpInt.HasValue)
                MimicAPI.GameAPI.ReflectionHelper.SetFieldValue(statManager, GameFieldNames.StatManager_MaxHp, MaxHpInt.Value);

            if (MoveSpeed.HasValue)
                MimicAPI.GameAPI.ReflectionHelper.SetFieldValue(statManager, GameFieldNames.StatManager_MoveSpeed, MoveSpeed.Value);
        }

        public void ApplyMultipliers(object statManager, float hpMult, float speedMult)
        {
            if (IsHpFloat && MaxHpFloat.HasValue)
                MimicAPI.GameAPI.ReflectionHelper.SetFieldValue(statManager, GameFieldNames.StatManager_MaxHp, MaxHpFloat.Value * hpMult);
            else if (!IsHpFloat && MaxHpInt.HasValue)
                MimicAPI.GameAPI.ReflectionHelper.SetFieldValue(statManager, GameFieldNames.StatManager_MaxHp, (int)(MaxHpInt.Value * hpMult));

            if (MoveSpeed.HasValue)
                MimicAPI.GameAPI.ReflectionHelper.SetFieldValue(statManager, GameFieldNames.StatManager_MoveSpeed, MoveSpeed.Value * speedMult);
        }
    }
}

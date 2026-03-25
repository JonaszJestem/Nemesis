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

            var hpVal = MimicAPI.GameAPI.ReflectionHelper.GetFieldValue(statManager, "_maxHp");
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

            var speedVal = MimicAPI.GameAPI.ReflectionHelper.GetFieldValue(statManager, "_moveSpeed");
            if (speedVal is float spd)
                stats.MoveSpeed = spd;

            return stats;
        }

        public void Restore(object statManager)
        {
            if (IsHpFloat && MaxHpFloat.HasValue)
                MimicAPI.GameAPI.ReflectionHelper.SetFieldValue(statManager, "_maxHp", MaxHpFloat.Value);
            else if (!IsHpFloat && MaxHpInt.HasValue)
                MimicAPI.GameAPI.ReflectionHelper.SetFieldValue(statManager, "_maxHp", MaxHpInt.Value);

            if (MoveSpeed.HasValue)
                MimicAPI.GameAPI.ReflectionHelper.SetFieldValue(statManager, "_moveSpeed", MoveSpeed.Value);
        }

        public void ApplyMultipliers(object statManager, float hpMult, float speedMult)
        {
            if (IsHpFloat && MaxHpFloat.HasValue)
                MimicAPI.GameAPI.ReflectionHelper.SetFieldValue(statManager, "_maxHp", MaxHpFloat.Value * hpMult);
            else if (!IsHpFloat && MaxHpInt.HasValue)
                MimicAPI.GameAPI.ReflectionHelper.SetFieldValue(statManager, "_maxHp", (int)(MaxHpInt.Value * hpMult));

            if (MoveSpeed.HasValue)
                MimicAPI.GameAPI.ReflectionHelper.SetFieldValue(statManager, "_moveSpeed", MoveSpeed.Value * speedMult);
        }
    }
}

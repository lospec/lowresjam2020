using System;

namespace HeroesGuild.Utility
{
    public static class WeaponUtil
    {
        public enum DamageType
        {
            None,
            Pierce,
            Blunt,
            Fire,
            Water,
            Electricity,
        }

        public static string GetDamageTypeName(DamageType type)
        {
            return Enum.GetName(typeof(DamageType), type);
        }
    }
}
using System;

namespace HeroesGuild.utility
{
    public static class WeaponUtil
    {
        public static string GetDamageTypeName(DamageType type)
        {
            return Enum.GetName(typeof(DamageType), type);
        }
    }
}
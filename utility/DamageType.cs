using System.Runtime.Serialization;

namespace HeroesGuild.utility
{
    public enum DamageType
    {
        [EnumMember(Value = "none")]
        None,
        Pierce,
        Blunt,
        Fire,
        Water,
        Electricity
    }
}
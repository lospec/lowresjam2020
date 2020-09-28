using HeroesGuild.utility;

namespace HeroesGuild.data
{
    public interface ICombatStat
    {
        int QuickDamage { get; }
        DamageType QuickDamageType { get; }
        string QuickStatusEffect { get; }
        float QuickEffectChance { get; }
        int HeavyDamage { get; }
        DamageType HeavyDamageType { get; }
        string HeavyStatusEffect { get; }
        float HeavyEffectChance { get; }
        int CounterDamage { get; }
        DamageType CounterDamageType { get; }
        string CounterStatusEffect { get; }
        float CounterEffectChance { get; }
    }
}
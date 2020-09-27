namespace HeroesGuild.data
{
    public interface ICombatStat
    {
        int QuickDamage { get; }
        string QuickDamageType { get; }
        string QuickStatusEffect { get; }
        float QuickEffectChance { get; }
        int HeavyDamage { get; }
        string HeavyDamageType { get; }
        string HeavyStatusEffect { get; }
        float HeavyEffectChance { get; }
        int CounterDamage { get; }
        string CounterDamageType { get; }
        string CounterStatusEffect { get; }
        float CounterEffectChance { get; }
    }
}
using HeroesGuild.Combat;

namespace HeroesGuild.StatusEffect.Effects
{
    public class Poison : Bases.StatusEffect
    {
        public int Duration { get; private set; } = 3;
        public int Damage { get; private set; } = 1;

        public Poison()
        {
            statusEffectName = "Poison";
        }

        public override void OnTurnEnd(CombatChar combatChar)
        {
            combatChar.TakeDamage(Damage, "None");
            Duration -= 1;
            if (Duration <= 0)
            {
                expired = true;
            }
        }
    }
}
using HeroesGuild.Combat;

namespace HeroesGuild.StatusEffect.Effects
{
    public class Confused : Bases.StatusEffect
    {
        public int Duration { get; private set; } = 2;

        public Confused()
        {
            statusEffectName = "Confused";
        }

        public override void OnTurnEnd(CombatChar combatChar)
        {
            Duration -= 1;
            if (Duration <= 0)
            {
                expired = true;
            }
        }
    }
}
using HeroesGuild.Combat;

namespace HeroesGuild.StatusEffects.Effects
{
    public class Confused : StatusEffect
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
using HeroesGuild.Combat;

namespace HeroesGuild.StatusEffects.Effects
{
    public class Weak : StatusEffect
    {
        public int Duration { get; private set; } = 2;

        public Weak()
        {
            statusEffectName = "Weak";
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
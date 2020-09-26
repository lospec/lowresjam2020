using HeroesGuild.combat;

namespace HeroesGuild.status_effects.effects
{
    public class Weak : StatusEffect
    {
        public Weak()
        {
            statusEffectName = "Weak";
        }

        public int Duration { get; private set; } = 2;

        public override void OnTurnEnd(CombatChar combatChar)
        {
            Duration -= 1;
            if (Duration <= 0) expired = true;
        }
    }
}
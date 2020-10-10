using HeroesGuild.combat;

namespace HeroesGuild.status_effects.effects
{
    public class Asleep : StatusEffect
    {
        public Asleep()
        {
            statusEffectName = "Asleep";
        }

        public int Duration { get; private set; } = 2;

        public override void OnTurnEnd(CombatChar combatChar)
        {
            Duration -= 1;
            if (Duration <= 0) expired = true;
        }
    }
}
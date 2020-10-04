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

        public override void OnTurnEnd(CombatController combatController)
        {
            Duration -= 1;
            if (Duration <= 0) expired = true;
        }
    }
}
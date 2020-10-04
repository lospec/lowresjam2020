using HeroesGuild.combat;

namespace HeroesGuild.status_effects.effects
{
    public class Confused : StatusEffect
    {
        public Confused()
        {
            statusEffectName = "Confused";
        }

        public int Duration { get; private set; } = 2;

        public override void OnTurnEnd(CombatController combatController)
        {
            Duration -= 1;
            if (Duration <= 0) expired = true;
        }
    }
}
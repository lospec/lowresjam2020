using HeroesGuild.combat;

namespace HeroesGuild.status_effects.effects
{
    public class Frozen : StatusEffect
    {
        public Frozen()
        {
            statusEffectName = "Frozen";
        }

        public int Duration { get; private set; } = 4;

        public override void OnTurnEnd(CombatController combatController)
        {
            Duration -= 1;
            if (Duration <= 0) expired = true;
        }
    }
}
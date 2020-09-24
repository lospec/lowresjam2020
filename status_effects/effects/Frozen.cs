using HeroesGuild.Combat;

namespace HeroesGuild.StatusEffects.Effects
{
    public class Frozen : StatusEffect
    {
        public int Duration { get; private set; } = 4;

        public Frozen()
        {
            statusEffectName = "Frozen";
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
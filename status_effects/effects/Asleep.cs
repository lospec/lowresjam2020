using HeroesGuild.Combat;

namespace HeroesGuild.StatusEffect.Effects
{
    public class Asleep : StatusEffect
    {
        public int Duration { get; private set; } = 2;

        public Asleep()
        {
            statusEffectName = "Asleep";
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
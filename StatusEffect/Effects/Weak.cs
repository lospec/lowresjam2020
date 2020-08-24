using HeroesGuild.Combat;

namespace HeroesGuild.StatusEffect.Effects
{
    public class Weak : Bases.StatusEffect
    {
        public int Duration { get; private set; } = 2;

        public Weak()
        {
            StatusEffectName = "Weak";
        }

        public override void OnTurnEnd(CombatChar combatChar)
        {
            Duration -= 1;
            if (Duration <= 0)
            {
                Expired = true;
            }
        }
    }
}
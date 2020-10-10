using HeroesGuild.combat;

namespace HeroesGuild.status_effects.effects
{
    public class Poison : StatusEffect
    {
        public Poison()
        {
            statusEffectName = "Poison";
        }

        public int Duration { get; private set; } = 3;
        public int Damage { get; } = 1;

        public override void OnTurnEnd(CombatChar combatChar)
        {
            combatChar.TakeDamage(Damage, "None");
            Duration -= 1;
            if (Duration <= 0) expired = true;
        }
    }
}
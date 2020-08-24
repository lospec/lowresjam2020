using Godot;
using HeroesGuild.Combat;

namespace HeroesGuild.StatusEffect.Effects
{
    public class Asleep : Bases.StatusEffect
    {
        public int Duration { get; private set; } = 2;

        public Asleep()
        {
            StatusEffectName = "Asleep";
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
using Godot;
using HeroesGuild.combat;
using HeroesGuild.utility;

namespace HeroesGuild.status_effects.effects
{
    public class OnFire : StatusEffect
    {
        private int _nextDamage = 0;

        public OnFire()
        {
            statusEffectName = "OnFire";
        }

        public int Duration { get; private set; } = 4;
        public int Delay { get; } = 2;
        public int Damage { get; } = 4;

        public override void OnTurnEnd(CombatChar combatChar)
        {
            _nextDamage -= 1;
            GD.Print($"NEXT FIRE DAMAGE : {_nextDamage}");

            if (_nextDamage <= 0)
            {
                GD.Print($"APPLY FIRE DAMAGE TO: {combatChar.Name}");
                combatChar.TakeDamage(Damage, DamageType.Fire);
                _nextDamage = Delay + 1;
            }

            Duration -= 1;
            if (Duration <= 0)
            {
                expired = true;
            }
        }
    }
}
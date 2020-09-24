using Godot;
using HeroesGuild.Combat;

namespace HeroesGuild.StatusEffect.Effects
{
    public class OnFire : Bases
    {
        public int Duration { get; private set; } = 4;
        public int Delay { get; private set; } = 2;
        public int Damage { get; private set; } = 4;

        private int _nextDamage = 0;

        public OnFire()
        {
            statusEffectName = "OnFire";
        }

        public override void OnTurnEnd(CombatChar combatChar)
        {
            _nextDamage -= 1;
            GD.Print($"NEXT FIRE DAMAGE : {_nextDamage}");

            if (_nextDamage <= 0)
            {
                GD.Print($"APPLY FIRE DAMAGE TO: {combatChar.Name}");
                combatChar.TakeDamage(Damage, "Fire");
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
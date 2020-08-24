using Godot;
using HeroesGuild.Combat;

namespace HeroesGuild.StatusEffect.Bases
{
    public abstract class StatusEffect : Resource
    {
        public string StatusEffectName = "none";
        public bool Expired = false;
        public bool Initialized = false;

        public abstract void OnTurnEnd(CombatChar combatChar);
    }
}
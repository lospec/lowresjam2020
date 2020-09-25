using Godot;
using HeroesGuild.Combat;

namespace HeroesGuild.StatusEffects
{
    public abstract class StatusEffect : Resource
    {
        public string statusEffectName = "none";
        public bool expired = false;
        public bool initialized = false;

        public abstract void OnTurnEnd(CombatChar combatChar);
    }
}
using Godot;
using HeroesGuild.combat;

namespace HeroesGuild.status_effects
{
    public abstract class StatusEffect : Resource
    {
        public bool expired = false;
        public bool initialized = false;
        public string statusEffectName = "none";

        public abstract void OnTurnEnd(CombatChar combatChar);
    }
}
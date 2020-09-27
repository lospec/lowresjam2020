using Godot;
using HeroesGuild.entities.base_entity;

namespace HeroesGuild.combat.combat_actions
{
    public abstract class BaseCombatAction
    {
        public abstract string ActionName { get; }
        public abstract Color ActionColor { get; }
        protected BaseEntity CharacterInstance { get; }

        protected BaseCombatAction(BaseEntity characterInstance)
        {
            CharacterInstance = characterInstance;
        }
    }
}
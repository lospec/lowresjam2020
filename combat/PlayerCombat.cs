using System;
using System.Linq;
using System.Threading.Tasks;
using HeroesGuild.combat.combat_actions;
using HeroesGuild.entities.base_entity;
using HeroesGuild.entities.player;
using HeroesGuild.utility;

namespace HeroesGuild.combat
{
    public class PlayerCombat : CombatController, IDependency<CombatMenu>
    {
        private new Player CharacterInstance => (Player) base.CharacterInstance;

        public void OnDependency(CombatMenu dependency)
        {
            dependency.OnActionSelected += OnCombatMenu_ActionSelected;
        }

        public override int GetBaseDamage(CombatAction action)
        {
            var damage = action.BaseDamage;
            damage *= 1 + MULTIPLIER_PER_COMBO * hitCombo;
            return damage / 2;
        }

        private TaskCompletionSource<BaseCombatAction> CompletionSource { get; set; }

        public override async Task<BaseCombatAction> GetAction()
        {
            if (new[] {"Confused", "Asleep", "Frozen"}
                .Any(key => CharacterInstance.statusEffects.ContainsKey(key)))
                return null;
            CompletionSource = new TaskCompletionSource<BaseCombatAction>();
            var action = await CompletionSource.Task;
            CompletionSource = null;
            return action;
        }


        private void OnCombatMenu_ActionSelected(Func<BaseEntity, BaseCombatAction> action)
        {
            CompletionSource?.SetResult(action(CharacterInstance));
        }
    }
}
using System.Linq;
using System.Threading.Tasks;
using Godot;
using HeroesGuild.combat.combat_actions;
using HeroesGuild.entities.enemies.base_enemy;

namespace HeroesGuild.combat
{
    public class EnemyCombat : CombatChar
    {
        private int _poolIndex = 0;
        private new BaseEnemy CharacterInstance => (BaseEnemy) base.CharacterInstance;

        private CombatAction PoolCharToAction(char combatActionChar)
        {
            return combatActionChar switch
            {
                'q' => new QuickAction(CharacterInstance),
                'c' => new CounterAction(CharacterInstance),
                'h' => new HeavyAction(CharacterInstance),
                _ => null
            };
        }

        public override int GetBaseDamage(CombatAction action)
        {
            var damage = action.BaseDamage;
            damage *= 1 + MULTIPLIER_PER_COMBO * hitCombo;
            return damage;
        }


        public override async Task<BaseCombatAction> GetAction()
        {
            await Task.Delay(0);
            if (new[] {"Confused", "Asleep", "Frozen"}
                .Any(key => CharacterInstance.statusEffects.ContainsKey(key)))
                return null;

            if (!string.IsNullOrWhiteSpace(CharacterInstance.Stat.AttackPool))
            {
                GD.Print(CharacterInstance.Stat.AttackPool);
                var action =
                    PoolCharToAction(
                        CharacterInstance.Stat.AttackPool.ToLower()[_poolIndex]);
                if (action != null)
                {
                    _poolIndex = (_poolIndex + 1) %
                                 CharacterInstance.Stat.AttackPool.Length;
                    return action;
                }
            }

            return CombatAction.GetRandom(CharacterInstance);
        }
    }
}
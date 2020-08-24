using System;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using HeroesGuild.Entities.enemies.base_enemy;

namespace HeroesGuild.Combat
{
    public class EnemyCombat : CombatChar
    {
        private new BaseEnemy CharacterInstance => (BaseEnemy) base.CharacterInstance;
        public int poolIndex = 0;

        private CombatUtil.CombatAction PoolCharToAction(char combatActionChar)
            => combatActionChar switch
            {
                'q' => CombatUtil.CombatAction.Quick,
                'c' => CombatUtil.CombatAction.Counter,
                'h' => CombatUtil.CombatAction.Heavy,
                _ => CombatUtil.CombatAction.Invalid
            };

        public override int GetBaseDamage(CombatUtil.CombatAction action)
        {
            var damage = action switch
            {
                CombatUtil.CombatAction.Quick => CharacterInstance.Stat.QuickDamage,
                CombatUtil.CombatAction.Counter => CharacterInstance.Stat.CounterDamage,
                CombatUtil.CombatAction.Heavy => CharacterInstance.Stat.HeavyDamage,
                _ => throw new ArgumentOutOfRangeException()
            };
            damage *= (1 + CombatUtil.MULTIPLIER_PER_COMBO * HitCombo);
            return damage;
        }

        protected override float GetEffectChance(CombatUtil.CombatAction action) => action switch
        {
            CombatUtil.CombatAction.Quick => CharacterInstance.Stat.QuickEffectChance,
            CombatUtil.CombatAction.Counter => CharacterInstance.Stat.CounterEffectChance,
            CombatUtil.CombatAction.Heavy => CharacterInstance.Stat.HeavyEffectChance,
            _ => 0f
        };

        protected override string GetStatusEffect(CombatUtil.CombatAction action) => action switch
        {
            CombatUtil.CombatAction.Quick => CharacterInstance.Stat.QuickStatusEffect,
            CombatUtil.CombatAction.Counter => CharacterInstance.Stat.CounterStatusEffect,
            CombatUtil.CombatAction.Heavy => CharacterInstance.Stat.HeavyStatusEffect,
            _ => "none"
        };

        public override string GetDamageType(CombatUtil.CombatAction action)
        {
            switch (action)
            {
                case CombatUtil.CombatAction.Quick:
                    GD.Print(CharacterInstance.EnemyName);
                    GD.Print(CharacterInstance.Stat.QuickDamageType);
                    return CharacterInstance.Stat.QuickDamageType;
                case CombatUtil.CombatAction.Counter:
                    GD.Print(CharacterInstance.EnemyName);
                    GD.Print(CharacterInstance.Stat.CounterDamageType);
                    return CharacterInstance.Stat.CounterDamageType;
                case CombatUtil.CombatAction.Heavy:
                    GD.Print(CharacterInstance.EnemyName);
                    GD.Print(CharacterInstance.Stat.HeavyDamageType);
                    return CharacterInstance.Stat.HeavyDamageType;
                default:
                    return "None";
            }
        }

        protected override async Task<CombatUtil.CombatAction> GetAction()
        {
            if (new[] {"Confused", "Asleep", "Frozen"}
                .Any(key => CharacterInstance.StatusEffects.ContainsKey(key)))
            {
                return CombatUtil.CombatAction.None;
            }

            if (!string.IsNullOrWhiteSpace(CharacterInstance.Stat.AttackPool))
            {
                GD.Print(CharacterInstance.Stat.AttackPool);
                var action = PoolCharToAction(CharacterInstance.Stat.AttackPool.ToLower()[poolIndex]);
                if (action != CombatUtil.CombatAction.Invalid)
                {
                    poolIndex = (poolIndex + 1) % CharacterInstance.Stat.AttackPool.Length;
                    return action;
                }
            }

            return (CombatUtil.CombatAction) (GD.Randi() % 3 + 1);
        }
    }
}
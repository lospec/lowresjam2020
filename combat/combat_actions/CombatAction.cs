using System;
using Godot;
using HeroesGuild.data;
using HeroesGuild.entities.base_entity;
using HeroesGuild.entities.enemies.base_enemy;
using HeroesGuild.entities.player;
using HeroesGuild.utility;

namespace HeroesGuild.combat.combat_actions
{
    public abstract class CombatAction : BaseCombatAction, IComparable<CombatAction>,
        IEquatable<CombatAction>
    {
        private static Data Data => Autoload.Get<Data>();

        protected ICombatStat InstanceStat => CharacterInstance switch
        {
            Player player => Data.itemData[player.EquippedWeapon],
            BaseEnemy enemy => enemy.Stat,
            _ => throw new Exception(
                $"{GetType()} :: Character instance is not player or enemy")
        };


        private static readonly Func<BaseEntity, CombatAction>[] Actions =
        {
            entity => new CounterAction(entity),
            entity => new QuickAction(entity),
            entity => new HeavyAction(entity),
        };

        public static CombatAction GetRandom(BaseEntity characterInstance) =>
            Actions[GD.Randi() % Actions.Length + 1].Invoke(characterInstance);

        public abstract int BaseDamage { get; }
        public abstract float EffectChance { get; }
        public abstract string StatusEffect { get; }
        public abstract string DamageType { get; }

        public virtual Color SecondaryColor => ActionColor;
        protected abstract bool IsWeakness(CombatAction action);

        public int CompareTo(CombatAction other)
        {
            if (other.IsWeakness(this))
            {
                return 1;
            }

            if (IsWeakness(other))
            {
                return -1;
            }

            return 0;
        }

        public static bool operator <(CombatAction x, CombatAction y) =>
            x.CompareTo(y) < 0;

        public static bool operator >(CombatAction x, CombatAction y) =>
            x.CompareTo(y) > 0;

        public bool Equals(CombatAction other)
        {
            return CompareTo(other) == 0;
        }

        public static TurnOutcome CompareActions(CombatAction playerAction,
            CombatAction enemyAction)
        {
            if (playerAction.Equals(enemyAction))
                return TurnOutcome.Tie;

            if (playerAction > enemyAction)
                return TurnOutcome.PlayerWin;

            if (playerAction < enemyAction)
                return TurnOutcome.EnemyWin;

            throw new Exception("failed compare actions");
        }

        protected CombatAction(BaseEntity characterInstance) : base(characterInstance)
        {
        }
    }
}
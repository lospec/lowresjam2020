using Godot;
using HeroesGuild.entities.base_entity;

namespace HeroesGuild.combat.combat_actions
{
    public class CounterAction : CombatAction
    {
        private const string Name = "Counter";

        public CounterAction(BaseEntity characterInstance) : base(characterInstance)
        {
        }

        public override int BaseDamage => InstanceStat.CounterDamage;

        public override float EffectChance => InstanceStat.CounterEffectChance;

        public override string StatusEffect => InstanceStat.CounterStatusEffect;

        public override string DamageType => InstanceStat.CounterDamageType;
        public override string ActionName => Name;
        public override Color ActionColor => ColorValues.AttackCounter;
        public override Color SecondaryColor => ColorValues.AttackHeavy;

        protected override bool IsWeakness(CombatAction action) =>
            action is HeavyAction;
    }
}
using Godot;
using HeroesGuild.entities.base_entity;
using HeroesGuild.utility;

namespace HeroesGuild.combat.combat_actions
{
    public class QuickAction : CombatAction
    {
        private const string Name = "Quick";

        public QuickAction(BaseEntity characterInstance) : base(characterInstance)
        {
        }

        public override int BaseDamage => InstanceStat.QuickDamage;

        public override float EffectChance => InstanceStat.QuickEffectChance;

        public override string StatusEffect => InstanceStat.QuickStatusEffect;

        public override DamageType DamageType => InstanceStat.QuickDamageType;
        public override string ActionName => Name;
        public override Color ActionColor => ColorValues.AttackQuick;

        protected override bool IsWeakness(CombatAction action) =>
            action is CounterAction;
    }
}
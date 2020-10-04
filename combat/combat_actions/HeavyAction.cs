using Godot;
using HeroesGuild.entities.base_entity;
using HeroesGuild.utility;

namespace HeroesGuild.combat.combat_actions
{
    public class HeavyAction : CombatAction
    {
        private const string Name = "Heavy";

        public HeavyAction(BaseEntity characterInstance) : base(characterInstance)
        {
        }

        public override int BaseDamage => InstanceStat.HeavyDamage;

        public override float EffectChance => InstanceStat.HeavyEffectChance;

        public override string StatusEffect => InstanceStat.HeavyStatusEffect;

        public override DamageType DamageType => InstanceStat.HeavyDamageType;
        public override string ActionName => Name;
        public override Color ActionColor => ColorValues.AttackHeavy;

        protected override bool IsWeakness(CombatAction action) =>
            action is QuickAction;
    }
}
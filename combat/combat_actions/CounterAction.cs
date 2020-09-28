using System.Threading.Tasks;
using Godot;
using HeroesGuild.entities.base_entity;
using HeroesGuild.entities.enemies.base_enemy;
using HeroesGuild.entities.player;
using HeroesGuild.utility;

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

        public override DamageType DamageType => InstanceStat.CounterDamageType;
        public override string ActionName => Name;
        public override Color ActionColor => ColorValues.AttackCounter;
        public override Color SecondaryColor => ColorValues.AttackHeavy;
        protected override AnimatedTexture Animation =>
            AnimationProvider.GetCounterAnimation();

        public override Task QueuedAnimation(CombatAttackAnim.OnPlay play)
        {
            return base.QueuedAnimation(play)
                .ContinueWith(async delegate
                {
                    switch (CharacterInstance)
                    {
                        case Player _:
                            await play(AnimationProvider.GetAnimation(DamageType),
                                SecondaryColor);
                            break;
                        case BaseEnemy _:
                            await play(Animation, ActionColor);
                            break;
                    }
                });
        }

        protected override bool IsWeakness(CombatAction action) =>
            action is HeavyAction;
    }
}
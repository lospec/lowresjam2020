using System;
using System.Threading.Tasks;
using Godot;
using Godot.Collections;
using HeroesGuild.entities.base_entity;

namespace HeroesGuild.combat.combat_actions
{
    public class FleeAction : BaseCombatAction
    {
        private enum FleeOutcome
        {
            Success,
            SuccessDmg,
            Fail
        }

        private struct FleeRule
        {
            private float SuccessChance { get; }
            private float SuccessWithDamageChange { get; }
            private float FailChance { get; }
            public float DamageModifier { get; }

            public FleeRule(float successChance, float successWithDamageChange,
                float failChance, float damageModifier)
            {
                SuccessChance = successChance;
                SuccessWithDamageChange = successWithDamageChange;
                FailChance = failChance;
                DamageModifier = damageModifier;
            }

            public readonly Dictionary<FleeOutcome, float> OutcomeTable =>
                new Dictionary<FleeOutcome, float>
                {
                    {FleeOutcome.Success, SuccessChance},
                    {FleeOutcome.SuccessDmg, SuccessWithDamageChange},
                    {FleeOutcome.Fail, FailChance}
                };
        }

        private static FleeRule GetFleeRule(CombatAction enemyAction) =>
            enemyAction switch
            {
                CounterAction _ => new FleeRule(1f, 0, 0, 0),
                QuickAction _ => new FleeRule(0.2f, 0.3f, 0.5f, 2.0f),
                HeavyAction _ => new FleeRule(0.2f, 0.5f, 0.3f, 1.0f),
                _ => throw new ArgumentOutOfRangeException()
            };

        private const string Name = "Flee";
        public override string ActionName => Name;
        public override Color ActionColor => ColorValues.Flee;

        public FleeAction(BaseEntity characterInstance) : base(characterInstance)
        {
        }

        private static FleeOutcome RollOutcome(FleeRule fleeRule)
        {
            var roll = GD.Randf();
            var chance = 0f;
            foreach (var pair in fleeRule.OutcomeTable)
            {
                chance += pair.Value;
                if (roll < chance)
                {
                    return pair.Key;
                }
            }

            throw new Exception("flee chances probably dont add to 100%");
        }

        public async Task<bool> Evaluate(CombatAction enemyAction, Func<Task> onSuccess,
            Func<float, Task> onSuccessWithDamage, Func<float, Task> onFail)
        {
            var fleeRule = GetFleeRule(enemyAction);
            switch (RollOutcome(fleeRule))
            {
                case FleeOutcome.Success:
                    await onSuccess.Invoke();
                    return true;
                case FleeOutcome.SuccessDmg:
                    await onSuccessWithDamage(fleeRule.DamageModifier);
                    return true;
                case FleeOutcome.Fail:
                    await onFail(fleeRule.DamageModifier);
                    return false;
                default:
                    throw new ArgumentOutOfRangeException(
                        $"flee outcome for enemy {nameof(enemyAction)} failed");
            }
        }
    }
}
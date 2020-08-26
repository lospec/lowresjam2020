using System;
using System.Collections.Generic;
using Godot;

namespace HeroesGuild.Combat
{
    public class CombatUtil
    {
        public enum CombatAction
        {
            Invalid = -1,
            None,
            Quick,
            Counter,
            Heavy,
            Flee
        }

        public enum CombatOutcome
        {
            CombatWin,
            CombatLose,
            CombatFlee
        }

        private static readonly Dictionary<CombatAction, float[]> FleeRules =
            new Dictionary<CombatAction, float[]>
            {
                {CombatAction.None, new[] {1f, 0, 0, 0}},
                {CombatAction.Counter, new[] {1f, 0, 0, 0}},
                {CombatAction.Quick, new[] {0.2f, 0.3f, 0.5f, 2.0f}},
                {CombatAction.Heavy, new[] {0.2f, 0.5f, 0.3f, 1.0f}}
            };

        public class FleeRule
        {
            public enum FleeOutcome
            {
                Success,
                SuccessDmg,
                Fail
            }

            private readonly Dictionary<FleeOutcome, float> _outcomeTable =
                new Dictionary<FleeOutcome, float>
                {
                    {FleeOutcome.Success, 0f},
                    {FleeOutcome.SuccessDmg, 0f},
                    {FleeOutcome.Fail, 0f}
                };

            public float damageModifier;

            public FleeRule(CombatAction combatAction)
                : this(FleeRules[combatAction][0], FleeRules[combatAction][1],
                    FleeRules[combatAction][2], FleeRules[combatAction][3])
            {
            }

            private FleeRule(float fleeNoDamageChance, float fleeDamageChance, float noFleeChance, float damageModifier)
            {
                _outcomeTable[FleeOutcome.Success] = fleeNoDamageChance;
                _outcomeTable[FleeOutcome.SuccessDmg] = fleeDamageChance;
                _outcomeTable[FleeOutcome.Fail] = noFleeChance;
                this.damageModifier = damageModifier;
            }

            public FleeOutcome Roll()
            {
                var roll = GD.Randf();
                var chance = 0f;
                foreach (var pair in _outcomeTable)
                {
                    chance += pair.Value;
                    if (roll < chance)
                    {
                        return pair.Key;
                    }
                }

                throw new Exception("flee chances probably dont add to 100%");
            }
        }

        public const int MULTIPLIER_PER_COMBO = 1;

        public static string GetActionName(CombatAction action) => action switch
        {
            CombatAction.None => "None",
            CombatAction.Quick => "Quick",
            CombatAction.Counter => "Counter",
            CombatAction.Heavy => "Heavy",
            CombatAction.Flee => "Flee",
            _ => throw new ArgumentOutOfRangeException()
        };

        public static CombatAction GetActionWeakness(CombatAction action) => action switch
        {
            CombatAction.Quick => CombatAction.Counter,
            CombatAction.Counter => CombatAction.Heavy,
            CombatAction.Heavy => CombatAction.Quick,
            _ => CombatAction.Invalid
        };

        public enum TurnOutcome
        {
            Tie,
            PlayerWin,
            EnemyWin
        }

        public static TurnOutcome ActionCompare(CombatAction playerAction, CombatAction enemyAction)
        {
            if (playerAction == CombatAction.Flee)
                return enemyAction == playerAction || enemyAction == CombatAction.Counter
                    ? TurnOutcome.PlayerWin
                    : TurnOutcome.EnemyWin;

            if (enemyAction == CombatAction.Flee)
                return playerAction == CombatAction.Counter
                    ? TurnOutcome.EnemyWin
                    : TurnOutcome.PlayerWin;

            if (playerAction == enemyAction)
                return TurnOutcome.Tie;

            if (enemyAction == CombatAction.None)
                return TurnOutcome.PlayerWin;

            if (playerAction == CombatAction.None)
                return TurnOutcome.EnemyWin;

            if (GetActionWeakness(enemyAction) == playerAction)
                return TurnOutcome.PlayerWin;

            if (GetActionWeakness(playerAction) == enemyAction)
                return TurnOutcome.EnemyWin;

            throw new Exception();
        }

        public static Color GetActionColor(CombatAction action) => action switch
        {
            CombatAction.None => ColorValues.White,
            CombatAction.Quick => ColorValues.AttackQuick,
            CombatAction.Counter => ColorValues.AttackCounter,
            CombatAction.Heavy => ColorValues.AttackHeavy,
            CombatAction.Flee => ColorValues.Flee,
            _ => new Func<Color>(() =>
            {
                GD.Print("ERROR CombatUtil.gd: Invalid Action while getting color");
                return ColorValues.Invalid;
            }).Invoke()
        };
    }
}
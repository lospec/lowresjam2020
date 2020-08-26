using System;
using Godot;

namespace HeroesGuild.AI.Conditions
{
    public class AI_Condition_TargetInRange : AI_State_Condition
    {
        [Export] public float maxRange;

        public override bool Evaluate(StateMachine stateMachine)
        {
            if (stateMachine.Target == null)
            {
                return false;
            }

            return stateMachine.DistanceToTarget < maxRange;
        }
    }
}
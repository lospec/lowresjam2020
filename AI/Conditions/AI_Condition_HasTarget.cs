using Godot;

namespace HeroesGuild.AI.Conditions
{
    public class AI_Condition_HasTarget : AI_State_Condition
    {
        [Export] public bool hasTarget;

        public override bool Evaluate(StateMachine stateMachine)
        {
            return hasTarget ? stateMachine.Target != null : stateMachine.Target == null;
        }
    }
}